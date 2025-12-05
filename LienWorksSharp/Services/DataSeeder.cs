using LienWorksSharp.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace LienWorksSharp.Services;

public class DataSeeder
{
    private readonly DataPaths _paths;
    private readonly TemplateService _templates;
    private readonly ClientService _clients;
    private readonly WorkOrderService _workOrders;

    public DataSeeder(DataPaths paths, TemplateService templates, ClientService clients, WorkOrderService workOrders)
    {
        _paths = paths;
        _templates = templates;
        _clients = clients;
        _workOrders = workOrders;
    }

    public async Task EnsureSeededAsync()
    {
        Directory.CreateDirectory(_paths.Root);
        Directory.CreateDirectory(_paths.GlobalTemplatesFolder);

        await SeedTemplatesAsync();
        await SeedClientsAsync();
        await SeedWorkOrdersAsync();
    }

    private async Task SeedTemplatesAsync()
    {
        if (File.Exists(_paths.GlobalTemplatesFile))
        {
            return;
        }

        foreach (var type in Enum.GetValues<DocumentType>())
        {
            var fileName = $"{type.ToDisplayName().Replace(" ", "")}.txt";
            var folder = _paths.GlobalTemplateFolder(type);
            Directory.CreateDirectory(folder);
            var content = $"Sample {type.ToDisplayName()} template for LienWorksSharp.";
            var seedFile = Path.Combine(folder, fileName);
            await File.WriteAllTextAsync(seedFile, content);

            var browserFile = new SeedBrowserFile(fileName, seedFile);
            await _templates.SaveGlobalTemplateAsync(type, browserFile);
        }
    }

    private async Task SeedClientsAsync()
    {
        var existing = await _clients.GetClientsAsync();
        if (existing.Any())
        {
            return;
        }

        var demoClient = new Client
        {
            Name = "Summit Construction",
            Address = "123 Main St, Columbus, OH",
            DefaultRequiredDocuments = new List<DocumentType>
            {
                DocumentType.OwnerReport,
                DocumentType.NoticeOfFurnishing
            },
            Contacts =
            {
                new Contact
                {
                    Name = "Alex Rivera",
                    Email = "alex.rivera@example.com",
                    Phone = "614-555-0101",
                    Role = "Project Manager",
                    IsPrimary = true
                },
                new Contact
                {
                    Name = "Jamie Patel",
                    Email = "jamie.patel@example.com",
                    Phone = "614-555-0102",
                    Role = "Accounts Payable",
                    IsPrimary = false
                }
            }
        };

        var lakeview = new Client
        {
            Name = "Lakeview Builders",
            Address = "44 Harbor Rd, Cleveland, OH",
            DefaultRequiredDocuments = new List<DocumentType> { DocumentType.ClaimOfLien },
            Contacts =
            {
                new Contact
                {
                    Name = "Taylor Reed",
                    Email = "taylor.reed@example.com",
                    Phone = "216-555-2222",
                    Role = "Operations",
                    IsPrimary = true
                }
            }
        };

        await _clients.AddClientAsync(demoClient);
        await _clients.AddClientAsync(lakeview);
    }

    private async Task SeedWorkOrdersAsync()
    {
        var existing = await _workOrders.GetWorkOrdersAsync();
        if (existing.Any())
        {
            return;
        }

        var clients = await _clients.GetClientsAsync();
        if (!clients.Any())
        {
            return;
        }

        var firstClient = clients.First();
        var workOrder = new WorkOrder
        {
            ClientId = firstClient.Id,
            Name = "Warehouse Renovation - Phase 1",
            SuppliedDate = DateTimeOffset.UtcNow.AddDays(-3),
            Status = WorkStatus.PendingDocuments,
            Jobs = new List<Job>
            {
                new Job
                {
                    Address = "100 Commerce Blvd, Columbus, OH",
                    County = "Franklin",
                    Owner = "Commerce Realty",
                    RequiredDocuments = new List<DocumentType>
                    {
                        DocumentType.OwnerReport,
                        DocumentType.NoticeOfFurnishing
                    },
                    Status = WorkStatus.Research
                },
                new Job
                {
                    Address = "200 Commerce Blvd, Columbus, OH",
                    County = "Franklin",
                    Owner = "Commerce Realty",
                    RequiredDocuments = new List<DocumentType>
                    {
                        DocumentType.ClaimOfLien
                    },
                    Status = WorkStatus.New
                }
            }
        };

        var secondOrder = new WorkOrder
        {
            ClientId = clients.Last().Id,
            Name = "Lakeside Apartments - Interiors",
            SuppliedDate = DateTimeOffset.UtcNow.AddDays(-7),
            Status = WorkStatus.ReadyToFile,
            Jobs = new List<Job>
            {
                new Job
                {
                    Address = "88 Shoreline Dr, Cleveland, OH",
                    County = "Cuyahoga",
                    Owner = "Lakeside Holdings",
                    RequiredDocuments = new List<DocumentType> { DocumentType.ClaimOfLien },
                    Status = WorkStatus.ReadyToFile
                }
            }
        };

        await _workOrders.AddOrUpdateAsync(workOrder);
        await _workOrders.AddOrUpdateAsync(secondOrder);
    }

    private sealed class SeedBrowserFile : IBrowserFile
    {
        private readonly string _path;

        public SeedBrowserFile(string name, string path)
        {
            Name = name;
            _path = path;
        }

        public DateTimeOffset LastModified => DateTimeOffset.UtcNow;
        public string Name { get; }
        public long Size => new FileInfo(_path).Length;
        public string ContentType => "text/plain";
        public Stream OpenReadStream(long maxAllowedSize = 512000, CancellationToken cancellationToken = default)
        {
            return File.OpenRead(_path);
        }
    }
}
