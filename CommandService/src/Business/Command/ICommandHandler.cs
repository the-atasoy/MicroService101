using CommandService.Data.Dto.Command;

namespace CommandService.Business.Command;

public interface ICommandHandler
{
    Task<CommandReadDto?> GetAsync(Guid platformId, Guid commandId);
    Task<IEnumerable<CommandReadDto>> GetAllAsync(Guid platformId);
    Task<bool> CreateAsync(CommandCreateDto command, Guid platformId);
}