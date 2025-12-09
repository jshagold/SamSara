public class GameContext
{
    public IUserInventoryRepository Inverntory {  get; }
    public GetMoneyUseCase GetMoneyUseCase { get; }
    public IDailyStateRepository DailyStateRepo {  get; }
    public DailyStateUseCase DailyStateUseCase { get; }

    public GameContext(IUserInventoryRepository inventory, IDailyStateRepository dailyStateRepo)
    {
        Inverntory = inventory;
        DailyStateRepo = dailyStateRepo;

        GetMoneyUseCase = new GetMoneyUseCase(inventory);
        DailyStateUseCase = new DailyStateUseCase(DailyStateRepo);
    }
}