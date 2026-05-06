using Scalar.AspNetCore;
using ServicioUsuario.Application.Services;
using ServicioUsuario.Domain.Ports;
using ServicioUsuario.Infrastructure.Configuration;
using ServicioUsuario.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

ConfigurationSingleton.Initialize(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Registrar repositorio e inyectar en el servicio
builder.Services.AddSingleton<IUsuarioRepositorio, UsuarioRepository>();
builder.Services.AddSingleton<IUsuarioService, UsuarioService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseAuthorization();

app.MapGet("/", () => Results.Redirect("/scalar/v1"));

app.MapControllers();

app.Run();
