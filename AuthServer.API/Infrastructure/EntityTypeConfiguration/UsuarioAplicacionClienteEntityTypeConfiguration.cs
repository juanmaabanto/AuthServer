using System;
using Expertec.Sigeco.AuthServer.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Expertec.Sigeco.AuthServer.API.Infrastructure.EntityTypeConfiguration
{
    class UsuarioAplicacionClienteEntityTypeConfiguration : IEntityTypeConfiguration<UsuarioAplicacionCliente>
    {
        public void Configure(EntityTypeBuilder<UsuarioAplicacionCliente> builder)
        {
            builder.ToTable(nameof(UsuarioAplicacionCliente), AuthContext.DEFAULT_SCHEMA);

            builder.HasKey(e => new { e.UsuarioId, e.AplicacionClienteId });

            builder.Property(b => b.UsuarioId)
                .HasMaxLength(128);

            builder.Property<string>(b => b.AplicacionClienteId)
                .HasMaxLength(128);

            builder.Property<DateTime>(b => b.Fecha)
                .HasColumnType("datetime")
                .IsRequired();

            builder.Property<bool>(b => b.Permitido)
                .IsRequired();

            builder.Property<bool>(b => b.TieneAcceso)
                .IsRequired();

            builder.HasOne(b => b.Usuario)
                .WithMany()
                .HasForeignKey(r => r.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.AplicacionCliente)
                .WithMany()
                .HasForeignKey(r => r.AplicacionClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            
        }
    }
}