using FluentValidation;
using Microsoft.Identity.Client;
using Reenbit.BlobTriggerFunc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reenbit.BlobTriggerFunc.Validators
{
    internal class ParamsValidator : AbstractValidator<TriggerParams>
    {
        public ParamsValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required");

            RuleFor(x => x.Metadata)
                    .NotEmpty().WithMessage("Metadata is required.")
                    .Must(metadata => metadata.ContainsKey("UserEmail")).WithMessage("UserEmail is required in metadata.");
        }
    }
}
