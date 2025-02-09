using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlatformService.API.Messaging.gRPC;
using PlatformService.API.Messaging.RabbitMQ;
using PlatformService.Business.Platform;
using PlatformService.Data;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var environment = builder.Environment;
ConfigureServices(builder.Services);

var application = builder.Build();
Configure(application);

application.Run();

void Configure(WebApplication app)
{
    app.UseHttpsRedirection();
    app.MapControllers();
    app.MapGrpcService<GrpcPlatformService>();
    PrepDb.Migrate(app);
}

void ConfigureServices(IServiceCollection services)
{
    Console.WriteLine($"--> Environment: {environment.EnvironmentName}");
    services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
    
    services.AddScoped<IPlatformHandler, PlatformHandler>();
    services.AddSingleton<IMessageBusClient, MessageBusClient>();
    services.AddGrpc();
    services.AddControllers(options =>
    {
        options.Filters.Add(new ConsumesAttribute("application/json"));
        options.Filters.Add(new ProducesAttribute("application/json"));
    });
    services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    
    Console.WriteLine($"--> CommandService Base URL: {builder.Configuration["CommandService:BaseUrl"]}");
}