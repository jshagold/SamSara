/// <summary>
/// [STUB] 캐릭터 도메인 로직. 구체 구현은 Character Feature 스펙에서 진행.
/// </summary>
public class CharacterUseCase
{
    private readonly string _logClass = $"[{nameof(CharacterUseCase)}]";

    private readonly CharacterRepository _characterRepo;

    public CharacterUseCase(CharacterRepository characterRepo)
    {
        _characterRepo = characterRepo;
    }
}
