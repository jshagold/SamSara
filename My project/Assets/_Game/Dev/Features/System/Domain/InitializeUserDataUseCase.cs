using UnityEngine;

public class InitializeUserDataUseCase
{
    private readonly string _logClass = $"[{nameof(InitializeUserDataUseCase)}]";

    private readonly CreateNewCharacterUseCase _createNewCharacter;
    private readonly CreateNewInventoryUseCase _createNewInventory;
    private readonly CreateNewDailyStateUseCase _createNewDailyState;

    public InitializeUserDataUseCase(
        CreateNewCharacterUseCase createNewCharacter,
        CreateNewInventoryUseCase createNewInventory,
        CreateNewDailyStateUseCase createNewDailyState)
    {
        _createNewCharacter = createNewCharacter ?? throw new System.ArgumentNullException(nameof(createNewCharacter));
        _createNewInventory = createNewInventory ?? throw new System.ArgumentNullException(nameof(createNewInventory));
        _createNewDailyState = createNewDailyState ?? throw new System.ArgumentNullException(nameof(createNewDailyState));
    }

    public void Execute()
    {
        if (_createNewCharacter.IsNewGameRequired())
        {
            Debug.Log($"{_logClass} 캐릭터 세이브 데이터 없음 -> 신규 데이터 생성");
            _createNewCharacter.Execute();
        }

        if (_createNewInventory.IsNewGameRequired())
        {
            Debug.Log($"{_logClass} 인벤토리 세이브 데이터 없음 -> 신규 데이터 생성");
            _createNewInventory.Execute();
        }

        if (_createNewDailyState.IsNewGameRequired())
        {
            Debug.Log($"{_logClass} Daily 세이브 데이터 없음 -> 신규 데이터 생성");
            _createNewDailyState.Execute();
        }
    }
}