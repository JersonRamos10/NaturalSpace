using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalSpaceApi.Domain.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresDate { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Revoked { get; set; }
        public string? ReplacedByToken { get; set; }

        // Propiedades calculadas
        public bool IsExpired => DateTime.UtcNow >= ExpiresDate;
        public bool IsRevoked => Revoked != null;
        public bool IsActive => !IsRevoked && !IsExpired;

        // Relación con User
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
