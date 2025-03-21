using AutoMapper;
using CommandService.Data;
using CommandService.Data.Dto.Platform;
using Microsoft.EntityFrameworkCore;

namespace CommandService.Business.Platform;

public class PlatformHandler(AppDbContext context, IMapper mapper) : IPlatformHandler
{
    public async Task<IEnumerable<PlatformReadDto>> GetAll() =>
        mapper.Map<IEnumerable<PlatformReadDto>>(await context.Platform.AsNoTracking().ToListAsync());

    public async Task Create(PlatformPublishedDto platform)
    {
        if (await context.Platform.AnyAsync(p => p.ExternalId == platform.Id)) return;
        var entity = mapper.Map<Data.Entity.Platform>(platform);
        await context.Platform.AddAsync(entity ?? throw new ArgumentNullException(nameof(platform)));
        await context.SaveChangesAsync();
    }

    public async Task Update(PlatformPublishedDto platform)
    {
        var entity = await context.Platform.Where(p => p.ExternalId == platform.Id).FirstOrDefaultAsync();
        if (entity == null) throw new ArgumentNullException(nameof(platform));
        mapper.Map(platform, entity);
        context.Platform.Update(entity);
        await context.SaveChangesAsync();
    }
}