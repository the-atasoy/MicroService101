using CommandService.API.Messaging.gRPC;
using CommandService.API.Messaging.RabbitMQ;
using CommandService.API.Messaging.RabbitMQ.EventProcessing;
using CommandService.Business.Command;
using CommandService.Business.Platform;
using CommandService.Data;
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
    PrepDb.Migrate(app).GetAwaiter().GetResult();
}

void ConfigureServices(IServiceCollection services)
{
    services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
    services.AddScoped<IPlatformHandler, PlatformHandler>();
    services.AddScoped<ICommandHandler, CommandHandler>();
    services.AddControllers(options =>
    {
        options.Filters.Add(new ConsumesAttribute("application/json"));
        options.Filters.Add(new ProducesAttribute("application/json"));
    });
    services.AddHostedService<MessageBusSubscriber>();
    services.AddScoped<IPlatformDataClient, PlatformDataClient>();
    services.AddSingleton<IEventProcessor, EventProcessor>();
    services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
}