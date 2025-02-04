using CommandService.Data;
using CommandService.Data.Repository.Command;
using CommandService.Data.Repository.Platform;
using CommandService.EventProcessing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
ConfigureServices(builder.Services);

var application = builder.Build();
Configure(application);

application.Run();

void Configure(WebApplication app)
{
    app.UseHttpsRedirection();
    app.MapControllers();
    PrepDb.Migrate(app);
}

void ConfigureServices(IServiceCollection services)
{
    services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
    services.AddScoped<IPlatformRepository, PlatformRepository>();
    services.AddScoped<ICommandRepository, CommandRepository>();
    services.AddControllers(options =>
    {
        options.Filters.Add(new ConsumesAttribute("application/json"));
        options.Filters.Add(new ProducesAttribute("application/json"));
    });
    services.AddSingleton<IEventProcessor, EventProcessor>();
    services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
}