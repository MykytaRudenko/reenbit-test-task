using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Net;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Reenbit.BlobTriggerFunc.Validators;
using Azure.Security.KeyVault.Secrets;
using Azure.Identity;
using Microsoft.Extensions.Configuration;

namespace Reenbit.BlobTriggerFunc
{
    public class Function1
    {
        private const string _connectionString = "DefaultEndpointsProtocol=https;AccountName=reenbitblobstorage2286;AccountKey=73850Gxmw67HGPWL+3346DtISJQWBhw1MUgoEROyqNjY0QxGBd3jFjBioD2WUR63qVJElkB0ruRr+AStoRrCXA==;BlobEndpoint=https://reenbitblobstorage2286.blob.core.windows.net/;TableEndpoint=https://reenbitblobstorage2286.table.core.windows.net/;QueueEndpoint=https://reenbitblobstorage2286.queue.core.windows.net/;FileEndpoint=https://reenbitblobstorage2286.file.core.windows.net/";
        private readonly string _containerName = "files";
        private const uint SAS_TOKEN_EXPIRY_IN_HOURS = 1;

        [FunctionName("SendEmailFunction")]
        public void Run([BlobTrigger("files/{name}", 
            Connection = "AzureWebJobsStorage")]Stream myBlob, 
            string name,
            IDictionary<string, string> metadata,
            ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

            var validator = new ParamsValidator();
            var validationResults = validator.Validate(new Models.TriggerParams
            {
                Name = name,
                Metadata = metadata,
            });

            if (!validationResults.IsValid)
            {
                foreach (var failure in validationResults.Errors)
                {
                    log.LogError($"Function can not be executed. {failure.ErrorMessage}");
                }
                return;
            }

            var blobContainerClient = new BlobContainerClient(_connectionString, _containerName);

            var client = blobContainerClient.GetBlobClient(name);
            
            var fileUrlWithSas = GetBlobSasToken(client, (int)SAS_TOKEN_EXPIRY_IN_HOURS);

            string userEmail = metadata["UserEmail"];

            SendEmail(userEmail, name, fileUrlWithSas, log);
        }

        private string GetBlobSasToken(BlobClient blobClient, int expiryInHours)
        {
            var blobSasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = blobClient.BlobContainerName,
                BlobName = blobClient.Name,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(expiryInHours),
                Protocol = SasProtocol.Https
            };

            blobSasBuilder.SetPermissions(BlobSasPermissions.Read);

            return blobClient.GenerateSasUri(blobSasBuilder).ToString();
        }

        private void SendEmail(string userEmail, string blobName, string sasLink, ILogger log)
        {
            string senderEmail = "mykyta.rudenko04@gmail.com";
            string senderPassword = "xbLrT4aHAVDsK7c0";

            var smtpClient = new SmtpClient("smtp-relay.brevo.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(senderEmail, senderPassword)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail),
                Subject = "Reenbit Test Task Sender",
                Body = $"Your file {blobName} have been successfuly uploaded to blob storage \n Here is link with SAS than will be availible 1 hour: \n {sasLink}",
                IsBodyHtml = true
            };

            mailMessage.To.Add(userEmail);

            smtpClient.SendMailAsync(mailMessage);

            log.LogError($"Email sent successful");
        }
    }
}
