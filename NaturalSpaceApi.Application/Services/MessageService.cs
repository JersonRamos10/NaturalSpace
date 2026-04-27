using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using NaturalSpaceApi.Application.DTOs.Message;
using NaturalSpaceApi.Application.Exceptions;
using NaturalSpaceApi.Application.Interfaces;
using NaturalSpaceApi.Domain.Entities;
using NaturalSpaceApi.Infrastructure.Data.Context;

namespace NaturalSpaceApi.Application.Services
{
    public class MessageService : IMessageService
    {
        private readonly NaturalSpaceContext _context;
        private readonly IValidator<CreateMessageRequest> _createValidator;
        private readonly IValidator<UpdateMessageRequest> _updateValidator;
        private readonly IPermissionService _permissions;
        private readonly IFileStorageService _fileStorage;

        public MessageService(
            NaturalSpaceContext context,
            IValidator<CreateMessageRequest> createValidator,
            IValidator<UpdateMessageRequest> updateValidator,
            IPermissionService permissions,
            IFileStorageService fileStorage)
        {
            _context = context;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _permissions = permissions;
            _fileStorage = fileStorage;
        }

        public async Task<MessageResponse> SendMessageAsync(Guid channelId, CreateMessageRequest request, Guid userId)
        {
            var validationResult = await _createValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var channel = await _context.Channels
                .Include(c => c.Members)
                .Include(c => c.WorkSpace)
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
                    throw new UnauthorizedException("You are not a member of this private channel");
                }
            }

            var message = new Message
            {
                Id = Guid.NewGuid(),
                Content = request.Content ?? string.Empty,
                UserId = userId,
                ChannelId = channelId,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.Messages.AddAsync(message);

            var attachments = new List<Domain.Entities.File>();
            if (request.Attachments != null && request.Attachments.Count > 0)
            {
                foreach (var file in request.Attachments)
                {
                    var filePath = await _fileStorage.SaveFileAsync(message.Id, file);
                    attachments.Add(new Domain.Entities.File
                    {
                        Id = Guid.NewGuid(),
                        FileName = file.FileName,
                        FilePath = filePath,
                        UserId = userId,
                        MessageId = message.Id,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                await _context.Files.AddRangeAsync(attachments);
            }

            await _context.SaveChangesAsync();

            var savedMessage = await _context.Messages
                .Include(m => m.User)
                .Include(m => m.Attachments)
                .FirstAsync(m => m.Id == message.Id);

            return savedMessage.Adapt<MessageResponse>();
        }

        public async Task<IEnumerable<MessageResponse>> GetMessagesByChannelAsync(
            Guid channelId,
            Guid userId,
            int pageNumber,
            int pageSize)
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
                    throw new UnauthorizedException("You are not a member of this private channel");
                }
            }

            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;

            var messages = await _context.Messages
                .AsNoTracking()
                .Where(m => m.ChannelId == channelId && !m.IsDeleted)
                .Include(m => m.User)
                .Include(m => m.Attachments)
                .OrderByDescending(m => m.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var responses = messages.Select(m => m.Adapt<MessageResponse>());
            return responses;
        }

        public async Task<MessageResponse> UpdateMessageAsync(Guid messageId, UpdateMessageRequest request, Guid userId)
        {
            var validationResult = await _updateValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var message = await _context.Messages
                .Include(m => m.User)
                .Include(m => m.Attachments)
                .FirstOrDefaultAsync(m => m.Id == messageId && !m.IsDeleted);

            if (message == null)
            {
                throw new NotFoundException("Message not found");
            }

            if (message.UserId != userId)
            {
                throw new UnauthorizedException("Only the author can edit this message");
            }

            message.Content = request.Content;
            message.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return message.Adapt<MessageResponse>();
        }

        public async Task DeleteMessageAsync(Guid messageId, Guid userId)
        {
            var message = await _context.Messages
                .Include(m => m.Attachments)
                .FirstOrDefaultAsync(m => m.Id == messageId && !m.IsDeleted);

            if (message == null)
            {
                throw new NotFoundException("Message not found");
            }

            var channel = await _context.Channels
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == message.ChannelId);

            if (channel == null)
            {
                throw new NotFoundException("Channel not found");
            }

            bool canDelete = message.UserId == userId;

            if (!canDelete)
            {
                try
                {
                    await _permissions.RequireAdminAccess(userId, channel.WorkSpaceId);
                    canDelete = true;
                }
                catch (UnauthorizedException)
                {
                    canDelete = false;
                }
            }

            if (!canDelete)
            {
                throw new UnauthorizedException("Only the author or a workspace admin can delete this message");
            }

            message.MarkAsDeleted();

            foreach (var file in message.Attachments.ToList())
            {
                await _fileStorage.DeleteFileAsync(file.FilePath);
                _context.Files.Remove(file);
            }

            await _context.SaveChangesAsync();
        }

    }
}
