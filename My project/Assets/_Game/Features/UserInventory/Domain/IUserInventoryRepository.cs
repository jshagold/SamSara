
using Cysharp.Threading.Tasks;

public interface IUserInventoryRepository
{
    UniTask<int> GetMoneyAsync();
    UniTask<UserInventory> LoadInventoryAsync();
    UniTask SaveUserInventoryAsync(UserInventory inventory);
}
