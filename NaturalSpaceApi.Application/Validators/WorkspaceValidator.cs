using FluentValidation;
using NaturalSpaceApi.Application.DTOs.Workspace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalSpaceApi.Application.Validators
{
    public class WorkspaceValidator : AbstractValidator<CreateWorkSpaceRequest>
    {

        public WorkspaceValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("The workspace name cannot exceed 100 characters.");
        }
    }
}
