using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NaturalSpaceApi.Domain.Entities;

namespace NaturalSpaceApi.Infrastructure.Configurations
{
    internal class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable("Messages");

            // Clave primaria
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Id).ValueGeneratedNever();

            // Propiedades
            builder.Property(m => m.Content)
                .IsRequired()
                .HasMaxLength(4000);

            builder.Property(m => m.UpdateAt)
                .IsRequired();

            builder.Property(m => m.CreatedAt)
                .IsRequired();

            builder.Property(m => m.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(m => m.UserId)
                .IsRequired();

            builder.Property(m => m.ChannelId)
                .IsRequired();

            // Relación N:1 con User
            builder.HasOne(m => m.User)
                .WithMany(u => u.Messages)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación N:1 con Channel
            builder.HasOne(m => m.Channel)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ChannelId)
                .OnDelete(DeleteBehavior.Cascade);

            // Índices
            builder.HasIndex(m => m.ChannelId);
            builder.HasIndex(m => m.UserId);
            builder.HasIndex(m => m.CreatedAt);

            // Índice filtrado para mensajes activos
            builder.HasIndex(m => new { m.Id, m.IsDeleted })
                .HasFilter("[IsDeleted] = 0");
        }
    }
}
