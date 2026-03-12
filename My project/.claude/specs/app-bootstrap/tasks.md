# GlobalBootstrapper + GameContext — Tasks

**Version:** 1.1.0 | **Date:** 2026-03-12 | **Status:** Ready for Claude Code
**Based on:** Specify v1.2.0 / Plan v1.0.0

---

## Pre-Implementation Checklist

> Claude Code MUST verify the following before starting any task.

- [ ] Read `CLAUDE.md` first
- [ ] Confirm `ISceneNavigator` and `IPopupManager` exist in `Assets/_Game/Core/`
  - If missing → create empty interface stubs first, then proceed
- [ ] Confirm `Assets/_Game/App/` folder exists

---

## TASK-01 — GameContext.cs

**Path:** `Assets/_Game/App/GameContext.cs`
**Type:** Pure C# Class — MonoBehaviour is FORBIDDEN
**Priority:** First (must precede GlobalBootstrapper)

### Fields

- Repository fields: `CharacterRepository`, `StageRepository` — `private readonly`
- UseCase fields: `CharacterUseCase`, `EvolutionUseCase`, `StageUseCase`, `BattleUseCase`, `EventUseCase`, `MiniGameUseCase` — `private readonly`
- Log tag: `private readonly string _logClass = $"[{nameof(GameContext)}]";`

### Constructor

- Parameter: MasterData container (replace with confirmed type)
- Instantiate Repositories first, then inject into UseCases via constructor
- `new` keyword is ONLY allowed inside this constructor

DI wiring order:

    _characterRepo    = new CharacterRepository();
    _stageRepo        = new StageRepository();
    _characterUseCase = new CharacterUseCase(_characterRepo);
    _evolutionUseCase = new EvolutionUseCase(_characterRepo);
    _stageUseCase     = new StageUseCase(_stageRepo);
    _battleUseCase    = new BattleUseCase(_characterRepo, _stageRepo);
    _eventUseCase     = new EventUseCase(_characterRepo, _stageRepo);
    _miniGameUseCase  = new MiniGameUseCase(_characterRepo);

### LoadAllDataAsync()

- Returns: `UniTask`
- Run all Repository `LoadDataAsync()` calls in parallel via `UniTask.WhenAll`
- On failure: throw `InvalidOperationException` immediately (Fail Fast)
- Requires: `using Cysharp.Threading.Tasks;`

### SaveAllDataSync()

- Returns: `void`
- Call all Repository `SaveDataSync()` sequentially
- For `OnApplicationPause` / `OnApplicationQuit` use only — async is strictly forbidden

### Public Accessors

- Expose each UseCase as a `public` read-only property
- Do NOT expose Repositories directly — UseCase is the only access point

### Rules

- No `MonoBehaviour` inheritance
- No `UnityEngine` namespace imports (UniTask excluded)

---

## TASK-02 — GlobalBootstrapper.cs

**Path:** `Assets/_Game/App/GlobalBootstrapper.cs`
**Type:** MonoBehaviour
**Priority:** After TASK-01

### Fields

    private static GlobalBootstrapper _instance;
    private GameContext _gameContext;
    private ISceneNavigator _sceneNavigator;
    private IPopupManager _popupManager;
    private readonly UniTaskCompletionSource _initTcs = new();
    private int _retryCount = 0;
    private readonly string _logClass = $"[{nameof(GlobalBootstrapper)}]";

    [SerializeField] private FallbackErrorCanvas _fallbackCanvas;

### InitializationTask (FR-03)

    public UniTask InitializationTask => _initTcs.Task;

### Singleton Guard — Awake() (FR-01)

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

### InitializeAsync() — Initialization Sequence (FR-02)

Must execute in this exact order. Wrap entire method in try-catch.

    // Step 1 — Load MasterData (ThreadPool)
    var masterData = await UniTask.RunOnThreadPool(
        () => Resources.LoadAll<ScriptableObject>("MasterData"));

    // Step 2 — Instantiate Core systems
    _sceneNavigator = new SceneNavigator();
    _popupManager   = new PopupManager();

    // Step 3 — Construct GameContext + DI wiring
    _gameContext = new GameContext(masterData);

    // Step 4 — Load runtime data (parallel)
    await _gameContext.LoadAllDataAsync();

    // Step 5 — Signal completion
    _initTcs.TrySetResult();

### Error Handling — Fallback Canvas (FR-04)

    catch (Exception e)
    {
        Debug.LogError($"{_logClass} Initialization failed: {e}");
        _retryCount++;
        if (_retryCount >= 3)
        {
            Application.Quit();
            return;
        }
        _fallbackCanvas.Show(
            onRetry: () => InitializeAsync().Forget(),
            onQuit:  () => Application.Quit()
        );
    }

### ApplicationPause / Quit Handling (FR-07)

    private void OnApplicationPause(bool pause)
    {
        if (pause) _gameContext?.SaveAllDataSync();
    }

    private void OnApplicationQuit()
    {
        _gameContext?.SaveAllDataSync();
    }

### OnDestroy

    private void OnDestroy()
    {
        if (_instance == this) _instance = null;
    }

### Rules

- `GetComponent` / `FindObjectOfType` forbidden (except self-reference in Awake)
- No null-guard defensive code on `[SerializeField]` fields (Constitution §7 Fail Fast)
- Use `?.` for all cleanup calls (Constitution §8 Safe Cleanup)

---

## TASK-03 — FallbackErrorCanvas.cs + Prefab Setup (Manual)

**Owner:** Developer (Unity Editor work)
**Priority:** After TASK-02

1. Create `Assets/_Game/App/Presentation/FallbackErrorCanvas.cs`
   - `public void Show(Action onRetry, Action onQuit)`
   - `public void Hide()`
   - Default state: `gameObject.SetActive(false)`
2. In Unity Editor, create Canvas as child of `GlobalBootstrapper` GameObject
   - `Render Mode = Screen Space - Overlay`, `Sort Order = 999`
   - Add: error message `TMP_Text`, Retry `Button`, Quit `Button`
3. Attach `FallbackErrorCanvas.cs` to Canvas root
4. Connect `GlobalBootstrapper._fallbackCanvas` slot in Inspector

---

## TASK-04 — Bootstrap Scene Setup (Manual)

**Owner:** Developer (Unity Editor work)
**Priority:** After TASK-03

1. Create or confirm `Assets/_Game/Scenes/Bootstrap.unity`
2. Create empty GameObject named `GlobalBootstrapper`
3. Attach `GlobalBootstrapper.cs` component
4. Connect `FallbackErrorCanvas` as child
5. Set Bootstrap Scene as **index 0** in Build Settings

---

## TASK-05 — Validation

**Priority:** Final — after all tasks complete

| # | Check | Method |
|---|---|---|
| V-01 | `GameContext` does not extend `MonoBehaviour` | Code review |
| V-02 | No Singleton other than `GlobalBootstrapper` | Search codebase for `Instance` |
| V-03 | Init order: MasterData → Core → GameContext → RuntimeData | Log order in Play Mode |
| V-04 | No async save in `OnApplicationPause` / `OnApplicationQuit` | Code review |
| V-05 | 3 Retry failures trigger forced `Application.Quit()` | Play Mode test (break MasterData path) |
| V-06 | `FallbackCanvas` works independently of `IPopupManager` | Test with `IPopupManager` null |
| V-07 | All `SceneBootstrapper`s await `InitializationTask` before self-init | Log order check |
| V-08 | All cleanup uses `?.` null-conditional | Code review (`OnDestroy`, `OnApplicationQuit`) |

---

## Claude Code Implementation Guide

- Read `CLAUDE.md` first before any implementation.
- **Files to create:**
  - `Assets/_Game/App/GameContext.cs`
  - `Assets/_Game/App/GlobalBootstrapper.cs`
  - `Assets/_Game/App/Presentation/FallbackErrorCanvas.cs`
- **Files to reference:**
  - `Assets/_Game/Core/ISceneNavigator.cs`
  - `Assets/_Game/Core/IPopupManager.cs`
- **Implementation order:**
  1. `GameContext.cs`
  2. `GlobalBootstrapper.cs`
  3. `FallbackErrorCanvas.cs`
  4. Unity Editor manual steps (TASK-03, TASK-04)
- **DO NOT** create files outside `Assets/_Game/`.
- **DO NOT** use `Manager.Instance` patterns anywhere.
- **DO NOT** make `GameContext` a MonoBehaviour.