using System;
using Expertec.Sigeco.AuthServer.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Expertec.Sigeco.AuthServer.API.Infrastructure.EntityTypeConfiguration
{
    class HistoricoClaveEntityTypeConfiguration : IEntityTypeConfiguration<HistoricoClave>
    {
        public void Configure(EntityTypeBuilder<HistoricoClave> builder)
        {
            builder.ToTable(nameof(HistoricoClave), AuthContext.DEFAULT_SCHEMA);

            builder.HasKey(b => b.HistoricoClaveId);

            builder.Property<string>(b => b.UsuarioId)
                .IsRequired();

            builder.Property<string>(b => b.Clave)
                .IsRequired();

            builder.Property<DateTime>(b => b.FechaRegistro)
                .HasColumnType("datetime")
                .IsRequired();

            builder.HasOne(b => b.Usuario)
                .WithMany()
                .HasForeignKey(r => r.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}