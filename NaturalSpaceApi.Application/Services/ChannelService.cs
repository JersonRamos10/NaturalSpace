using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using NaturalSpaceApi.Application.DTOs.Channel;
using NaturalSpaceApi.Application.Exceptions;
using NaturalSpaceApi.Application.Interfaces;
using NaturalSpaceApi.Domain.Entities;
using NaturalSpaceApi.Infrastructure.Data.Context;

namespace NaturalSpaceApi.Application.Services
{
    public class ChannelService : IChannelService
    {
        private readonly NaturalSpaceContext _context;
        private readonly IValidator<CreateChannelRequest> _createValidator;
        private readonly IValidator<UpdateChannelRequest> _updateValidator;
        private readonly IPermissionService _permissions;

        public ChannelService(
            NaturalSpaceContext context,
            IValidator<CreateChannelRequest> createValidator,
            IValidator<UpdateChannelRequest> updateValidator,
            IPermissionService permissions)
        {
            _context = context;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _permissions = permissions;
        }

        public async Task<ChannelResponse> CreateChannelAsync(
            CreateChannelRequest channelRequest,
            Guid workspaceId,
            Guid userId)
        {
            var validationResult = await _createValidator.ValidateAsync(channelRequest);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            await _permissions.RequireAdminAccess(userId, workspaceId);

            var workspace = await _context.WorkSpaces
                .FirstOrDefaultAsync(w => w.Id == workspaceId && !w.IsDeleted);

            if (workspace == null)
            {
                throw new NotFoundException("Workspace not found");
            }

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
            var validationResult = await _updateValidator.ValidateAsync(channelRequest);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var channel = await _context.Channels
                .FirstOrDefaultAsync(c => c.Id == channelId && !c.IsDeleted);

            if (channel == null)
            {
                throw new NotFoundException("Channel not found");
            }

            await _permissions.RequireAdminAccess(userId, channel.WorkSpaceId);

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
            var channel = await _context.Channels
                .FirstOrDefaultAsync(c => c.Id == channelId && !c.IsDeleted);

            if (channel == null)
            {
                throw new NotFoundException("Channel not found");
            }

            await _permissions.RequireAdminAccess(userId, channel.WorkSpaceId);

            channel.IsDeleted = true;
            channel.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task<ChannelResponse> GetByIdAsync(Guid channelId, Guid userId)
        {
            var channel = await _context.Channels
                .Include(c => c.Members)
                .FirstOrDefaultAsync(c => c.Id == channelId && !c.IsDeleted);

            if (channel == null)
            {
                throw new NotFoundException("Channel not found");
            }

            await _permissions.RequireMembership(userId, channel.WorkSpaceId);

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
            await _permissions.RequireMembership(userId, workspaceId);

            var channels = await _context.Channels
                .Include(c => c.Members)
                .Where(c => c.WorkSpaceId == workspaceId && !c.IsDeleted)
                .ToListAsync();

            var visibleChannels = channels.Where(c =>
            {
                if (!c.IsPrivate) return true;
                return c.Members.Any(m => m.UserId == userId);
            });

            return visibleChannels.Adapt<IEnumerable<ChannelResponse>>();
        }

        public async Task VerifyUserCanModifyChannel(Guid channelId, Guid userId)
        {
            var channel = await _context.Channels
                .FirstOrDefaultAsync(c => c.Id == channelId && !c.IsDeleted);

            if (channel == null)
            {
                throw new NotFoundException("Channel not found");
            }

            await _permissions.RequireAdminAccess(userId, channel.WorkSpaceId);
        }
    }
}
