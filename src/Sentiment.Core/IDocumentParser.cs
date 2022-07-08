namespace Sentiment.Core
{
    public interface IDocumentParser
    {
        Task<DocumentContent> ParseDocumentAsync(Document document);
    }
}
