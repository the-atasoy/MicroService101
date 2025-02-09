using CommandService.Data.Dto.Platform;

namespace CommandService.Business.Platform;

public interface IPlatformHandler
{
    Task<IEnumerable<PlatformReadDto>> GetAllAsync();
    Task CreateAsync(PlatformPublishedDto platform);
}