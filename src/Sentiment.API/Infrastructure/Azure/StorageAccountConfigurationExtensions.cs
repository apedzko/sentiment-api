namespace Sentiment.API.Infrastructure.Azure
{
    public static class StorageConfigurationExtensions
    {
        public static WebApplicationBuilder AddStorageConfiguration(this WebApplicationBuilder app) 
        {
            ArgumentNullException.ThrowIfNull(app);

            StorageAccountConfiguration options = new StorageAccountConfiguration();
            app.Configuration.GetSection("StorageAccount").Bind(options, c => c.BindNonPublicProperties = true);
            app.Services.AddSingleton(options);

            app.Services.AddScoped<IBlobStorageClient, BlobStorageClient>();

            return app;
        }
    }
}
