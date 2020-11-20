using System;
using Expertec.Sigeco.AuthServer.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Expertec.Sigeco.AuthServer.API.Infrastructure.EntityTypeConfiguration
{
    class EspacioTrabajoEntityTypeConfiguration : IEntityTypeConfiguration<EspacioTrabajo>
    {
        public void Configure(EntityTypeBuilder<EspacioTrabajo> builder)
        {
            builder.ToTable(nameof(EspacioTrabajo), AuthContext.DEFAULT_SCHEMA);

            builder.HasKey(b => b.EspacioTrabajoId);

            builder.Property(b => b.EspacioTrabajoId)
                .ValueGeneratedOnAdd();

            builder.Property<int>(b => b.TipoEspacioTrabajoId)
                .IsRequired();
            
            builder.Property<string>(b => b.Codigo)
                .HasColumnType("varchar(5)")
                .IsRequired();

            builder.Property<string>(b => b.Nombre)
                .HasColumnType("varchar(50)")
                .IsRequired();

            builder.Property<string>(b => b.Dominio)
                .HasColumnType("varchar(20)")
                .IsRequired();

            builder.Property<string>(b => b.Contacto)
                .HasColumnType("varchar(100)")
                .IsRequired();

            builder.Property<string>(b => b.Direccion)
                .HasColumnType("varchar(100)")
                .IsRequired();

            builder.Property<string>(b => b.Ciudad)
                .HasColumnType("varchar(20)")
                .IsRequired();

            builder.Property<string>(b => b.Telefono)
                .HasColumnType("varchar(20)")
                .IsRequired();

            builder.Property<string>(b => b.Correo)
                .HasColumnType("varchar(30)")
                .IsRequired();
            
            builder.Property<DateTime>(b => b.FechaCreacion)
                .HasColumnType("datetime")
                .IsRequired();

            builder.Property<DateTime?>(b => b.FechaFin)
                .HasColumnType("datetime")
                .IsRequired(false);

            builder.Property<bool>(b => b.Activo)
                .IsRequired();

            builder.HasOne(b => b.TipoEspacioTrabajo)
                .WithMany()
                .HasForeignKey(r => r.TipoEspacioTrabajoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}