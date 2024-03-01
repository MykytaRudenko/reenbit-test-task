using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Components.Forms;
using Azure.Storage.Blobs.Models;
using Reenbit.WebApp.Models;

namespace Reenbit.WebApp.Services
{
    public class FileService : IFileService
    {
        private readonly BlobContainerClient _blobContainer;

        public FileService()
        {
            string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            string containerName = Environment.GetEnvironmentVariable("CONTAINER_NAME");
            _blobContainer = new BlobContainerClient(connectionString, containerName);
        }

        public FileService(BlobContainerClient blobContainer)
        {
            _blobContainer = blobContainer;
        }

        public async Task<Models.FileUploadResult> UploadAsync(string email, IBrowserFile file)
        {
            var uploadResult = new Models.FileUploadResult();
            
            var validator = new UploadFileParamsValidator();

            var validationResults = validator.Validate(new UploadFileRequest
            {
                Email = email,
                File = file
            });
            
            if (!validationResults.IsValid)
            {
                foreach (var failure in validationResults.Errors)
                {
                    uploadResult.Success = false;
                    uploadResult.ErrorMessage = failure.ErrorMessage;
                    return uploadResult;
                }
            }

            BlobClient client = _blobContainer.GetBlobClient(file.Name);
            IDictionary<string, string> metadata = new Dictionary<string, string>
            {
                { "UserEmail", email }
            };

            try
            {
                await using (Stream data = file.OpenReadStream())
                {
                    var response = await client.UploadAsync(data, new BlobUploadOptions
                    {
                        HttpHeaders = new BlobHttpHeaders { ContentType = "application/octet-stream" },
                        Metadata = metadata
                    });

                    uploadResult.FileUrl = client.Uri.AbsoluteUri;
                    uploadResult.Success = true;
                }
            }
            catch (Exception ex)
            {
                uploadResult.ErrorMessage = ex.Message;
                uploadResult.Success = false;
            }

            return uploadResult;
        }
    }

    public class FileUploadResult
    {
        public bool Success { get; set; }
        public string FileUrl { get; set; }
        public string ErrorMessage { get; set; }
    }
}