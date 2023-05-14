using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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
            return await _dbInteractor.Tokens.ToListAsync();
        }

    }
}