using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameContext
{
    private readonly string _logClass = $"[{nameof(GameContext)}]";

    // [Repositories]
    public IInventoryRepository InventoryRepo { get; }
    public ICharacterRepository CharacterRepo { get; }
    public IDailyStateRepository DailyStateRepo { get; }

    // [UseCases]
    public GetMoneyUseCase GetMoneyUseCase { get; }
    public DailyStateUseCase DailyStateUseCase { get; }
    public GetCharacterSummaryUseCase GetCharacterSummaryUseCase { get; }
    public GetCharacterDetailUseCase GetCharacterDetailUseCase { get; }
    public GetSkillListUseCase GetSkillListUseCase { get; }
    public GetInventoryUseCase GetInventoryUseCase { get; }
    public GetEvolutionTreeUseCase GetEvolutionTreeUseCase { get; }

    // [MasterDataManager]
    public MasterDataManager MasterDataManager { get; }

    // [App Systems] (Constitution §6: access via GameContext, not Singleton)
    public PopupManager PopupManager { get; }

    public GameContext(
        IInventoryRepository inventoryRepo,
        ICharacterRepository characterRepo,
        IDailyStateRepository dailyStateRepo,
        MasterDataManager masterDataManager,
        PopupManager popupManager)
    {
        MasterDataManager = masterDataManager ?? throw new ArgumentNullException(nameof(masterDataManager));
        PopupManager = popupManager ?? throw new ArgumentNullException(nameof(popupManager));

        InventoryRepo = inventoryRepo;
        CharacterRepo = characterRepo;
        DailyStateRepo = dailyStateRepo;

        GetMoneyUseCase = new GetMoneyUseCase(inventoryRepo: InventoryRepo);
        DailyStateUseCase = new DailyStateUseCase(gameStateRepository: DailyStateRepo);
        GetCharacterSummaryUseCase = new GetCharacterSummaryUseCase(
            characterRepo: CharacterRepo,
            characterMasterRepo: masterDataManager.CharacterRepo,
            dailyStateRepo: DailyStateRepo
        );
        GetCharacterDetailUseCase = new GetCharacterDetailUseCase(
            characterRepo: characterRepo,
            characterMasterRepo: masterDataManager.CharacterRepo
        );
        GetSkillListUseCase = new GetSkillListUseCase(
            characterRepo: characterRepo,
            characterMasterRepo: masterDataManager.CharacterRepo
        );
        GetInventoryUseCase = new GetInventoryUseCase(
            inventoryRepo: InventoryRepo,
            itemMasterRepo: masterDataManager.ItemRepo
        );
        GetEvolutionTreeUseCase = new GetEvolutionTreeUseCase(
            characterRepository: characterRepo,
            characterMasterRepository: masterDataManager.CharacterRepo);
    }

    public async UniTask LoadAllDataAsync()
    {
        try
        {
            var taskInventory = InventoryRepo.LoadDataAsync();
            var taskCharacter = CharacterRepo.LoadDataAsync();
            var taskDailyState = DailyStateRepo.LoadDataAsync();

            await UniTask.WhenAll(taskInventory, taskCharacter, taskDailyState);
        }
        catch (Exception e)
        {
            Debug.LogError($"{_logClass} LoadAllDataAsync failed: {e}");
            throw;
        }
    }
}