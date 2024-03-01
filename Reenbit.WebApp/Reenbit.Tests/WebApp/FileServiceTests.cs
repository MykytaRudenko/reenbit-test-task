using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Components.Forms;
using Moq;
using Reenbit.WebApp.Services;
using Azure;
using Azure.Storage.Blobs.Models;

namespace Reenbit.Tests.WebApp
{
    public class FileServiceTests
    {
        [Fact]
        public async Task UploadAsync_Success()
        {
            // Arrange
            var mockBlobContainer = new Mock<BlobContainerClient>();
            var mockBlobClient = new Mock<BlobClient>();
            mockBlobContainer.Setup(x => x.GetBlobClient(It.IsAny<string>())).Returns(mockBlobClient.Object);
            var file = new Mock<IBrowserFile>();
            file.Setup(x => x.Name).Returns("test.docx");
            var response = new Mock<Response<BlobContentInfo>>();
            response.Setup(x => x.Value).Returns(new InvocationFunc());

            mockBlobClient.Setup(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<BlobUploadOptions>(), default)).ReturnsAsync(response.Object);
            var fileService = new FileService(mockBlobContainer.Object);

            // Act
            var result = await fileService.UploadAsync("test@example.com", file.Object);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal("https://example.com/test.docx", result.FileUrl); // Replace with actual URL
            Assert.Null(result.ErrorMessage);
        }

        [Fact]
        public async Task UploadAsync_Failure()
        {
            // Arrange
            var mockBlobContainer = new Mock<BlobContainerClient>();
            var mockBlobClient = new Mock<BlobClient>();
            mockBlobContainer.Setup(x => x.GetBlobClient(It.IsAny<string>())).Returns(mockBlobClient.Object);
            var file = new Mock<IBrowserFile>();
            file.Setup(x => x.Name).Returns("test.docx");

            mockBlobClient.Setup(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<BlobUploadOptions>(), default)).ThrowsAsync(new Exception());
            var fileService = new FileService(mockBlobContainer.Object);

            // Act
            var result = await fileService.UploadAsync("test@example.com", file.Object);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Null(result.FileUrl);
            Assert.NotNull(result.ErrorMessage);
        }
    }
}
