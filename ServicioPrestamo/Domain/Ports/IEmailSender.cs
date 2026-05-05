using ServicioPrestamo.Domain.Common;

namespace ServicioPrestamo.Domain.Ports;

public interface IEmailSender
{
    Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default);
}
