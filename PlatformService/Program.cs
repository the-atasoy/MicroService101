using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using PlatformService.Data.Repository.Platform;
using PlatformService.SyncDataServices.Http;

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
    PrepDb.Migrate(app);
}

void ConfigureServices(IServiceCollection services)
{
    Console.WriteLine($"--> Environment: {environment.EnvironmentName}");
    services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
    
    services.AddScoped<IPlatformRepository, PlatformRepository>();
    services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
    services.AddControllers(options =>
    {
        options.Filters.Add(new ConsumesAttribute("application/json"));
        options.Filters.Add(new ProducesAttribute("application/json"));
    });
    services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    
    Console.WriteLine($"--> CommandService Base URL: {builder.Configuration["CommandService:BaseUrl"]}");
}