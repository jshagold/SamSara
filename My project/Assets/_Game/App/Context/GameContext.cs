public class GameContext
{
    public IUserInventoryRepository InventoryRepo {  get; }
    public GetMoneyUseCase GetMoneyUseCase { get; }
    public IDailyStateRepository DailyStateRepo {  get; }
    public DailyStateUseCase DailyStateUseCase { get; }

    public GameContext(IUserInventoryRepository inventory, IDailyStateRepository dailyStateRepo)
    {
        InventoryRepo = inventory;
        DailyStateRepo = dailyStateRepo;

        GetMoneyUseCase = new GetMoneyUseCase(inventory);
        DailyStateUseCase = new DailyStateUseCase(DailyStateRepo);
    }
}