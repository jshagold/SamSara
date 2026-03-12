using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 게임 전체 DI 컨테이너. Repository 생성 및 UseCase 주입을 담당.
/// MonoBehaviour 금지. 이 클래스의 new는 오직 생성자 안에서만 허용.
/// </summary>
public class GameContext
{
    private readonly string _logClass = $"[{nameof(GameContext)}]";

    // ──────────────────────────────────────────────
    // Repositories (외부 노출 금지 — UseCase를 통해서만 접근)
    // ──────────────────────────────────────────────
    private readonly CharacterRepository _characterRepo;
    private readonly StageRepository     _stageRepo;

    // ──────────────────────────────────────────────
    // UseCases (public 프로퍼티로만 노출)
    // ──────────────────────────────────────────────
    private readonly CharacterUseCase  _characterUseCase;
    private readonly EvolutionUseCase  _evolutionUseCase;
    private readonly StageUseCase      _stageUseCase;
    private readonly BattleUseCase     _battleUseCase;
    private readonly EventUseCase      _eventUseCase;
    private readonly MiniGameUseCase   _miniGameUseCase;

    // ──────────────────────────────────────────────
    // Public Accessors — UseCase만 공개
    // ──────────────────────────────────────────────
    public CharacterUseCase CharacterUseCase  => _characterUseCase;
    public EvolutionUseCase EvolutionUseCase  => _evolutionUseCase;
    public StageUseCase     StageUseCase      => _stageUseCase;
    public BattleUseCase    BattleUseCase     => _battleUseCase;
    public EventUseCase     EventUseCase      => _eventUseCase;
    public MiniGameUseCase  MiniGameUseCase   => _miniGameUseCase;

    // ──────────────────────────────────────────────
    // Constructor — DI 조립. new 사용은 여기서만 허용.
    // ──────────────────────────────────────────────
    /// <param name="masterData">GlobalBootstrapper가 ThreadPool에서 로드한 MasterData 전체.</param>
    public GameContext(ScriptableObject[] masterData)
    {
        // Step 1 — Repository 생성
        _characterRepo    = new CharacterRepository();
        _stageRepo        = new StageRepository();

        // Step 2 — UseCase 생성 (Repository 주입)
        _characterUseCase = new CharacterUseCase(_characterRepo);
        _evolutionUseCase = new EvolutionUseCase(_characterRepo);
        _stageUseCase     = new StageUseCase(_stageRepo);
        _battleUseCase    = new BattleUseCase(_characterRepo, _stageRepo);
        _eventUseCase     = new EventUseCase(_characterRepo, _stageRepo);
        _miniGameUseCase  = new MiniGameUseCase(_characterRepo);

        Debug.Log($"{_logClass} DI 조립 완료.");
    }

    // ──────────────────────────────────────────────
    // LoadAllDataAsync — 병렬 로드 (FR-02)
    // ──────────────────────────────────────────────
    /// <summary>
    /// 모든 Repository의 런타임 데이터를 병렬로 로드한다.
    /// 하나라도 실패하면 즉시 InvalidOperationException (Fail Fast).
    /// </summary>
    public async UniTask LoadAllDataAsync()
    {
        try
        {
            await UniTask.WhenAll(
                _characterRepo.LoadDataAsync(),
                _stageRepo.LoadDataAsync()
            );

            Debug.Log($"{_logClass} 런타임 데이터 로드 완료.");
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"{_logClass} 데이터 로드 실패: {e.Message}", e);
        }
    }

    // ──────────────────────────────────────────────
    // SaveAllDataSync — OnApplicationPause / OnApplicationQuit 전용 (FR-07)
    // async 절대 금지.
    // ──────────────────────────────────────────────
    /// <summary>
    /// 모든 Repository를 순차적으로 동기 저장한다.
    /// OnApplicationPause / OnApplicationQuit 에서만 호출할 것.
    /// </summary>
    public void SaveAllDataSync()
    {
        _characterRepo.SaveDataSync();
        _stageRepo.SaveDataSync();

        Debug.Log($"{_logClass} 긴급 동기 저장 완료.");
    }
}
