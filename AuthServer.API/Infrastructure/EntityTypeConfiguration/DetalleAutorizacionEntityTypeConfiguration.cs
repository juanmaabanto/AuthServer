using System;
using Expertec.Sigeco.AuthServer.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Expertec.Sigeco.AuthServer.API.Infrastructure.EntityTypeConfiguration
{
    class DetalleAutorizacionEntityTypeConfiguration : IEntityTypeConfiguration<DetalleAutorizacion>
    {
        public void Configure(EntityTypeBuilder<DetalleAutorizacion> builder)
        {
            builder.ToTable(nameof(DetalleAutorizacion), AuthContext.DEFAULT_SCHEMA);

            builder.HasKey(b =>new { b.EmpresaId, b.OpcionId, b.UsuarioId, b.DetalleOpcionId });

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

            builder.HasOne(b => b.DetalleOpcion)
                .WithMany()
                .HasForeignKey(r => r.DetalleOpcionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}