using NaturalSpaceApi.Application.DTOs.Auth;
using NaturalSpaceApi.Application.DTOs.Member;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalSpaceApi.Application.Interfaces
{
    public interface IMemberService
    {
        Task<UserResponse> AddMemberAsync(Guid workspaceId, Guid userId, Guid currentUserId);

        Task RemoveMemberAsync(Guid workspaceId, Guid userId, Guid currentUserId);

        Task<IEnumerable<MemberResponse>> GetMembersAsync(Guid workspaceId, Guid currentUserId);
    }
}
