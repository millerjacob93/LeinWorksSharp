using LienWorksSharp.Models;
using LienWorksSharp.Services;
using Microsoft.AspNetCore.Components;

namespace LienWorksSharp.Pages;

public partial class ClientsPage : ComponentBase
{
    [Inject] public ClientService ClientService { get; set; } = default!;
    [Inject] public TemplateService TemplateService { get; set; } = default!;
    [Inject] public NavigationManager NavigationManager { get; set; } = default!;

    protected List<Client> Clients { get; private set; } = new();
    protected List<DocumentType> DocumentTypes { get; private set; } = Enum.GetValues<DocumentType>().ToList();
    protected bool ShowAddModal { get; private set; }
    protected Client NewClient { get; private set; } = new();
    protected Dictionary<DocumentType, bool> docSelections = new();
    private readonly Dictionary<DocumentType, string?> _globalTemplateLinks = new();

    protected override async Task OnInitializedAsync()
    {
        ResetSelections();
        await LoadGlobalTemplateLinksAsync();
        Clients = await ClientService.GetClientsAsync();
    }

    protected Contact? GetPrimaryContact(Client client) => client.Contacts.FirstOrDefault(c => c.IsPrimary);

    protected ClientTemplateView? GetClientTemplate(Client client, DocumentType type)
    {
        var template = client.Templates.FirstOrDefault(t => t.Type == type);
        if (template?.Current == null)
        {
            return null;
        }

        return new ClientTemplateView
        {
            Version = template.Current,
            DownloadUrl = ClientService.GetDownloadPath(template.Current)
        };
    }

    protected string? GetGlobalTemplateUrl(DocumentType type)
    {
        return _globalTemplateLinks.GetValueOrDefault(type);
    }

    protected void OpenAddModal()
    {
        ResetSelections();
        NewClient = new Client();
        ShowAddModal = true;
    }

    protected void CloseAddModal()
    {
        ShowAddModal = false;
    }

    protected async Task SaveClientAsync()
    {
        NewClient.DefaultRequiredDocuments = docSelections.Where(kv => kv.Value).Select(kv => kv.Key).ToList();
        await ClientService.AddClientAsync(NewClient);
        Clients = await ClientService.GetClientsAsync();
        ShowAddModal = false;
        StateHasChanged();
    }

    protected void GoToClient(Guid id) => NavigationManager.NavigateTo($"/clients/{id}");

    private async Task LoadGlobalTemplateLinksAsync()
    {
        foreach (var doc in DocumentTypes)
        {
            var version = await TemplateService.GetCurrentGlobalTemplateAsync(doc);
            _globalTemplateLinks[doc] = version != null ? TemplateService.GetDownloadPath(version) : null;
        }
    }

    private void ResetSelections()
    {
        docSelections = DocumentTypes.ToDictionary(d => d, _ => false);
    }

    protected class ClientTemplateView
    {
        public TemplateVersion Version { get; set; } = default!;
        public string? DownloadUrl { get; set; }
    }
}
