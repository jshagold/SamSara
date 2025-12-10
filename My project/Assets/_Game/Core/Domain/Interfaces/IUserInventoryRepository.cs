
using Cysharp.Threading.Tasks;

public interface IUserInventoryRepository
{
    int GetMoneyAsync();


    UniTask<UserInventory> LoadDataAsync();
    UniTask SaveDataAsync();
}
