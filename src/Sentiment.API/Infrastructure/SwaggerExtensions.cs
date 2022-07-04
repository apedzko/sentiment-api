using System.Reflection;

namespace Sentiment.API.Infrastructure
{
    public static class SwaggerExtensions
    {
        public static void AddSwagger(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services);

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options => {
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });
        }

        public static void UseSwagger(this IApplicationBuilder application, IWebHostEnvironment environment)
        {
            ArgumentNullException.ThrowIfNull(application);
            ArgumentNullException.ThrowIfNull(environment);

            if (environment.IsDevelopment())
            {
                application.UseSwagger();
                application.UseSwaggerUI();
            }
        }
    }
}
