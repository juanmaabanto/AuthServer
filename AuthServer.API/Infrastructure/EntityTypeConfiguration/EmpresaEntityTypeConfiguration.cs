using System;
using Expertec.Sigeco.AuthServer.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Expertec.Sigeco.AuthServer.API.Infrastructure.EntityTypeConfiguration
{
    class EmpresaEntityTypeConfiguration : IEntityTypeConfiguration<Empresa>
    {
        public void Configure(EntityTypeBuilder<Empresa> builder)
        {
            builder.ToTable(nameof(Empresa), AuthContext.DEFAULT_SCHEMA);

            builder.HasKey(p => p.EmpresaId);

            builder.Property(p => p.EmpresaId)
                .ValueGeneratedOnAdd();

            builder.Property<int>(b => b.EspacioTrabajoId)
                .IsRequired();

            builder.Property<string>(b => b.Codigo)
                .HasColumnType("varchar(5)")
                .IsRequired();

            builder.Property<string>(b => b.Ruc)
                .HasColumnType("varchar(11)")
                .IsRequired();

            builder.Property<string>(b => b.RazonSocial)
                .HasColumnType("varchar(50)")
                .IsRequired();

            builder.Property<string>(b => b.DomicilioFiscal)
                .HasColumnType("varchar(200)")
                .IsRequired();

            builder.Property<string>(b => b.NombreComercial)
                .HasColumnType("varchar(100)")
                .IsRequired();

            builder.Property<string>(b => b.Ubigeo)
                .HasColumnType("varchar(10)")
                .IsRequired();
            
            builder.Property<string>(b => b.CuentaDetraccion)
                .HasColumnType("varchar(30)")
                .IsRequired(false);
                
            builder.Property<string>(b => b.Logo)
                .HasColumnType("varchar(50)")
                .IsRequired(false);

            builder.Property<string>(b => b.ImagenUri)
                .HasColumnType("varchar(50)")
                .IsRequired();

            builder.Property<bool>(b => b.ExoneradoIGV)
                .IsRequired();

            builder.Property<bool>(b => b.IncluyeIGV)
                .IsRequired();

            builder.Property<bool>(b => b.Activo)
                .IsRequired();

            builder.Property<bool>(b => b.Anulado)
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

            builder.HasOne(b => b.EspacioTrabajo)
                .WithMany()
                .HasForeignKey(b => b.EspacioTrabajoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}