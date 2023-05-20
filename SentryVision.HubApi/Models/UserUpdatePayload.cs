namespace SentryVision.HubApi.Models;

public class UserUpdatePayload
{
    public User UserToChange { get; set; }
    public string Action { get; set; }
    public string ReplacedField { get; set; }
}