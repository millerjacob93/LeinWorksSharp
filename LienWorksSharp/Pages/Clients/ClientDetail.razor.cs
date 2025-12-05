using LienWorksSharp.Models;
using LienWorksSharp.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace LienWorksSharp.Pages;

public partial class ClientDetailPage : ComponentBase
{
    [Parameter] public Guid Id { get; set; }
    [Inject] public ClientService ClientService { get; set; } = default!;
    [Inject] public TemplateService TemplateService { get; set; } = default!;
    [Inject] public NavigationManager Navigation { get; set; } = default!;

    protected Client? Client { get; private set; }
    protected bool IsLoading { get; private set; } = true;
    protected List<DocumentType> DocumentTypes { get; private set; } = Enum.GetValues<DocumentType>().ToList();
    protected Dictionary<DocumentType, bool> docSelections = new();
    protected bool ShowContactModal { get; private set; }
    protected Contact EditableContact { get; private set; } = new();
    protected string ContactModalTitle { get; private set; } = "Add Contact";
    protected bool IsEditingContact { get; private set; }
    protected bool ShowUploadModal { get; private set; }
    protected DocumentType? UploadingType { get; private set; }
    protected IBrowserFile? SelectedFile { get; private set; }
    protected string SelectedFileName { get; private set; } = string.Empty;
    protected bool ShowHistoryModal { get; private set; }
    protected DocumentType? HistoryType { get; private set; }

    private readonly Dictionary<DocumentType, string?> _globalTemplateLinks = new();

    protected bool CanUpload => SelectedFile != null;

    protected override async Task OnParametersSetAsync()
    {
        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        IsLoading = true;
        Client = await ClientService.GetClientAsync(Id);
        await LoadGlobalTemplateLinksAsync();
        if (Client != null)
        {
            docSelections = DocumentTypes.ToDictionary(d => d, d => Client.DefaultRequiredDocuments.Contains(d));
        }
        IsLoading = false;
    }

    protected ClientTemplate? GetClientTemplate(DocumentType type) => Client?.Templates.FirstOrDefault(t => t.Type == type);

    protected string GetTemplateLink(DocumentType type)
    {
        var template = GetClientTemplate(type);
        if (template?.Current != null)
        {
            return ClientService.GetDownloadPath(template.Current);
        }

        return _globalTemplateLinks.GetValueOrDefault(type) ?? "#";
    }

    protected void GoBack() => Navigation.NavigateTo("/clients");

    protected async Task SaveDetailsAsync()
    {
        if (Client == null)
        {
            return;
        }

        Client.DefaultRequiredDocuments = docSelections.Where(kv => kv.Value).Select(kv => kv.Key).ToList();
        await ClientService.UpdateClientAsync(Client);
        await LoadAsync();
    }

    protected void OpenContactModal(Contact? contact)
    {
        if (Client == null)
        {
            return;
        }

        if (contact == null)
        {
            ContactModalTitle = "Add Contact";
            EditableContact = new Contact();
            IsEditingContact = false;
        }
        else
        {
            ContactModalTitle = "Edit Contact";
            EditableContact = new Contact
            {
                Id = contact.Id,
                Name = contact.Name,
                Email = contact.Email,
                Phone = contact.Phone,
                Role = contact.Role,
                IsPrimary = contact.IsPrimary
            };
            IsEditingContact = true;
        }

        ShowContactModal = true;
    }

    protected void CloseContactModal()
    {
        ShowContactModal = false;
    }

    protected async Task SaveContactAsync()
    {
        if (Client == null)
        {
            return;
        }

        if (EditableContact.IsPrimary)
        {
            foreach (var contact in Client.Contacts)
            {
                contact.IsPrimary = false;
            }
        }

        var existing = Client.Contacts.FirstOrDefault(c => c.Id == EditableContact.Id);
        if (existing != null)
        {
            existing.Name = EditableContact.Name;
            existing.Email = EditableContact.Email;
            existing.Phone = EditableContact.Phone;
            existing.Role = EditableContact.Role;
            existing.IsPrimary = EditableContact.IsPrimary;
        }
        else
        {
            if (!EditableContact.IsPrimary && Client.Contacts.All(c => !c.IsPrimary))
            {
                EditableContact.IsPrimary = true;
            }

            Client.Contacts.Add(EditableContact);
        }

        await ClientService.UpdateClientAsync(Client);
        ShowContactModal = false;
    }

    protected async Task DeleteContact(Guid contactId)
    {
        if (Client == null)
        {
            return;
        }

        Client.Contacts.RemoveAll(c => c.Id == contactId);
        if (Client.Contacts.Any() && Client.Contacts.All(c => !c.IsPrimary))
        {
            Client.Contacts.First().IsPrimary = true;
        }

        await ClientService.UpdateClientAsync(Client);
    }

    protected void OpenUploadModal(DocumentType type)
    {
        UploadingType = type;
        SelectedFile = null;
        SelectedFileName = string.Empty;
        ShowUploadModal = true;
    }

    protected void CloseUploadModal()
    {
        ShowUploadModal = false;
    }

    protected void OnFileSelected(InputFileChangeEventArgs args)
    {
        SelectedFile = args.File;
        SelectedFileName = args.File?.Name ?? string.Empty;
    }

    protected async Task SaveTemplateAsync()
    {
        if (Client == null || SelectedFile == null || UploadingType == null)
        {
            return;
        }

        await ClientService.SaveClientTemplateAsync(Client, UploadingType.Value, SelectedFile);
        Client = await ClientService.GetClientAsync(Client.Id);
        ShowUploadModal = false;
        SelectedFile = null;
        SelectedFileName = string.Empty;
        UploadingType = null;
    }

    protected void OpenHistory(DocumentType type)
    {
        HistoryType = type;
        ShowHistoryModal = true;
    }

    protected void CloseHistory()
    {
        HistoryType = null;
        ShowHistoryModal = false;
    }

    protected async Task UseVersionAsync(DocumentType type, Guid versionId)
    {
        if (Client == null)
        {
            return;
        }

        await ClientService.SetClientCurrentTemplateAsync(Client.Id, type, versionId);
        Client = await ClientService.GetClientAsync(Client.Id);
        StateHasChanged();
    }

    private async Task LoadGlobalTemplateLinksAsync()
    {
        foreach (var doc in DocumentTypes)
        {
            var version = await TemplateService.GetCurrentGlobalTemplateAsync(doc);
            _globalTemplateLinks[doc] = version != null ? TemplateService.GetDownloadPath(version) : null;
        }
    }
}
