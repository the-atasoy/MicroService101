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
        const int maxRetries = 5;
        const int delaySeconds = 5;
        
        for (var i = 0; i < maxRetries; i++)
        {
            try
            {
                Console.WriteLine($"--> Attempting to apply migration (Attempt {i + 1}/{maxRetries})");
                serviceScope.ServiceProvider.GetService<AppDbContext>()!.Database.Migrate();
                Console.WriteLine("--> Migration successful");
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Migration failed: {e.Message}");
                
                if (i == maxRetries - 1)
                {
                    Console.WriteLine("--> Migration failed after all retry attempts");
                    throw;
                }
                
                Console.WriteLine($"--> Retrying in {delaySeconds} seconds...");
                Thread.Sleep(TimeSpan.FromSeconds(delaySeconds));
            }
        }
    }
}