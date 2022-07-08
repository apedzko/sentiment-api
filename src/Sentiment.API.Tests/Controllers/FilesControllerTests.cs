using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Sentiment.API.Controllers;
using Sentiment.API.Infrastructure.Azure;
using System.Text;
using Xunit;

namespace Sentiment.API.Tests.Controllers
{
    public sealed class FilesControllerTests
    {
        private const string FileName = "file.txt";

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
        {
            Mock<IBlobStorageClient> storageMock = GetStorageMock();

            Assert.Throws<ArgumentNullException>(() => new FilesController(null!, storageMock.Object));
        }        

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenBlobClientIsNull()
        {
            Mock<ILogger<FilesController>> loggerMock = GetLoggerMock();

            Assert.Throws<ArgumentNullException>(() => new FilesController(loggerMock.Object, null!));
        }
        

        [Fact]
        public async void UploadFile_ReturnsBadRequest_WhenFileIsNull()
        {
            FilesController controller = GetFilesController();

            var result = await controller.UploadFile(null!);

            Assert.Equal(400, ((StatusCodeResult)result).StatusCode);
        }       

        [Fact]
        public async void UploadFile_ReturnsBadRequest_WhenFileNameIsEmpty()
        {
            FilesController controller = GetFilesController();
            Mock<IFormFile> fileMock = new Mock<IFormFile>();

            var result = await controller.UploadFile(fileMock.Object);

            Assert.Equal(400, ((StatusCodeResult)result).StatusCode);
        }

        [Fact]
        public async void UploadFile_SavesUploadedFile_ToBlob()
        {
            Mock<ILogger<FilesController>> loggerMock = GetLoggerMock();
            
            Mock<IBlobStorageClient> storageMock = GetStorageMock();

            Mock<IFormFile> fileMock = new Mock<IFormFile>();
            
            ConfigureFileName(fileMock);

            using (Stream fileStream = ConfigureFileStream(fileMock))
            {
                FilesController controller = new FilesController(loggerMock.Object, storageMock.Object);

                await controller.UploadFile(fileMock.Object);

                storageMock.Verify(
                    x => x.UploadFileAsync(It.IsAny<string>(), FileName, fileStream));
            }

        }        

        [Fact]
        public async void UploadFile_ReturnsHttp201_WhenTheFileWasUploadedSuccessfully()
        {
            Mock<ILogger<FilesController>> loggerMock = GetLoggerMock();

            Mock<IBlobStorageClient> storageMock = GetStorageMock();

            Mock<IFormFile> fileMock = new Mock<IFormFile>();

            ConfigureFileName(fileMock);

            using (Stream fileStream = ConfigureFileStream(fileMock))
            {
                FilesController controller = new FilesController(loggerMock.Object, storageMock.Object);

                var result = await controller.UploadFile(fileMock.Object);

                Assert.Equal(201, ((StatusCodeResult)result).StatusCode);
            }
        }

        private static FilesController GetFilesController()
        {
            Mock<ILogger<FilesController>> loggerMock = GetLoggerMock();
            Mock<IBlobStorageClient> storageMock = GetStorageMock();

            FilesController controller = new FilesController(loggerMock.Object, storageMock.Object);
            return controller;
        }

        private static Stream ConfigureFileStream(Mock<IFormFile> fileMock)
        {
            Stream fileStream = new MemoryStream(Encoding.UTF8.GetBytes("Sample file content"));
            fileMock.Setup(x => x.OpenReadStream())
                    .Returns(fileStream);
            return fileStream;
        }

        private static void ConfigureFileName(Mock<IFormFile> fileMock)
        {
            fileMock.Setup(x => x.FileName).Returns(FileName);
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
