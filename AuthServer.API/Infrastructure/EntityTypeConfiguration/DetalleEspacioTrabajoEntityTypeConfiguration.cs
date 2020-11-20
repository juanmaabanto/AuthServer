using System;
using Expertec.Sigeco.AuthServer.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Expertec.Sigeco.AuthServer.API.Infrastructure.EntityTypeConfiguration
{
    class DetalleEspacioTrabajoEntityTypeConfiguration : IEntityTypeConfiguration<DetalleEspacioTrabajo>
    {
        public void Configure(EntityTypeBuilder<DetalleEspacioTrabajo> builder)
        {
            builder.ToTable(nameof(DetalleEspacioTrabajo), AuthContext.DEFAULT_SCHEMA);

            builder.HasKey(p => p.DetalleEspacioTrabajoId);

            builder.Property(p => p.DetalleEspacioTrabajoId)
                .ValueGeneratedOnAdd();

            builder.Property<int>(b => b.EspacioTrabajoId)
                .IsRequired();

            builder.Property<int>(b => b.ModuloId)
                .IsRequired();

            builder.Property<DateTime>(b => b.FechaInicio)
                .HasColumnType("datetime")
                .IsRequired();

            builder.Property<DateTime?>(b => b.FechaFin)
                .HasColumnType("datetime")
                .IsRequired(false);

            builder.Property<bool>(b => b.Activo)
                .IsRequired();

            builder.HasOne(b => b.EspacioTrabajo)
                .WithMany()
                .HasForeignKey(r => r.EspacioTrabajoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.Modulo)
                .WithMany()
                .HasForeignKey(r => r.ModuloId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}