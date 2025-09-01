using Application.Abstractions.EmailService;
using Application.Dtos;
using Application.Responses.Infrastructure;
using Infrastructure.Clients.Interfaces;
using Polly;
using Refit;

namespace Infrastructure.Clients;

public class NotificationServiceClient(
    HttpClient httpClient,
    ResiliencePipeline resiliencePipeline,
    INotificationServiceApi notificationServiceApi)
    : BaseHttpService(httpClient, resiliencePipeline), INotificationServiceClient
{
    public async Task<EmailServiceResult> SendEmailAsync(EmailCodeDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            await resiliencePipeline.ExecuteAsync(async token =>
                await notificationServiceApi.SendVerificationCode(dto), cancellationToken);

            return new EmailServiceResult
            {
                IsSuccess = true,
                ErrorMessage = null
            };
        }
        catch (ApiException apiEx)
        {
            return new EmailServiceResult
            {
                IsSuccess = false,
                ErrorMessage = FormatApiError(apiEx)
            };
        }
        catch (Exception e)
        {
            return new EmailServiceResult
            {
                IsSuccess = false,
                ErrorMessage = $"Unexpected error: {e.Message}"
            };
        }
    }
    
    private static string FormatApiError(ApiException apiEx)
        {
            var errorBody = apiEx.Content ?? string.Empty;
            return string.IsNullOrWhiteSpace(errorBody)
                ? $"HTTP {(int)apiEx.StatusCode} {apiEx.ReasonPhrase}"
                : $"HTTP {(int)apiEx.StatusCode} {apiEx.ReasonPhrase}: {errorBody}";
        }
}