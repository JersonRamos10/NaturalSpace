using NaturalSpaceApi.Application.DTOs.Channel;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalSpaceApi.Application.Interfaces
{
    public interface IChannelService
    {

        Task<ChannelResponse> CreateChannelAsync (CreateChannelRequest channelRequest, Guid workspaceId, Guid userId);

        Task DeleteChannelAsync(Guid channelId);

        Task<ChannelResponse> GetByIdAsync(Guid channelId);


        Task<ChannelResponse> UpdateChannelAsync (Guid channelId, UpdateChannelRequest channelRequest);
    }
}
