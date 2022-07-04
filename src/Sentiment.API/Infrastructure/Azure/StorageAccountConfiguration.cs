
namespace Sentiment.API.Infrastructure.Azure
{
    public sealed class StorageAccountConfiguration
    {
        private string _name = null!;
        private string _sasToken = null!;

        public string Name
        {
            get => _name;
            set
            {
                ArgumentNullException.ThrowIfNull(value);
                _name = value;
            }
        }
        public string SASToken
        {
            get => _sasToken;
            set
            {
                ArgumentNullException.ThrowIfNull(value);
                _sasToken = value;
            }
        }
    }
}
