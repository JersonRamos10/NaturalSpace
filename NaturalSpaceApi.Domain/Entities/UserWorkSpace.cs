using NaturalSpaceApi.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalSpaceApi.Domain.Entities
{
    public class UserWorkSpace
    {

        //llaves foraneas 
        public Guid UserId { get; set; }

        public Guid WorkSpaceId { get; set; }

        public Role Role { get; set; } = Role.Member;

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        // Propiedades de Navegacion
        public User User { get; set; } = null!;
        public WorkSpace WorkSpace { get; set; } = null!;



    }
}
