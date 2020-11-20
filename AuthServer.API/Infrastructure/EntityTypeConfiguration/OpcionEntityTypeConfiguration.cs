using System;
using Expertec.Sigeco.AuthServer.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Expertec.Sigeco.AuthServer.API.Infrastructure.EntityTypeConfiguration
{
    class OpcionEntityTypeConfiguration : IEntityTypeConfiguration<Opcion>
    {
        public void Configure(EntityTypeBuilder<Opcion> builder)
        {
            builder.ToTable(nameof(Opcion), AuthContext.DEFAULT_SCHEMA);

            builder.HasKey(b => b.OpcionId);

            builder.Property(b => b.OpcionId)
                .ValueGeneratedOnAdd();

            builder.Property<int>(b => b.ModuloId)
                .IsRequired();

            builder.Property<int?>(b => b.PadreId)
                .IsRequired(false);
            
            builder.Property<string>(b => b.Nombre)
                .HasColumnType("varchar(30)")
                .IsRequired();

            builder.Property<string>(b => b.Tooltip)
                .HasColumnType("varchar(100)")
                .IsRequired();

            builder.Property<int>(b => b.Secuencia)
                .IsRequired();

            builder.Property<string>(b => b.ViewClass)
                .HasColumnType("varchar(100)")
                .IsRequired();

            builder.Property<string>(b => b.ViewType)
                .HasColumnType("varchar(50)")
                .IsRequired();

            builder.Property<string>(b => b.Icono)
                .HasColumnType("varchar(30)")
                .IsRequired();

            builder.Property<bool>(b => b.Formulario)
                .IsRequired();

            builder.Property<bool>(b => b.Activo)
                .IsRequired();

            builder.Property<DateTime>(b => b.FechaRegistro)
                .HasColumnType("datetime")
                .IsRequired();

            builder.Property<bool>(b => b.Reactivo)
                .IsRequired();

            builder.Property<string>(b => b.Ruta)
                .HasMaxLength(256)
                .IsRequired(false);

            builder.HasOne(b => b.Padre)
                .WithMany()
                .HasForeignKey(r => r.PadreId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(b => b.Detalles)
                .WithOne()
                .HasForeignKey(r => r.OpcionId)
                .OnDelete(DeleteBehavior.Restrict);

            var navigation = builder.Metadata.FindNavigation(nameof(Opcion.Detalles));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}