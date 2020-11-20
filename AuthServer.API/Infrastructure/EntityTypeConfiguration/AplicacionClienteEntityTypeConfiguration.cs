using Expertec.Sigeco.AuthServer.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Expertec.Sigeco.AuthServer.API.Infrastructure.EntityTypeConfiguration
{
    class AplicacionClienteEntityTypeConfiguration : IEntityTypeConfiguration<AplicacionCliente>
    {
        public void Configure(EntityTypeBuilder<AplicacionCliente> builder)
        {
            builder.ToTable(nameof(AplicacionCliente), AuthContext.DEFAULT_SCHEMA);

            builder.HasKey(e => e.AplicacionClienteId);

            builder.Property<string>(b => b.AplicacionClienteId)
                .HasMaxLength(128);

            builder.Property<string>(b => b.Nombre)
                .HasColumnType("varchar(50)")
                .IsRequired();

            builder.Property<string>(b => b.Secreto)
                .HasMaxLength(128)
                .IsRequired();

            builder.Property<int>(b => b.TipoAplicacion)
                .IsRequired();

            builder.Property<int>(b => b.TiempoVidaTokenActualizacion)
                .IsRequired();

            builder.Property<string>(b => b.OrigenPermitido)
                .HasColumnType("varchar(50)")
                .IsRequired();

            builder.Property<string>(b => b.RedirigirUri)
                .HasColumnType("varchar(100)")
                .IsRequired();

            builder.Property<bool>(b => b.EsTercero)
                .IsRequired();

            builder.Property<bool>(b => b.Activo)
                .IsRequired();
        }
    }
}