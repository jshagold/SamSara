using Cysharp.Threading.Tasks;
using Unity.Profiling;

public class GameContext
{
    // [Repositories]
    public IInventoryRepository InventoryRepo { get; }
    public ICharacterRepository CharacterRepo { get; }
    public IDailyStateRepository DailyStateRepo { get; }

    // [UseCases]
    public GetMoneyUseCase GetMoneyUseCase { get; }
    public DailyStateUseCase DailyStateUseCase { get; }
    public GetCharacterSummaryUseCase GetCharacterSummaryUseCase { get; }
    public GetCharacterDetailUseCase GetCharacterDetailUseCase {  get; }
    public GetSkillListUseCase GetSkillListUseCase { get; }
    public GetInventoryUseCase GetInventoryUseCase { get; }

    // [MasterDataManager]
    public MasterDataManager MasterDataManager { get; }

    public GameContext(
        IInventoryRepository inventoryRepo, 
        ICharacterRepository characterRepo,
        IDailyStateRepository dailyStateRepo,
        MasterDataManager masterDataManager)
    {
        MasterDataManager = masterDataManager;

        InventoryRepo = inventoryRepo;
        CharacterRepo = characterRepo;
        DailyStateRepo = dailyStateRepo;

        GetMoneyUseCase = new GetMoneyUseCase(userInventoryRepository: InventoryRepo);
        DailyStateUseCase = new DailyStateUseCase(gameStateRepository: DailyStateRepo);
        GetCharacterSummaryUseCase = new GetCharacterSummaryUseCase(dailyStateRepo: DailyStateRepo);
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
    }

    public async UniTask LoadAllDataAsync()
    {
        var taskInventory = InventoryRepo.LoadDataAsync();
        var taskCharacter = CharacterRepo.LoadDataAsync();
        var taskDailyState = DailyStateRepo.LoadDataAsync();

        await UniTask.WhenAll(taskInventory, taskCharacter, taskDailyState);
    }
}