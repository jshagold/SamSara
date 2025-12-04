public class GetMoneyUseCase
{
    private readonly IUserInventroyRepository _userInventroyRepository;

    public GetMoneyUseCase(IUserInventroyRepository userInventroyRepository) 
    {
        _userInventroyRepository = userInventroyRepository;
    }


    public UserInventory GetInventory()
    {
        return _userInventroyRepository.GetInventory();
    }

}