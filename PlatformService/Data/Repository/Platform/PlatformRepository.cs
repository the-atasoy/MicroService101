using Microsoft.EntityFrameworkCore;

namespace PlatformService.Data.Repository.Platform;

public class PlatformRepository(AppDbContext context) : IPlatformRepository
{
    private readonly AppDbContext _context = context;
    public async Task<bool> SaveChangesAsync() => await _context.SaveChangesAsync() >= 0;
    public async Task<IEnumerable<Models.Platform>> GetAllAsync() => await _context.Platform.ToListAsync();
    public async Task<Models.Platform?> GetByIdAsync(Guid id) => await _context.Platform.FirstOrDefaultAsync(p => p.Id == id);
    public async Task CreateAsync(Models.Platform platform) =>
        await _context.Platform.AddAsync(platform ?? throw new ArgumentNullException(nameof(platform)));
}