namespace recall_roster.Models;

public static class StaffRanks
{
    public const string Employee = "Employee";
    public const string ElementChief = "Element Chief";
    public const string FlightChief = "Flight Chief";
    public const string SquadronDirector = "Squadron Director";
    public static bool IsSupported(string? rank) => rank is Employee or ElementChief or FlightChief or SquadronDirector;
}
