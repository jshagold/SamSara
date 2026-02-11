using UnityEngine;

public class CreateNewInventoryUseCase
{
    private readonly string _logClass = $"{nameof(CreateNewInventoryUseCase)}";

    private readonly IInventoryRepository _inventoryRepo;
    private readonly NewGameConfig _newGameConfig;

    public CreateNewInventoryUseCase(
        IInventoryRepository inventoryRepo,
        NewGameConfig newGameConfig)
    {
        _inventoryRepo = inventoryRepo;
        _newGameConfig = newGameConfig;
    }

    /// <summary>True when no inventory save exists and new game data should be created.</summary>
    public bool IsNewGameRequired() => !_inventoryRepo.HasSaveData();

    public void Execute()
    {
        var newSaveData = new InventorySaveData();

        if (_newGameConfig.InitialMoney > 0)
        {
            newSaveData.ItemList.Add(new ItemSaveData
            {
                Id = ItemConstants.MONEY_ID,
                Count = _newGameConfig.InitialMoney
            });
        }

        _inventoryRepo.InitializeData(newSaveData);
        Debug.Log($"{_logClass} 신규데이터 생성 (초기 자금: {_newGameConfig.InitialMoney})");
    }
}