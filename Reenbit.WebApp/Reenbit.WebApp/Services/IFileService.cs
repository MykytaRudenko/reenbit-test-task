using Azure.Storage.Blobs.Models;
using Azure;
using Microsoft.AspNetCore.Components.Forms;

namespace Reenbit.WebApp.Services
{
    public interface IFileService
    {
        Task<Response<BlobContentInfo>> UploadAsync(string email, IBrowserFile file);
    }
}
