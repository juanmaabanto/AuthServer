using System;
using Expertec.Sigeco.AuthServer.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Expertec.Sigeco.AuthServer.API.Infrastructure.EntityTypeConfiguration
{
    class TokenActualizacionEntityTypeConfiguration : IEntityTypeConfiguration<TokenActualizacion>
    {
        public void Configure(EntityTypeBuilder<TokenActualizacion> builder)
        {
            builder.ToTable(nameof(TokenActualizacion), AuthContext.DEFAULT_SCHEMA);

            builder.HasKey(b => b.TokenActualizacionId);

            builder.Property<string>(b => b.TokenActualizacionId)
                .HasMaxLength(128)
                .IsRequired();

            builder.Property<string>(b => b.AplicacionClienteId)
                .HasMaxLength(128)
                .IsRequired();

            builder.Property<string>(b => b.UsuarioId)
                .HasMaxLength(128)
                .IsRequired();

            builder.Property<DateTime>(b => b.FechaEmision)
                .HasColumnType("datetime")
                .IsRequired();

            builder.Property<DateTime>(b => b.FechaExpiracion)
                .HasColumnType("datetime")
                .IsRequired();

            builder.Property<byte[]>(b => b.TicketProtegido)
                .HasColumnType("varbinary(max)")
                .IsRequired();

            builder.HasOne(b => b.AplicacionCliente)
                .WithMany()
                .HasForeignKey(r => r.AplicacionClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.Usuario)
                .WithMany()
                .HasForeignKey(r => r.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}