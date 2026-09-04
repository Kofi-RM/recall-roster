namespace recall_roster.Services;

public static class PhoneNumbers
{
    // Existing roster numbers are North American ten-digit numbers. Preserve explicit international codes.
    public static string Normalize(string value)
    {
        var digits = new string(value.Where(char.IsAsciiDigit).ToArray());
        if (digits.Length == 10) return "+1" + digits;
        if (digits.Length == 11 && digits.StartsWith("1")) return "+" + digits;
        if (value.TrimStart().StartsWith("+") && digits.Length is >= 8 and <= 15) return "+" + digits;
        throw new ArgumentException("A valid phone number is required.");
    }
}
