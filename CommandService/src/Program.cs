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

void ConfigureServices(IServiceCollection services)
{
    services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
    
    services.AddCors(options =>
    {
        options.AddPolicy("CustomPolicy",
            policyBuilder => policyBuilder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
    });
    
    services.AddScoped<IPlatformHandler, PlatformHandler>();
    services.AddScoped<ICommandHandler, CommandHandler>();
    services.AddControllers(options =>
    {
        options.Filters.Add(new ProducesAttribute("application/json"));
    });
    services.AddHostedService<MessageBusSubscriber>();
    services.AddGrpc();
    services.AddSingleton<IEventProcessor, EventProcessor>();
    services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
}

void Configure(WebApplication app)
{
    app.UseHttpsRedirection();
    app.UseCors("CustomPolicy");
    app.MapControllers();
    app.MapGrpcService<GrpcCommandService>();
    PrepDb.Migrate(app);
}