namespace Reenbit.WebApp.Services;

public interface IBlobContainerClient
{
    IBlobClient GetBlobClient(string blobName);
}