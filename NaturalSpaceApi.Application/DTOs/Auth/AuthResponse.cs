using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalSpaceApi.Application.DTOs
{
    public sealed record AuthResponse(
     string Token,
     DateTime Expiration,
     string RefreshToken,   
     DateTime RefreshTokenExpiration
    );
}
