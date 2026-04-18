using System;

namespace NaturalSpaceApi.Application.DTOs.Channel
{
    public sealed record ChannelResponse
    (
        Guid Id,
        string Name,
        bool IsPrivate,
        DateTime CreatedAt,
        Guid CreatedById,
        Guid WorkSpaceId
    );
}
