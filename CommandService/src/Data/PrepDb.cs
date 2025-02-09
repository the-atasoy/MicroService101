using CommandService.API.Messaging.gRPC;
using CommandService.Business.Platform;
using CommandService.Data.Entity;
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
            await SeedData(serviceScope.ServiceProvider.GetService<IPlatformHandler>()!, platforms);
        }
        catch (Exception e)
        {
            Console.WriteLine($"--> Could not call the Grpc Server {e.Message}");
            throw;
        }
    }
    
    private static async Task SeedData(IPlatformHandler handler, IEnumerable<Platform> platforms)
    {
        Console.WriteLine("--> Seeding Data...");
        foreach (var platform in platforms)
        {
        }
    }
}