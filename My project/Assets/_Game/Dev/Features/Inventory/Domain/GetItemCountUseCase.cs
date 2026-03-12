using System;

public class GetItemCountUseCase
{
    private readonly IInventoryRepository _inventoryRepo;

    public event Action OnInventoryChanged
    {
        add => _inventoryRepo.OnInventoryChanged += value;
        remove => _inventoryRepo.OnInventoryChanged -= value;
    }

    public GetItemCountUseCase(IInventoryRepository inventoryRepo)
    {
        _inventoryRepo = inventoryRepo;
    }

    public int Execute(int itemId)
    {
        return _inventoryRepo.GetItemCount(itemId);
    }
}