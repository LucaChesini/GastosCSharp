using Gastos.Api.Data;
using Gastos.Api.Endpoints;
using Gastos.Api.Services;

using Microsoft.EntityFrameworkCore;

using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Connection string 'Default' não configurada.");

builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<CategoriaService>();
builder.Services.AddScoped<DespesaService>();
builder.Services.AddScoped<TagService>();
builder.Services.AddScoped<ComprovanteService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapCategoriaEndpoints();
app.MapTagEndpoints();
app.MapDespesaEndpoints();
app.MapComprovanteEndpoints();

app.Run();
