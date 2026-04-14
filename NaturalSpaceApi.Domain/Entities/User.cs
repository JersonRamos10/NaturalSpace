using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalSpaceApi.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty; 

        public string PasswordHash { get; set; } = string.Empty;

        public string? AvatarUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        //relaciones 1:N
        public ICollection<UserWorkSpace> UserWorkSpaces { get; set; } = [];

        public ICollection<ChannelMember> ChannelMembers { get; set; } = [];

        public ICollection<Message> Messages { get; set; } = [];

        public ICollection<File> Files { get; set; } = [];
    }
}
