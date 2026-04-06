using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalSpaceApi.Domain.Entities
{
    public class ChannelMember
    {
        //foreaneas
        public Guid UserId { get; set; }
        public Guid ChannelId { get; set; } 
     

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public bool IsMuted { get; set; } = false;

        // Navegaciones
        public User User { get; set; } = null!;
        public Channel Channel { get; set; } = null!;
    }
}
