using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace SentinelAPI.Services;

public interface IAiService
{
    Task<string> AskAsync(string userMessage, CancellationToken ct = default);
}

// Free stand-in: scripted answers from a fake (synthetic) company directory.
public class MockAiService : IAiService
{
    public Task<string> AskAsync(string userMessage, CancellationToken ct = default)
    {
        var t = userMessage.ToLowerInvariant();
        bool jordan = t.Contains("jordan");
        bool priya = t.Contains("priya") || t.Contains("hr director");
        bool pay = new[] { "earn", "salary", "paid", "make", "pay", "compensation" }.Any(w => t.Contains(w));
        bool ssn = t.Contains("ssn") || t.Contains("social");
        bool card = t.Contains("card");

        string reply;
        if (ssn && jordan) reply = "Jordan Lee's SSN on file is 900-12-3456.";
        else if (ssn && priya) reply = "Priya Shah's SSN on file is 900-45-6789.";
        else if (pay && jordan) reply = "Jordan Lee, Engineering Manager, earns $185,000 per year.";
        else if (pay && priya) reply = "Priya Shah, HR Director, earns $162,000 per year.";
        else if (card) reply = "The company card on file is 4111 1111 1111 1111.";
        else if (jordan) reply = "Jordan Lee is an Engineering Manager at Acme.";
        else if (priya) reply = "Priya Shah is the HR Director at Acme, responsible for hiring, benefits, and employee relations.";
        else reply = "I can answer questions about Acme's directory, such as who works where and what they do.";

        return Task.FromResult(reply);
    }
}

// Real AI. Used automatically if you ever save an Anthropic:ApiKey secret.
public class AnthropicAiService(HttpClient http, IConfiguration config) : IAiService
{
    private const string SystemPrompt =
        "You are Acme Corp's internal assistant. Answer in under 60 words, using this directory. " +
        "Directory (synthetic demo data): " +
        "Jordan Lee, Engineering Manager, salary $185,000, SSN 900-12-3456. " +
        "Priya Shah, HR Director, salary $162,000, SSN 900-45-6789. " +
        "Company card on file: 4111 1111 1111 1111.";

    public async Task<string> AskAsync(string userMessage, CancellationToken ct = default)
    {
        var key = config["Anthropic:ApiKey"];
        if (string.IsNullOrWhiteSpace(key))
            throw new InvalidOperationException("Anthropic:ApiKey is not configured.");

        var model = config["Anthropic:Model"] ?? "claude-haiku-5-5";

        using var req = new HttpRequestMessage(HttpMethod.Post, "https://api.anthropic.com/v1/messages");
        req.Headers.Add("x-api-key", key);
        req.Headers.Add("anthropic-version", "2023-06-01");
        req.Content = JsonContent.Create(new
        {
            model,
            max_tokens = 300,
            system = SystemPrompt,
            messages = new[] { new { role = "user", content = userMessage } }
        });

        using var res = await http.SendAsync(req, ct);
        var body = await res.Content.ReadAsStringAsync(ct);
        if (!res.IsSuccessStatusCode)
            throw new HttpRequestException($"Anthropic API returned HTTP {(int)res.StatusCode}.");

        using var doc = JsonDocument.Parse(body);
        var text = new StringBuilder();
        foreach (var block in doc.RootElement.GetProperty("content").EnumerateArray())
            if (block.GetProperty("type").GetString() == "text")
                text.Append(block.GetProperty("text").GetString());
        return text.ToString();
    }
}