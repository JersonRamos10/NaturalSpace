using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalSpaceApi.Domain.Entities
{
    public class Channel
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public bool IsPrivate { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedAt { get; set; } 
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        // navegaciones

        public Guid WorkSpaceId { get; set;  }

        public WorkSpace WorkSpace { get; set; } = null!;

        public Guid CreatedById { get; set; }

        public User CreatedByUser { get; set; } = null!;
        //relaciones 

        //N:M
        public ICollection<ChannelMember> Members { get; set; } = [];

        //1:N

        public ICollection<Message> Messages { get; set; } = [];




    }
}
