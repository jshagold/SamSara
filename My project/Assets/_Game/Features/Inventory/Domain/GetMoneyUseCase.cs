using Cysharp.Threading.Tasks;

public class GetMoneyUseCase
{
    private readonly IInventoryRepository _userInventroyRepository;

    public GetMoneyUseCase(IInventoryRepository userInventoryRepository) 
    {
        _userInventroyRepository = userInventoryRepository;
    }
    
    /// <summary>
    /// 현재 소지금을 가져온다
    /// </summary>
    /// <returns></returns>
    public int GetInventoryMoneyAsync()
    {

        return _userInventroyRepository.GetItemCount(ItemConstants.MONEY_ID);
    }
}