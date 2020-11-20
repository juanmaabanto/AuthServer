using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Expertec.Sigeco.AuthServer.API.Infrastructure.EntityTypeConfiguration
{
    class UsuarioReclamacionesEntityTypeConfiguration : IEntityTypeConfiguration<IdentityUserClaim<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserClaim<string>> builder)
        {
            builder.ToTable("UsuarioReclamacion", AuthContext.DEFAULT_SCHEMA);

            builder.HasKey(u => new { u.Id });

            builder.Property(p => p.Id)
                .HasColumnName("UsuarioReclamacionId");

            builder.Property(p => p.UserId)
                .HasColumnName("UsuarioId")
                .HasMaxLength(128)
                .IsRequired();

            builder.Property(p => p.ClaimType)
                .HasColumnName("Tipo")
                .IsRequired();

            builder.Property(p => p.ClaimValue)
                .HasColumnName("Valor")
                .IsRequired();

        }
    }
}