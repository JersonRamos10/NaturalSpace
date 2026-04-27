using NaturalSpaceApi.Application.DTOs.Workspace;

namespace NaturalSpaceApi.Application.Interfaces
{
    public interface IWorkspaceService
    {
        Task<WorkspaceResponse> CreateWorkspaceAsync(CreateWorkSpaceRequest spaceRequest, Guid userId);
        Task DeleteWorkspaceAsync(Guid workspaceId, Guid userId);
        Task<WorkspaceResponse> GetByIdAsync(Guid workspaceId);
        Task<IEnumerable<WorkspaceResponse>> GetWorkspacesByUserAsync(Guid userId);
        Task<WorkspaceResponse> UpdateWorkspace(Guid workspaceId, UpdateWorkspaceRequest request, Guid userId);
    }
}
