using UnityEngine;

public class InitializeUserDataUseCase
{
    private readonly string _logClass = $"[{nameof(InitializeUserDataUseCase)}]";

    private readonly GameContext _gameContext;
    private readonly NewGameConfig _newGameConfig;

    public InitializeUserDataUseCase(
        GameContext gameContext,
        NewGameConfig newGameConfig)
    {
        _gameContext = gameContext;
        _newGameConfig = newGameConfig;
    }

    public void Execute()
    {
        if (!_gameContext.CharacterRepo.HasSaveData())
        {
            Debug.Log($"{_logClass} 캐릭터 세이브 데이터 없음 -> 신규 데이터 생성");

            new CreateNewCharacterUseCase(
                characterRepo: _gameContext.CharacterRepo,
                characterMasterRepo: _gameContext.MasterDataManager.CharacterRepo,
                newGameConfig: _newGameConfig
            ).Execute();
        }

        if(!_gameContext.InventoryRepo.HasSaveData())
        {
            Debug.Log($"{_logClass} 인벤토리 세이브 데이터 없음 -> 신규 데이터 생성");

            new CreateNewInventoryUseCase(
                inventoryRepo: _gameContext.InventoryRepo,
                newGameConfig: _newGameConfig
            ).Execute();
        }

        if (!_gameContext.DailyStateRepo.HasSaveData())
        {
            Debug.Log($"{_logClass} Daily 세이브 데이터 없음 -> 신규 데이터 생성");

            new CreateNewDailyStateUseCase(
                newGameConfig: _newGameConfig,
                dailyStateRepo: _gameContext.DailyStateRepo
            ).Execute();
        }
    }
}