using FluentValidation;
using NaturalSpaceApi.Application.DTOs.Workspace;
using NaturalSpaceApi.Application.Interfaces;
using NaturalSpaceApi.Infrastructure.Data.Context;
using NaturalSpaceApi.Domain.Entities;
using NaturalSpaceApi.Application.Exceptions;
using Mapster;
using NaturalSpaceApi.Domain.Enum;

namespace NaturalSpaceApi.Application.Services
{
    public class WorkspaceService : IWorkspaceService
    {
        private readonly IValidator<CreateWorkSpaceRequest> _validator;
        private readonly NaturalSpaceContext _context;
        private readonly IPermissionService _permissions;

        public WorkspaceService(
            IValidator<CreateWorkSpaceRequest> validator, 
            NaturalSpaceContext context,
            IPermissionService permissions)
        {
            _validator = validator;
            _context = context;
            _permissions = permissions;
        }

        public async Task<WorkspaceResponse> CreateWorkspaceAsync(CreateWorkSpaceRequest spaceRequest, Guid userId)
        {
            // validate the request
            var validationResult = await _validator.ValidateAsync(spaceRequest);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            // create the workspace entity
            var workspace = new WorkSpace
            {
                Name = spaceRequest.Name,
                Description = spaceRequest.Description,
                OwnerId = userId,
                CreatedAt = DateTime.UtcNow,
                UserWorkSpaces = new List<UserWorkSpace>
                {
                    new UserWorkSpace
                    {
                        UserId = userId,
                        Role = Role.Owner, 
                        JoinedAt = DateTime.UtcNow
                    }
                }   
            };

            await _context.WorkSpaces.AddAsync(workspace);
            await _context.SaveChangesAsync();

            return workspace.Adapt<WorkspaceResponse>();
        }

        public async Task DeleteWorkspaceAsync(Guid workspaceId, Guid userId)
        {
            // Solo el Owner puede eliminar el workspace
            await _permissions.RequireOwnership(userId, workspaceId);

            var workspace = await _context.WorkSpaces.FindAsync(workspaceId);

            if (workspace == null || workspace.IsDeleted)
            {
                throw new NotFoundException("Workspace not found");
            }

            workspace.IsDeleted = true;
            workspace.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task<WorkspaceResponse> GetByIdAsync(Guid workspaceId)
        {
            var workspace = await _context.WorkSpaces.FindAsync(workspaceId);

            if (workspace == null || workspace.IsDeleted)
            {
                throw new NotFoundException("Workspace not found");
            }

            return workspace.Adapt<WorkspaceResponse>();
        }

        public async Task<WorkspaceResponse> UpdateWorkspace(Guid workspaceId, UpdateWorkspaceRequest request, Guid userId)
        {
            // Solo Owner o Admin pueden actualizar
            await _permissions.RequireAdminAccess(userId, workspaceId);

            var workspace = await _context.WorkSpaces.FindAsync(workspaceId);

            if (workspace == null || workspace.IsDeleted)
            {
                throw new NotFoundException("Workspace not found");
            }

            // update the workspace properties
            workspace.Name = request.Name ?? workspace.Name;
            workspace.Description = request.Description ?? workspace.Description;
            workspace.UpdatedAt = DateTime.UtcNow;  

            await _context.SaveChangesAsync();

            return workspace.Adapt<WorkspaceResponse>();
        }
    }
}
