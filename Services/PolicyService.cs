using System.Text.RegularExpressions;

namespace SentinelAPI.Services;

public record PolicyDecision(bool Allowed, string[] Detected, string[] Violations);

public class PolicyService
{
    // Words in a QUESTION that signal a sensitive topic.
    private static readonly Dictionary<string, string[]> Keywords = new()
    {
        ["salary"] = new[] { "salary", "compensation", "pay grade" },
        ["social_security"] = new[] { "social security", "ssn" },
        ["credit_card"] = new[] { "credit card", "card number" },
        ["credentials"] = new[] { "password", "passcode" }
    };

    // Actual sensitive VALUES that can appear in any text.
    private static readonly Regex SsnPattern = new(@"\b\d{3}-\d{2}-\d{4}\b", RegexOptions.Compiled);
    private static readonly Regex CardPattern = new(@"\b(?:\d[ -]?){13,19}\b", RegexOptions.Compiled);
    private static readonly Regex MoneyPattern = new(@"\$\s?\d{1,3}(?:,\d{3})+\b", RegexOptions.Compiled);

    // Who may handle what. Any role not listed gets nothing.
    private static readonly Dictionary<string, string[]> RoleAccess =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["hr"] = new[] { "salary", "social_security" },
            ["finance"] = new[] { "salary", "credit_card" },
            ["it"] = new[] { "credentials" },
            ["employee"] = Array.Empty<string>()
        };

    // For questions: keywords plus values.
    public string[] Detect(string text)
    {
        var lower = text.ToLowerInvariant();
        var found = new HashSet<string>();
        foreach (var (category, words) in Keywords)
            if (words.Any(w => lower.Contains(w))) found.Add(category);
        found.UnionWith(DetectValues(text));
        return found.ToArray();
    }

    // For replies: values only, so "I can't discuss salaries" isn't flagged.
    public string[] DetectValues(string text)
    {
        var found = new HashSet<string>();
        if (SsnPattern.IsMatch(text)) found.Add("social_security");
        if (MoneyPattern.IsMatch(text)) found.Add("salary");
        foreach (Match m in CardPattern.Matches(text))
            if (PassesLuhn(m.Value)) { found.Add("credit_card"); break; }
        return found.ToArray();
    }

    public PolicyDecision Evaluate(string message, string role) => Decide(Detect(message), role);

    public PolicyDecision EvaluateReply(string reply, string role) => Decide(DetectValues(reply), role);

    private static PolicyDecision Decide(string[] detected, string role)
    {
        var allowed = RoleAccess.TryGetValue(role ?? "", out var categories)
            ? categories
            : Array.Empty<string>();
        var violations = detected.Where(d => !allowed.Contains(d)).ToArray();
        return new PolicyDecision(violations.Length == 0, detected, violations);
    }

    private static bool PassesLuhn(string candidate)
    {
        var digits = candidate.Where(char.IsDigit).Select(c => c - '0').ToArray();
        if (digits.Length < 13 || digits.Length > 19) return false;

        int sum = 0;
        bool doubleIt = false;
        for (int i = digits.Length - 1; i >= 0; i--)
        {
            int d = digits[i];
            if (doubleIt) { d *= 2; if (d > 9) d -= 9; }
            sum += d;
            doubleIt = !doubleIt;
        }
        return sum % 10 == 0;
    }
}