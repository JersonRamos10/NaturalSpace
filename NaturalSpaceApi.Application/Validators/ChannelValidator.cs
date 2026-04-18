using FluentValidation;
using NaturalSpaceApi.Application.DTOs.Channel;

namespace NaturalSpaceApi.Application.Validators
{
    public class CreateChannelValidator : AbstractValidator<CreateChannelRequest>
    {
        public CreateChannelValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Channel name cannot exceed 100 characters.");
        }
    }

    public class UpdateChannelValidator : AbstractValidator<UpdateChannelRequest>
    {
        public UpdateChannelValidator()
        {
            RuleFor(x => x.Name)
                .MaximumLength(100).WithMessage("Channel name cannot exceed 100 characters.")
                .When(x => !string.IsNullOrEmpty(x.Name));
        }
    }
}
