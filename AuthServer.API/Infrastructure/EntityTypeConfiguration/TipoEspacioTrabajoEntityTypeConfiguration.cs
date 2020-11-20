using Expertec.Sigeco.AuthServer.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Expertec.Sigeco.AuthServer.API.Infrastructure.EntityTypeConfiguration
{
    class TipoEspacioTrabajoEntityTypeConfiguration : IEntityTypeConfiguration<TipoEspacioTrabajo>
    {
        public void Configure(EntityTypeBuilder<TipoEspacioTrabajo> builder)
        {
            builder.ToTable("TipoEspacioTrabajo", AuthContext.DEFAULT_SCHEMA);

            builder.HasKey(b => b.Id);

            builder.Property(c => c.Id)
                .HasColumnName("TipoEspacioTrabajoId")
                .ValueGeneratedNever();

            builder.Property(c => c.Nombre)
                .HasColumnName("Nombre")
                .HasColumnType("varchar(20)")
                .IsRequired();

            builder.HasData(
                TipoEspacioTrabajo.List()
            );
        }
    }
}