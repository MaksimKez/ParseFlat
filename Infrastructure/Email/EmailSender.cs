using Infrastructure.Email.Configs;
using Infrastructure.Email.Interfaces;
using Infrastructure.Email.Results;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Infrastructure.Email;

public class EmailSender : IEmailSender
{
    private readonly MailjetClient _client;
    private readonly ILogger<EmailSender> _logger;
    private const int MaxRetryAttempts = 3;
    private const int RetryDelayMs = 1000;

    public EmailSender(IOptions<MailjetKeys> keys, ILogger<EmailSender> logger)
    {
        _client = new MailjetClient(keys.Value.ApiKey, keys.Value.SecretKey);
        _logger = logger;
    }

    public async Task<EmailSenderResult> SendAsync(JObject emailMessage, CancellationToken cancellationToken)
    {
        for (var attempt = 1; attempt <= MaxRetryAttempts; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var request = new MailjetRequest
                {
                    Resource = SendV31.Resource
                }.Property(Send.Messages, emailMessage["Messages"]);

                var response = await _client.PostAsync(request);

                if (response.IsSuccessStatusCode)
                    return EmailSenderResult.Success();

                var error = response.GetErrorMessage();
                _logger.LogWarning("Email send failed with status {StatusCode} on attempt {Attempt}: {Error}",
                    response.StatusCode, attempt, error);

                if (response.StatusCode is >= 400 and < 500)
                    return EmailSenderResult.Failure(error, attempt);

                if (attempt == MaxRetryAttempts)
                    return EmailSenderResult.Failure(error, attempt);
            }
            catch (Exception ex) when (attempt < MaxRetryAttempts)
            {
                _logger.LogWarning(ex, "Exception on attempt {Attempt}, will retry", attempt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception on attempt {Attempt}", attempt);
                return EmailSenderResult.Failure(ex.Message, attempt);
            }

            //1st try - 1s, 2nd - 2s, 3rd - 4s
            var delayMs = RetryDelayMs * (int)Math.Pow(2, attempt - 1);
            _logger.LogInformation("Delaying for {Delay}ms before next attempt", delayMs);
            await Task.Delay(delayMs, cancellationToken);
        }

        _logger.LogError("Exceeded maximum retry attempts ({MaxRetryAttempts})", MaxRetryAttempts);
        return EmailSenderResult.Failure("Exceeded retry attempts", MaxRetryAttempts);
    }
}

