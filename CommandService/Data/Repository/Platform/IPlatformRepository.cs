namespace CommandService.Data.Repository.Platform;

public interface IPlatformRepository
{
    Task<bool> SaveChangesAsync();
    Task<IEnumerable<Models.Platform>> GetAllAsync();
    Task CreateAsync(Models.Platform platform);
    Task<bool> IsPlatformExistAsync(Guid id);
}