public class GetInventoryUseCase
{
    private readonly IInventoryRepository _inventoryRepo;

    public GetInventoryUseCase(IInventoryRepository inventoryRepo)
    {
        _inventoryRepo = inventoryRepo;
    }

    public InventoryInfo Execute()
    {
        return _inventoryRepo.GetInventory();
    }
}