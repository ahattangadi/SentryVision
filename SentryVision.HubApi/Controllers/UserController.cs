using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SentryVision.HubApi.Models;

namespace SentryVision.HubApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = UserRoles.Superadmin)]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly DatabaseInteractor _dbInteractor;

        public UserController(ILogger<UserController> logger, DatabaseInteractor dbInteractor)
        {
            _logger = logger;
            _dbInteractor = dbInteractor;
        }

        [HttpGet]
        public async Task<List<string>> Get()
        {
            _logger.LogInformation("{Time}: Getting all users from database", DateTimeOffset.UtcNow);
            
            return await _dbInteractor.Users.Select(u => u.Username).ToListAsync();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] User payload)
        {
            _logger.LogInformation("{Time}: Received user creation request from {ip}", DateTimeOffset.UtcNow, HttpContext.Connection.RemoteIpAddress);
            
            if (String.IsNullOrEmpty(payload.Username) || String.IsNullOrEmpty(payload.Password) || String.IsNullOrEmpty(payload.Role))
            {
                _logger.LogError("{Time}: No username, password, or role provided by {ip}", DateTimeOffset.UtcNow, HttpContext.Connection.RemoteIpAddress);
                return StatusCode(401);
            }
            else
            {
                if (await _dbInteractor.Users.AnyAsync(u => u.Username == payload.Username))
                {
                    _logger.LogError("{Time}: Username {username} already exists", DateTimeOffset.UtcNow, payload.Username);
                    return StatusCode(409);
                }
                else
                {
                    _logger.LogInformation("{Time}: Creating user {username}", DateTimeOffset.UtcNow, payload.Username);
                    User hashedPayload = new User
                    {
                        Username = payload.Username,
                        Password = Modules.Sha256.GenerateSha256(payload.Password),
                        Role = payload.Role
                    };
                    
                    await _dbInteractor.Users.AddAsync(hashedPayload);
                    await _dbInteractor.SaveChangesAsync();
                    return StatusCode(201);
                }
            }
        }

        [HttpPut]

        public async Task<IActionResult> Put([FromBody] UserUpdatePayload payload)
        {
            _logger.LogInformation("{Time}: Received user update request from {ip}", DateTimeOffset.UtcNow, HttpContext.Connection.RemoteIpAddress);

            if (String.IsNullOrEmpty(payload.UserToChange.Username) || String.IsNullOrEmpty(payload.UserToChange.Password) ||
                String.IsNullOrEmpty(payload.Action) || String.IsNullOrEmpty(payload.ReplacedField))
            {
                _logger.LogInformation("{Time}: Any of the following field provided by {ip} were blank: Username, Password, Action, UpdatedField", DateTimeOffset.UtcNow, HttpContext.Connection.RemoteIpAddress);
            }
            else
            {
                if (await _dbInteractor.Users.AnyAsync(u => u.Username == payload.UserToChange.Username))
                {
                    if (payload.Action == "updatePwd")
                    {
                        _logger.LogInformation("{Time}: Updating password for user {username}", DateTimeOffset.UtcNow, payload.UserToChange.Username);
                        User user = await _dbInteractor.Users.FirstAsync(u => u.Username == payload.UserToChange.Username);
                        user.Password = Modules.Sha256.GenerateSha256(payload.ReplacedField);
                        await _dbInteractor.SaveChangesAsync();
                        return StatusCode(200);
                    }
                    else if (payload.Action == "updateUname")
                    {
                        _logger.LogInformation("{Time}: Updating username for user {username}", DateTimeOffset.UtcNow, payload.UserToChange.Username);
                        User user = await _dbInteractor.Users.FirstAsync(u => u.Username == payload.UserToChange.Username);
                        user.Username = payload.ReplacedField;
                        await _dbInteractor.SaveChangesAsync();
                        return StatusCode(200);
                    }
                    else
                    {
                        _logger.LogError("{Time}: Invalid toUpdate field provided by {ip}", DateTimeOffset.UtcNow, HttpContext.Connection.RemoteIpAddress);
                        return StatusCode(400);
                    }
                }
                else
                {
                    _logger.LogError("{Time}: User {username} does not exist", DateTimeOffset.UtcNow, payload.UserToChange.Username);
                    return StatusCode(404);
                }
                
            }
            return StatusCode(400);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] String userName)
        {
            _logger.LogInformation("{Time}: Received user deletion request from {ip}", DateTimeOffset.UtcNow, HttpContext.Connection.RemoteIpAddress);

            if (String.IsNullOrEmpty(userName))
            {
                _logger.LogInformation("{Time}: No username provided by {ip}", DateTimeOffset.UtcNow, HttpContext.Connection.RemoteIpAddress);
                return StatusCode(401);
            }
            else
            {
                if (await _dbInteractor.Users.AnyAsync(u => u.Username == userName))
                {
                    _logger.LogInformation("{Time}: User to be deleted: {username}", DateTimeOffset.UtcNow, userName);
                    _logger.LogInformation("{Time}: Deleting user {username}", DateTimeOffset.UtcNow, userName);
                    User user = await _dbInteractor.Users.FirstAsync(u => u.Username == userName);
                    _dbInteractor.Users.Remove(user);
                    await _dbInteractor.SaveChangesAsync();
                    return StatusCode(200);
                }
                else
                {
                    _logger.LogInformation("{Time}: User {username} does not exist", DateTimeOffset.UtcNow, userName);
                    return StatusCode(404);
                }
            }
        }
    }
}
