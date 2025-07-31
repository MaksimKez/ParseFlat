using Infrastructure.Email.Interfaces;
using Newtonsoft.Json.Linq;

namespace Infrastructure.Email;

public class EmailBuilder : IEmailBuilder
{
    private string fromEmail;
    private string fromName;
    private string toEmail;
    private string toName;
    private string subject;
    private string textBody;

    public IEmailBuilder SetFrom(string email, string name)
    {
        fromEmail = email;
        fromName = name;
        return this;
    }

    public IEmailBuilder SetTo(string email, string name)
    {
        toEmail = email;
        toName = name;
        return this;
    }

    public IEmailBuilder SetSubject(string subject)
    {
        this.subject = subject;
        return this;
    }

    public IEmailBuilder SetTextBody(string text)
    {
        textBody = text;
        return this;
    }

    public JObject Build()
    {
        if (string.IsNullOrEmpty(fromEmail) || string.IsNullOrEmpty(toEmail))
            throw new InvalidOperationException("From and To must be set.");

        var message = new JObject
        {
            ["From"] = new JObject
            {
                ["Email"] = fromEmail,
                ["Name"] = fromName
            },
            ["To"] = new JArray
            {
                new JObject
                {
                    ["Email"] = toEmail,
                    ["Name"] = toName
                }
            },
            ["Subject"] = subject,
            ["TextPart"] = textBody
        };

        return new JObject
        {
            ["Messages"] = new JArray { message }
        };
    }
}
