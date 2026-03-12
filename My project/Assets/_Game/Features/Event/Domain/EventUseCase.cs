/// <summary>
/// [STUB] 이벤트 도메인 로직. 구체 구현은 Event Feature 스펙에서 진행.
/// </summary>
public class EventUseCase
{
    private readonly string _logClass = $"[{nameof(EventUseCase)}]";

    private readonly CharacterRepository _characterRepo;
    private readonly StageRepository _stageRepo;

    public EventUseCase(CharacterRepository characterRepo, StageRepository stageRepo)
    {
        _characterRepo = characterRepo;
        _stageRepo = stageRepo;
    }
}
