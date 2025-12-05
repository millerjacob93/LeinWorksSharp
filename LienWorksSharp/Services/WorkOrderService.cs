using LienWorksSharp.Models;

namespace LienWorksSharp.Services;

public class WorkOrderStore
{
    public List<WorkOrder> WorkOrders { get; set; } = new();
}

public class WorkOrderService
{
    private readonly JsonRepository<WorkOrderStore> _repository;

    public WorkOrderService(DataPaths paths)
    {
        _repository = new JsonRepository<WorkOrderStore>(paths.WorkOrdersFile);
    }

    public async Task<List<WorkOrder>> GetWorkOrdersAsync()
    {
        var store = await _repository.ReadAsync();
        return store.WorkOrders
            .OrderByDescending(o => o.SuppliedDate)
            .ToList();
    }

    public async Task AddOrUpdateAsync(WorkOrder workOrder)
    {
        var store = await _repository.ReadAsync();
        var existingIndex = store.WorkOrders.FindIndex(o => o.Id == workOrder.Id);
        if (existingIndex >= 0)
        {
            store.WorkOrders[existingIndex] = workOrder;
        }
        else
        {
            store.WorkOrders.Add(workOrder);
        }

        await _repository.WriteAsync(store);
    }
}
