using LienWorksSharp.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace LienWorksSharp.Services;

public class ClientStore
{
    public List<Client> Clients { get; set; } = new();
}

public class ClientService
{
    private readonly DataPaths _paths;
    private readonly JsonRepository<ClientStore> _repository;
    private readonly long _maxFileSize = 20 * 1024 * 1024;

    public ClientService(DataPaths paths)
    {
        _paths = paths;
        _repository = new JsonRepository<ClientStore>(_paths.ClientsFile);
    }

    public async Task<List<Client>> GetClientsAsync()
    {
        var store = await _repository.ReadAsync();
        return store.Clients.OrderBy(c => c.Name).ToList();
    }

    public async Task<Client?> GetClientAsync(Guid id)
    {
        var store = await _repository.ReadAsync();
        return store.Clients.FirstOrDefault(c => c.Id == id);
    }

    public async Task<Client> AddClientAsync(Client client)
    {
        var store = await _repository.ReadAsync();
        store.Clients.Add(client);
        await _repository.WriteAsync(store);
        return client;
    }

    public async Task UpdateClientAsync(Client client)
    {
        var store = await _repository.ReadAsync();
        var existing = store.Clients.FindIndex(c => c.Id == client.Id);
        if (existing >= 0)
        {
            store.Clients[existing] = client;
            await _repository.WriteAsync(store);
        }
    }

    public async Task<ClientTemplate> SaveClientTemplateAsync(Client client, DocumentType type, IBrowserFile file)
    {
        var store = await _repository.ReadAsync();
        var existing = store.Clients.FirstOrDefault(c => c.Id == client.Id);
        if (existing == null)
        {
            throw new InvalidOperationException("Client not found");
        }

        var template = existing.Templates.FirstOrDefault(t => t.Type == type);
        if (template == null)
        {
            template = new ClientTemplate { Type = type };
            existing.Templates.Add(template);
        }

        var version = await SaveTemplateFileAsync(existing.Id, type, file);
        template.Versions.Insert(0, version);
        template.CurrentId = version.Id;

        await _repository.WriteAsync(store);
        return template;
    }

    public async Task SetClientCurrentTemplateAsync(Guid clientId, DocumentType type, Guid versionId)
    {
        var store = await _repository.ReadAsync();
        var client = store.Clients.FirstOrDefault(c => c.Id == clientId);
        if (client == null)
        {
            return;
        }

        var template = client.Templates.FirstOrDefault(t => t.Type == type);
        if (template == null)
        {
            return;
        }

        if (template.Versions.All(v => v.Id != versionId))
        {
            return;
        }

        template.CurrentId = versionId;
        await _repository.WriteAsync(store);
    }

    public string GetDownloadPath(TemplateVersion version) => $"/data/{version.StoragePath.Replace("\\\\", "/").Replace("\\", "/")}";

    private async Task<TemplateVersion> SaveTemplateFileAsync(Guid clientId, DocumentType type, IBrowserFile file)
    {
        var folder = _paths.ClientTemplateFolder(clientId, type);
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
