
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using Sentiment.API.Infrastructure.Middleware;
using System.Net;
using Xunit;

namespace Sentiment.API.Tests.Infrastructure.Middleware
{
    public sealed class ApiExceptionHandlingMiddlewareTests
    {
        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenRequestDelegateIsNull()
        {
            Mock<ILogger<ApiExceptionHandlingMiddleware>> loggerMock = GetLoggerMock();
            Mock<IActionResultExecutor<ObjectResult>> executorMock = GetExecutorMock();

            Assert.Throws<ArgumentNullException>(() => new ApiExceptionHandlingMiddleware(null!, loggerMock.Object, executorMock.Object));
        }        

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
        {
            Mock<IActionResultExecutor<ObjectResult>> executorMock = GetExecutorMock();
            RequestDelegate mockNextMiddleware = GetMockMiddleware();

            Assert.Throws<ArgumentNullException>(() => new ApiExceptionHandlingMiddleware(mockNextMiddleware, null!, executorMock.Object));
        }        

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenRequestExecutorIsNull()
        {
            RequestDelegate mockNextMiddleware = GetMockMiddleware();
            Mock<ILogger<ApiExceptionHandlingMiddleware>> loggerMock = GetLoggerMock();

            Assert.Throws<ArgumentNullException>(() => new ApiExceptionHandlingMiddleware(mockNextMiddleware, loggerMock.Object, null!));
        }

        [Fact]
        public async void Middleware_ReturnsHttp500_WhenExceptionIsThrown()
        {
            Mock<IActionResultExecutor<ObjectResult>> executorMock = await ExecuteRequestWithExceptionThrown();

            executorMock.Verify(x => x.ExecuteAsync(It.IsAny<ActionContext>(),
                It.Is<ObjectResult>(r => r.StatusCode == (int)HttpStatusCode.InternalServerError)));
        }        

        [Fact]
        public async void Middleware_CapturesValidProblemDetails_WhenExceptionIsThrown()
        {
            Mock<IActionResultExecutor<ObjectResult>> executorMock = await ExecuteRequestWithExceptionThrown();

            executorMock.Verify(x => x.ExecuteAsync(It.IsAny<ActionContext>(),
                It.Is<ObjectResult>((r) => VerifyProblemDetails(r))));
        }

        [Fact]
        public async void Middleware_LogsException_WhenExceptionIsThrown()
        {
            Mock<ILogger<ApiExceptionHandlingMiddleware>> loggerMock = GetLoggerMock();
            Exception expectedException = new InvalidOperationException();

            await ExecuteRequestWithExceptionThrown(expectedException, loggerMock);

            loggerMock.Verify(logger => logger.Log(
                                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                                It.Is<EventId>(eventId => eventId.Id == 0),
                                It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == "An unhandled exception has occurred."),
                                expectedException,
                                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                                Times.Once);
        }

        private bool VerifyProblemDetails(ObjectResult result)
        {
            Assert.NotNull(result.Value);

            ProblemDetails? details = result.Value as ProblemDetails;

            Assert.NotNull(details);

            Assert.Equal("https://tools.ietf.org/html/rfc7231#section-6.6.1", details?.Type);
            Assert.Equal("Internal Server Error", details?.Title);
            Assert.Equal("Internal server error occured.", details?.Detail);
            Assert.Equal("/", details?.Instance);

            return true;
        }

        private static async Task<Mock<IActionResultExecutor<ObjectResult>>> ExecuteRequestWithExceptionThrown(Exception? expectedException = null, Mock<ILogger<ApiExceptionHandlingMiddleware>>? loggerMock = null)
        {
            if(expectedException == null)
                expectedException = new InvalidOperationException();

            RequestDelegate mockNextMiddleware = (HttpContext) =>
            {
                return Task.FromException(expectedException);
            };

            if(loggerMock == null)
                loggerMock = GetLoggerMock();

            Mock<IActionResultExecutor<ObjectResult>> executorMock = GetExecutorMock();

            ApiExceptionHandlingMiddleware middleware = new ApiExceptionHandlingMiddleware(mockNextMiddleware, loggerMock.Object, executorMock.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = "/";

            await middleware.Invoke(httpContext);
            return executorMock;
        }

        private static RequestDelegate GetMockMiddleware()
        {
            return (HttpContext) =>
            {
                return Task.FromResult("result");
            };
        }

        private static Mock<IActionResultExecutor<ObjectResult>> GetExecutorMock()
        {
            return new Mock<IActionResultExecutor<ObjectResult>>();
        }

        private static Mock<ILogger<ApiExceptionHandlingMiddleware>> GetLoggerMock()
        {
            return new Mock<ILogger<ApiExceptionHandlingMiddleware>>();
        }
    }
}
