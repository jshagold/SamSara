using Cysharp.Threading.Tasks;
using Unity.Profiling;
using UnityEngine.Localization;
using System;

public class GameContext
{
    public IUserInventoryRepository InventoryRepo {  get; }
    public GetMoneyUseCase GetMoneyUseCase { get; }
    public IDailyStateRepository DailyStateRepo {  get; }
    public DailyStateUseCase DailyStateUseCase { get; }

    // TODO GetCharacterSummaryUseCase에 맞는 Repo 구현후에 넣어야함.
    public GetCharacterSummaryUseCase GetCharacterSummaryUseCase { get; }

    public SettingsData Settings { get; private set; } = new SettingsData();

    public bool IsDirty { get; set; } = false;

    public GameContext(
        IUserInventoryRepository inventory, 
        IDailyStateRepository dailyStateRepo)
    {
        InventoryRepo = inventory;
        DailyStateRepo = dailyStateRepo;

        GetMoneyUseCase = new GetMoneyUseCase(inventory);
        DailyStateUseCase = new DailyStateUseCase(DailyStateRepo);
        GetCharacterSummaryUseCase = new GetCharacterSummaryUseCase(dailyStateRepo: DailyStateRepo);
    }

    public async UniTask LoadAllDataAsync()
    {
        var task1 = InventoryRepo.LoadDataAsync();
        var task2 = DailyStateRepo.LoadDataAsync();

        await UniTask.WhenAll(task1, task2);
    }
}

[Serializable]
public class SettingsData
{
    public Locale locale;
    public bool autoBattle = false;
    public bool qteEnabled = true;
    public float bgmVolume = 0.8f;
    public float sfxVolume = 1.0f;
}