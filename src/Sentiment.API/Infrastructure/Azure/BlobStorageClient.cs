using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Sentiment.API.Infrastructure.Helpers;
using System.Diagnostics.CodeAnalysis;

namespace Sentiment.API.Infrastructure.Azure
{
    [ExcludeFromCodeCoverage]
    public sealed class BlobStorageClient : IBlobStorageClient
    {
        private readonly StorageAccountConfiguration _configuration;

        public BlobStorageClient(StorageAccountConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);
            _configuration = configuration;
        }

        public async Task UploadFileAsync(string containerName, string fileName, Stream fileContent)
        {
            Guard.ThrowIfNullOrEmpty(containerName, nameof(containerName));
            Guard.ThrowIfNullOrEmpty(fileName, nameof(fileName));
            ArgumentNullException.ThrowIfNull(fileContent);

            BlobServiceClient client = GetBlobServiceClient(_configuration.Name, _configuration.SASToken);

            BlobContainerClient container = await CreateContainerAsync(containerName, client);

            await UploadBlob(container, fileName, fileContent);
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
            BlobContainerClient container = await client.CreateBlobContainerAsync(containerName);

            if (await container.ExistsAsync())
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
