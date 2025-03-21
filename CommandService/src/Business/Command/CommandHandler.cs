using AutoMapper;
using CommandService.Data;
using CommandService.Data.Dto.Command;
using Microsoft.EntityFrameworkCore;

namespace CommandService.Business.Command;

public class CommandHandler(AppDbContext context, IMapper mapper) : ICommandHandler
{
    public async Task<CommandReadDto?> Get(Guid platformId, Guid commandId) =>
        mapper.Map<CommandReadDto>(await context.Command.AsNoTracking().Where(c => c.PlatformId == platformId && c.Id == commandId).FirstOrDefaultAsync());

    public async Task<IEnumerable<CommandReadDto>> GetAll(Guid platformId)
    {
        if(!await context.Platform.AnyAsync(p => p.Id == platformId)) return [];
        
        var result = await context.Command
            .Where(c => c.PlatformId == platformId)
            .OrderBy(c => c.Platform.Name)
            .AsNoTracking()
            .ToListAsync();
        
        return mapper.Map<IEnumerable<CommandReadDto>>(result);
    }

    public async Task<bool> Create(CommandCreateDto command, Guid platformId)
    {
        if (!await context.Platform.AnyAsync(p => p.Id == platformId)) return false;
        if (await context.Command.AnyAsync(c => c.CommandLine == command.CommandLine && c.PlatformId == platformId)) return false;
        
        var entity = mapper.Map<Data.Entity.Command>(command);
        entity.PlatformId = platformId;
        await context.AddAsync(entity ?? throw new ArgumentNullException(nameof(entity)));
        var result = await context.SaveChangesAsync();
        return result >= 0;
    }
    
    public async Task<bool> Update(CommandUpdateDto command, Guid commandId)
    {
        var entity = await context.Command.FirstOrDefaultAsync(c => c.Id == commandId);
        if (entity is null) return false;
        mapper.Map(command, entity);
        context.Update(entity);
        var result = await context.SaveChangesAsync();
        return result >= 0;
    }
    
    public async Task<bool> Delete(Guid commandId)
    {
        var entity = await context.Command.FirstOrDefaultAsync(c => c.Id == commandId);
        if (entity is null) return false;
        context.Remove(entity);
        var result = await context.SaveChangesAsync();
        return result >= 0;
    }
    
    public async Task<bool> DeleteAll(Guid platformId)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var commands = await context.Command.Where(c => c.PlatformId == platformId).ToListAsync();
            var platform = await context.Platform.FirstOrDefaultAsync(p => p.Id == platformId);

            if (commands.Count != 0) context.RemoveRange(commands);
            if (platform != null) context.Remove(platform);

            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            return false;
        }
    }
}