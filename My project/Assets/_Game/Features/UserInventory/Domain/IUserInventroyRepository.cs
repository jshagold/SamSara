
using Cysharp.Threading.Tasks;

public interface IUserInventroyRepository
{
    UniTask<UserInventory> LoadInventoryAsync();
    UniTask SaveUserInventoryAsync(UserInventory inventory);
}
