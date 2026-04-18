using Microsoft.EntityFrameworkCore;
using NaturalSpaceApi.Application.Exceptions;
using NaturalSpaceApi.Application.Interfaces;
using NaturalSpaceApi.Domain.Entities;
using NaturalSpaceApi.Domain.Enum;
using NaturalSpaceApi.Infrastructure.Data.Context;

namespace NaturalSpaceApi.Application.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly NaturalSpaceContext _context;

        public PermissionService(NaturalSpaceContext context)
        {
            _context = context;
        }

        public async Task<UserWorkSpace> RequireMembership(Guid userId, Guid workspaceId)
        {
            var membership = await _context.UserWorkSpaces
                .FirstOrDefaultAsync(uw => uw.UserId == userId && uw.WorkSpaceId == workspaceId);

            if (membership == null)
            {
                throw new UnauthorizedException("You are not a member of this workspace");
            }

            return membership;
        }

        public async Task<UserWorkSpace> RequireAdminAccess(Guid userId, Guid workspaceId)
        {
            var membership = await RequireMembership(userId, workspaceId);

            if (membership.Role == Role.Member)
            {
                throw new UnauthorizedException("Only workspace owners and admins can perform this action");
            }

            return membership;
        }

        public async Task<UserWorkSpace> RequireOwnership(Guid userId, Guid workspaceId)
        {
            var membership = await RequireMembership(userId, workspaceId);

            if (membership.Role != Role.Owner)
            {
                throw new UnauthorizedException("Only the workspace owner can perform this action");
            }

            return membership;
        }

        public async Task<Role?> GetUserRole(Guid userId, Guid workspaceId)
        {
            var membership = await _context.UserWorkSpaces
                .FirstOrDefaultAsync(uw => uw.UserId == userId && uw.WorkSpaceId == workspaceId);

            return membership?.Role;
        }
    }
}
