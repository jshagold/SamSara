using Core.ErrorHandling;
using UnityEngine;

public class CreateNewInventoryUseCase
{
    private readonly string _logClass = $"{nameof(CreateNewInventoryUseCase)}";

    private readonly IInventoryRepository _inventoryRepo;
    private readonly NewGameConfig _newGameConfig;
    private readonly IStabilityFlag _stabilityFlag;

    public CreateNewInventoryUseCase(
        IInventoryRepository inventoryRepo,
        NewGameConfig newGameConfig,
        IStabilityFlag stabilityFlag = null)
    {
        _inventoryRepo = inventoryRepo;
        _newGameConfig = newGameConfig;
        _stabilityFlag = stabilityFlag;
    }

    /// <summary>True when no inventory save exists and new game data should be created.</summary>
    public bool IsNewGameRequired() => !_inventoryRepo.HasSaveData();

    public void Execute()
    {
        if (_stabilityFlag != null && !_stabilityFlag.IsSaveAllowed) return;

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