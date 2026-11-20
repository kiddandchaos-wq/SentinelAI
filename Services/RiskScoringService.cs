namespace SentinelAPI.Services;

public class RiskScoringService
{
    public int CalculateRisk(string text)
    {
        text = text.ToLower();

        if (text.Contains("salary"))
            return 90;

        if (text.Contains("social security"))
            return 100;

        if (text.Contains("credit card"))
            return 100;

        if (text.Contains("password"))
            return 80;

        return 10;
    }
}