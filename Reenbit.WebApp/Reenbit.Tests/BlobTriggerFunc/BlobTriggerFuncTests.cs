using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Reenbit.BlobTriggerFunc;

namespace Reenbit.Tests.BlobTriggerFunc
{
    public class BlobTriggerFunctionTests
    {
        [Fact]
        public void Run_ValidParameters_ExpectSendEmail()
        {
            var loggerMock = new Mock<ILogger<BlobTriggerFunction>>();
            loggerMock.Setup(x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()
                )
            );

            var blobServiceClientMock = new Mock<BlobServiceClient>();
            var blobContainerClientMock = new Mock<BlobContainerClient>();
            var blobClientMock = new Mock<BlobClient>();
            var configurationMock = new Mock<IConfiguration>();
            
            configurationMock.Setup(x => x["WEBSITE_CONTENTAZUREFILECONNECTIONSTRING"]).Returns("DefaultEndpointsProtocol=https;AccountName=testtest;AccountKey=73850Gxmw67HGPWL+3346DtISJQWBhw1MUgoEROyqNjY0QxGBd3jFjBioD2WUR63qVJElkB0ruRr+AStoRrCXA==;EndpointSuffix=core.windows.net");
            configurationMock.Setup(x => x["WEBSITE_CONTENTSHARE"]).Returns("files");
            configurationMock.Setup(x => x["SENDER_EMAIL"]).Returns("test@test.com");
            configurationMock.Setup(x => x["SENDER_PASSWORD"]).Returns("testTest042");
            configurationMock.Setup(x => x["SENDER_URI"]).Returns("smtp-relay.brevo.com");
            configurationMock.Setup(x => x["SENDER_PORT"]).Returns("245");

            blobServiceClientMock.Setup(c => c.GetBlobContainerClient(It.IsAny<string>())).Returns(blobContainerClientMock.Object);
            blobContainerClientMock.Setup(c => c.GetBlobClient(It.IsAny<string>())).Returns(blobClientMock.Object);
            blobClientMock.Setup(c => c.Uri).Returns(new Uri("https://example.blob.core.windows.net/testfile.txt"));
            blobClientMock.Setup(c => c.GenerateSasUri(It.IsAny<BlobSasBuilder>())).Returns(new Uri("https://example.blob.core.windows.net/testfile.txt?sasToken"));
            
            var sendEmailFunction = new BlobTriggerFunction(configurationMock.Object);

            // Act
            sendEmailFunction.Run(new MemoryStream(), "testfile.txt",
                new Dictionary<string, string> { { "UserEmail", "test@test.com" } }, loggerMock.Object);

            // Assert
            loggerMock.Verify(m => m.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.AtLeastOnce);

            loggerMock.Verify(m => m.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Never);
        }
    }
}
