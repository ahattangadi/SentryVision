using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SentryVision.HubApi.Models;

namespace SentryVision.HubApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JwtController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly DatabaseInteractor _databaseInteractor;

        public JwtController(ILogger<JwtController> logger, IConfiguration configuration, DatabaseInteractor databaseInteractor)
        {
            _logger = logger;
            _configuration = configuration;
            _databaseInteractor = databaseInteractor;
        }
        
        [HttpGet]

        public async Task<IResult> Get([FromBody] RolelessUser jwtUser)
        {
            string[] _users = await _databaseInteractor.Users.Select(u => u.Username).ToArrayAsync();
            string[] _passwords = await _databaseInteractor.Users.Select(p => p.Password).ToArrayAsync();
            
            if (_users.Contains(jwtUser.Username) && _passwords.Contains(Modules.Sha256.GenerateSha256(jwtUser.Password)))
            {
              var issuer = _configuration.GetValue<string>("Jwt:Issuer");
              var audience = _configuration.GetValue<string>("Jwt:Audience");
              var key = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("Jwt:Key"));

              string role = await _databaseInteractor.Users.Where(u => u.Username == jwtUser.Username).Select(r => r.Role).FirstOrDefaultAsync();
              var tokenDescriptor = new SecurityTokenDescriptor
              {
                  Subject = new ClaimsIdentity(new[]
                  {
                      new Claim("Id", Guid.NewGuid().ToString()),
                      new Claim(JwtRegisteredClaimNames.Sub, jwtUser.Username),
                      new Claim(JwtRegisteredClaimNames.Email, jwtUser.Username),
                      new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                      new Claim(ClaimTypes.Role, role)
                  }),
                  Expires = DateTime.UtcNow.AddHours(1),
                  Issuer = issuer,
                  Audience = audience,
                  SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                      SecurityAlgorithms.HmacSha512Signature)
              };
              var tokenHandler = new JwtSecurityTokenHandler();
              var token = tokenHandler.CreateToken(tokenDescriptor);
              var jwtToken = tokenHandler.WriteToken(token);
              var stringToken = tokenHandler.WriteToken(token);
              return Results.Ok(stringToken);
            }
            else
            {
                return Results.Unauthorized();
            }


        }
    }
}
