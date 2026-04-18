using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NaturalSpaceApi.Domain.Entities;

namespace NaturalSpaceApi.Infrastructure.Configurations
{
    internal class WorkSpaceConfiguration : IEntityTypeConfiguration<WorkSpace>
    {
        public void Configure(EntityTypeBuilder<WorkSpace> builder)
        {
            builder.ToTable("WorkSpaces");

            // Clave primaria
            builder.HasKey(w => w.Id);
            builder.Property(w => w.Id).ValueGeneratedNever();

            // Propiedades
            builder.Property(w => w.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(w => w.Description)
                .HasMaxLength(100);

            builder.Property(w => w.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(w => w.DeletedAt);

            builder.Property(w => w.CreatedAt)
                .IsRequired();

            builder.Property(w => w.UpdatedAt);

            builder.Property(w => w.OwnerId)
                .IsRequired();

            // Relación N:1 con Owner (User)
            builder.HasOne(w => w.Owner)
                .WithMany()
                .HasForeignKey(w => w.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación 1:N con UserWorkSpaces
            builder.HasMany(w => w.UserWorkSpaces)
                .WithOne(uw => uw.WorkSpace)
                .HasForeignKey(uw => uw.WorkSpaceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Índice filtrado para obtener solo WorkSpaces activos
            builder.HasIndex(w => new { w.Id, w.IsDeleted })
                .HasFilter("[IsDeleted] = 0");
        }
    }
}
