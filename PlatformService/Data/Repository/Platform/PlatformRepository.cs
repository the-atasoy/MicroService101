using Microsoft.EntityFrameworkCore;

namespace PlatformService.Data.Repository.Platform;

public class PlatformRepository(AppDbContext context) : IPlatformRepository
{
    public async Task<bool> SaveChangesAsync() => await context.SaveChangesAsync() >= 0;
    
    public async Task<IEnumerable<Models.Platform>> GetAllAsync() => await context.Platform.ToListAsync();
    
    public async Task<Models.Platform?> GetByIdAsync(Guid id) => await context.Platform.FirstOrDefaultAsync(p => p.Id == id);
    
    public async Task CreateAsync(Models.Platform platform) =>
        await context.Platform.AddAsync(platform ?? throw new ArgumentNullException(nameof(platform)));
}