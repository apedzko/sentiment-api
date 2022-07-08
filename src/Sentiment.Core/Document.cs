namespace Sentiment.Core
{
    public record Document(Guid Id, string Path, DocumentStatus Status);
}
