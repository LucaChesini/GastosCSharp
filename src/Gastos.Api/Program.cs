using Gastos.Api.Data;
using Gastos.Api.Endpoints;
using Gastos.Api.Services;

using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<DadosEmMemoria>();
builder.Services.AddScoped<CategoriaService>();
builder.Services.AddScoped<DespesaService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapCategoriaEndpoints();
app.MapDespesaEndpoints();

app.Run();
