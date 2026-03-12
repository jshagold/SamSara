/// <summary>
/// [STUB] 진화 도메인 로직. 구체 구현은 Evolution Feature 스펙에서 진행.
/// </summary>
public class EvolutionUseCase
{
    private readonly string _logClass = $"[{nameof(EvolutionUseCase)}]";

    private readonly CharacterRepository _characterRepo;

    public EvolutionUseCase(CharacterRepository characterRepo)
    {
        _characterRepo = characterRepo;
    }
}
