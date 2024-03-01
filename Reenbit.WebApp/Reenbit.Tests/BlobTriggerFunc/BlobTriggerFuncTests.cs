using Microsoft.Extensions.Logging;
using Moq;
using Reenbit.BlobTriggerFunc;

namespace Reenbit.Tests.BlobTriggerFunc;

public class BlobTriggerFuncTests
{
    [Fact]
    public void Run_ValidBlob_ExpectNoErrors()
    {
        // Arrange
        var mockBlob = new Mock<Stream>();
        var mockLogger = new Mock<ILogger>();
        var name = "test.txt";
        var metadata = new Dictionary<string, string>
        {
            { "UserEmail", "test@example.com" }
        };

        var function = new BlobTriggerFunction();

        // Act
        function.Run(mockBlob.Object, name, metadata, mockLogger.Object);

        // Assert
        // If no exception is thrown, then the test passes.
    }
}