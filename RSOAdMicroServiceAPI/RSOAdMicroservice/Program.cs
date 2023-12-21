using AdServiceRSO.Repository;
using Carter;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using RSO.Core.AdModels;
using RSO.Core.BL;
using RSO.Core.Configurations;
using RSO.Core.Repository;
using RSO.Core.Health;

var builder = WebApplication.CreateBuilder(args);

//Database settings
builder.Services.AddDbContext<AdServicesRSOContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("AdServicesRSOdB")));

builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("Database")
    .AddCheck<ExternalAPICheck>("LavbicAPI");

// Register the IOptions object.
builder.Services.AddOptions<JwtSecurityTokenConfiguration>()
    .BindConfiguration("JwtSecurityTokenConfiguration");
// Explicitly register the settings objects by delegating to the IOptions object.
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<JwtSecurityTokenConfiguration>>().Value);

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAdRepository, AdRepository>();

builder.Services.AddScoped<IAdLogic, AdLogic>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddCarter();

// Add services to the container.
builder.Services.AddAuthorization();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swagger =>
{
    swagger.SwaggerDoc("v1", new OpenApiInfo()
    {
        Description = "Ad microservice for E-commerce app.",
        Title = "RSO project.",
        Version = "v1",
        Contact = new OpenApiContact()
        {
            Name = "Aleksander Kovac & Urban Poljsak",
            Url = new Uri("https://www.youtube.com/watch?v=dQw4w9WgXcQ")
        }
    });
});

var app = builder.Build();

//app.UseHttpsRedirection();

app.MapCarter();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API"));

//app.UseAuthentication();
//app.UseAuthorization();

app.Run();