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

void ConfigureServices(IServiceCollection services)
{
    Console.WriteLine($"--> Environment: {environment.EnvironmentName}");
    services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
    
    services.AddCors(options =>
    {
        options.AddPolicy("CustomPolicy",
            policyBuilder => policyBuilder
                .WithOrigins("http://localhost:3000")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
    });
    
    services.AddScoped<IPlatformHandler, PlatformHandler>();
    services.AddSingleton<IMessageBusClient, MessageBusClient>();
    services.AddControllers(options =>
    {
        options.Filters.Add(new ProducesAttribute("application/json"));
    });
    services.AddScoped<IGrpcCommandClient, GrpcCommandClient>();
    services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    
    Console.WriteLine($"--> CommandService Base URL: {builder.Configuration["CommandService:BaseUrl"]}");
}

void Configure(WebApplication app)
{
    app.UseHttpsRedirection();
    app.UseCors("CustomPolicy");
    app.MapControllers();
    PrepDb.Migrate(app);
}