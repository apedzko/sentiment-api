namespace Sentiment.Core
{
    public record DocumentAnalysisResult(Guid DocumentId, string Language, string Summary);
}
