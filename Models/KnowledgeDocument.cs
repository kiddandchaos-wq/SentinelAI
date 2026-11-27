namespace SentinelAPI.Models
{
    public class KnowledgeDocument
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;

        public string FilePath { get; set; } = string.Empty;

        public string Department { get; set; } = string.Empty;

        public string Owner { get; set; } = string.Empty;

        public DateTime UploadedDate { get; set; } = DateTime.UtcNow;
    }
}