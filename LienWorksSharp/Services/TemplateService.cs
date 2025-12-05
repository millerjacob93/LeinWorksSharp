using LienWorksSharp.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace LienWorksSharp.Services;

public class TemplateService
{
    private readonly DataPaths _paths;
    private readonly JsonRepository<TemplateLibrary> _repository;
    private readonly long _maxFileSize = 20 * 1024 * 1024;

    public TemplateService(DataPaths paths)
    {
        _paths = paths;
        _repository = new JsonRepository<TemplateLibrary>(_paths.GlobalTemplatesFile);
    }

    public async Task<TemplateHistory> GetGlobalHistoryAsync(DocumentType type)
    {
        var library = await _repository.ReadAsync();
        var history = library.GlobalTemplates.FirstOrDefault(t => t.Type == type);
        if (history != null)
        {
            return history;
        }

        history = new TemplateHistory { Type = type };
        library.GlobalTemplates.Add(history);
        await _repository.WriteAsync(library);
        return history;
    }

    public async Task<TemplateVersion?> GetCurrentGlobalTemplateAsync(DocumentType type)
    {
        var history = await GetGlobalHistoryAsync(type);
        return history.Current;
    }

    public async Task<TemplateHistory> SaveGlobalTemplateAsync(DocumentType type, IBrowserFile file)
    {
        var library = await _repository.ReadAsync();
        var history = library.GlobalTemplates.FirstOrDefault(t => t.Type == type);
        if (history == null)
        {
            history = new TemplateHistory { Type = type };
            library.GlobalTemplates.Add(history);
        }

        var version = await SaveTemplateFileAsync(_paths.GlobalTemplateFolder(type), type, file);
        history.Versions.Insert(0, version);
        history.CurrentId = version.Id;
        await _repository.WriteAsync(library);
        return history;
    }

    public async Task SetGlobalCurrentAsync(DocumentType type, Guid versionId)
    {
        var library = await _repository.ReadAsync();
        var history = library.GlobalTemplates.FirstOrDefault(t => t.Type == type);
        if (history == null)
        {
            return;
        }

        if (history.Versions.All(v => v.Id != versionId))
        {
            return;
        }

        history.CurrentId = versionId;
        await _repository.WriteAsync(library);
    }

    public string GetDownloadPath(TemplateVersion version) => $"/data/{version.StoragePath.Replace("\\\\", "/").Replace("\\", "/")}";

    private async Task<TemplateVersion> SaveTemplateFileAsync(string folder, DocumentType type, IBrowserFile file)
    {
        Directory.CreateDirectory(folder);
        var safeName = Path.GetFileName(file.Name);
        var timestamp = DateTimeOffset.UtcNow.ToString("yyyyMMddHHmmss");
        var targetFileName = $"{timestamp}_{safeName}";
        var targetPath = Path.Combine(folder, targetFileName);

        await using var writeStream = File.Create(targetPath);
        await using var readStream = file.OpenReadStream(_maxFileSize);
        await readStream.CopyToAsync(writeStream);

        return new TemplateVersion
        {
            Type = type,
            FileName = safeName,
            StoragePath = Path.GetRelativePath(_paths.Root, targetPath),
            UploadedAt = DateTimeOffset.UtcNow
        };
    }
}
