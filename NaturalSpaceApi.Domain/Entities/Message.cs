using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalSpaceApi.Domain.Entities
{
    public class Message
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Content { get; set; } = string.Empty;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; } 

        public Guid UserId { get; set; }

        //relaciones 1:N
        //propiedades de navegacion
        public User User { get; set; } = null!;

        public Guid ChannelId { get; set; }

        public Channel Channel { get; set; } = null!;

        public ICollection<File> Attachments { get; set; } = [];

        public void MarkAsDeleted()
        {
            this.IsDeleted = true;
            this.UpdatedAt = DateTime.UtcNow;
        }
    }
}
