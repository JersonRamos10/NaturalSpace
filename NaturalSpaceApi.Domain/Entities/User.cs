using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalSpaceApi.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; } 

        public string Name { get; private set; } = string.Empty;

        public string UserName { get; private set; } = string.Empty;

        public string Email { get; private set; } = string.Empty; 

        public string PasswordHash { get; private set; } = string.Empty;

        public string? AvatarUrl { get; set; }

        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        //relaciones 1:N
        public ICollection<UserWorkSpace> UserWorkSpaces { get; set; } = [];

        public ICollection<ChannelMember> ChannelMembers { get; set; } = [];

        public ICollection<Message> Messages { get; set; } = [];

        public ICollection<File> Files { get; set; } = [];

        private User() { } // Para EF Core

        
    }
}
