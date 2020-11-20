using System.Reflection;
using Autofac;
using Expertec.Sigeco.AuthServer.API.Application.Queries;
using Expertec.Sigeco.AuthServer.API.Application.Services;
using Expertec.Sigeco.AuthServer.API.Infrastructure.Repositories;
using Expertec.Sigeco.AuthServer.API.Providers;
using Expertec.Sigeco.CrossCutting.LoggingEvent;
using Expertec.Sigeco.CrossCutting.LoggingEvent.Repositories;
using Microsoft.AspNetCore.Http;

namespace Expertec.Sigeco.AuthServer.API.Infrastructure.AutofacModules
{
    public class ApplicationModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AuthorizationServerProvider>()
                .InstancePerLifetimeScope();
                
            builder.RegisterType<Logger>()
                .As<ILogger>()
                .InstancePerLifetimeScope();

            builder.RegisterType<EventLogRepository>()
                .As<IEventLogRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<HttpContextAccessor>()
                .As<IHttpContextAccessor>()
                .SingleInstance();

            builder.RegisterType<IdentityService>()
                .As<IIdentityService>()
                .InstancePerDependency();

            var assembly = Assembly.GetExecutingAssembly();

            builder.RegisterAssemblyTypes(assembly)
                .Where(t => t.Name.EndsWith("Queries"))
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(assembly)
                .Where(t => t.Name.EndsWith("Repository"))
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(assembly)
                .Where(t => t.Name.EndsWith("Service"))
                .AsImplementedInterfaces();
        }
    }
}