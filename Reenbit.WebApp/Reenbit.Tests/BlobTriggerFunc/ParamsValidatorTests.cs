using FluentValidation.TestHelper;
using Microsoft.Extensions.Logging.Abstractions;
using Reenbit.BlobTriggerFunc;
using Reenbit.BlobTriggerFunc.Models;
using Reenbit.BlobTriggerFunc.Validators;

namespace Reenbit.Tests.BlobTriggerFunc;

public class ParamsValidatorTests
{
    [Fact]
    public void Validate_InvalidName_ExpectValidationError()
    {
        // Arrange
        var validator = new ParamsValidator();
        var metadata = new Dictionary<string, string>
        {
            { "UserEmail", "test@example.com" }
        };
        var triggerParams = new TriggerParams
        {
            Name = null,
            Metadata = metadata,
            Blob = new MemoryStream(),
            Log = new NullLogger<BlobTriggerFunction>()
        };

        // Act
        var validationResult = validator.TestValidate(triggerParams);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Name is required");
    }

    [Fact]
    public void Validate_MetadataIsEmpty_ExpectValidationError()
    {
        // Arrange
        var validator = new ParamsValidator();
        var metadata = new Dictionary<string, string>();
        var triggerParams = new TriggerParams
        {
            Name = "exampleName",
            Metadata = metadata,
            Blob = new MemoryStream(),
            Log = new NullLogger<BlobTriggerFunction>()
        };
        // Act
        var result = validator.TestValidate(triggerParams);
        
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Metadata)
            .WithErrorMessage("Metadata is required.");
    }

    [Fact]
    public void Validate_MetadataUserEmailKeyDoesntExist_ExpectValidationError()
    {
        // Arrange
        var validator = new ParamsValidator();
        var metadata = new Dictionary<string, string>
        {
            { "test", "test" }
        };
        var triggerParams = new TriggerParams
        {
            Name = "exampleName",
            Metadata = metadata,
            Blob = new MemoryStream(),
            Log = new NullLogger<BlobTriggerFunction>()
        };
        
        // Act
        var result = validator.TestValidate(triggerParams);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Metadata)
            .WithErrorMessage("UserEmail is required in metadata.");
    }
    
    [Fact]
    public void Validate_MetadataUserEmailIsEmpty_ExpectValidationError()
    {
        // Arrange
        var validator = new ParamsValidator();
        var metadata = new Dictionary<string, string>
        {
            { "UserEmail", "" }
        };
        var triggerParams = new TriggerParams
        {
            Name = "exampleName",
            Metadata = metadata,
            Blob = new MemoryStream(),
            Log = new NullLogger<BlobTriggerFunction>()
        };
        
        // Act
        var result = validator.TestValidate(triggerParams);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Metadata)
            .WithErrorMessage("UserEmail is required in metadata.");
    }

    [Fact]
    public void Validate_LoggerIsNull_ExpectValidationError()
    {
        // Arrange
        var validator = new ParamsValidator();
        var metadata = new Dictionary<string, string>
        {
            { "UserEmail", "test@test.com" }
        };
        var triggerParams = new TriggerParams
        {
            Name = "exampleName",
            Metadata = metadata,
            Blob = new MemoryStream(),
            Log = null
        };
        
        // Act
        var result = validator.TestValidate(triggerParams);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Log)
            .WithErrorMessage("Logger is required");
    }

    [Fact]
    public void Validate_BlobIsNull_ExpectValidationError()
    {
        // Arrange
        var validator = new ParamsValidator();
        var metadata = new Dictionary<string, string>
        {
            { "UserEmail", "test@test.com" }
        };
        var triggerParams = new TriggerParams
        {
            Name = "exampleName",
            Metadata = metadata,
            Blob = null,
            Log = new NullLogger<BlobTriggerFunction>()
        };
        
        // Act
        var result = validator.TestValidate(triggerParams);
        
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Blob)
            .WithErrorMessage("Blob is required");
    }
}