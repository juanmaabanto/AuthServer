using System.Threading;
using System.Threading.Tasks;
using Expertec.Sigeco.AuthServer.API.Infrastructure.EntityTypeConfiguration;
using Expertec.Sigeco.AuthServer.API.Models;
using Expertec.Sigeco.AuthServer.API.SeedWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Expertec.Sigeco.AuthServer.API.Infrastructure
{
    public class AuthContext : DbContext, IUnitOfWork
    {
        #region Constantes

        public const string DEFAULT_SCHEMA = "seguridad";

        #endregion

        #region Constructor

        public AuthContext(DbContextOptions<AuthContext> options) : base(options) { }

        #endregion

        #region DbSets

        public DbSet<AplicacionCliente> AplicacionesCliente { get; set; }
        public DbSet<Favorito> Favoritos { get; set; }
        public DbSet<HistoricoClave> HistoricoClaves { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<UsuarioAplicacionCliente> UsuarioAplicacionesCliente { get; set; }
        public DbSet<UsuarioEmpresa> UsuarioEmpresas { get; set; }
        public DbSet<UsuarioRol> UsuarioRoles { get; set; }

        #endregion

        #region Override

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new AplicacionClienteEntityTypeConfiguration());
            builder.ApplyConfiguration(new AutorizacionEntityTypeConfiguration());
            builder.ApplyConfiguration(new DetalleAutorizacionEntityTypeConfiguration());
            builder.ApplyConfiguration(new DetalleEspacioTrabajoEntityTypeConfiguration());
            builder.ApplyConfiguration(new DetalleOpcionEntityTypeConfiguration());
            builder.ApplyConfiguration(new EmpresaEntityTypeConfiguration());
            builder.ApplyConfiguration(new EspacioTrabajoEntityTypeConfiguration());
            builder.ApplyConfiguration(new FavoritoEntityTypeConfiguration());
            builder.ApplyConfiguration(new HistoricoClaveEntityTypeConfiguration());
            builder.ApplyConfiguration(new ModuloEntityTypeConfiguration());
            builder.ApplyConfiguration(new ModuloRecursoEntityTypeConfiguration());
            builder.ApplyConfiguration(new OpcionEntityTypeConfiguration());
            builder.ApplyConfiguration(new PersonaEntityTypeConfiguration());
            builder.ApplyConfiguration(new RecursoEntityTypeConfiguration());
            builder.ApplyConfiguration(new RolEntityTypeConfiguration());
            builder.ApplyConfiguration(new TipoEspacioTrabajoEntityTypeConfiguration());
            builder.ApplyConfiguration(new TokenActualizacionEntityTypeConfiguration());
            builder.ApplyConfiguration(new UsuarioAplicacionClienteEntityTypeConfiguration());
            builder.ApplyConfiguration(new UsuarioEmpresaEntityTypeConfiguration());
            builder.ApplyConfiguration(new UsuarioEntityTypeConfiguration());
            builder.ApplyConfiguration(new UsuarioReclamacionesEntityTypeConfiguration());
            builder.ApplyConfiguration(new UsuarioRolEntityTypeConfiguration());
        }

        #endregion

        #region Metodos

        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            return await base.Database.BeginTransactionAsync(cancellationToken);
        }

        #endregion

    }
}