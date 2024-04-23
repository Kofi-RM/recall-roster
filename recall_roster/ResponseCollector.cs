using System.Timers;
public static class ResponseCollector
{
    private static System.Timers.Timer timer = new System.Timers.Timer(10000); // Sets the interval to 10 seconds
    private static Dictionary<string, int> responses = new Dictionary<string, int>();

    static ResponseCollector()
    {
        responses["Squadron Director"] = 0;
        responses["Flight Chief"] = 0;
        responses["Element Chief"] = 0;
        responses["Employee"] = 0;

        timer.Elapsed += OnTimedEvent; // Subscribe to the Elapsed event
        timer.AutoReset = false; // Ensure the timer runs only once
        timer.Enabled = true; // Start the timer
    }

    private static void OnTimedEvent(Object source, ElapsedEventArgs e)
    {
        DisplayResults();
        timer.Stop(); // Stop the timer after processing
    }

    public static void AddResponse(string rank)
    {
        if (responses.ContainsKey(rank))
        {
            responses[rank]++;
        }
    }

    private static void DisplayResults()
    {
        foreach (var rank in responses.Keys)
        {
            Console.WriteLine($"{rank}: {responses[rank]}");
        }
    }

    public static void StartNewSession()
    {
        // Reset counts and restart timer
        foreach (var key in responses.Keys.ToList())
        {
            responses[key] = 0;
        }
        timer.Start();
    }
}
