using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NaturalSpaceApi.Infrastructure.Configurations
{
    internal class FileConfiguration : IEntityTypeConfiguration<NaturalSpaceApi.Domain.Entities.File>
    {
        public void Configure(EntityTypeBuilder<NaturalSpaceApi.Domain.Entities.File> builder)
        {
            builder.ToTable("Files");

            // Clave primaria
            builder.HasKey(f => f.Id);
            builder.Property(f => f.Id).ValueGeneratedNever();

            // Propiedades
            builder.Property(f => f.FileName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(f => f.FilePath)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(f => f.CreatedAt)
                .IsRequired();

            builder.Property(f => f.UserId)
                .IsRequired();

            builder.Property(f => f.MessageId)
                .IsRequired();

            // Relación N:1 con User (UploadBy)
            builder.HasOne(f => f.UploadBy)
                .WithMany(u => u.Files)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación N:1 con Message
            builder.HasOne(f => f.Message)
                .WithMany(m => m.Attachments)
                .HasForeignKey(f => f.MessageId)
                .OnDelete(DeleteBehavior.Cascade);

            // Índices
            builder.HasIndex(f => f.UserId);
            builder.HasIndex(f => f.MessageId);
        }
    }
}
