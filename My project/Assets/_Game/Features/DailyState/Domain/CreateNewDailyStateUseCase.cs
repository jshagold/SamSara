using System.Collections.Generic;

public class CreateNewDailyStateUseCase
{
    private readonly IDailyStateRepository _dailyStateRepo;
    private readonly NewGameConfig _newGameConfig;

    public CreateNewDailyStateUseCase(
        NewGameConfig newGameConfig,
        IDailyStateRepository dailyStateRepo)
    {
        _newGameConfig = newGameConfig;
        _dailyStateRepo = dailyStateRepo;
    }

    public void Execute()
    {
        var initialMap = new Dictionary<int, bool[]>();
        int characterId = _newGameConfig.StartingCharacterId;

        bool[] slotList = new bool[_newGameConfig.DefaultActionSlots];
        for (int i = 0; i < slotList.Length; i++) slotList[i] = true;

        initialMap.Add(characterId, slotList);

        var saveData = new DailyStateSaveData
        {
            currentDay = _newGameConfig.StartDay,
            characterActionMap = initialMap
        };

        _dailyStateRepo.InitializeData(initData: saveData);
    }
}