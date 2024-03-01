using FluentValidation;
using Reenbit.BlobTriggerFunc.Models;

namespace Reenbit.BlobTriggerFunc.Validators
{
    public class ParamsValidator : AbstractValidator<TriggerParams>
    {
        public ParamsValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required");

            RuleFor(x => x.Metadata)
                    .NotEmpty().WithMessage("Metadata is required.")
                    .Must(metadata => metadata.ContainsKey("UserEmail") && !string.IsNullOrEmpty(metadata["UserEmail"])).WithMessage("UserEmail is required in metadata.");

            RuleFor(x => x.Blob)
                .NotNull().WithMessage("Blob is required");

            RuleFor(x => x.Log)
                .NotNull().WithMessage("Logger is required");
        }
    }
}
