namespace LienWorksSharp.Models;

public enum DocumentType
{
    OwnerReport,
    NoticeOfFurnishing,
    ClaimOfLien
}

public static class DocumentTypeExtensions
{
    public static string ToDisplayName(this DocumentType type) =>
        type switch
        {
            DocumentType.OwnerReport => "Owner Report",
            DocumentType.NoticeOfFurnishing => "Notice of Furnishing",
            DocumentType.ClaimOfLien => "Claim of Lien",
            _ => type.ToString()
        };
}
