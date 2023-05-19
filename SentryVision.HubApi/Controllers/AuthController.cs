using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.EntityFrameworkCore;
using SentryVision.HubApi.Models;
using SentryVision.HubApi.Modules;

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
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<List<Token>> Get()
        {
            // TODO: Add authentication to ensure only the server owner can access this endpoint
            _logger.LogInformation("{Time}: Getting all tokens from database", DateTimeOffset.UtcNow);
            return await _dbInteractor.Tokens.ToListAsync();
        }

        // POST: api/Auth

        [HttpPost]
        [AllowAnonymous]
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

        [HttpPost("generateToken")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> Post()
        {
            _logger.LogInformation("{Time}: Generating new token", DateTimeOffset.UtcNow);
            Token newToken = new Token();
            const string chars = "ABCDEFGHJIKLMNOPQRSTUVWXYZ";
            const string nums = "1234567890";
            var random = new Random();

            string tokenLetters =
                new string(Enumerable.Repeat(chars, 4).Select(s => s[random.Next(s.Length)]).ToArray());
            string tokenNumbers = new string(Enumerable.Repeat(nums, 4).Select(s => s[random.Next(s.Length)]).ToArray());

            string finallyGeneratedToken = tokenLetters + tokenNumbers;

            using (var sha256 = new SHA256Managed())
            {
                newToken.CreationTime = (int)(DateTime.UtcNow - DateTime.UnixEpoch).TotalSeconds; // This gets the current time in UNIX epoch by subtracting the current date from the unix epoch and getting the seconds as an integer
                newToken.AccessToken = (string)(Modules.Sha256.GenerateSha256(finallyGeneratedToken));
            }


            try
            {
                _logger.LogInformation("{Time}: Attempting to add new token to database", DateTimeOffset.UtcNow);
                await _dbInteractor.Tokens.AddAsync(newToken);
                await _dbInteractor.SaveChangesAsync();
                _logger.LogInformation("{Time}: Token added to database", DateTimeOffset.UtcNow);
            }
            catch (Exception e)
            {
                _logger.LogError("{Time}: Token could not be added to database with error message \n {error}", DateTimeOffset.UtcNow, e);
                return StatusCode(500);
            }

            return Ok(finallyGeneratedToken);
        }
    }
}