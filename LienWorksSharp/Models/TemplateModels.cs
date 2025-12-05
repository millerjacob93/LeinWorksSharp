using System.Text.Json.Serialization;

namespace LienWorksSharp.Models;

public class TemplateVersion
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DocumentType Type { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string StoragePath { get; set; } = string.Empty;
    public DateTimeOffset UploadedAt { get; set; } = DateTimeOffset.UtcNow;
}

public class TemplateHistory
{
    public DocumentType Type { get; set; }
    public Guid CurrentId { get; set; }
    public List<TemplateVersion> Versions { get; set; } = new();

    [JsonIgnore]
    public TemplateVersion? Current => Versions.FirstOrDefault(v => v.Id == CurrentId);
}

public class TemplateLibrary
{
    public List<TemplateHistory> GlobalTemplates { get; set; } = new();
}
