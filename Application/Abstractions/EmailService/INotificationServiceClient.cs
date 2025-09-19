using Application.Dtos;
using Application.Responses;

namespace Application.Abstractions.EmailService;

public interface INotificationServiceClient
{
    Task<Result> SendEmailAsync(
        EmailCodeDto dto,
        CancellationToken cancellationToken = default);
}
