using Microsoft.Extensions.Logging;
using Moq;
using Sentiment.API.Controllers;
using Xunit;

namespace Sentiment.API.Tests.Controllers
{
    public sealed class HealthCheckControllerTests
    {
        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
        {
            ILogger<HealthCheckController>? logger = null;
            Assert.Throws<ArgumentNullException>(() => new HealthCheckController(logger!));            
        }

        [Fact]
        public void Get_ReturnsSuccess()
        {
            Mock<ILogger<HealthCheckController>> loggerMock = new Mock<ILogger<HealthCheckController>>();

            HealthCheckController controller = new HealthCheckController(loggerMock.Object);

            Assert.Equal("success", controller.Get());
        }
    }
}