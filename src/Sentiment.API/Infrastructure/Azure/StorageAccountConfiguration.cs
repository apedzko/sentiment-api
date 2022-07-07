
using System.Diagnostics.CodeAnalysis;

namespace Sentiment.API.Infrastructure.Azure
{
    [ExcludeFromCodeCoverage]
    public sealed class StorageAccountConfiguration
    {
        private string _name = null!;
        private string _sasToken = null!;

        public string Name
        {
            get => _name;
            private set
            {
                ArgumentNullException.ThrowIfNull(value);
                _name = value;
            }
        }
        public string SASToken
        {
            get => _sasToken;
            private set
            {
                ArgumentNullException.ThrowIfNull(value);
                _sasToken = value;
            }
        }
    }
}
