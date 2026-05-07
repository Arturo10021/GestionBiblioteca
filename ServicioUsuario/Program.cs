using Scalar.AspNetCore;
using ServicioUsuario.Application.Interfaces;
using ServicioUsuario.Application.Services;
using ServicioUsuario.Domain.Ports;
using ServicioUsuario.Infrastructure.Configuration;
using ServicioUsuario.Infrastructure.Email;
using ServicioUsuario.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

ConfigurationSingleton.Initialize(builder.Configuration);

// Load local email password (gitignored, each dev creates their own)
builder.Configuration.AddJsonFile("emailsettings.json", optional: true, reloadOnChange: false);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHttpClient();

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection(EmailSettings.SectionName));

// Repositorio
builder.Services.AddSingleton<UsuarioRepository>();

// Email sender
builder.Services.AddSingleton<IEmailSender, SmtpEmailSender>();

// Servicios
builder.Services.AddSingleton<IUserCredentialProvisioningService, UserCredentialProvisioningService>();
builder.Services.AddSingleton<IUsuarioService, UsuarioService>();
builder.Services.AddSingleton<IEmailSender>(sp => EmailSenderFactory.Create(sp));
builder.Services.AddSingleton<IUserCredentialProvisioningService, UserCredentialProvisioningService>();

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
