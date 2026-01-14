using System;

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

    public event Action OnInventoryChanged
    {
        add => _inventoryRepo.OnInventoryChanged += value;
        remove => _inventoryRepo.OnInventoryChanged -= value;
    }
}