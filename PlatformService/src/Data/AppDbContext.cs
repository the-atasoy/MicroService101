using Microsoft.EntityFrameworkCore;
using PlatformService.Data.Entity;

namespace PlatformService.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Platform> Platform { get; set; }
}