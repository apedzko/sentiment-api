using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Azure;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
using Sentiment.API.Infrastructure.Azure;

namespace Sentiment.API.Controllers
{
    [ApiController]
    [Route("files")]
    public class FilesController : ControllerBase
    {
        private readonly ILogger<FilesController> _logger;
        private readonly IBlobStorageClient _blobClient;

        public FilesController(ILogger<FilesController> logger, IBlobStorageClient blobClient)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(blobClient);

            _logger = logger;
            _blobClient = blobClient;
        }


        /// <summary>
        /// Uploads a file for sentiment analysis.
        /// </summary>
        /// <param name="file">The file contents to upload.</param>
        /// <returns></returns>
        /// <response code="201">The file has been uploaded successfully.</response>
        /// <response code="400">The request validation has failed.</response>
        /// <response code="500">Unexpected server error has occured.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> UploadFile([Required]IFormFile file)
        {
            if (file == null || string.IsNullOrEmpty(file.FileName))
                return BadRequest();

            string containerName = Guid.NewGuid().ToString();

            _logger.LogDebug($"Uploading file {file.FileName} to container {containerName}");

            await _blobClient.UploadFileAsync(containerName, file.FileName, file.OpenReadStream());

            _logger.LogDebug($"Successfully uploaded {file.FileName} to container {containerName}");

            return StatusCode(201);                       
        }

        
    }
}