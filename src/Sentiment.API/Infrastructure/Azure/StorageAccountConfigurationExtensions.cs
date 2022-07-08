using System.Diagnostics.CodeAnalysis;

namespace Sentiment.API.Infrastructure.Azure
{
    [ExcludeFromCodeCoverage]
    public static class StorageConfigurationExtensions
    {
        public static void AddStorageConfiguration(this IServiceCollection services, IConfiguration configuration) 
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configuration);

            StorageAccountConfiguration options = new StorageAccountConfiguration();
            configuration.GetSection("StorageAccount").Bind(options, c => c.BindNonPublicProperties = true);
            services.AddSingleton(options);

            services.AddScoped<IBlobStorageClient, BlobStorageClient>();
        }
    }
}
