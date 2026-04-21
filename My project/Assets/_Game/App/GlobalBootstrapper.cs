using System;
using Cysharp.Threading.Tasks;
using Samsara.App;
using Samsara.App.Popup;
using Samsara.Core.AssetLoading;
using Samsara.Core.Navigation;
using Samsara.Core.Popup;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 프로젝트 유일의 Singleton. Bootstrap Scene에서 시작하며 DontDestroyOnLoad로 유지된다.
/// 초기화 순서: MasterData 로드 → Core 시스템 생성 → GameContext DI 조립 → 런타임 데이터 로드 → 완료 신호.
/// </summary>
public class GlobalBootstrapper : MonoBehaviour
{
    private readonly string _logClass = $"[{nameof(GlobalBootstrapper)}]";

    // ──────────────────────────────────────────────
    // Singleton
    // ──────────────────────────────────────────────
    private static GlobalBootstrapper _instance;
    public static GlobalBootstrapper Instance => _instance;

    // ──────────────────────────────────────────────
    // Fields
    // ──────────────────────────────────────────────
    private GameContext      _gameContext;
    private ISceneNavigator  _sceneNavigator;
    private IPopupManager    _popupManager;

    private readonly UniTaskCompletionSource _initTcs = new();
    private int _retryCount = 0;

    [SerializeField] private FallbackErrorCanvas _fallbackCanvas;
    [SerializeField] private CommonPopupView     _popupViewPrefab;
    [SerializeField] private RunConfigSO         _runConfig;

    // ──────────────────────────────────────────────
    // Public API
    // ──────────────────────────────────────────────

    /// <summary>
    /// 초기화 완료를 기다리는 Task. SceneBootstrapper에서 await 용도.
    /// </summary>
    public UniTask InitializationTask => _initTcs.Task;

    /// <summary>
    /// 초기화가 완료된 GameContext. InitializationTask 완료 이후에만 접근할 것.
    /// </summary>
    public GameContext GameContext => _gameContext;

    /// <summary>
    /// ISceneNavigator. InitializationTask 완료 이후에만 접근할 것.
    /// </summary>
    public ISceneNavigator SceneNavigator => _sceneNavigator;

    // ──────────────────────────────────────────────
    // Singleton Guard (FR-01)
    // ──────────────────────────────────────────────
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeAsync().Forget();
    }

    // ──────────────────────────────────────────────
    // Initialization Sequence (FR-02)
    // ──────────────────────────────────────────────
    private async UniTaskVoid InitializeAsync()
    {
        // 재시도 시 열려 있던 FallbackCanvas를 닫는다.
        _fallbackCanvas.Hide();

        try
        {
            Debug.Log($"{_logClass} 초기화 시작 (시도 {_retryCount + 1})");

            // Step 1 — MasterData 로드 (메인 스레드에서 동기 로드)
            var masterData = Resources.LoadAll<ScriptableObject>("MasterData");

            if (_runConfig == null)
                throw new InvalidOperationException(
                    $"{_logClass} RunConfigSO가 Inspector에 연결되지 않았습니다.");

            Debug.Log($"{_logClass} Step 1 완료 — MasterData {masterData.Length}개 로드, RunConfig={_runConfig.name}");

            // Step 2 — Core 시스템 생성
            _sceneNavigator = new SceneNavigator();

            // PopupCanvas 생성 (sortingOrder 100, ScreenSpaceOverlay)
            var popupCanvasGO = new GameObject("PopupCanvas",
                typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            popupCanvasGO.transform.SetParent(transform);

            var canvas = popupCanvasGO.GetComponent<Canvas>();
            canvas.renderMode    = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder  = 100;

            var scaler = popupCanvasGO.GetComponent<CanvasScaler>();
            scaler.uiScaleMode       = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);

            _popupManager = new PopupManager(popupCanvasGO.transform, _popupViewPrefab);

            Debug.Log($"{_logClass} Step 2 완료 — Core 시스템 생성");

            // Step 3 — GameContext 생성 및 DI 조립
            var spriteLoader = new AddressableSpriteLoader();
            _gameContext = new GameContext(masterData, _popupManager, _runConfig, spriteLoader, _sceneNavigator);

            Debug.Log($"{_logClass} Step 3 완료 — GameContext 조립");

            // Step 4 — 런타임 데이터 병렬 로드
            await _gameContext.LoadAllDataAsync();

            Debug.Log($"{_logClass} Step 4 완료 — 런타임 데이터 로드");

            // Step 4-A — 신규 런 자동 초기화 (저장 데이터 없을 때)
            // New-user path only: auto-initialize RunData on first app launch.
            // ReplayScene handles restart after an ended run via a separate path
            // and does not inherit this auto-initialization.
            // (Per ReplayScene Specify RQ-01 / RQ-12)
            {
                var characterRunRepo = _gameContext.CharacterRunRepo;
                var runData          = characterRunRepo.RunData;

                bool isUninitialized = runData.Day == 0 || string.IsNullOrEmpty(runData.EvolutionNodeId);
                if (isUninitialized)
                {
                    characterRunRepo.InitializeNewRun(_runConfig);
                    _gameContext.StageRepo.InitializeNewRun(_runConfig);
                    _gameContext.ShopRepo.InitializeNewRun(_runConfig);

                    // EvolutionNodeSO.BaseStats → CharacterRunData 스탯 적용
                    var nodeId = characterRunRepo.RunData.EvolutionNodeId;
                    foreach (var node in _gameContext.EvolutionNodes)
                    {
                        if (node.NodeId != nodeId || node.BaseStats == null) continue;
                        var d       = characterRunRepo.RunData;
                        d.Hp        = node.BaseStats.Hp;
                        d.MaxHp     = node.BaseStats.Hp;
                        d.Strength  = node.BaseStats.Strength;
                        d.Toughness = node.BaseStats.Toughness;
                        d.Agility   = node.BaseStats.Agility;
                        break;
                    }

                    await UniTask.WhenAll(
                        characterRunRepo.SaveDataAsync(),
                        _gameContext.StageRepo.SaveAsync(),
                        _gameContext.ShopRepo.SaveDataAsync()
                    );

                    Debug.Log($"{_logClass} Step 4-A 완료 — 신규 런 초기화 (RunConfig 기반).");
                }
            }

            // Validation: RunConfigSO ID references must exist in MasterData
            {
                var defaultNodeId = _runConfig.DefaultEvolutionNodeId.ToString();
                if (!System.Array.Exists(_gameContext.EvolutionNodes, n => n.NodeId == defaultNodeId))
                    throw new InvalidOperationException(
                        $"{_logClass} [Validation] RunConfigSO.DefaultEvolutionNodeId({_runConfig.DefaultEvolutionNodeId})에 해당하는 EvolutionNodeSO가 MasterData에 없습니다.");

                try { _gameContext.StageMasterDataRepo.GetStageById(_runConfig.StartStageId.ToString()); }
                catch { throw new InvalidOperationException(
                    $"{_logClass} [Validation] RunConfigSO.StartStageId({_runConfig.StartStageId})에 해당하는 StageSO가 MasterData에 없습니다."); }
            }

            // Step 5 — 초기화 완료 신호
            _initTcs.TrySetResult();

            Debug.Log($"{_logClass} 초기화 완료.");

            // Step 6 — 씬 전환 (에디터 직접 실행 시 Battle로, 정상 실행 시 Main으로)
#if UNITY_EDITOR
            if (Samsara.Features.BattleScene.Presentation.BattleSceneBootstrapper.IsDirectTestMode)
            {
                Samsara.Features.BattleScene.Presentation.BattleSceneBootstrapper.IsDirectTestMode = false;
                await _sceneNavigator.NavigateToAsync(SceneKey.Battle);
                return;
            }
#endif
            await _sceneNavigator.NavigateToAsync(SceneKey.Main);
        }
        catch (Exception e)
        {
            Debug.LogError($"{_logClass} 초기화 실패: {e}");

            _retryCount++;

            if (_retryCount >= 3)
            {
                Debug.LogError($"{_logClass} 재시도 횟수 초과 — 앱 종료");
                Application.Quit();
                return;
            }

            _fallbackCanvas.Show(
                onRetry: () => InitializeAsync().Forget(),
                onQuit:  () => Application.Quit()
            );
        }
    }

    // ──────────────────────────────────────────────
    // Application Lifecycle — 긴급 저장 (FR-07)
    // async 절대 금지.
    // ──────────────────────────────────────────────
    private void OnApplicationPause(bool pause)
    {
        if (pause) _gameContext?.SaveAllDataSync();
    }

    private void OnApplicationQuit()
    {
        _gameContext?.SaveAllDataSync();
    }

    // ──────────────────────────────────────────────
    // Cleanup
    // ──────────────────────────────────────────────
    private void OnDestroy()
    {
        if (_instance == this) _instance = null;
    }
}
