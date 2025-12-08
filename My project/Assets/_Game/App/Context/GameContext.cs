public class GameContext
{
    public UserInventoryRepository Inverntory {  get; }

    public GameContext(UserInventoryRepository inventory)
    {
        Inverntory = inventory;
    }
}