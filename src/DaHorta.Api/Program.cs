using DaHorta.Api.Application.Abstractions;
using DaHorta.Api.Application.Services;
using DaHorta.Api.Infrastructure.Exceptions;
using DaHorta.Api.Infrastructure.Persistence;
using DaHorta.Api.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Host=localhost;Port=5432;Database=dahorta;Username=dahorta;Password=dahorta_local";

builder.Services.AddDbContext<DaHortaDbContext>(options =>
{
    options.UseNpgsql(connectionString, npgsql => npgsql.UseNetTopologySuite());
});

builder.Services.AddScoped<IFarmRepository, FarmRepository>();
builder.Services.AddScoped<IListingRepository, ListingRepository>();
builder.Services.AddScoped<IFarmService, FarmService>();
builder.Services.AddScoped<IListingService, ListingService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Info ??= new();
        document.Info.Title = "Da Horta API";
        document.Info.Version = "v1";
        document.Info.Description = "API para gerir produtores, listagens de produtos frescos e reservas locais do Da Horta.";

        return Task.CompletedTask;
    });
});
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (!app.Environment.IsProduction())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
