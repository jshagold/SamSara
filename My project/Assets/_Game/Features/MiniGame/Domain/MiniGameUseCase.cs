/// <summary>
/// [STUB] 미니게임 도메인 로직. 구체 구현은 MiniGame Feature 스펙에서 진행.
/// </summary>
public class MiniGameUseCase
{
    private readonly string _logClass = $"[{nameof(MiniGameUseCase)}]";

    private readonly CharacterRepository _characterRepo;

    public MiniGameUseCase(CharacterRepository characterRepo)
    {
        _characterRepo = characterRepo;
    }
}
