using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Expertec.Sigeco.AuthServer.API.Infrastructure.EntityTypeConfiguration
{
    class RolEntityTypeConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.ToTable("Rol", AuthContext.DEFAULT_SCHEMA);

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id)
                .HasColumnName("RolId")
                .HasMaxLength(128);

            builder.Property<string>("ConcurrencyStamp")
                .HasColumnName("SelloConcurrencia")
                .HasColumnType("nvarchar(max)");
            
            builder.Property<string>("Name")
                .HasColumnName("Nombre")
                .HasMaxLength(256)
                .IsRequired();
            
            builder.Property<string>("NormalizedName")
                .HasColumnName("NombreNormalizado")
                .HasMaxLength(256)
                .IsRequired();

            builder.HasData(new IdentityRole(){
                Id = "884215f8-8450-4914-9a3e-7769f01e8f85",//Guid.NewGuid().ToString(),
                Name = "Administrador",
                NormalizedName = "ADMINISTRADOR"
            }, new IdentityRole() {
                Id = "9c871375-b636-444e-b75c-0250dd97b64d",//Guid.NewGuid().ToString(),
                Name = "Usuario",
                NormalizedName = "USUARIO"
            });

        }
    }
}