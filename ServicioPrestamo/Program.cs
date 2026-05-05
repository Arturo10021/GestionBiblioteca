using ServicioPrestamo.Application.Fachadas;
using ServicioPrestamo.Application.Interfaces;
using ServicioPrestamo.Application.Services;
using ServicioPrestamo.Domain.Ports;
using ServicioPrestamo.Infrastructure.Configuration;
using ServicioPrestamo.Infrastructure.Creators;
using ServicioPrestamo.Infrastructure.Persistence;
using ServicioPrestamo.Infrastructure.Email;

var builder = WebApplication.CreateBuilder(args);

ConfigurationSingleton.Initialize(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(builder.Configuration["Cors:FrontendUrl"] ?? "https://localhost:7003")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// FACTORY METHOD — Creators (Singleton)
builder.Services.AddSingleton<AutorRepositoryCreator>();
builder.Services.AddSingleton<LibroRepositoryCreator>();
builder.Services.AddSingleton<EjemplarRepositoryCreator>();
builder.Services.AddSingleton<PrestamoRepositoryCreator>();
builder.Services.AddSingleton<DetalleRepositoryCreator>();

// PRODUCTS via Factory (Scoped)
builder.Services.AddScoped<IAutorRepositorio>(sp =>
    (IAutorRepositorio)sp.GetRequiredService<AutorRepositoryCreator>().CreateRepository());
builder.Services.AddScoped<ILibroRepositorio>(sp =>
    (ILibroRepositorio)sp.GetRequiredService<LibroRepositoryCreator>().CreateRepository());
builder.Services.AddScoped<IEjemplarRepositorio>(sp =>
    (IEjemplarRepositorio)sp.GetRequiredService<EjemplarRepositoryCreator>().CreateRepository());
builder.Services.AddScoped<IPrestamoRepositorio>(sp =>
    sp.GetRequiredService<PrestamoRepositoryCreator>().CreateRepository());
builder.Services.AddScoped<IDetalleRepositorio>(sp =>
    sp.GetRequiredService<DetalleRepositoryCreator>().CreateRepository());

// Services
builder.Services.AddScoped<IAutorServicio, AutorServicio>();
builder.Services.AddScoped<ILibroServicio, LibroServicio>();
builder.Services.AddScoped<IEjemplarServicio, EjemplarServicio>();
builder.Services.AddScoped<IPrestamoServicio, PrestamoServicio>();
builder.Services.AddScoped<IDetalleServicio>(sp => new DetalleServicio(
    sp.GetRequiredService<IDetalleRepositorio>(),
    sp.GetRequiredService<IEjemplarRepositorio>()
));

// Fachadas
builder.Services.AddScoped<IPrestamoFachada, PrestamoFachada>();
builder.Services.AddScoped<IAnulacionFachada, AnulacionFachada>();
builder.Services.AddScoped<IEjemplarDisponibilidadFachada, EjemplarDisponibilidadFachada>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();
app.Run();
