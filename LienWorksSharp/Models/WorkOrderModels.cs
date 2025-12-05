namespace LienWorksSharp.Models;

public enum WorkStatus
{
    New,
    PendingDocuments,
    Research,
    ReadyToFile,
    Completed,
    OnHold
}

public class Job
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Address { get; set; } = string.Empty;
    public string County { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;
    public List<DocumentType> RequiredDocuments { get; set; } = new();
    public WorkStatus Status { get; set; } = WorkStatus.New;
}

public class WorkOrder
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ClientId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTimeOffset SuppliedDate { get; set; } = DateTimeOffset.UtcNow;
    public WorkStatus Status { get; set; } = WorkStatus.New;
    public List<Job> Jobs { get; set; } = new();
}
