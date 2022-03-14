using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApiProject.Entities.DbSet;

namespace WebApiProject.DataService.Data;

public class AppDbContext : IdentityDbContext
{
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }
}

