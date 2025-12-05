using LienWorksSharp.Models;
using LienWorksSharp.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace LienWorksSharp.Pages;

public partial class GlobalTemplatesPage : ComponentBase
{
    [Inject] public TemplateService TemplateService { get; set; } = default!;

    protected List<DocumentType> DocumentTypes { get; private set; } = Enum.GetValues<DocumentType>().ToList();
    protected Dictionary<DocumentType, TemplateHistory> Histories { get; private set; } = new();
    protected bool ShowUploadModal { get; private set; }
    protected DocumentType? UploadingType { get; private set; }
    protected IBrowserFile? SelectedFile { get; private set; }
    protected string SelectedFileName { get; private set; } = string.Empty;
    protected bool ShowHistoryModal { get; private set; }
    protected DocumentType? HistoryType { get; private set; }

    protected bool CanUpload => SelectedFile != null;

    protected override async Task OnInitializedAsync()
    {
        foreach (var doc in DocumentTypes)
        {
            var history = await TemplateService.GetGlobalHistoryAsync(doc);
            Histories[doc] = history;
        }
    }

    protected TemplateVersion? GetCurrent(DocumentType type) => Histories.GetValueOrDefault(type)?.Current;

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
        if (UploadingType == null || SelectedFile == null)
        {
            return;
        }

        var history = await TemplateService.SaveGlobalTemplateAsync(UploadingType.Value, SelectedFile);
        Histories[UploadingType.Value] = history;
        ShowUploadModal = false;
        SelectedFile = null;
        SelectedFileName = string.Empty;
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
        await TemplateService.SetGlobalCurrentAsync(type, versionId);
        var history = await TemplateService.GetGlobalHistoryAsync(type);
        Histories[type] = history;
        StateHasChanged();
    }
}
