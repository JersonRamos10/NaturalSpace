using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalSpaceApi.Application.DTOs.Auth
{
    public sealed record LoginRequest(
        string? UserName,
        string? Email,
        string Password
    );
   
}
