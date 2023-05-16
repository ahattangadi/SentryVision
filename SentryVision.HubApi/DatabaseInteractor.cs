using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using SentryVision.HubApi.Models;

namespace SentryVision.HubApi;

public class DatabaseInteractor : DbContext
{
    public DatabaseInteractor(DbContextOptions<DatabaseInteractor> options) : base(options)
    {
    }
    
    public DbSet<Token> Tokens { get; set; }
    public DbSet<User> Users { get; set; }
    
}