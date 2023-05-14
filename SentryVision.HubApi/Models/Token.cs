using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace SentryVision.HubApi.Models;

[Keyless]
public class Token
{
    public int Id { get; set; }
    public int CreationTime { get; set; }
    public string AccessToken { get; set; }
}