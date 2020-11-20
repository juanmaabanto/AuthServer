using Expertec.Sigeco.AuthServer.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Expertec.Sigeco.AuthServer.API.Infrastructure.EntityTypeConfiguration
{
    class UsuarioRolEntityTypeConfiguration : IEntityTypeConfiguration<UsuarioRol>
    {
        public void Configure(EntityTypeBuilder<UsuarioRol> builder)
        {
            builder.ToTable(nameof(UsuarioRol), AuthContext.DEFAULT_SCHEMA);

            builder.HasKey(b => new { b.UserId, b.RoleId });

            builder.Property(b => b.RoleId)
                .HasColumnName("RolId")
                .HasMaxLength(128);

            builder.Property(b => b.UserId)
                .HasColumnName("UsuarioId")
                .HasMaxLength(128);

            builder.HasOne(b => b.Rol)
                .WithMany()
                .HasForeignKey(r => r.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}