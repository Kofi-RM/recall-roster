using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace recall_roster.Services;

public sealed class TwilioSmsSender(IConfiguration configuration) : ISmsSender
{
    private string Required(string key) => !string.IsNullOrWhiteSpace(configuration[key])
        ? configuration[key]! : throw new InvalidOperationException("SMS configuration is incomplete.");
    private TwilioRestClient Client() => new(Required("Twilio:AccountSid"), Required("Twilio:AuthToken"));
    private CreateMessageOptions Options(string recipient, string body) => new(new PhoneNumber(PhoneNumbers.Normalize(recipient)))
    {
        From = new PhoneNumber(Required("Twilio:FromNumber")), Body = body
    };
    public void Send(string recipient, string body) => MessageResource.Create(Options(recipient, body), client: Client());
    public async Task SendAsync(string recipient, string body) => await MessageResource.CreateAsync(Options(recipient, body), client: Client());
}
