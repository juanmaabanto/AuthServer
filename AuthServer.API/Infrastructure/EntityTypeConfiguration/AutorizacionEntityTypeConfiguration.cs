using System;
using Expertec.Sigeco.AuthServer.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Expertec.Sigeco.AuthServer.API.Infrastructure.EntityTypeConfiguration
{
    class AutorizacionEntityTypeConfiguration : IEntityTypeConfiguration<Autorizacion>
    {
        public void Configure(EntityTypeBuilder<Autorizacion> builder)
        {
            builder.ToTable(nameof(Autorizacion), AuthContext.DEFAULT_SCHEMA);

            builder.HasKey(b =>new { b.EmpresaId, b.OpcionId, b.UsuarioId });

            builder.Property<DateTime>(b => b.FechaInicio)
                .HasColumnType("datetime")
                .IsRequired();

            builder.Property<DateTime>(b => b.FechaFin)
                .HasColumnType("datetime")
                .IsRequired();

            builder.Property<DateTime>(b => b.FechaRegistro)
                .HasColumnType("datetime")
                .IsRequired();
            
            builder.Property<string>(b => b.UsuarioRegistro)
                .HasColumnType("varchar(30)")
                .IsRequired();

            builder.Property<DateTime?>(b => b.FechaModificacion)
                .HasColumnType("datetime")
                .IsRequired(false);

            builder.Property<string>(b => b.UsuarioModificacion)
                .HasColumnType("varchar(30)")
                .IsRequired(false);

            builder.HasOne(b => b.Empresa)
                .WithMany()
                .HasForeignKey(r => r.EmpresaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.Opcion)
                .WithMany()
                .HasForeignKey(r => r.OpcionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.Usuario)
                .WithMany()
                .HasForeignKey(r => r.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(b => b.Detalles)
                .WithOne()
                .HasForeignKey(r => new { r.EmpresaId, r.OpcionId, r.UsuarioId })
                .OnDelete(DeleteBehavior.Restrict);

            var navigation = builder.Metadata.FindNavigation(nameof(Autorizacion.Detalles));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}