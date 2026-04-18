using System;

namespace NaturalSpaceApi.Application.DTOs.Channel
{
    public sealed record UpdateChannelRequest
    (
        string? Name = null,
        bool? IsPrivate = null
    );
}
