using NaturalSpaceApi.Domain.Enum;
using System;

namespace NaturalSpaceApi.Application.DTOs.Member
{
    public sealed record MemberResponse
    (
        Guid UserId,
        string Name,
        string UserName,
        string Email,
        string? AvatarUrl,
        Role Role,
        DateTime JoinedAt
    );
}
