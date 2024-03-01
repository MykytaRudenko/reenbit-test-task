using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Net;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Reenbit.BlobTriggerFunc.Validators;

namespace Reenbit.BlobTriggerFunc
{
    public class BlobTriggerFunction
    {
        private const uint SAS_TOKEN_EXPIRY_IN_HOURS = 1;

        private readonly IConfiguration _configuration;

        public BlobTriggerFunction(IConfiguration configuration)
        {
            _configuration = configuration;
        }

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
                Blob = myBlob,
                Log = log
            });

            if (!validationResults.IsValid)
            {
                foreach (var failure in validationResults.Errors)
                {
                    log.LogError($"Function can not be executed. {failure.ErrorMessage}");
                }
                return;
            }
            
            var connectionString = Environment.GetEnvironmentVariable("WEBSITE_CONTENTAZUREFILECONNECTIONSTRING") ?? _configuration["WEBSITE_CONTENTAZUREFILECONNECTIONSTRING"];
            var containerName = Environment.GetEnvironmentVariable("WEBSITE_CONTENTSHARE") ?? _configuration["WEBSITE_CONTENTSHARE"];

            var blobContainerClient = new BlobContainerClient(connectionString, containerName);

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
            string senderEmail = Environment.GetEnvironmentVariable("SENDER_EMAIL") ?? _configuration["SENDER_EMAIL"];
            string senderPassword = Environment.GetEnvironmentVariable("SENDER_PASSWORD") ?? _configuration["SENDER_PASSWORD"];
            string senderUri = Environment.GetEnvironmentVariable("SENDER_URI") ?? _configuration["SENDER_URI"];
            int senderPort;
            if(!int.TryParse(Environment.GetEnvironmentVariable("SENDER_PORT"), out senderPort))
            {
                senderPort = int.Parse(_configuration["SENDER_PORT"]);
            }

            var smtpClient = new SmtpClient(senderUri, senderPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(senderEmail, senderPassword)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail),
                Subject = "Reenbit Test Task Sender",
                Body = $"Your file have been successfuly uploaded to blob storage \n Here is link with SAS than will be availible 1 hour: \n <a href=\"{sasLink}\">{blobName}</a>",
                IsBodyHtml = true
            };

            mailMessage.To.Add(userEmail);

            smtpClient.SendMailAsync(mailMessage);

            log.LogInformation($"Email sent successful");
        }
    }
}
