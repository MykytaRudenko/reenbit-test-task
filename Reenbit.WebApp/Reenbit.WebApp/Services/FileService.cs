using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace Reenbit.WebApp.Services
{
    public class FileService
    {
        private readonly BlobContainerClient _blobContainer;

        public FileService()
        {
            string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            string containerName = Environment.GetEnvironmentVariable("CONTAINER_NAME");
            _blobContainer = new BlobContainerClient(connectionString, containerName);
        }

        public async Task<Response<BlobContentInfo>> UploadAsync(string email, IBrowserFile file)
        {
            Response<BlobContentInfo> result;
            
            BlobClient client = _blobContainer.GetBlobClient(file.Name);
            IDictionary<string, string> metadata = new Dictionary<string, string>
            {
                { "UserEmail", email }
            };

            try
            {
                await using (Stream data = file.OpenReadStream())
                {
                    result = await client.UploadAsync(data, new BlobUploadOptions
                    {
                        HttpHeaders = new BlobHttpHeaders { ContentType = "application/octet-stream" },
                        Metadata = metadata
                    });
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return result;
        }
    }
}
