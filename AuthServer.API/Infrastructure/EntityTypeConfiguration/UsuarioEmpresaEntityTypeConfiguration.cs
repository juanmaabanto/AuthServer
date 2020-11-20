using Expertec.Sigeco.AuthServer.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Expertec.Sigeco.AuthServer.API.Infrastructure.EntityTypeConfiguration
{
    class UsuarioEmpresaEntityTypeConfiguration : IEntityTypeConfiguration<UsuarioEmpresa>
    {
        public void Configure(EntityTypeBuilder<UsuarioEmpresa> builder)
        {
            builder.ToTable(nameof(UsuarioEmpresa), AuthContext.DEFAULT_SCHEMA);

            builder.HasKey(b =>new { b.UsuarioId, b.EmpresaId });

            builder.Property<bool>(b => b.Principal)
                .IsRequired();

            builder.HasOne(b => b.Empresa)
                .WithMany()
                .HasForeignKey(r => r.EmpresaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}