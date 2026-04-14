using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalSpaceApi.Application.DTOs.Auth
{
    public sealed record RegisterRequest
    (
        string Name,
        string UserName,
        string Email,
        string Password,
        string ConfirmPassword,
        string? AvatarUrl
    );
}
