using System;
using Expertec.Sigeco.AuthServer.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Expertec.Sigeco.AuthServer.API.Infrastructure.EntityTypeConfiguration
{
    class ModuloEntityTypeConfiguration : IEntityTypeConfiguration<Modulo>
    {
        public void Configure(EntityTypeBuilder<Modulo> builder)
        {
            
            builder.ToTable(nameof(Modulo), AuthContext.DEFAULT_SCHEMA);

            builder.Property(c => c.ModuloId)
                .ValueGeneratedOnAdd();

            builder.Property<string>(b => b.AplicacionClienteId)
                .HasMaxLength(128)
                .IsRequired();

            builder.Property<string>(b => b.Nombre)
                .HasColumnType("varchar(30)")
                .IsRequired();

            builder.Property<string>(b => b.NombreCorto)
                .HasColumnType("varchar(30)")
                .IsRequired();

            builder.Property<string>(b => b.NombreRuta)
                .HasColumnType("varchar(30)")
                .IsRequired();
            
            builder.Property<string>(b => b.UriRuta)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property<string>(b => b.Imagen)
                .HasColumnType("varchar(15)")
                .IsRequired();

            builder.Property<bool>(b => b.Activo)
                .IsRequired();

            builder.Property<DateTime>(b => b.FechaRegistro)
                .HasColumnType("datetime")
                .IsRequired();

            builder.Property<string>(b => b.UsuarioRegistro)
                .HasColumnType("varchar(30)")
                .IsRequired();

            builder.Property<string>(b => b.NombreHost)
                .HasColumnType("varchar(30)")
                .IsRequired();
            
            builder.Property<string>(b => b.Host)
                .HasMaxLength(256)
                .IsRequired();

            builder.HasOne(b => b.AplicacionCliente)
                .WithMany()
                .HasForeignKey(r => r.AplicacionClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(b => b.Opciones)
                .WithOne()
                .HasForeignKey(r => r.ModuloId)
                .OnDelete(DeleteBehavior.Restrict);

            var navigation = builder.Metadata.FindNavigation(nameof(Modulo.Opciones));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

        }
    }
}