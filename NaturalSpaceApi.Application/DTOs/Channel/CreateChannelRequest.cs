using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalSpaceApi.Application.DTOs.Channel
{
    public sealed record CreateChannelRequest
    (
        string Name,
        bool IsPrivate = false
    );
}
