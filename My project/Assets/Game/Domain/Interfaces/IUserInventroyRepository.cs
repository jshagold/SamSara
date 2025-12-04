
public interface IUserInventroyRepository
{
    UserInventory GetInventory();
    void SaveInventory(UserInventory inventory);
}
