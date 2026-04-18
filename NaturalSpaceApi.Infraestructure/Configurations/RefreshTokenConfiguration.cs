using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NaturalSpaceApi.Domain.Entities;

namespace NaturalSpaceApi.Infrastructure.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");

            // Clave primaria
            builder.HasKey(rt => rt.Id);

            // El token debe ser único
            builder.HasIndex(rt => rt.Token).IsUnique();

            // Configurar propiedades
            builder.Property(rt => rt.Token)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(rt => rt.ExpiresDate)
                .IsRequired();

            builder.Property(rt => rt.Created)
                .IsRequired();

            builder.Property(rt => rt.Revoked);

            builder.Property(rt => rt.ReplacedByToken)
                .HasMaxLength(500);

            // Relación N:1 con User
            builder.HasOne(rt => rt.User)
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Índice para búsqueda por UserId
            builder.HasIndex(rt => rt.UserId);
        }
    }
}
