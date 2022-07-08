namespace Sentiment.Core
{
    public interface IDocumentAnalysisResultRepository
    {
        Task CreateAsync(DocumentAnalysisResult result);
    }
}
