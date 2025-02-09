using AutoMapper;
using CommandService.Data;
using CommandService.Data.Dto.Command;
using Microsoft.EntityFrameworkCore;

namespace CommandService.Business.Command;

public class CommandHandler(AppDbContext context, IMapper mapper) : ICommandHandler
{
    public async Task<CommandReadDto?> GetAsync(Guid platformId, Guid commandId) =>
        mapper.Map<CommandReadDto>(await context.Command.FirstOrDefaultAsync());

    public async Task<IEnumerable<CommandReadDto>> GetAllAsync(Guid platformId)
    {
        if(!await context.Platform.AnyAsync(p => p.Id == platformId)) return [];
        
        var result = await context.Command
            .Where(c => c.PlatformId == platformId)
            .OrderBy(c => c.Platform.Name)
            .ToListAsync();
        
        return mapper.Map<IEnumerable<CommandReadDto>>(result);
    }

    public async Task<bool> CreateAsync(CommandCreateDto command, Guid platformId)
    {
        if (!await context.Platform.AnyAsync(p => p.Id == platformId)) return false;
        var entity = mapper.Map<Data.Entity.Command>(command);
        entity.PlatformId = platformId;
        await context.AddAsync(entity ?? throw new ArgumentNullException(nameof(entity)));
        var result = await context.SaveChangesAsync();
        return result >= 0;
    }
}