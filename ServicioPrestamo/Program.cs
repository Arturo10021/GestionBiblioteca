using ServicioPrestamo.Application.Fachadas;
using ServicioPrestamo.Application.Interfaces;
using ServicioPrestamo.Application.Services;
using ServicioPrestamo.Infrastructure.Configuration;
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

// Repositorios
builder.Services.AddScoped<AutorRepository>();
builder.Services.AddScoped<LibroRepository>();
builder.Services.AddScoped<EjemplarRepository>();
builder.Services.AddScoped<PrestamoRepository>();
builder.Services.AddScoped<DetalleRepository>();

// Servicios de Aplicación
builder.Services.AddScoped<IAutorServicio, AutorServicio>();
builder.Services.AddScoped<ILibroServicio, LibroServicio>();
builder.Services.AddScoped<IEjemplarServicio, EjemplarServicio>();
builder.Services.AddScoped<IPrestamoServicio, PrestamoServicio>();
builder.Services.AddScoped<IDetalleServicio, DetalleServicio>();

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
