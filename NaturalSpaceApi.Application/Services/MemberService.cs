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
        private readonly IPermissionService _permissions;

        public MemberService(
            NaturalSpaceContext context,
            IPermissionService permissions)
        {
            _context = context;
            _permissions = permissions;
        }

        public async Task<UserResponse> AddMemberAsync(Guid workspaceId, Guid userId, Guid currentUserId)
        {
            // Solo Owner o Admin pueden agregar miembros
            await _permissions.RequireAdminAccess(currentUserId, workspaceId);

            // Verificar que el usuario a agregar existe
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            // Verificar que no es ya miembro
            var existingMember = await _context.UserWorkSpaces
                .FirstOrDefaultAsync(uw => uw.UserId == userId && uw.WorkSpaceId == workspaceId);

            if (existingMember != null)
            {
                throw new ConflictException("User is already a member of this workspace");
            }

            // Crear membresía
            var userWorkSpace = new UserWorkSpace
            {
                UserId = userId,
                WorkSpaceId = workspaceId,
                Role = Role.Member,
                JoinedAt = DateTime.UtcNow
            };

            await _context.UserWorkSpaces.AddAsync(userWorkSpace);
            await _context.SaveChangesAsync();

            return user.Adapt<UserResponse>();
        }

        public async Task RemoveMemberAsync(Guid workspaceId, Guid userId, Guid currentUserId)
        {
            // Obtener datos de ambos usuarios
            var currentUser = await _permissions.RequireMembership(currentUserId, workspaceId);
            var userToRemove = await _context.UserWorkSpaces
                .FirstOrDefaultAsync(uw => uw.UserId == userId && uw.WorkSpaceId == workspaceId);

            if (userToRemove == null)
            {
                throw new NotFoundException("User is not a member of this workspace");
            }

            // Validar permisos según el caso
            if (currentUserId != userId)
            {
                // Está eliminando a otro: necesita ser Owner o Admin
                if (currentUser.Role == Role.Member)
                {
                    throw new UnauthorizedException("Only workspace owners and admins can remove members");
                }

                // Admin no puede eliminar al Owner
                if (userToRemove.Role == Role.Owner && currentUser.Role == Role.Admin)
                {
                    throw new UnauthorizedException("Admins cannot remove the workspace owner");
                }
            }
            else
            {
                // Se está eliminando a sí mismo
                if (userToRemove.Role == Role.Owner)
                {
                    throw new UnauthorizedException("Workspace owner cannot leave the workspace. Transfer ownership first or delete the workspace.");
                }
            }

            _context.UserWorkSpaces.Remove(userToRemove);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<MemberResponse>> GetMembersAsync(Guid workspaceId, Guid currentUserId)
        {
            // Cualquier miembro puede ver la lista
            await _permissions.RequireMembership(currentUserId, workspaceId);

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
