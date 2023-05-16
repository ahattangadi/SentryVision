using Microsoft.EntityFrameworkCore;

namespace SentryVision.HubApi.Models;

[Keyless]
public class User
{
    public string Username { get; set; }
    public string Password { get; set; }
}