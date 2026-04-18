using NaturalSpaceApi.Domain.Entities;
using NaturalSpaceApi.Domain.Enum;

namespace NaturalSpaceApi.Application.Interfaces
{
    public interface IPermissionService
    {
        Task<UserWorkSpace> RequireMembership(Guid userId, Guid workspaceId);

        Task<UserWorkSpace> RequireAdminAccess(Guid userId, Guid workspaceId);

        Task<UserWorkSpace> RequireOwnership(Guid userId, Guid workspaceId);

        Task<Role?> GetUserRole(Guid userId, Guid workspaceId);
    }
}
