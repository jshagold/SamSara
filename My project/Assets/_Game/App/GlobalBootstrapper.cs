using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

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

            // Step 1 — MasterData 로드 (ThreadPool에서 I/O 처리)
            var masterData = await UniTask.RunOnThreadPool(
                () => Resources.LoadAll<ScriptableObject>("MasterData"));

            Debug.Log($"{_logClass} Step 1 완료 — MasterData {masterData.Length}개 로드");

            // Step 2 — Core 시스템 생성
            _sceneNavigator = new SceneNavigator();
            _popupManager   = new PopupManager();

            Debug.Log($"{_logClass} Step 2 완료 — Core 시스템 생성");

            // Step 3 — GameContext 생성 및 DI 조립
            _gameContext = new GameContext(masterData);

            Debug.Log($"{_logClass} Step 3 완료 — GameContext 조립");

            // Step 4 — 런타임 데이터 병렬 로드
            await _gameContext.LoadAllDataAsync();

            Debug.Log($"{_logClass} Step 4 완료 — 런타임 데이터 로드");

            // Step 5 — 초기화 완료 신호
            _initTcs.TrySetResult();

            Debug.Log($"{_logClass} 초기화 완료.");
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
