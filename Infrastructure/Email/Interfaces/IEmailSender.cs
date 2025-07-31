using Infrastructure.Email.Results;
using Newtonsoft.Json.Linq;

namespace Infrastructure.Email.Interfaces;

public interface IEmailSender
{
    Task<EmailSenderResult> SendAsync(JObject emailMessage, CancellationToken cancellationToken);

}