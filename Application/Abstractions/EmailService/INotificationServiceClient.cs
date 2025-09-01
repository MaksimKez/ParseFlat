using Application.Dtos;
using Application.Responses.Infrastructure;

namespace Application.Abstractions.EmailService;

public interface INotificationServiceClient
{
    Task<EmailServiceResult> SendEmailAsync(
        EmailCodeDto dto,
        CancellationToken cancellationToken = default);
}
