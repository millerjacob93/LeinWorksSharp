namespace LienWorksSharp.Models;

public class Contact
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
}

public class ClientTemplate
{
    public DocumentType Type { get; set; }
    public Guid CurrentId { get; set; }
    public List<TemplateVersion> Versions { get; set; } = new();

    public TemplateVersion? Current => Versions.FirstOrDefault(v => v.Id == CurrentId);
}

public class Client
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public List<DocumentType> DefaultRequiredDocuments { get; set; } = new();
    public List<Contact> Contacts { get; set; } = new();
    public List<ClientTemplate> Templates { get; set; } = new();
}
