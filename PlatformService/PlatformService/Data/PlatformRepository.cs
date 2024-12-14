using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data;

public class PlatformRepository(AppDbContext context) : IPlatformRepository
{
    private readonly AppDbContext _context = context;
    public async Task<bool> SaveChangesAsync() => await _context.SaveChangesAsync() >= 0;
    public async Task<IEnumerable<Platform>> GetAllAsync() => await _context.Platforms.ToListAsync();
    public async Task<Platform?> GetByIdAsync(Guid id) => await _context.Platforms.FirstOrDefaultAsync(p => p.Id == id);
    public async Task CreateAsync(Platform platform) => await _context.Platforms.AddAsync(platform ?? throw new ArgumentNullException(nameof(platform)));
}