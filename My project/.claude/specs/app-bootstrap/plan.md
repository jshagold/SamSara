# GlobalBootstrapper + GameContext — Plan

**Version:** 1.1.0 | **Date:** 2026-03-12 | **Status:** Ready for Tasks
**Based on:** Specify v1.2.0

---

## 1. Overview

This Plan translates FR-01 ~ FR-08 from Specify v1.2.0 into a concrete implementation design.
Two files will be created. Implementation order follows dependency direction:
`GameContext` → `GlobalBootstrapper`.

---

## 2. Files to Create

| Path | Type | Note |
|---|---|---|
| `Assets/_Game/App/GameContext.cs` | Pure C# Class | No MonoBehaviour |
| `Assets/_Game/App/GlobalBootstrapper.cs` | MonoBehaviour | DontDestroyOnLoad Singleton |

> `FallbackErrorCanvas.prefab` — manual Unity Editor work, out of code-gen scope.

---

## 3. GameContext Design

### 3-1. Role

- **Service Container** for the entire game session
- Owns all Repository and UseCase instances
- Performs Manual DI wiring in its constructor
- Must be a pure C# class — MonoBehaviour is forbidden

### 3-2. Owned Instances

| Type | Class |
|---|---|
| Repository | `CharacterRepository`, `StageRepository` |
| UseCase | `CharacterUseCase`, `EvolutionUseCase`, `StageUseCase`, `BattleUseCase`, `EventUseCase`, `MiniGameUseCase` |

### 3-3. Public API

    + GameContext(masterData)           // Constructor: instantiate repos & usecases, wire DI
    + LoadAllDataAsync() : UniTask      // Parallel load via UniTask.WhenAll
    + SaveAllDataSync() : void          // Sequential SaveDataSync() for OnPause/Quit

### 3-4. DI Wiring Pattern

Repositories are constructed first, then injected into UseCases via constructor.
No UseCase receives the entire GameContext — only what it needs.

    _characterRepo    = new CharacterRepository();
    _stageRepo        = new StageRepository();
    _characterUseCase = new CharacterUseCase(_characterRepo);
    _evolutionUseCase = new EvolutionUseCase(_characterRepo);
    _stageUseCase     = new StageUseCase(_stageRepo);
    _battleUseCase    = new BattleUseCase(_characterRepo, _stageRepo);
    _eventUseCase     = new EventUseCase(_characterRepo, _stageRepo);
    _miniGameUseCase  = new MiniGameUseCase(_characterRepo);

---

## 4. GlobalBootstrapper Design

### 4-1. Role

- The sole permitted Singleton in the entire game (Constitution §2)
- **Composition Root** — creates and wires all Core systems and GameContext
- Persists across scene loads via `DontDestroyOnLoad`

### 4-2. Singleton Guard

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

### 4-3. Initialization Sequence (FR-02)

| Step | Action | Detail |
|---|---|---|
| 1 | Load MasterData | `Resources.LoadAll` — offloaded to ThreadPool |
| 2 | Instantiate Core systems | `SceneNavigator`, `PopupManager` |
| 3 | Construct `GameContext` | Pass MasterData; perform DI wiring |
| 4 | Load runtime data | `GameContext.LoadAllDataAsync()` — parallel via `UniTask.WhenAll` |
| 5 | Signal completion | `_initTcs.TrySetResult()` |

### 4-4. InitializationTask Exposure (FR-03)

    private readonly UniTaskCompletionSource _initTcs = new();
    public UniTask InitializationTask => _initTcs.Task;

All `SceneBootstrapper`s must `await GlobalBootstrapper.Instance.InitializationTask`
before starting their own initialization.

### 4-5. Error Handling — Fallback Canvas (FR-04)

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

- `FallbackCanvas` is a self-contained Canvas child of the GlobalBootstrapper GameObject
- No dependency on `IPopupManager`

### 4-6. ApplicationPause / Quit Handling (FR-07)

    private void OnApplicationPause(bool pause)
    {
        if (pause) _gameContext?.SaveAllDataSync();
    }

    private void OnApplicationQuit()
    {
        _gameContext?.SaveAllDataSync();
    }

- `?.` null-conditional used — Constitution §8 Safe Cleanup compliance
- Async save is strictly forbidden here

---

## 5. Core Interface Dependencies

The following interfaces must exist in `Assets/_Game/Core/` before implementation begins.

| Interface | Purpose |
|---|---|
| `ISceneNavigator` | Scene transition abstraction |
| `IPopupManager` | Popup display abstraction |

---

## 6. Implementation Order

1. Verify `Assets/_Game/Core/` interfaces exist
2. Implement `GameContext.cs` (pure C#, zero Unity dependencies)
3. Implement `GlobalBootstrapper.cs` (MonoBehaviour)
4. Manually create `FallbackErrorCanvas` Prefab in Unity Editor
5. Place `GlobalBootstrapper` GameObject in Bootstrap Scene

---

## 7. Validation Checklist

- [ ] `GameContext` does not extend `MonoBehaviour`
- [ ] No Singleton exists other than `GlobalBootstrapper`
- [ ] Init order: MasterData → Core systems → GameContext → RuntimeData
- [ ] No async save used in `OnApplicationPause` / `OnApplicationQuit`
- [ ] All `SceneBootstrapper`s await `InitializationTask` before self-init
- [ ] `FallbackCanvas` operates independently of `IPopupManager`
- [ ] 3 consecutive Retry failures trigger forced `Application.Quit()`
- [ ] All cleanup calls in `Dispose` / `OnDestroy` use `?.` null-conditional