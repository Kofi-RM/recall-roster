namespace recall_roster.Services;

// Narrow provider boundary shared by normal recalls and the verified demo.
public interface ISmsSender
{
    void Send(string recipient, string body);
    Task SendAsync(string recipient, string body);
}
