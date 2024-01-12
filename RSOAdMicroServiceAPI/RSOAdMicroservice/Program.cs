using AdServiceRSO.Repository;
using Carter;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RSO.Core.AdModels;
using RSO.Core.BL;
using RSO.Core.Configurations;
using RSO.Core.Health;
using RSO.Core.Repository;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

//Database settings
builder.Services.AddDbContext<AdServicesRSOContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("AdServicesRSOdB")));

builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("Database")
    .AddCheck<ExternalAPICheck>("CurrencyConverter");

// Register the IOptions object.
builder.Services.AddOptions<ApiCredentialsConfiguration>()
    .BindConfiguration("ApiCredentialsConfiguration");
// Explicitly register the settings objects by delegating to the IOptions object.
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<ApiCredentialsConfiguration>>().Value);

// Register the IOptions object.
builder.Services.AddOptions<JwtSecurityTokenConfiguration>()
    .BindConfiguration("JwtSecurityTokenConfiguration");
// Explicitly register the settings objects by delegating to the IOptions object.
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<JwtSecurityTokenConfiguration>>().Value);

builder.Services.AddRazorPages();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAdRepository, AdRepository>();

builder.Services.AddScoped<IAdLogic, AdLogic>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddCarter();

// Add services to the container.
builder.Services.AddAuthorization();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(options =>
{
    options.PostProcess = document =>
    {
        document.Info = new()
        {
            Version = "v1",
            Title = "Ad microservices API",
            Description = "Ad microservices API endpoints",
            TermsOfService = "Lol.",
            Contact = new()
            {
                Name = "Aleksander Kovac & Urban Poljsak",
                Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ"
            }
        };
    };
    options.UseControllerSummaryAsTagDescription = true;
});
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();


app.MapCarter();
app.UseOpenApi();
app.UseSwaggerUi3(options =>
{
    options.Path = "/openapi";
    options.TagsSorter = "Ads";
});
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
    endpoints.MapControllers();
});

app.UseSerilogRequestLogging();

app.Run();