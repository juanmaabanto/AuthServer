using Expertec.Sigeco.AuthServer.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Expertec.Sigeco.AuthServer.API.Infrastructure.EntityTypeConfiguration
{
    class PersonaEntityTypeConfiguration : IEntityTypeConfiguration<Persona>
    {
        public void Configure(EntityTypeBuilder<Persona> builder)
        {
            builder.ToTable(nameof(Persona), AuthContext.DEFAULT_SCHEMA);

            builder.HasKey(b => b.PersonaId);

            builder.Property(b => b.PersonaId)
                .ValueGeneratedNever();
            
            builder.Property<string>(b => b.NroDocumento)
                .HasColumnType("varchar(15)")
                .IsRequired();

            builder.Property<string>(b => b.PrimerNombre)
                .HasColumnType("varchar(30)")
                .IsRequired();

            builder.Property<string>(b => b.SegundoNombre)
                .HasColumnType("varchar(30)")
                .IsRequired(false);

            builder.Property<string>(b => b.ApellidoPaterno)
                .HasColumnType("varchar(50)")
                .IsRequired();

            builder.Property<string>(b => b.ApellidoMaterno)
                .HasColumnType("varchar(50)")
                .IsRequired();
        }
    }
}