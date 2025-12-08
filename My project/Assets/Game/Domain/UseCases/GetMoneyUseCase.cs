using Cysharp.Threading.Tasks;

public class GetMoneyUseCase
{
    private readonly IUserInventroyRepository _userInventroyRepository;

    public GetMoneyUseCase(IUserInventroyRepository userInventroyRepository) 
    {
        _userInventroyRepository = userInventroyRepository;
    }


    public UniTask<UserInventory> LoadInventoryAsync()
    {
        return _userInventroyRepository.LoadInventoryAsync();
    }

}