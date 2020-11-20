using Expertec.Sigeco.AuthServer.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Expertec.Sigeco.AuthServer.API.Infrastructure.EntityTypeConfiguration
{
    class ModuloRecursoEntityTypeConfiguration : IEntityTypeConfiguration<ModuloRecurso>
    {
        public void Configure(EntityTypeBuilder<ModuloRecurso> builder)
        {
            builder.ToTable(nameof(ModuloRecurso), AuthContext.DEFAULT_SCHEMA);

            builder.HasKey(b =>new { b.ModuloId, b.RecursoId });

            builder.Property<bool>(b => b.Activo)
                .IsRequired();

            builder.HasOne(b => b.Modulo)
                .WithMany()
                .HasForeignKey(r => r.ModuloId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.Recurso)
                .WithMany()
                .HasForeignKey(r => r.RecursoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}