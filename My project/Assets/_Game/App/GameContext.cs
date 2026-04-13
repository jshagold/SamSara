using System;
using Cysharp.Threading.Tasks;
using Samsara.Core.Popup;
using Samsara.Features.BattleScene.Domain;
using Samsara.Features.Character.Data;
using Samsara.Features.Character.Domain;
using Samsara.Features.Character.MasterData;
using Samsara.Features.Event.Data;
using Samsara.Features.Event.Domain;
using Samsara.Features.MiniGame.Domain;
using Samsara.Features.Shop.Data;
using Samsara.Features.Shop.Domain;
using Samsara.Features.Skill.Data;
using Samsara.Features.Skill.Domain;
using Samsara.Features.Stage.Data;
using Samsara.Features.Stage.Domain;
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
    private readonly CharacterRepository         _characterRepo;
    private readonly IStageRepository            _stageRepo;
    private readonly IStageMasterDataRepository  _stageMasterDataRepo;
    private readonly ICharacterRunRepository     _characterRunRepo;
    private readonly ICharacterAccountRepository _characterAccountRepo;
    private readonly ISkillMasterDataRepository    _skillMasterDataRepo;
    private readonly IEventMasterDataRepository   _eventMasterDataRepo;
    private readonly IShopMasterDataRepository    _shopMasterDataRepo;
    private readonly IShopRepository              _shopRepo;

    // ──────────────────────────────────────────────
    // MasterData Cache
    // ──────────────────────────────────────────────
    private readonly EvolutionNodeSO[] _evolutionNodes;

    // ──────────────────────────────────────────────
    // UseCases (public 프로퍼티로만 노출)
    // ──────────────────────────────────────────────
    private readonly CharacterUseCase  _characterUseCase;
    private readonly EvolutionUseCase  _evolutionUseCase;
    private readonly StageUseCase      _stageUseCase;
    private readonly EventUseCase      _eventUseCase;
    private readonly MiniGameUseCase   _miniGameUseCase;
    private readonly SkillUseCase      _skillUseCase;
    private readonly ShopUseCase       _shopUseCase;

    // ──────────────────────────────────────────────
    // Public Accessors — UseCase 및 Core 시스템
    // ──────────────────────────────────────────────
    public IPopupManager               PopupManager         { get; }
    public IStageRepository            StageRepo            => _stageRepo;
    public ICharacterRunRepository     CharacterRunRepo     => _characterRunRepo;
    public ICharacterAccountRepository CharacterAccountRepo => _characterAccountRepo;
    public IStageMasterDataRepository  StageMasterDataRepo  => _stageMasterDataRepo;
    public ISkillMasterDataRepository  SkillMasterDataRepo  => _skillMasterDataRepo;
    public IEventMasterDataRepository  EventMasterDataRepo  => _eventMasterDataRepo;
    public IShopMasterDataRepository   ShopMasterDataRepo   => _shopMasterDataRepo;
    public IShopRepository             ShopRepo             => _shopRepo;
    public EvolutionNodeSO[]           EvolutionNodes       => _evolutionNodes;
    public CharacterUseCase CharacterUseCase  => _characterUseCase;
    public EvolutionUseCase EvolutionUseCase  => _evolutionUseCase;
    public StageUseCase     StageUseCase      => _stageUseCase;
    public EventUseCase     EventUseCase      => _eventUseCase;
    public MiniGameUseCase  MiniGameUseCase   => _miniGameUseCase;
    public SkillUseCase     SkillUseCase      => _skillUseCase;
    public ShopUseCase      ShopUseCase       => _shopUseCase;

    /// <summary>씬 간 데이터 전달 — 훈련 대상 StatType. MiniGameScene 진입 전 설정, 진입 후 즉시 소비.</summary>
    public StatType? PendingTrainingStat { get; set; }

    /// <summary>씬 간 데이터 전달 — 전투 진입 컨텍스트. BattleScene 진입 전 설정, 진입 후 즉시 소비.</summary>
    public PendingBattleContext PendingBattleContext { get; set; }

    /// <summary>씬 간 데이터 전달 — 이벤트 진입 컨텍스트. EventScene 진입 전 설정, 진입 후 소비.</summary>
    public PendingEventContext PendingEventContext { get; set; }

    /// <summary>씬 간 데이터 전달 — 마지막 전투 결과. BattleScene 종료 시 설정, StageScene에서 소비.</summary>
    public BattleResult? LastBattleResult { get; set; }

    // ──────────────────────────────────────────────
    // Constructor — DI 조립. new 사용은 여기서만 허용.
    // ──────────────────────────────────────────────
    /// <param name="masterData">GlobalBootstrapper가 로드한 MasterData 전체.</param>
    /// <param name="popupManager">GlobalBootstrapper가 생성한 IPopupManager 인스턴스.</param>
    public GameContext(ScriptableObject[] masterData, IPopupManager popupManager)
    {
        PopupManager = popupManager;

        // MasterData Cache — masterData 배열에서 EvolutionNodeSO만 필터링
        var nodeList = new System.Collections.Generic.List<EvolutionNodeSO>();
        foreach (var so in masterData)
            if (so is EvolutionNodeSO node) nodeList.Add(node);
        _evolutionNodes = nodeList.ToArray();

        // Step 1 — Repository 생성
        _characterRepo        = new CharacterRepository();
        _stageRepo            = new StageRepository();
        _stageMasterDataRepo  = new StageMasterDataRepository();
        _characterRunRepo     = new CharacterRunRepository();
        _characterAccountRepo = new CharacterAccountRepository();
        _skillMasterDataRepo  = new SkillMasterDataRepository();
        _eventMasterDataRepo  = new EventMasterDataRepository();
        _shopMasterDataRepo   = new ShopMasterDataRepository();
        _shopRepo             = new ShopRepository();
        Debug.Log($"{_logClass} [V-02] StageRepo={_stageRepo.GetType().Name} / StageMasterDataRepo={_stageMasterDataRepo.GetType().Name} 등록 확인.");

        // Step 2 — UseCase 생성 (Repository 주입)
        _characterUseCase = new CharacterUseCase(_characterRepo);
        _evolutionUseCase = new EvolutionUseCase(_characterRepo);
        _stageUseCase     = new StageUseCase(_stageRepo);
        _eventUseCase     = new EventUseCase(_eventMasterDataRepo, _characterRunRepo, _stageRepo);
        _miniGameUseCase  = new MiniGameUseCase(_characterRunRepo);
        _skillUseCase     = new SkillUseCase(_skillMasterDataRepo);
        _shopUseCase      = new ShopUseCase(_shopRepo, _shopMasterDataRepo, _characterRunRepo);

        Debug.Log($"{_logClass} DI 조립 완료.");
    }

    // ──────────────────────────────────────────────
    // LoadAllDataAsync — 병렬 로드 (FR-02)
    // ──────────────────────────────────────────────
    /// <summary>
    /// MasterData 초기화 후 모든 Repository의 런타임 데이터를 병렬로 로드한다.
    /// 하나라도 실패하면 즉시 InvalidOperationException (Fail Fast).
    /// </summary>
    public async UniTask LoadAllDataAsync()
    {
        try
        {
            // StageMasterDataRepository는 Resources API 사용으로 메인 스레드에서 동기 초기화
            _stageMasterDataRepo.Initialize();

            // SkillMasterDataRepository — Resources API 사용으로 메인 스레드에서 초기화
            await _skillMasterDataRepo.LoadAsync();

            await UniTask.WhenAll(
                _characterRepo.LoadDataAsync(),
                _stageRepo.LoadAsync(),
                _characterRunRepo.LoadDataAsync(),
                _characterAccountRepo.LoadDataAsync(),
                _shopRepo.LoadDataAsync()
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
        _stageRepo.SaveSync();
        _characterRunRepo.SaveDataSync();
        _characterAccountRepo.SaveDataSync();
        _shopRepo.SaveDataSync();

        Debug.Log($"{_logClass} 긴급 동기 저장 완료.");
    }
}
