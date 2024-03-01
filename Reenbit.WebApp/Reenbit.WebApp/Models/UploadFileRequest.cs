using Microsoft.AspNetCore.Components.Forms;

namespace Reenbit.WebApp.Models;

public class UploadFileRequest
{
    public string Email { get; set; }
    public IBrowserFile File { get; set; }
}