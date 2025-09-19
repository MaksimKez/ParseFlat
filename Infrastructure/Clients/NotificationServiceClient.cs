using Application.Abstractions.EmailService;
using Application.Dtos;
using Application.Responses;
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
    public async Task<Result> SendEmailAsync(EmailCodeDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            await resiliencePipeline.ExecuteAsync(async token =>
                await notificationServiceApi.SendVerificationCode(dto), cancellationToken);

            return Result.Success();
        }
        catch (ApiException apiEx)
        {
            return Result.Failure(FormatApiError(apiEx));
        }
        catch (Exception e)
        {
            return Result.Failure($"Unexpected error: {e.Message}");
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