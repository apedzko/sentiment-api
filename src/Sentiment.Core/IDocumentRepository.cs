namespace Sentiment.Core
{
    public interface IDocumentRepository
    {
        Task<Document> CreateDocumentAsync(DocumentFile file);
    }
}
