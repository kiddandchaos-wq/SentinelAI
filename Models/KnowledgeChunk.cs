namespace SentinelAPI.Models;

public class KnowledgeChunk
{
    public int Id { get; set; }

    public int KnowledgeDocumentId { get; set; }

    public string Content { get; set; } = string.Empty;
}