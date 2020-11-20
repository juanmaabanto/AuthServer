using Expertec.Sigeco.AuthServer.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Expertec.Sigeco.AuthServer.API.Infrastructure.EntityTypeConfiguration
{
    class DetalleOpcionEntityTypeConfiguration : IEntityTypeConfiguration<DetalleOpcion>
    {
        public void Configure(EntityTypeBuilder<DetalleOpcion> builder)
        {
            builder.ToTable(nameof(DetalleOpcion), AuthContext.DEFAULT_SCHEMA);

            builder.HasKey(b => b.DetalleOpcionId);

            builder.Property(b => b.DetalleOpcionId)
                .ValueGeneratedOnAdd();

            builder.Property<int>(b => b.OpcionId)
                .IsRequired();
            
            builder.Property<string>(b => b.Nombre)
                .HasColumnType("varchar(50)")
                .IsRequired();

            builder.Property<string>(b => b.NombreControlador)
                .HasColumnType("varchar(50)")
                .IsRequired();

            builder.Property<string>(b => b.NombreAccion)
                .HasColumnType("varchar(50)")
                .IsRequired();

            builder.Property<bool>(b => b.Activo)
                .IsRequired();
        }
    }
}