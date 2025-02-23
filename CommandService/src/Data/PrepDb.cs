using CommandService.API.Messaging.gRPC;
using CommandService.Business.Platform;
using CommandService.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace CommandService.Data;

public static class PrepDb
{
    public static void Migrate(IApplicationBuilder app)
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
    }
}