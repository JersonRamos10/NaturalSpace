using NaturalSpaceApi.Application.DTOs;
using NaturalSpaceApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalSpaceApi.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);
        Task<AuthResponse> RefreshTokenAsync(string token, string ipAddress);

        Task RevokeTokenAsync(string token, string ipAddress);

    }
}
