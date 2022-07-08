namespace Sentiment.API.Infrastructure.Azure
{
    public interface IBlobStorageClient
    {
        public Task UploadFileAsync(string containerName, string fileName, Stream fileContent);
    }
}
