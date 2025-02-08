using CommandService.Data.Repository.Platform;
using CommandService.Models;
using CommandService.SyncDataServices.Grpc;
using Microsoft.EntityFrameworkCore;

namespace CommandService.Data;

public static class PrepDb
{
    public static async Task Migrate(IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        Console.WriteLine("--> Attempting to Applying Migration");
        try
        {
            serviceScope.ServiceProvider.GetService<AppDbContext>()!.Database.Migrate();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Migration failed: {e.Message}");
            throw;
        }

        try
        {
            var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();
            var platforms = grpcClient!.ReturnAllPlatforms();
            await SeedData(serviceScope.ServiceProvider.GetService<IPlatformRepository>()!, platforms);
        }
        catch (Exception e)
        {
            Console.WriteLine($"--> Could not call the Grpc Server {e.Message}");
            throw;
        }
    }

    private static async Task SeedData(IPlatformRepository repository, IEnumerable<Platform> platforms)
    {
        Console.WriteLine("--> Seeding Data...");
        foreach (var platform in platforms)
        {
            if (!await repository.IsExternalPlatformExistAsync(platform.ExternalId))
                await repository.CreateAsync(platform);
        }
        await repository.SaveChangesAsync();
    }
}