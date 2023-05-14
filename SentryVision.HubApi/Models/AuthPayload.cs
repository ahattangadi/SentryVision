using System.Security.Principal;

namespace SentryVision.HubApi.Models;

public class AuthPayload
{
    public string ip { get; set; }
    public string user { get; set; }
    public string password { get; set; }
    public string path { get; set; }
    public string protocol { get; set; }
    public string id { get; set; }
    public string action { get; set; }
    public string query { get; set; }
}