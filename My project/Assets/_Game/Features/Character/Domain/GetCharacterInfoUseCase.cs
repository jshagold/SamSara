public class GetCharacterInfoUseCase
{
    private readonly ICharacterRepository _characterRepo;

    public GetCharacterInfoUseCase(ICharacterRepository characterRepo)
    {
        _characterRepo = characterRepo;
    }

    public CharacterInfo Execute()
    {
        return _characterRepo.GetCharacterInfo();
    }
}