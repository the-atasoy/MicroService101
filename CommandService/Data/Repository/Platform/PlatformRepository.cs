using Microsoft.EntityFrameworkCore;

namespace CommandService.Data.Repository.Platform;

public class PlatformRepository(AppDbContext context) : IPlatformRepository
{
    public async Task<bool> SaveChangesAsync() => await context.SaveChangesAsync() >= 0;

    public async Task<IEnumerable<Models.Platform>> GetAllAsync() =>
        await context.Platform.ToListAsync();

    public async Task CreateAsync(Models.Platform platform) =>
        await context.Platform.AddAsync(platform ?? throw new ArgumentNullException(nameof(platform)));

    public async Task<bool> IsPlatformExistAsync(Guid id) =>
        await context.Platform.AnyAsync(p => p.Id == id);
    
    public async Task<bool> IsExternalPlatformExistAsync(Guid externalId) =>
        await context.Platform.AnyAsync(p => p.ExternalId == externalId);
}