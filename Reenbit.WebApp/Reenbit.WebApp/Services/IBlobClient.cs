using Azure;
using Azure.Storage.Blobs.Models;

namespace Reenbit.WebApp.Services;

public interface IBlobClient
{
    Task<Response<BlobContentInfo>> UploadAsync(Stream data, BlobUploadOptions options, System.Threading.CancellationToken cancellationToken);
}