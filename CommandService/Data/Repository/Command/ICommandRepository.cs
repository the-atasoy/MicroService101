namespace CommandService.Data.Repository.Command;

public interface ICommandRepository
{
    Task<bool> SaveChangesAsync();
    Task<IEnumerable<Models.Command>> GetAllAsync(Guid platformId);
    Task<Models.Command?> GetAsync(Guid platformId, Guid commandId);
    Task CreateAsync(Models.Command command);
}