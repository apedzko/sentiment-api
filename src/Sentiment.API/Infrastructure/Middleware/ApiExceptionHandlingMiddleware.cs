using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Net;
using System.Text.Json;

namespace Sentiment.API.Infrastructure.Middleware
{
    public sealed class ApiExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiExceptionHandlingMiddleware> _logger;
        private static readonly ActionDescriptor EmptyActionDescriptor = new();
        private static readonly RouteData EmptyRouteData = new();
        private readonly IActionResultExecutor<ObjectResult> _executor;

        public ApiExceptionHandlingMiddleware(RequestDelegate next, ILogger<ApiExceptionHandlingMiddleware> logger, IActionResultExecutor<ObjectResult> executor)
        {
            _next = next;
            _logger = logger;
            _executor = executor;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            _logger.LogError(ex, $"An unhandled exception has occurred, {ex.Message}");

            var problemDetails = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Title = "Internal Server Error",
                Status = (int)HttpStatusCode.InternalServerError,
                Instance = context.Request.Path,
                Detail = "Internal server error occured!"
            };

            var routeData = context.GetRouteData() ?? EmptyRouteData;

            var actionContext = new ActionContext(context, routeData, EmptyActionDescriptor);

            var result = new ObjectResult(problemDetails)
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
            };

            await _executor.ExecuteAsync(actionContext, result);

            await context.Response.CompleteAsync();
        }
    }
}
