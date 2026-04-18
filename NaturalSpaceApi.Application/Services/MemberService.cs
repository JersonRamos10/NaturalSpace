using Mapster;
using Microsoft.EntityFrameworkCore;
using NaturalSpaceApi.Application.DTOs.Auth;
using NaturalSpaceApi.Application.DTOs.Member;
using NaturalSpaceApi.Application.Exceptions;
using NaturalSpaceApi.Application.Interfaces;
using NaturalSpaceApi.Domain.Entities;
using NaturalSpaceApi.Domain.Enum;
using NaturalSpaceApi.Infrastructure.Data.Context;

namespace NaturalSpaceApi.Application.Services
{
    public class MemberService : IMemberService
    {
        private readonly NaturalSpaceContext _context;

        public MemberService(NaturalSpaceContext context)
        {
            _context = context;
        }

        public async Task<UserResponse> AddMemberAsync(Guid workspaceId, Guid userId, Guid currentUserId)
        {
            // 1. Verificar que el workspace existe
            var workspace = await _context.WorkSpaces
                .FirstOrDefaultAsync(w => w.Id == workspaceId && !w.IsDeleted);

            if (workspace == null)
            {
                throw new NotFoundException("Workspace not found");
            }

            // 2. Verificar que el usuario actual tiene permisos (Owner o Admin)
            var currentUserWorkspace = await _context.UserWorkSpaces
                .FirstOrDefaultAsync(uw => uw.UserId == currentUserId && uw.WorkSpaceId == workspaceId);

            if (currentUserWorkspace == null)
            {
                throw new UnauthorizedException("You are not a member of this workspace");
            }

            if (currentUserWorkspace.Role == Role.Member)
            {
                throw new UnauthorizedException("Only workspace owners and admins can add members");
            }

            // 3. Verificar que el usuario a agregar existe
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            // 4. Verificar que el usuario no es ya miembro del workspace
            var existingMember = await _context.UserWorkSpaces
                .FirstOrDefaultAsync(uw => uw.UserId == userId && uw.WorkSpaceId == workspaceId);

            if (existingMember != null)
            {
                throw new ConflictException("User is already a member of this workspace");
            }

            // 5. Crear la relación UserWorkSpace
            var userWorkSpace = new UserWorkSpace
            {
                UserId = userId,
                WorkSpaceId = workspaceId,
                Role = Role.Member,
                JoinedAt = DateTime.UtcNow
            };

            await _context.UserWorkSpaces.AddAsync(userWorkSpace);
            await _context.SaveChangesAsync();

            // 6. Retornar UserResponse mapeado desde el User entity
            return user.Adapt<UserResponse>();
        }

        public async Task RemoveMemberAsync(Guid workspaceId, Guid userId, Guid currentUserId)
        {
            // 1. Verificar que el workspace existe
            var workspace = await _context.WorkSpaces
                .FirstOrDefaultAsync(w => w.Id == workspaceId && !w.IsDeleted);

            if (workspace == null)
            {
                throw new NotFoundException("Workspace not found");
            }

            // 2. Verificar que el usuario actual tiene permisos
            var currentUserWorkspace = await _context.UserWorkSpaces
                .FirstOrDefaultAsync(uw => uw.UserId == currentUserId && uw.WorkSpaceId == workspaceId);

            if (currentUserWorkspace == null)
            {
                throw new UnauthorizedException("You are not a member of this workspace");
            }

            // 3. Verificar que el usuario a eliminar existe en el workspace
            var userWorkSpace = await _context.UserWorkSpaces
                .FirstOrDefaultAsync(uw => uw.UserId == userId && uw.WorkSpaceId == workspaceId);

            if (userWorkSpace == null)
            {
                throw new NotFoundException("User is not a member of this workspace");
            }

            // 4. Validar permisos según el rol
            if (currentUserId != userId)
            {
                // Si no se está eliminando a sí mismo, verificar permisos
                if (currentUserWorkspace.Role == Role.Member)
                {
                    throw new UnauthorizedException("Only workspace owners and admins can remove members");
                }

                // Un admin no puede eliminar al Owner
                if (userWorkSpace.Role == Role.Owner && currentUserWorkspace.Role == Role.Admin)
                {
                    throw new UnauthorizedException("Admins cannot remove the workspace owner");
                }
            }
            else
            {
                // Si se está eliminando a sí mismo
                if (userWorkSpace.Role == Role.Owner)
                {
                    throw new UnauthorizedException("Workspace owner cannot leave the workspace. Transfer ownership first or delete the workspace.");
                }
            }

            _context.UserWorkSpaces.Remove(userWorkSpace);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<MemberResponse>> GetMembersAsync(Guid workspaceId, Guid currentUserId)
        {
            // 1. Verificar que el workspace existe
            var workspace = await _context.WorkSpaces
                .FirstOrDefaultAsync(w => w.Id == workspaceId && !w.IsDeleted);

            if (workspace == null)
            {
                throw new NotFoundException("Workspace not found");
            }

            // 2. Verificar que el usuario actual es miembro del workspace
            var currentUserWorkspace = await _context.UserWorkSpaces
                .FirstOrDefaultAsync(uw => uw.UserId == currentUserId && uw.WorkSpaceId == workspaceId);

            if (currentUserWorkspace == null)
            {
                throw new UnauthorizedException("You are not a member of this workspace");
            }

            // 3. Obtener todos los miembros con sus datos de usuario
            var members = await _context.UserWorkSpaces
                .Where(uw => uw.WorkSpaceId == workspaceId)
                .Include(uw => uw.User)
                .Select(uw => new MemberResponse(
                    uw.UserId,
                    uw.User.Name,
                    uw.User.UserName,
                    uw.User.Email,
                    uw.User.AvatarUrl,
                    uw.Role,
                    uw.JoinedAt
                ))
                .ToListAsync();

            return members;
        }
    }
}
