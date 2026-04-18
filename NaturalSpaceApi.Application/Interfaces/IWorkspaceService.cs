using NaturalSpaceApi.Application.DTOs.Workspace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalSpaceApi.Application.Interfaces
{
    public interface IWorkspaceService
    {

        Task <WorkspaceResponse> CreateWorkspaceAsync(CreateWorkSpaceRequest spaceRequest, Guid userId);
        Task DeleteWorkspaceAsync(Guid workspaceId);

        Task<WorkspaceResponse> GetByIdAsync(Guid workspaceId);
        Task<WorkspaceResponse> UpdateWorkspace(Guid workspaceId, UpdateWorkspaceRequest resquest);
    }
}
