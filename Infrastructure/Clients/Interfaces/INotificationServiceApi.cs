using Application.Dtos;
using Application.Responses;
using Polly;
using Refit;

namespace Infrastructure.Clients.Interfaces;

public interface INotificationServiceApi
{
    [Post("/Verification/send-code")]
    Task SendVerificationCode([Body] EmailCodeDto dto);
}