using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalSpaceApi.Domain.Entities
{
    public class WorkSpace
    {
        public Guid Id { get; set; } 

        public string Name { get; set;} = string.Empty;

        public string Description { get; set;} = string.Empty;  

        public bool IsDeleted { get; set;}

        public DateTime? DeletedAt { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } 
        //navegaciones

        public Guid OwnerId { get; set; }

        public User Owner { get; set; } = null!; 

        //relaciones 
        public ICollection<UserWorkSpace> UserWorkSpaces { get; set; } = [];

        public ICollection<Channel> Channels { get; set; } = [];

    }
}
