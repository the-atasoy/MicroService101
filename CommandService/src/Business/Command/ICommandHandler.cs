using CommandService.Data.Dto.Command;

namespace CommandService.Business.Command;

public interface ICommandHandler
{
    Task<CommandReadDto?> Get(Guid platformId, Guid commandId);
    Task<IEnumerable<CommandReadDto>> GetAll(Guid platformId);
    Task<bool> Create(CommandCreateDto command, Guid platformId);
    Task<bool> Delete(Guid platformId, Guid commandId);
    Task<bool> Update(CommandUpdateDto command, Guid platformId, Guid commandId);
}