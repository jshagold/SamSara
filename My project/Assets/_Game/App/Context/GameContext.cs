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

    // [MasterDataManager]
    public MasterDataManager MasterDataManager { get; }

    public GameContext(
        IInventoryRepository inventoryRepo, 
        ICharacterRepository characterRepo,
        IDailyStateRepository dailyStateRepo,
        MasterDataManager masterDataManager)
    {
        InventoryRepo = inventoryRepo;
        CharacterRepo = characterRepo;
        DailyStateRepo = dailyStateRepo;

        GetMoneyUseCase = new GetMoneyUseCase(userInventoryRepository: InventoryRepo);
        DailyStateUseCase = new DailyStateUseCase(gameStateRepository: DailyStateRepo);
        GetCharacterSummaryUseCase = new GetCharacterSummaryUseCase(dailyStateRepo: DailyStateRepo);

        MasterDataManager = masterDataManager;
    }

    public async UniTask LoadAllDataAsync()
    {
        var taskInventory = InventoryRepo.LoadDataAsync();
        var taskCharacter = CharacterRepo.LoadDataAsync();
        var taskDailyState = DailyStateRepo.LoadDataAsync();

        await UniTask.WhenAll(taskInventory, taskCharacter, taskDailyState);
    }
}