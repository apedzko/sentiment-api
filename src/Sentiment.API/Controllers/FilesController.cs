using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Sentiment.API.Infrastructure;
using Azure;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations;

namespace Sentiment.API.Controllers
{
    [ApiController]
    [Route("files")]
    public class FilesController : ControllerBase
    {
        private readonly ILogger<FilesController> _logger;
        private readonly StorageAccountOptions _configuration;

        public FilesController(ILogger<FilesController> logger, StorageAccountOptions configuration)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(configuration);

            _logger = logger;
            _configuration = configuration;
        }


        /// <summary>
        /// Uploads a file for sentiment analysis.
        /// </summary>
        /// <param name="file">The file contents to upload.</param>
        /// <returns></returns>
        /// <response code="201">The file has been uploaded successfully.</response>
        /// <response code="400">The request validation has failed.</response>
        /// <response code="500">Unexpected server error has occured.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadFile([Required]IFormFile file)
        {
            if (file == null || string.IsNullOrEmpty(file.FileName))
                return BadRequest();

            string containerName = Guid.NewGuid().ToString();

            _logger.LogDebug($"Uploading file {file.FileName} to container {containerName}");

            BlobServiceClient client = GetBlobServiceClient(_configuration.Name, _configuration.SASToken);

            BlobContainerClient container = await CreateContainerAsync(containerName, client);

            await UploadBlob(container, file.FileName, file.OpenReadStream());

            _logger.LogDebug($"Successfully uploaded {file.FileName} to container {containerName}");

            return StatusCode(201);                       
        }

        public static async Task UploadBlob(BlobContainerClient containerClient, string fileName, Stream fileStream)
        {
            BlockBlobClient blockBlobClient = containerClient.GetBlockBlobClient(fileName);

            using (Stream blobStream = await blockBlobClient.OpenWriteAsync(true))
            {                
                using (fileStream)
                {
                   
                    await fileStream.CopyToAsync(blobStream);
                }                
            }
        }

        private static async Task<BlobContainerClient> CreateContainerAsync(string containerName, BlobServiceClient client)
        {
            BlobContainerClient container =  await client.CreateBlobContainerAsync(containerName);
            
            if(await container.ExistsAsync())
                return container;

            throw new InvalidOperationException($"Failed to create container {containerName}");
        }

        private static BlobServiceClient GetBlobServiceClient(string accountName, string sasToken)
        {
            string blobUri = $"https://{accountName}.blob.core.windows.net";

            AzureSasCredential credentials = new AzureSasCredential(sasToken);

            return new BlobServiceClient(new Uri(blobUri), credentials);
        }
    }
}