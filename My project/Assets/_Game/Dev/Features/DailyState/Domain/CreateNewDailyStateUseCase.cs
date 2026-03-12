using System.Collections.Generic;
using Core.ErrorHandling;

public class CreateNewDailyStateUseCase
{
    private readonly IDailyStateRepository _dailyStateRepo;
    private readonly NewGameConfig _newGameConfig;
    private readonly IStabilityFlag _stabilityFlag;

    public CreateNewDailyStateUseCase(
        NewGameConfig newGameConfig,
        IDailyStateRepository dailyStateRepo,
        IStabilityFlag stabilityFlag = null)
    {
        _newGameConfig = newGameConfig;
        _dailyStateRepo = dailyStateRepo;
        _stabilityFlag = stabilityFlag;
    }

    /// <summary>True when no daily state save exists and new game data should be created.</summary>
    public bool IsNewGameRequired() => !_dailyStateRepo.HasSaveData();

    public void Execute()
    {
        if (_stabilityFlag != null && !_stabilityFlag.IsSaveAllowed) return;

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