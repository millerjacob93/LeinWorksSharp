namespace LienWorksSharp.Models;

public static class StatusHelpers
{
    public static string ToBadgeClass(this WorkStatus status) =>
        status switch
        {
            WorkStatus.Completed => "bg-success",
            WorkStatus.ReadyToFile => "bg-primary",
            WorkStatus.Research => "bg-info",
            WorkStatus.PendingDocuments => "bg-warning text-dark",
            WorkStatus.OnHold => "bg-secondary",
            _ => "bg-light text-dark"
        };

    public static string ToDisplayName(this WorkStatus status) =>
        status switch
        {
            WorkStatus.PendingDocuments => "Pending Docs",
            WorkStatus.ReadyToFile => "Ready to File",
            WorkStatus.OnHold => "On Hold",
            _ => status.ToString()
        };
}
