using AutoMapper;
using CommandService.Data;
using CommandService.Data.Dto.Command;
using Microsoft.EntityFrameworkCore;

namespace CommandService.Business.Command;

public class CommandHandler(AppDbContext context, IMapper mapper) : ICommandHandler
{
    public async Task<CommandReadDto?> Get(Guid platformId, Guid commandId) =>
        mapper.Map<CommandReadDto>(await context.Command.AsNoTracking().FirstOrDefaultAsync());

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
        var entities = await context.Command.Where(c => c.PlatformId == platformId).ToListAsync();
        if (!entities.Any()) return false;
        context.RemoveRange(entities);
        var result = await context.SaveChangesAsync();
        return result >= 0;
    }
}