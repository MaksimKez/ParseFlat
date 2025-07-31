using Newtonsoft.Json.Linq;

namespace Infrastructure.Email.Interfaces;

public interface IEmailBuilder
{
    IEmailBuilder SetFrom(string email, string name);
    IEmailBuilder SetTo(string email, string name);
    IEmailBuilder SetSubject(string subject);
    IEmailBuilder SetTextBody(string text);

    JObject Build();
}