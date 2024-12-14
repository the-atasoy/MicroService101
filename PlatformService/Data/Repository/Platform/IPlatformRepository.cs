namespace PlatformService.Data.Repository.Platform;

public interface IPlatformRepository
{
    Task<bool> SaveChangesAsync();
    Task<IEnumerable<Models.Platform>> GetAllAsync();
    Task<Models.Platform?> GetByIdAsync(Guid id);
    Task CreateAsync(Models.Platform platform);
}