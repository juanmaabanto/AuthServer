using System;
using Expertec.Sigeco.AuthServer.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Expertec.Sigeco.AuthServer.API.Infrastructure.EntityTypeConfiguration
{
    class RecursoEntityTypeConfiguration : IEntityTypeConfiguration<Recurso>
    {
        public void Configure(EntityTypeBuilder<Recurso> builder)
        {
            builder.ToTable(nameof(Recurso), AuthContext.DEFAULT_SCHEMA);

            builder.HasKey(b => b.RecursoId);

            builder.Property(b => b.RecursoId)
                .ValueGeneratedOnAdd();
            
            builder.Property<string>(b => b.Nombre)
                .HasColumnType("varchar(30)")
                .IsRequired();

            builder.Property<string>(b => b.NombreHost)
                .HasColumnType("varchar(30)")
                .IsRequired();

            builder.Property<string>(b => b.UriHost)
                .HasColumnType("nvarchar(256)")
                .IsRequired();

            builder.Property<bool>(b => b.Activo)
                .IsRequired();

            builder.Property<DateTime>(b => b.FechaRegistro)
                .HasColumnType("datetime")
                .IsRequired();
        }
    }
}