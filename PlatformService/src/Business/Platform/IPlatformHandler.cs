using PlatformService.Data.Dto.Platform;

namespace PlatformService.Business.Platform;

public interface IPlatformHandler
{
    Task<IEnumerable<PlatformReadDto>> GetAllAsync();
    Task<PlatformReadDto?> GetByIdAsync(Guid id);
    Task<bool> CreateAsync(PlatformCreateDto platform);
}