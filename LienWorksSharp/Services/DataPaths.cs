using LienWorksSharp.Models;
using Microsoft.Extensions.Options;

namespace LienWorksSharp.Services;

public class DataPaths
{
    public DataPaths(IOptions<DataStorageOptions> options)
    {
        Root = options.Value.RootPath;
    }

    public string Root { get; }
    public string ClientsFile => Path.Combine(Root, "clients.json");
    public string WorkOrdersFile => Path.Combine(Root, "workorders.json");
    public string GlobalTemplatesFile => Path.Combine(Root, "templates", "global", "templates.json");

    public string TemplatesFolder => Path.Combine(Root, "templates");
    public string GlobalTemplatesFolder => Path.Combine(TemplatesFolder, "global");
    public string GlobalTemplateFolder(DocumentType type) => Path.Combine(GlobalTemplatesFolder, type.ToString());

    public string ClientFolder(Guid clientId) => Path.Combine(Root, "clients", clientId.ToString());
    public string ClientTemplatesFolder(Guid clientId) => Path.Combine(ClientFolder(clientId), "templates");
    public string ClientTemplateFolder(Guid clientId, DocumentType type) =>
        Path.Combine(ClientTemplatesFolder(clientId), type.ToString());
}
