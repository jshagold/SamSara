public class GetItemCountUseCase
{
    private readonly IInventoryRepository _inventoryRepo;
    
    public GetItemCountUseCase(IInventoryRepository inventoryRepo)
    {
        _inventoryRepo = inventoryRepo;
    }

    public int Execute(int itemId)
    {
        return _inventoryRepo.GetItemCount(itemId);
    }
}