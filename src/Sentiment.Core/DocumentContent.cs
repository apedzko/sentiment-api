namespace Sentiment.Core
{
    public record DocumentContent(Guid DocumentId, ICollection<string> lines);
}
