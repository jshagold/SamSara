using System;
using System.Collections.Generic;

public class GetInventoryUseCase
{
    private readonly string _logClass = $"{nameof(GetInventoryUseCase)}";
    private readonly IInventoryRepository _inventoryRepo;
    private readonly IItemMasterRepository _itemMasterRepo;

    public GetInventoryUseCase(
        IInventoryRepository inventoryRepo,
        IItemMasterRepository itemMasterRepo)
    {
        _inventoryRepo = inventoryRepo;
        _itemMasterRepo = itemMasterRepo;
    }

    public InventoryInfo Execute()
    {
        var saveData = _inventoryRepo.GetInventory();
        if (saveData == null)
        {
            throw new InvalidOperationException($"{_logClass} saveData Load fail");
        }

        var domainItemList = new List<ItemInfo>();
        foreach (var saveItem in saveData.ItemList)
        {
            var masterItem = _itemMasterRepo.GetData(saveItem.Id);
            if (masterItem != null)
            {
                domainItemList.Add(masterItem.ToDomain(count: saveItem.Count));
            } 
            else
            {
                throw new InvalidOperationException($"{_logClass} ItemMasterData null - charId: {saveItem.Id}");
            }
        }

        return new InventoryInfo
        {
            ItemList = domainItemList,
        };
    }

    public event Action OnInventoryChanged
    {
        add => _inventoryRepo.OnInventoryChanged += value;
        remove => _inventoryRepo.OnInventoryChanged -= value;
    }
}