using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NaturalSpaceApi.Domain.Entities;

namespace NaturalSpaceApi.Infrastructure.Configurations
{
    internal class ChannelConfiguration : IEntityTypeConfiguration<Channel>
    {
        public void Configure(EntityTypeBuilder<Channel> builder)
        {
            builder.ToTable("Channels");

            // Clave primaria
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedNever();

            // Propiedades
            builder.Property(c => c.Name)
                .HasMaxLength(100);

            builder.Property(c => c.IsPrivate)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(c => c.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(c => c.DeletedAt);

            builder.Property(c => c.CreatedAt)
                .IsRequired();

            builder.Property(c => c.WorkSpaceId)
                .IsRequired();

            builder.Property(c => c.CreatedById)
                .IsRequired();

            // Relación N:1 con WorkSpace
            builder.HasOne(c => c.WorkSpace)
                .WithMany(w => w.Channels)
                .HasForeignKey(c => c.WorkSpaceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación N:1 con User (CreatedBy)
            builder.HasOne(c => c.CreatedByUser)
                .WithMany()
                .HasForeignKey(c => c.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación N:M con User a través de ChannelMember
            builder.HasMany(c => c.Members)
                .WithOne(cm => cm.Channel)
                .HasForeignKey(cm => cm.ChannelId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación 1:N con Messages
            builder.HasMany(c => c.Messages)
                .WithOne(m => m.Channel)
                .HasForeignKey(m => m.ChannelId)
                .OnDelete(DeleteBehavior.Cascade);

            // Índice filtrado para canales activos
            builder.HasIndex(c => new { c.Id, c.IsDeleted })
                .HasFilter("[IsDeleted] = 0");

            builder.HasIndex(c => c.WorkSpaceId);
        }
    }
}
