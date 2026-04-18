using NaturalSpaceApi.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalSpaceApi.Application.Interfaces
{
    public interface IMemberService
    {
        Task AddMemberAsync(Guid workspaceId, Guid userId);

        Task RemoveMemberAsync(Guid workspaceId, Guid userId);

        Task<IEnumerable<UserResponse>> GetMembersAsync(Guid workspaceId);
    }
}
