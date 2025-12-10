using Cysharp.Threading.Tasks;

public class GetMoneyUseCase
{
    private readonly IUserInventoryRepository _userInventroyRepository;

    public GetMoneyUseCase(IUserInventoryRepository userInventroyRepository) 
    {
        _userInventroyRepository = userInventroyRepository;
    }
    
    /// <summary>
    /// 현재 소지금을 가져온다
    /// </summary>
    /// <returns></returns>
    public int GetInventoryMoneyAsync()
    {
        return _userInventroyRepository.GetMoney();
    }


    public UniTask<UserInventory> LoadInventoryAsync()
    {
        return _userInventroyRepository.LoadDataAsync();
    }


}