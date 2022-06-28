using Microsoft.AspNetCore.Mvc;

namespace Sentiment.API.Controllers
{
    [ApiController]
    [Route("health-check")]
    public class HealthCheckController : ControllerBase
    {
        private readonly ILogger<HealthCheckController> _logger;

        public HealthCheckController(ILogger<HealthCheckController> logger)
        {
            _logger = logger;
        }

        [HttpGet()]
        public string Get()
        {
            return "success";
        }
    }
}