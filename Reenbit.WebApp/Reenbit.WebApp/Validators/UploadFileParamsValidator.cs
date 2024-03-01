using FluentValidation;
using Microsoft.AspNetCore.Components.Forms;
using Reenbit.WebApp.Models;

namespace Reenbit.WebApp.Services;

public class UploadFileParamsValidator: AbstractValidator<UploadFileRequest>
{
    public UploadFileParamsValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Invalid email format.");

        RuleFor(x => x.File)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .Must(IsFileValid)
            .WithMessage("Invalid file format.");
    }
    
    private bool IsFileValid(IBrowserFile file)
    {
        return file.ContentType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
    }
}