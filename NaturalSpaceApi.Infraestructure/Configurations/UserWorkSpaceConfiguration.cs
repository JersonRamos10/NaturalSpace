using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NaturalSpaceApi.Domain.Entities;

namespace NaturalSpaceApi.Infrastructure.Configurations
{
    internal class UserWorkSpaceConfiguration : IEntityTypeConfiguration<UserWorkSpace>
    {
        public void Configure(EntityTypeBuilder<UserWorkSpace> builder)
        {
            builder.ToTable("UserWorkSpaces");

            // Clave primaria compuesta
            builder.HasKey(uw => new { uw.UserId, uw.WorkSpaceId });

            // Propiedades
            builder.Property(uw => uw.UserId)
                .IsRequired();

            builder.Property(uw => uw.WorkSpaceId)
                .IsRequired();

            builder.Property(uw => uw.Role)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(uw => uw.JoinedAt)
                .IsRequired();

            // Relación N:1 con User
            builder.HasOne(uw => uw.User)
                .WithMany(u => u.UserWorkSpaces)
                .HasForeignKey(uw => uw.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación N:1 con WorkSpace
            builder.HasOne(uw => uw.WorkSpace)
                .WithMany(w => w.UserWorkSpaces)
                .HasForeignKey(uw => uw.WorkSpaceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Índices
            builder.HasIndex(uw => uw.UserId);
            builder.HasIndex(uw => uw.WorkSpaceId);
        }
    }
}
