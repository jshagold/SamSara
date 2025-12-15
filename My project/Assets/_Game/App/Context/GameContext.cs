using Cysharp.Threading.Tasks;
using Unity.Profiling;

public class GameContext
{
    public IUserInventoryRepository InventoryRepo {  get; }
    public GetMoneyUseCase GetMoneyUseCase { get; }
    public IDailyStateRepository DailyStateRepo {  get; }
    public DailyStateUseCase DailyStateUseCase { get; }

    // TODO GetCharacterSummaryUseCase에 맞는 Repo 구현후에 넣어야함.
    public GetCharacterSummaryUseCase GetCharacterSummaryUseCase { get; }

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