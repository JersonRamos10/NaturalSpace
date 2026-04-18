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
        string GenerateRefreshTokenAsync();


    }
}
