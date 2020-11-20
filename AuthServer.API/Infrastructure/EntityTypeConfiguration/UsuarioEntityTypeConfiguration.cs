using Expertec.Sigeco.AuthServer.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Expertec.Sigeco.AuthServer.API.Infrastructure.EntityTypeConfiguration
{
    class UsuarioEntityTypeConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable(nameof(Usuario), AuthContext.DEFAULT_SCHEMA);

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Id)
                .HasColumnName("UsuarioId")
                .HasMaxLength(128);

            builder.Property<bool>(b => b.LockoutEnabled)
                .HasColumnName("BloqueoHabilitado");
            
            builder.Property<string>(b => b.PasswordHash)
                .HasColumnName("Clave")
                .HasColumnType("nvarchar(max)")
                .IsRequired(false);

            builder.Property<string>(b => b.Email)
                .HasColumnName("Correo")
                .HasMaxLength(256)
                .IsRequired(false);

            builder.Property<bool>(b => b.EmailConfirmed)
                .HasColumnName("CorreoConfirmado");

            builder.Property<string>(b => b.NormalizedEmail)
                .HasColumnName("CorreoNormalizado")
                .HasMaxLength(256)
                .IsRequired(false);

            builder.Property<int>(b => b.AccessFailedCount)
                .HasColumnName("CuentaAccesoFallido");

            builder.Property<bool>(b => b.TwoFactorEnabled)
                .HasColumnName("DobleFactorHabilitado");

            builder.Property(p => p.LockoutEnd)
                .HasColumnName("FinBloqueo");

            builder.Property<string>(b => b.UserName)
                .HasColumnName("NombreUsuario")
                .HasMaxLength(256)
                .IsRequired();

            builder.Property<string>(b => b.NormalizedUserName)
                .HasColumnName("NombreUsuarioNormalizado")
                .HasMaxLength(256)
                .IsRequired();

            builder.Property<string>(b => b.ConcurrencyStamp)
                .HasColumnName("SelloConcurrencia")
                .HasColumnType("nvarchar(max)")
                .IsRequired(false);

            builder.Property<string>(b => b.SecurityStamp)
                .HasColumnName("SelloSeguridad")
                .HasColumnType("nvarchar(max)")
                .IsRequired(false);

            builder.Property<string>(b => b.PhoneNumber)
                .HasColumnName("Telefono")
                .HasColumnType("nvarchar(max)")
                .IsRequired(false);

            builder.Property<bool>(b => b.PhoneNumberConfirmed)
                .HasColumnName("TelefonoConfirmado");

            builder.Property<int>(b => b.EspacioTrabajoId);

            builder.Property<int>(b => b.PersonaId)
                .IsRequired();

            builder.Property<string>(b => b.Nombre)
                .HasColumnType("varchar(50)")
                .IsRequired();
            
            builder.Property<byte[]>(b => b.Imagen)
                .HasColumnType("varbinary(max)")
                .IsRequired(false);

            builder.Property(p => p.ExpiraClave);

            builder.Property<bool>(b => b.ExpiraClaveHabilitado)
                .IsRequired();

            builder.Property<bool>(b => b.RequiereCambioClave)
                .IsRequired();

            builder.Property<bool>(b => b.Activo);

            builder.HasOne(b => b.EspacioTrabajo)
                .WithMany()
                .HasForeignKey(r => r.EspacioTrabajoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.Persona)
                .WithMany()
                .HasForeignKey(r => r.PersonaId)
                .OnDelete(DeleteBehavior.Restrict);

            var navigation = builder.Metadata.FindNavigation(nameof(Usuario.Empresas));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(b => b.Roles)
                .WithOne()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            var navigation2 = builder.Metadata.FindNavigation(nameof(Usuario.Roles));
            navigation2.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(b => b.Reclamaciones)
                .WithOne()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            var navigation3 = builder.Metadata.FindNavigation(nameof(Usuario.Reclamaciones));
            navigation3.SetPropertyAccessMode(PropertyAccessMode.Field);

        }
    }
}