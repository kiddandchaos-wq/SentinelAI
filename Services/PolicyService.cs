namespace SentinelAPI.Services;

public record PolicyDecision(bool Allowed, string[] Detected, string[] Violations);

public class PolicyService
{
    // What counts as each kind of sensitive data.
    private static readonly Dictionary<string, string[]> Keywords = new()
    {
        ["salary"] = new[] { "salary", "compensation", "pay grade" },
        ["social_security"] = new[] { "social security", "ssn" },
        ["credit_card"] = new[] { "credit card", "card number" },
        ["credentials"] = new[] { "password", "passcode" }
    };

    // Who may handle what. Any role not listed here gets nothing.
    private static readonly Dictionary<string, string[]> RoleAccess =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["hr"] = new[] { "salary", "social_security" },
            ["finance"] = new[] { "salary", "credit_card" },
            ["it"] = new[] { "credentials" },
            ["employee"] = Array.Empty<string>()
        };

    public PolicyDecision Evaluate(string message, string role)
    {
        var text = message.ToLowerInvariant();

        var detected = Keywords
            .Where(k => k.Value.Any(word => text.Contains(word)))
            .Select(k => k.Key)
            .ToArray();

        var allowed = RoleAccess.TryGetValue(role ?? "", out var categories)
            ? categories
            : Array.Empty<string>();

        var violations = detected.Where(d => !allowed.Contains(d)).ToArray();
        return new PolicyDecision(violations.Length == 0, detected, violations);
    }
}