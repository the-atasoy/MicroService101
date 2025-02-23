using PlatformService.Data.Dto.Platform;

namespace PlatformService.Business.Platform;

public interface IPlatformHandler
{
    Task<IEnumerable<PlatformReadDto>> GetAll();
    Task<PlatformReadDto?> Get(Guid id);
    Task<bool> Create(PlatformCreateDto platform);
    Task<bool> Delete(Guid id);
}