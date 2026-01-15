public class GetCharacterDetailUseCase
{
    private readonly ICharacterRepository _characterRepo;

    public GetCharacterDetailUseCase(ICharacterRepository characterRepo)
    {
        _characterRepo = characterRepo;
    }

    public CharacterInfo Execute()
    {
        return _characterRepo.GetCharacterInfo();
    }
}