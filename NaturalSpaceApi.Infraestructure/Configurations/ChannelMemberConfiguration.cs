using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NaturalSpaceApi.Domain.Entities;

namespace NaturalSpaceApi.Infrastructure.Configurations
{
    internal class ChannelMemberConfiguration : IEntityTypeConfiguration<ChannelMember>
    {
        public void Configure(EntityTypeBuilder<ChannelMember> builder)
        {
            builder.ToTable("ChannelMembers");

            // Clave primaria compuesta
            builder.HasKey(cm => new { cm.UserId, cm.ChannelId });

            // Propiedades
            builder.Property(cm => cm.UserId)
                .IsRequired();

            builder.Property(cm => cm.ChannelId)
                .IsRequired();

            builder.Property(cm => cm.JoinedAt)
                .IsRequired();

            builder.Property(cm => cm.IsMuted)
                .IsRequired()
                .HasDefaultValue(false);

            // Relación N:1 con User
            builder.HasOne(cm => cm.User)
                .WithMany(u => u.ChannelMembers)
                .HasForeignKey(cm => cm.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación N:1 con Channel
            builder.HasOne(cm => cm.Channel)
                .WithMany(c => c.Members)
                .HasForeignKey(cm => cm.ChannelId)
                .OnDelete(DeleteBehavior.Cascade);

            // Índices
            builder.HasIndex(cm => cm.UserId);
            builder.HasIndex(cm => cm.ChannelId);
        }
    }
}
