using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SentryVision.HubApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RtspController : ControllerBase
    {
        private readonly ILogger _logger;
        private static readonly HttpClient Client = new()
        {
            BaseAddress = new Uri("http://127.0.0.1:9997/v1/")
        };

        public RtspController(ILogger<RtspController> logger)
        {
            _logger = logger;
        }

        
        // GET: api/Rtsp
        [HttpGet]
        public async Task<string> Get()
        {
            try
            {
                _logger.LogInformation("{Time}: Getting RTSP paths from MediaMTX", DateTimeOffset.UtcNow);
                var response = await Client.GetStringAsync("paths/list");
                _logger.LogInformation("{Time}: Received response from server {Response}", DateTimeOffset.UtcNow, response.ToString());
                return response.ToString();
            }
            catch (Exception e)
            {
                _logger.LogError("{Time}: Error getting RTSP paths from MediaMTX", DateTimeOffset.UtcNow);
                return "Error";
            }
        }
        
    }
}
