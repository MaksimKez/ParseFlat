using Newtonsoft.Json.Linq;

namespace Infrastructure.Email.Interfaces;

public interface IEmailSender
{
    Task SendAsync(JObject emailMessage, CancellationToken cancellationToken);

}