using AutoMapper;
using CommandService.Data;
using CommandService.Data.Dto.Platform;
using Microsoft.EntityFrameworkCore;

namespace CommandService.Business.Platform;

public class PlatformHandler(AppDbContext context, IMapper mapper) : IPlatformHandler
{
    public async Task<IEnumerable<PlatformReadDto>> GetAll() =>
        mapper.Map<IEnumerable<PlatformReadDto>>(await context.Platform.ToListAsync());

    public async Task Create(PlatformPublishedDto platform)
    {
        if (!await context.Platform.AnyAsync(p => p.ExternalId == platform.Id)) return;
        var entity = mapper.Map<Data.Entity.Platform>(platform);
        await context.Platform.AddAsync(entity ?? throw new ArgumentNullException(nameof(platform)));
        await context.SaveChangesAsync();
    }
}