using PlatformService.Models;

namespace PlatformService.Data;

public interface IPlatformRepository
{
    Task<bool> SaveChangesAsync();
    Task<IEnumerable<Platform>> GetAllAsync();
    Task<Platform?> GetByIdAsync(Guid id);
    Task CreateAsync(Platform platform);
}