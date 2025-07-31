using Infrastructure.Email.Interfaces;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Infrastructure.Email;

public class EmailSender : IEmailSender
{
    private readonly MailjetClient _client;
    private const int MaxRetryAttempts = 3;
    private const int RetryDelayMs = 1000;

    public EmailSender(IOptions<MailjetKeys> keys)
    {
        _client = new MailjetClient(keys.Value.ApiKey, keys.Value.SecretKey);
    }

    public async Task SendAsync(JObject emailMessage, CancellationToken cancellationToken)
    {
        for (int attempt = 1; attempt <= MaxRetryAttempts; attempt++)
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
                    return;

                var error = response.GetErrorMessage();

                if (attempt == MaxRetryAttempts)
                    throw new InvalidOperationException(
                        $"Ошибка отправки письма после {MaxRetryAttempts} попыток: {error}");

                await Task.Delay(RetryDelayMs * (int)Math.Pow(2, attempt - 1), cancellationToken);
            }
            catch (Exception ex) when (attempt < MaxRetryAttempts)
            {
                await Task.Delay(RetryDelayMs * (int)Math.Pow(2, attempt - 1), cancellationToken);
                if (attempt == MaxRetryAttempts - 1)
                    throw;
            }
        }
    }
}


public class MailjetKeys
{
    public string SecretKey { get; set; }
    public string ApiKey { get; set; }
}