using Samsara.Features.Stage.Domain;

/// <summary>
/// [STUB] 전투 도메인 로직. 구체 구현은 Battle Feature 스펙에서 진행.
/// </summary>
public class BattleUseCase
{
    private readonly string _logClass = $"[{nameof(BattleUseCase)}]";

    private readonly CharacterRepository _characterRepo;
    private readonly IStageRepository _stageRepo;

    public BattleUseCase(CharacterRepository characterRepo, IStageRepository stageRepo)
    {
        _characterRepo = characterRepo;
        _stageRepo = stageRepo;
    }
}
