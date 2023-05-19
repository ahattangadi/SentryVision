using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace SentryVision.HubApi.Models;

public class Token
{
    [Key]
    public int Id { get; set; }
    public int CreationTime { get; set; }
    public string AccessToken { get; set; }
}