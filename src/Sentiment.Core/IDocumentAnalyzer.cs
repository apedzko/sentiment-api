namespace Sentiment.Core
{
    public interface IDocumentAnalyzer
    {
        Task<DocumentAnalysisResult> AnalyzeDocumentAsync(Document document);
    }
}
