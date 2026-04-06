using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalSpaceApi.Domain.Entities
{
    public class WorkSpace
    {
        public Guid Id { get; set; } 

        public string Name { get; set;} = string.Empty;
       
        public bool IsDeleted { get; set;}

        public DateTime? DeletedAt { get; private set; } 
        public DateTime CreatedAt { get; private set; } 


        //navegaciones

        public Guid OwnerId { get; set; }

        public User Owner { get; set; } = null!; 

        //relaciones 
        public ICollection<UserWorkSpace> UserWorkSpaces { get; set; } = [];

        public ICollection<Channel> Channels { get; set; } = [];

        private WorkSpace() { } // Para EF Core

        public WorkSpace(string name, Guid ownerId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Nombre requerido");
            if (ownerId == Guid.Empty)
                throw new ArgumentException("Owner requerido");


            Id = Guid.NewGuid();
            Name = name;
            OwnerId = ownerId;
            CreatedAt = DateTime.UtcNow;


        }

        public static WorkSpace Create(string name, Guid ownerId)
        {
            return new WorkSpace(name, ownerId);
        }



    }
}
