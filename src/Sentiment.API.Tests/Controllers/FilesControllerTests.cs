using Microsoft.Extensions.Logging;
using Moq;
using Sentiment.API.Controllers;
using Sentiment.API.Infrastructure.Azure;
using Xunit;

namespace Sentiment.API.Tests.Controllers
{
    public sealed class FilesControllerTests
    {
        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
        {
            ILogger<FilesController>? logger = null;
            Mock<IBlobStorageClient> storageMock = GetStorageMock();

            Assert.Throws<ArgumentNullException>(() => new FilesController(logger!, storageMock.Object));
        }        

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenBlobClientIsNull()
        {
            Mock<ILogger<FilesController>> loggerMock = GetLoggerMock();
            IBlobStorageClient? storage = null;

            Assert.Throws<ArgumentNullException>(() => new FilesController(loggerMock.Object, storage!));
        }
        

        [Fact]
        public void UploadFile_ReturnsBadRequest_WhenFileIsNull()
        {
        }

        [Fact]
        public void UploadFile_ReturnsBadRequest_WhenFileNameIsEmpty()
        {
        }

        [Fact]
        public void UploadFile_SavesUploadedFile_ToBlob()
        {
        }

        [Fact]
        public void UploadFile_ReturnsHttp201_WhenTheFileWasUploadedSuccessfully()
        {
        }

        private static Mock<IBlobStorageClient> GetStorageMock()
        {
            return new Mock<IBlobStorageClient>();
        }

        private static Mock<ILogger<FilesController>> GetLoggerMock()
        {
            return new Mock<ILogger<FilesController>>();
        }
    }
}
