using LienWorksSharp.Models;
using LienWorksSharp.Services;
using Microsoft.AspNetCore.Components;

namespace LienWorksSharp.Pages;

public partial class WorkOrdersPage : ComponentBase
{
    [Inject] public WorkOrderService WorkOrdersService { get; set; } = default!;
    [Inject] public ClientService ClientService { get; set; } = default!;

    protected List<WorkOrderViewModel> WorkOrders { get; private set; } = new();
    protected HashSet<Guid> ExpandedIds { get; } = new();

    protected override async Task OnInitializedAsync()
    {
        var clients = await ClientService.GetClientsAsync();
        var workOrders = await WorkOrdersService.GetWorkOrdersAsync();

        WorkOrders = workOrders
            .Select(order =>
            {
                var clientName = clients.FirstOrDefault(c => c.Id == order.ClientId)?.Name ?? "Unknown Client";
                return new WorkOrderViewModel
                {
                    Id = order.Id,
                    Name = order.Name,
                    SuppliedDate = order.SuppliedDate,
                    Status = order.Status,
                    ClientName = clientName,
                    Jobs = order.Jobs
                };
            })
            .OrderByDescending(o => o.SuppliedDate)
            .ToList();
    }

    protected void Toggle(Guid id)
    {
        if (!ExpandedIds.Add(id))
        {
            ExpandedIds.Remove(id);
        }
    }

    protected class WorkOrderViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTimeOffset SuppliedDate { get; set; }
        public WorkStatus Status { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public List<Job> Jobs { get; set; } = new();
    }
}
