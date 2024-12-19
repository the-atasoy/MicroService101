using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
ConfigureServices(builder.Services);
var application = builder.Build();
Configure(application);
application.Run();

void Configure(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }
    app.UseHttpsRedirection();
    app.MapControllers();
    //PrepDb.PrepPopulation(app);
}

void ConfigureServices(IServiceCollection services)
{
    services.AddOpenApi();
    // services.AddDbContext<AppDbContext>(options =>
    // {
    //     options.UseInMemoryDatabase("InMem");
    // });
    //services.AddScoped<IPlatformRepository, PlatformRepository>();
    services.AddControllers(options =>
    {
        options.Filters.Add(new ConsumesAttribute("application/json"));
        options.Filters.Add(new ProducesAttribute("application/json"));
    });
    services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
}