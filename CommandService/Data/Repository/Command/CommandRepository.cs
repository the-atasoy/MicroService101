using Microsoft.EntityFrameworkCore;

namespace CommandService.Data.Repository.Command;

public class CommandRepository(AppDbContext context) : ICommandRepository
{
    public async Task<bool> SaveChangesAsync() => await context.SaveChangesAsync() >= 0;

    public async Task<IEnumerable<Models.Command>> GetAllAsync(Guid platformId) =>
        await context.Command
            .Where(c => c.PlatformId == platformId)
            .OrderBy(c => c.Platform.Name)
            .ToListAsync();

    public async Task<Models.Command?> GetAsync(Guid platformId, Guid commandId) =>
        await context.Command.FirstOrDefaultAsync();
    
    public async Task CreateAsync(Models.Command command) =>
        await context.AddAsync(command ?? throw new ArgumentNullException(nameof(command)));
}