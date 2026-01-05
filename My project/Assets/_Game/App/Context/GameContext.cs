using Cysharp.Threading.Tasks;
using Unity.Profiling;

public class GameContext
{
    // [Repositories]
    public IUserInventoryRepository InventoryRepo { get; }
    public IDailyStateRepository DailyStateRepo { get; }

    // [UseCases]
    public GetMoneyUseCase GetMoneyUseCase { get; }
    public DailyStateUseCase DailyStateUseCase { get; }
    public GetCharacterSummaryUseCase GetCharacterSummaryUseCase { get; }

    // [MasterDataManager]
    public MasterDataManager MasterDataManager { get; }

    public GameContext(
        IUserInventoryRepository inventory, 
        IDailyStateRepository dailyStateRepo,
        MasterDataManager masterDataManager)
    {
        InventoryRepo = inventory;
        DailyStateRepo = dailyStateRepo;

        GetMoneyUseCase = new GetMoneyUseCase(userInventoryRepository: InventoryRepo);
        DailyStateUseCase = new DailyStateUseCase(gameStateRepository: DailyStateRepo);
        GetCharacterSummaryUseCase = new GetCharacterSummaryUseCase(dailyStateRepo: DailyStateRepo);

        MasterDataManager = masterDataManager;
    }

    public async UniTask LoadAllDataAsync()
    {
        var task1 = InventoryRepo.LoadDataAsync();
        var task2 = DailyStateRepo.LoadDataAsync();

        await UniTask.WhenAll(task1, task2);
    }
}