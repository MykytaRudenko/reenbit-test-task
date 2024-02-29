using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace Reenbit.WebApp.Services
{
    public class FileService
    {
        private readonly string _connectionString = "DefaultEndpointsProtocol=https;AccountName=reenbitblobstorage2286;AccountKey=73850Gxmw67HGPWL+3346DtISJQWBhw1MUgoEROyqNjY0QxGBd3jFjBioD2WUR63qVJElkB0ruRr+AStoRrCXA==;EndpointSuffix=core.windows.net";
        private readonly string _containerName = "files";
        
        private readonly BlobContainerClient _blobContainer;

        public FileService()
        {
            _blobContainer = new BlobContainerClient(_connectionString, _containerName);
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
