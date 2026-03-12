/// <summary>
/// [STUB] 스테이지 도메인 로직. 구체 구현은 Stage Feature 스펙에서 진행.
/// </summary>
public class StageUseCase
{
    private readonly string _logClass = $"[{nameof(StageUseCase)}]";

    private readonly StageRepository _stageRepo;

    public StageUseCase(StageRepository stageRepo)
    {
        _stageRepo = stageRepo;
    }
}
