using System.Net;
using System.Net.Mail;
using ServicioUsuario.Application.Interfaces;

namespace ServicioUsuario.Infrastructure.Email;

public class SmtpEmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(IConfiguration configuration, ILogger<SmtpEmailSender> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        var useDevMode = _configuration.GetValue<bool>("Email:UseDevelopmentMode");
        var fromName = _configuration["Email:FromName"] ?? "Sistema Bibliotecario";
        var fromAddress = _configuration["Email:FromAddress"] ?? "biblioteca@starbook.com";
        var host = _configuration["Email:Smtp:Host"];
        var port = _configuration.GetValue<int>("Email:Smtp:Port", 587);
        var username = _configuration["Email:Smtp:Username"];
        var password = _configuration["Email:Smtp:Password"];

        if (useDevMode || string.IsNullOrWhiteSpace(host))
        {
            _logger.LogWarning("Email dev mode: To={To}, Subject={Subject}, Body={Body}",
                message.To, message.Subject, message.PlainTextContent);
            return true;
        }

        try
        {
            using var client = new SmtpClient(host, port)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(username, password)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromAddress, fromName),
                Subject = message.Subject,
                Body = message.PlainTextContent ?? message.HtmlContent,
                IsBodyHtml = !string.IsNullOrWhiteSpace(message.HtmlContent)
            };

            mailMessage.To.Add(message.To);

            await client.SendMailAsync(mailMessage, cancellationToken);
            _logger.LogInformation("Email sent to {To}", message.To);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}: {Error}", message.To, ex.Message);
            throw;
        }
    }
}
