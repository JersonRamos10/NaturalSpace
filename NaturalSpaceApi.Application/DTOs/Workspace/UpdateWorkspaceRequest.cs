using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalSpaceApi.Application.DTOs.Workspace
{
    public sealed record UpdateWorkspaceRequest
    (
        string? Name,
        string? Description

    );
    
}
