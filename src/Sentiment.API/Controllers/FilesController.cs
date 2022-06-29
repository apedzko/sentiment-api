using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Sentiment.API.Infrastructure;
using Azure;

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
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || string.IsNullOrEmpty(file.FileName))
                return BadRequest();

            string fileId = Guid.NewGuid().ToString();
            
            BlobServiceClient client = GetBlobServiceClient(_configuration.Name, _configuration.SASToken);

            BlobContainerClient container = await CreateContainerAsync(fileId, client);

            await UploadBlob(container, file.FileName, file.OpenReadStream());

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
            //TokenCredential credential = new DefaultAzureCredential();

            string blobUri = $"https://{accountName}.blob.core.windows.net";

            AzureSasCredential credentials = new AzureSasCredential(sasToken);

            return new BlobServiceClient(new Uri(blobUri), credentials);
        }
    }
}