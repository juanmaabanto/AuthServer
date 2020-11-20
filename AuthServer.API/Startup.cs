using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Autofac;
using Expertec.Sigeco.AuthServer.API.Application.IntegrationEvents;
using Expertec.Sigeco.AuthServer.API.Infrastructure;
using Expertec.Sigeco.AuthServer.API.Infrastructure.AutofacModules;
using Expertec.Sigeco.AuthServer.API.Models;
using Expertec.Sigeco.AuthServer.API.Providers;
using Expertec.Sigeco.CrossCutting.EventBus;
using Expertec.Sigeco.CrossCutting.EventBus.Abstractions;
using Expertec.Sigeco.CrossCutting.EventBusServiceBus;
using Expertec.Sigeco.CrossCutting.IntegrationEventLog;
using Expertec.Sigeco.CrossCutting.IntegrationEventLog.Services;
using Expertec.Sigeco.CrossCutting.LoggingEvent;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Azure.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Expertec.Sigeco.AuthServer.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(@"/var/lib/dataprotected/"))
                .ProtectKeysWithCertificate(new X509Certificate2(Configuration["Kestrel:Certificates:Default:Path"], Configuration["Kestrel:Certificates:Default:Password"]));
            
            services.Configure<AuthSettings>(Configuration);

            services.AddDbContext<AuthContext>(options =>
                 options.UseSqlServer(Configuration.GetConnectionString("Api")));
                
            services.AddDbContext<LogContext>(options =>
                 options.UseSqlServer(Configuration.GetConnectionString("Logging")));

            services.AddDbContext<IntegrationEventLogContext>(options =>
                 options.UseSqlServer(Configuration.GetConnectionString("Logging")));

            services.AddIdentity<Usuario, IdentityRole>(options =>{
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
                options.Lockout.MaxFailedAccessAttempts = 3;
            })
            .AddUserStore<UserStore<Usuario,IdentityRole, AuthContext, string, IdentityUserClaim<string>, UsuarioRol, IdentityUserLogin<string>, IdentityUserToken<string>, IdentityRoleClaim<string>>>()
            .AddEntityFrameworkStores<AuthContext>()
            .AddDefaultTokenProviders();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.WithOrigins(Configuration["AllowedOrigins"].Split(";"))
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options => {
                options.Cookie.Name = "Sigeco.Auth";
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = _env.IsDevelopment() ? CookieSecurePolicy.None : CookieSecurePolicy.Always;
                options.ExpireTimeSpan = TimeSpan.FromDays(365);
                options.LoginPath = new PathString("/signin");
                options.LogoutPath = new PathString("/signout");
                options.Events.OnRedirectToLogin = CookieAuthenticationProvider.RedirectToLoginAsync;
                options.Events.OnValidatePrincipal = CookieAuthenticationProvider.ValidatePrincipalAsync;
            })
            .AddOpenIdConnectServer(options =>
            {
               options.ProviderType = typeof(AuthorizationServerProvider);
                //endpoints
                options.AuthorizationEndpointPath = new PathString("/connect/authorize");
                options.LogoutEndpointPath = "/connect/logout";
                options.TokenEndpointPath = "/connect/token";
                options.UserinfoEndpointPath = "/connect/userinfo";
                //
                options.ApplicationCanDisplayErrors = true;
                options.AllowInsecureHttp = _env.IsDevelopment();
                options.AccessTokenLifetime = TimeSpan.FromMinutes(10);
                //
                options.AccessTokenHandler = new JwtSecurityTokenHandler
                {
                    InboundClaimTypeMap = new Dictionary<string, string>(),
                    OutboundClaimTypeMap = new Dictionary<string, string>(),
                    MaximumTokenSizeInBytes = 2048
                };
                
                options.SigningCredentials.AddCertificate(new X509Certificate2(Configuration["Kestrel:Certificates:Default:Path"], Configuration["Kestrel:Certificates:Default:Password"]));
            })
            .AddJwtBearer(options =>
            {
                options.Audience = "sigecoservices";
                options.Authority = Configuration["Services:IdentityUrl"];
                
                options.RequireHttpsMetadata = !_env.IsDevelopment();
            });

            services.AddControllers().AddNewtonsoftJson();
            services.AddRazorPages();
            services.AddAntiforgery(o => o.SuppressXFrameOptionsHeader = true);

            services.AddMediatR(Assembly.GetExecutingAssembly());

            //service bus
            services.AddTransient<Func<DbConnection, IIntegrationEventLogService>>(
                sp => (DbConnection c) => new IntegrationEventLogService(c));
            
            services.AddTransient<IAuthIntegrationEventService, AuthIntegrationEventService>();

            services.AddSingleton<IServiceBusPersisterConnection>(sp =>
            {
                var serviceBusConnection = new ServiceBusConnectionStringBuilder(Configuration["ServiceBus:Connection"]);
                serviceBusConnection.EntityPath = Configuration["ServiceBus:TopicName"];

                return new DefaultServiceBusPersisterConnection(serviceBusConnection);
            });

            services.AddSingleton<IEventBus, EventBusServiceBus>(sp =>
            {
                var serviceBusPersisterConnection = sp.GetRequiredService<IServiceBusPersisterConnection>();
                var subscriptionClientName = Configuration["ServiceBus:SubscriptionName"];
                var subscriptionClient = new SubscriptionClient(serviceBusPersisterConnection.ServiceBusConnectionStringBuilder, subscriptionClientName);
                var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();
                var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();

                return new EventBusServiceBus(serviceBusPersisterConnection, eventBusSubcriptionsManager, subscriptionClient, iLifetimeScope);

            });

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new ApplicationModule());
            builder.RegisterModule(new MediatorModule());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHttpsRedirection();
            }

            app.UseCors("CorsPolicy");
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = context =>
                {
                    context.Context.Response.Headers.Add("cache-control", new[] { "public,max-age=31536000" });
                    context.Context.Response.Headers.Add("Expires", new[] { DateTime.UtcNow.AddYears(1).ToString("R") });
                }
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }
}
