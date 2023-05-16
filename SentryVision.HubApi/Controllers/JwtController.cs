using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public JwtController(ILogger<JwtController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }
        
        [HttpGet]

        public IResult Get([FromBody] User jwtUser)
        {
            if (jwtUser.Username == _configuration.GetValue<string>("UserCredentials:Username") && jwtUser.Password == _configuration.GetValue<string>("UserCredentials:Password"))
            {
              var issuer = _configuration.GetValue<string>("Jwt:Issuer");
              var audience = _configuration.GetValue<string>("Jwt:Audience");
              var key = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("Jwt:Key"));

              var tokenDescriptor = new SecurityTokenDescriptor
              {
                  Subject = new ClaimsIdentity(new[]
                  {
                      new Claim("Id", Guid.NewGuid().ToString()),
                      new Claim(JwtRegisteredClaimNames.Sub, jwtUser.Username),
                      new Claim(JwtRegisteredClaimNames.Email, jwtUser.Username),
                      new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                  }),
                  Expires = DateTime.UtcNow.AddMinutes(5),
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
