using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Components.Forms;
using Moq;
using Reenbit.WebApp.Models;
using Reenbit.WebApp.Services;

namespace Reenbit.Tests.WebApp;

public class UploadFileParamsValidatorTests
{
[Fact]
    public void Validate_EmailIsNull_ExpectValidationError()
    {
        // Arrange
        var validator = new UploadFileParamsValidator();
        var request = new UploadFileRequest { Email = null, File = new Mock<IBrowserFile>().Object };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_EmailIsInvalidFormat_ExpectValidationError()
    {
        // Arrange
        var validator = new UploadFileParamsValidator();
        var request = new UploadFileRequest { Email = "invalidemail", File = new Mock<IBrowserFile>().Object };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email).WithErrorMessage("Invalid email format.");
    }

    [Fact]
    public void Validate_FileIsNull_ExpectValidationError()
    {
        // Arrange
        var validator = new UploadFileParamsValidator();
        var request = new UploadFileRequest { Email = "test@example.com", File = null };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.File);
    }

    [Fact]
    public void Validate_FileIsInvalidFormat_ExpectValidationError()
    {
        // Arrange
        var validator = new UploadFileParamsValidator();
        var file = new Mock<IBrowserFile>();
        file.Setup(x => x.ContentType).Returns("invalid/content-type");
        var request = new UploadFileRequest { Email = "test@example.com", File = file.Object };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.File).WithErrorMessage("Invalid file format.");
    }
}