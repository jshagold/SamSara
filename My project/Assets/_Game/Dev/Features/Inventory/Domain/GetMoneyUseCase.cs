using System;

public class GetMoneyUseCase
{
    private readonly IInventoryRepository _inventoryRepo;

    public event Action OnInventoryChanged
    {
        add => _inventoryRepo.OnInventoryChanged += value;
        remove => _inventoryRepo.OnInventoryChanged -= value;
    }

    public GetMoneyUseCase(IInventoryRepository inventoryRepo) 
    {
        _inventoryRepo = inventoryRepo;
    }
    
    /// <summary>
    /// 현재 소지금을 가져온다
    /// </summary>
    /// <returns></returns>
    public int GetInventoryMoneyAsync()
    {

        return _inventoryRepo.GetItemCount(ItemConstants.MONEY_ID);
    }
}