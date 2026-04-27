using FluentValidation;
using NaturalSpaceApi.Application.DTOs.Message;

namespace NaturalSpaceApi.Application.Validators
{
    public class CreateMessageValidator : AbstractValidator<CreateMessageRequest>
    {
        public CreateMessageValidator()
        {
            RuleFor(x => x)
                .Must(HasContentOrAttachments)
                .WithMessage("A message must have text content or at least one attachment.");
        }

        private static bool HasContentOrAttachments(CreateMessageRequest request)
        {
            bool hasContent = !string.IsNullOrWhiteSpace(request.Content);
            bool hasAttachments = request.Attachments != null && request.Attachments.Count > 0;
            return hasContent || hasAttachments;
        }
    }

    public class UpdateMessageValidator : AbstractValidator<UpdateMessageRequest>
    {
        public UpdateMessageValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Message content cannot be empty.")
                .MaximumLength(4000).WithMessage("Message content cannot exceed 4000 characters.");
        }
    }
}
