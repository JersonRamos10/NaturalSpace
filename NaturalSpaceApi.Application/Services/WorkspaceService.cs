using FluentValidation;
using NaturalSpaceApi.Application.DTOs.Workspace;
using NaturalSpaceApi.Application.Interfaces;
using NaturalSpaceApi.Infrastructure.Data.Context;
using NaturalSpaceApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using NaturalSpaceApi.Application.Exceptions;
using Mapster;
using NaturalSpaceApi.Domain.Enum;

namespace NaturalSpaceApi.Application.Services
{
    public class WorkspaceService : IWorkspaceService
    {

        private readonly IValidator<CreateWorkSpaceRequest> _validator;

        private readonly NaturalSpaceContext _context;

        public WorkspaceService(IValidator<CreateWorkSpaceRequest> validator, NaturalSpaceContext context)
        {
            _validator = validator;
            _context = context;
        }
        public async Task<WorkspaceResponse> CreateWorkspaceAsync(CreateWorkSpaceRequest spaceRequest, Guid userId)
        {
            // validate the request
            var validationResult = _validator.Validate(spaceRequest);

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

        public Task DeleteWorkspaceAsync(Guid workspaceId)
        {
            var workSpaceExists = _context.WorkSpaces.Find(workspaceId) ?? throw new NotFoundException("Workspace not found");

            workSpaceExists.IsDeleted = true;
            workSpaceExists.DeletedAt = DateTime.UtcNow;

            return _context.SaveChangesAsync();
        }

        public async Task<WorkspaceResponse> GetByIdAsync(Guid workspaceId)
        {
            var workspace = await _context.WorkSpaces.FindAsync(workspaceId);

            if (workspace == null || workspace.IsDeleted == true)
            {
                throw new NotFoundException("Workspace not found");
            }

            return workspace.Adapt<WorkspaceResponse>();
        }

        public async Task<WorkspaceResponse> UpdateWorkspace(Guid workspaceId, UpdateWorkspaceRequest resquest)
        {
            var workspace = await _context.WorkSpaces.FindAsync(workspaceId);

            if (workspace == null || workspace.IsDeleted == true)
            {
                throw new NotFoundException("Workspace not found");
            }

            //update the workspace properties

            workspace.Name = resquest.Name ?? workspace.Name;
            workspace.Description = resquest.Description ?? workspace.Description;
            workspace.UpdatedAt = DateTime.UtcNow;  

            await _context.SaveChangesAsync();

            return workspace.Adapt<WorkspaceResponse>();
        }
    }
}
