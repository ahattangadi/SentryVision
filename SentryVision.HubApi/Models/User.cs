using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SentryVision.HubApi.Models;

public class User
{
    [Key]
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}