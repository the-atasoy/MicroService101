using CommandService.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace CommandService.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Platform> Platform { get; set; }
    public DbSet<Command> Command { get; set; }
}