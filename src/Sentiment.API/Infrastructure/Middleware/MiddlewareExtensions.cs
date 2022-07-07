using System.Diagnostics.CodeAnalysis;

namespace Sentiment.API.Infrastructure.Middleware
{
    [ExcludeFromCodeCoverage]
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseApiExceptionHandling(this IApplicationBuilder app)
            => app.UseMiddleware<ApiExceptionHandlingMiddleware>();
    }
}
