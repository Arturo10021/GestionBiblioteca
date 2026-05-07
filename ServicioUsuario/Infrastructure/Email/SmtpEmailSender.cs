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
        var host = _configuration["Email:SmtpHost"];
        var portStr = _configuration["Email:SmtpPort"];
        var from = _configuration["Email:From"] ?? "biblioteca@starbook.com";
        var username = _configuration["Email:Username"];
        var password = _configuration["Email:Password"];

        // If no SMTP config, log to console (development fallback)
        if (string.IsNullOrWhiteSpace(host))
        {
            _logger.LogWarning("SMTP not configured. Email to {To} logged to console:\nSubject: {Subject}\nBody: {Body}",
                message.To, message.Subject, message.PlainTextContent);
            return true;
        }

        try
        {
            using var client = new SmtpClient(host, int.TryParse(portStr, out var port) ? port : 587)
            {
                EnableSsl = true,
                Credentials = string.IsNullOrWhiteSpace(username)
                    ? null
                    : new NetworkCredential(username, password)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(from),
                Subject = message.Subject,
                Body = message.PlainTextContent ?? message.HtmlContent,
                IsBodyHtml = !string.IsNullOrWhiteSpace(message.HtmlContent)
            };

            mailMessage.To.Add(message.To);

            await client.SendMailAsync(mailMessage, cancellationToken);
            _logger.LogInformation("Email sent successfully to {To}", message.To);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", message.To);
            throw;
        }
    }
}
