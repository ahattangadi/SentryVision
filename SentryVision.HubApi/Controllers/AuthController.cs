using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.EntityFrameworkCore;
using SentryVision.HubApi.Models;

namespace SentryVision.HubApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly DatabaseInteractor _dbInteractor;

        public AuthController(ILogger<AuthController> logger, DatabaseInteractor dbInteractor)
        {
            _logger = logger;
            _dbInteractor = dbInteractor;
        }

        // GET: api/Auth
        [HttpGet]
        public async Task<List<Token>> Get()
        {
            // TODO: Add authentication to ensure only the server owner can access this endpoint
            _logger.LogInformation("{Time}: Getting all tokens from database", DateTimeOffset.UtcNow);
            return await _dbInteractor.Tokens.ToListAsync();
        }

        // POST: api/Auth

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AuthPayload payload)
        {
            string token = payload.password;
            string ip = payload.ip;
            _logger.LogInformation("{Time}: Received authentication request from {ip}", DateTimeOffset.UtcNow, ip);
            if (token == "")
            {
                _logger.LogError("{Time}: No token provided by {ip}", DateTimeOffset.UtcNow, ip);
                return StatusCode(401);
            }
            else
            {
                Array tokenArray = await _dbInteractor.Tokens.ToArrayAsync();
                foreach (Token t in tokenArray)
                {
                    if (t.AccessToken == token)
                    {
                        _logger.LogInformation("{Time}: Token provided by {ip} is valid", DateTimeOffset.UtcNow, ip);
                        return Ok();
                    }
                    else
                    {
                        _logger.LogError("{Time}: Token provided by {ip} is invalid", DateTimeOffset.UtcNow, ip);
                        return StatusCode(403);
                    }
                }

            }
            return StatusCode(500);
        }
    }
}