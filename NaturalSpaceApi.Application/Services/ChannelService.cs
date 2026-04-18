using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using NaturalSpaceApi.Application.DTOs.Channel;
using NaturalSpaceApi.Application.Exceptions;
using NaturalSpaceApi.Application.Interfaces;
using NaturalSpaceApi.Domain.Entities;
using NaturalSpaceApi.Domain.Enum;
using NaturalSpaceApi.Infrastructure.Data.Context;

namespace NaturalSpaceApi.Application.Services
{
    public class ChannelService : IChannelService
    {
        private readonly NaturalSpaceContext _context;
        private readonly IValidator<CreateChannelRequest> _createValidator;
        private readonly IValidator<UpdateChannelRequest> _updateValidator;

        public ChannelService(
            NaturalSpaceContext context,
            IValidator<CreateChannelRequest> createValidator,
            IValidator<UpdateChannelRequest> updateValidator)
        {
            _context = context;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<ChannelResponse> CreateChannelAsync(
            CreateChannelRequest channelRequest, 
            Guid workspaceId, 
            Guid userId)
        {
            // 1. Validar request
            var validationResult = await _createValidator.ValidateAsync(channelRequest);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            // 2. Verificar que el workspace existe
            var workspace = await _context.WorkSpaces
                .FirstOrDefaultAsync(w => w.Id == workspaceId && !w.IsDeleted) 
                        ?? throw new NotFoundException("Workspace not found");

            // 3. LÓGICA DE PERMISOS: Verificar que el usuario es Owner o Admin del workspace
            var userWorkspace = await _context.UserWorkSpaces
                .FirstOrDefaultAsync(uw => uw.UserId == userId && uw.WorkSpaceId == workspaceId);

            if (userWorkspace == null)
            {
                throw new UnauthorizedException("You are not a member of this workspace");
            }

            if (userWorkspace.Role == Role.Member)
            {
                throw new UnauthorizedException("Only Owner or Admin can create channels");
            }

            // 4. Crear el canal
            var channel = new Channel
            {
                Id = Guid.NewGuid(),
                Name = channelRequest.Name,
                IsPrivate = channelRequest.IsPrivate,
                WorkSpaceId = workspaceId,
                CreatedById = userId,
                IsDeleted = false
            };

            await _context.Channels.AddAsync(channel);
            await _context.SaveChangesAsync();

            return channel.Adapt<ChannelResponse>();
        }

        public async Task<ChannelResponse> UpdateChannelAsync(
            Guid channelId, 
            UpdateChannelRequest channelRequest,
            Guid userId)
        {
            // 1. Validar request
            var validationResult = await _updateValidator.ValidateAsync(channelRequest);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            // 2. Buscar el canal
            var channel = await _context.Channels
                .FirstOrDefaultAsync(c => c.Id == channelId && !c.IsDeleted);
                
            if (channel == null)
            {
                throw new NotFoundException("Channel not found");
            }

            // 3. LÓGICA DE PERMISOS: Verificar que el usuario es Owner o Admin del workspace
            var userWorkspace = await _context.UserWorkSpaces
                .FirstOrDefaultAsync(uw => uw.UserId == userId && uw.WorkSpaceId == channel.WorkSpaceId);

            if (userWorkspace == null)
            {
                throw new UnauthorizedException("You are not a member of this workspace");
            }

            if (userWorkspace.Role == Role.Member)
            {
                throw new UnauthorizedException("Only Owner or Admin can update channels");
            }

            // 4. Actualizar propiedades (solo si se proporcionan)
            if (!string.IsNullOrEmpty(channelRequest.Name))
            {
                channel.Name = channelRequest.Name;
            }

            if (channelRequest.IsPrivate.HasValue)
            {
                channel.IsPrivate = channelRequest.IsPrivate.Value;
            }

            await _context.SaveChangesAsync();

            return channel.Adapt<ChannelResponse>();
        }

        public async Task DeleteChannelAsync(Guid channelId, Guid userId)
        {
            // 1. Buscar el canal
            var channel = await _context.Channels
                .FirstOrDefaultAsync(c => c.Id == channelId && !c.IsDeleted);
                
            if (channel == null)
            {
                throw new NotFoundException("Channel not found");
            }

            // 2. LÓGICA DE PERMISOS: Verificar que el usuario es Owner o Admin del workspace
            var userWorkspace = await _context.UserWorkSpaces
                .FirstOrDefaultAsync(uw => uw.UserId == userId && uw.WorkSpaceId == channel.WorkSpaceId);

            if (userWorkspace == null)
            {
                throw new UnauthorizedException("You are not a member of this workspace");
            }

            if (userWorkspace.Role == Role.Member)
            {
                throw new UnauthorizedException("Only Owner or Admin can delete channels");
            }

            // 3. Soft delete
            channel.IsDeleted = true;
            channel.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task<ChannelResponse> GetByIdAsync(Guid channelId, Guid userId)
        {
            // 1. Buscar el canal
            var channel = await _context.Channels
                .Include(c => c.Members)
                .FirstOrDefaultAsync(c => c.Id == channelId && !c.IsDeleted);
                
            if (channel == null)
            {
                throw new NotFoundException("Channel not found");
            }

            // 2. Verificar que el usuario es miembro del workspace
            var userWorkspace = await _context.UserWorkSpaces
                .FirstOrDefaultAsync(uw => uw.UserId == userId && uw.WorkSpaceId == channel.WorkSpaceId);

            if (userWorkspace == null)
            {
                throw new UnauthorizedException("You are not a member of this workspace");
            }

            // VISIBILIDAD: Si es privado, verificar que es miembro del canal
            if (channel.IsPrivate)
            {
                var isChannelMember = channel.Members.Any(m => m.UserId == userId);
                if (!isChannelMember)
                {
                    throw new UnauthorizedException("This is a private channel. You must be a member to view it.");
                }
            }

            return channel.Adapt<ChannelResponse>();
        }

        public async Task<IEnumerable<ChannelResponse>> GetChannelsByWorkspaceAsync(
            Guid workspaceId, 
            Guid userId)
        {
            // 1. Verificar que el usuario es miembro del workspace
            var userWorkspace = await _context.UserWorkSpaces
                .FirstOrDefaultAsync(uw => uw.UserId == userId && uw.WorkSpaceId == workspaceId);

            if (userWorkspace == null)
            {
                throw new UnauthorizedException("You are not a member of this workspace");
            }

            // 2. Obtener todos los canales del workspace
            var channels = await _context.Channels
                .Include(c => c.Members)
                .Where(c => c.WorkSpaceId == workspaceId && !c.IsDeleted)
                .ToListAsync();

            //  VISIBILIDAD: Filtrar según privacidad
            var visibleChannels = channels.Where(c =>
            {
                // Canales públicos: visibles para todos
                if (!c.IsPrivate) 
                    return true;
                
                // Canales privados: solo visibles para miembros
                return c.Members.Any(m => m.UserId == userId);
            });

            return visibleChannels.Adapt<IEnumerable<ChannelResponse>>();
        }

        // Método helper para verificar permisos (puede usarse en el controller antes de llamar a UpdateChannelAsync)
        public async Task VerifyUserCanModifyChannel(Guid channelId, Guid userId)
        {
            var channel = await _context.Channels
                .FirstOrDefaultAsync(c => c.Id == channelId && !c.IsDeleted);
                
            if (channel == null)
            {
                throw new NotFoundException("Channel not found");
            }

            var userWorkspace = await _context.UserWorkSpaces
                .FirstOrDefaultAsync(uw => uw.UserId == userId && uw.WorkSpaceId == channel.WorkSpaceId);

            if (userWorkspace == null)
            {
                throw new UnauthorizedException("You are not a member of this workspace");
            }

            if (userWorkspace.Role == Role.Member)
            {
                throw new UnauthorizedException("Only Owner or Admin can modify channels");
            }
        }
    }
}
