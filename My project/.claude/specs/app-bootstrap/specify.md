# GlobalBootstrapper + GameContext — Specify

**Version:** 1.1.0 | **Date:** 2026-03-11 | **Status:** All OQs Resolved
**Category:** App
**Constitution Ref:** §2 (Singleton Rule), §3 (Bootstrapper Hierarchy), §6 (Global Architecture)

---

## 1. Purpose

`GlobalBootstrapper` is the sole permitted Singleton in the entire game (Constitution §2).
It initializes once per app lifetime, acts as the **Composition Root**, and wires all Core systems
and `GameContext` via constructor injection.

`GameContext` is the **Service Container** for the session.
It holds all Repository and UseCase instances.
Feature classes receive only the specific UseCases/Repositories they need — never the entire `GameContext`.

---

## 2. Functional Requirements

### FR-01: Single Singleton Guarantee
- `GlobalBootstrapper` persists across scene loads via `DontDestroyOnLoad`.
- Duplicate instances self-destruct on `Awake`.

### FR-02: Ordered Initialization Sequence
Must execute in this exact order:
1. MasterData async load (`Resources.LoadAll` on ThreadPool)
2. Core system instantiation (`SceneNavigator`, `PopupManager`)
3. `GameContext` construction + Manual DI wiring
4. Runtime data async load (`GameContext.LoadAllDataAsync()`)
5. `InitializationTask` completion signal

### FR-03: InitializationTask Exposure
- Expose `public UniTask InitializationTask` as a public property.
- All `SceneBootstrapper`s must `await` this before starting their own initialization.

### FR-04: Error Handling — Fallback Canvas UI
- On any exception during initialization: `Debug.LogError` → show **Fallback Canvas UI** (Retry / Quit).
- Fallback Canvas is a minimal self-contained Canvas owned by `GlobalBootstrapper`.
  No dependency on `IPopupManager`.
- Retry → restart initialization loop from FR-02 Step 1.
- Quit → `Application.Quit()`.
- After 3 consecutive Retry failures → force Quit automatically.

### FR-05: GameContext — Repository & UseCase Ownership
`GameContext` constructs and owns:

| Type | Class |
|---|---|
| Repository | `CharacterRepository`, `StageRepository` |
| UseCase | `CharacterUseCase`, `EvolutionUseCase`, `StageUseCase`, `BattleUseCase`, `EventUseCase`, `MiniGameUseCase` |

Each UseCase receives its required Repositories via constructor.

### FR-06: LoadAllDataAsync
- Runs all Repository `LoadDataAsync()` calls in parallel via `UniTask.WhenAll`.
- On failure: throw `InvalidOperationException` (Fail Fast).

### FR-07: ApplicationPause / Quit Handling
- Call all Repository `SaveDataSync()` on `OnApplicationPause(true)` and `OnApplicationQuit()`.
- Async save is **forbidden** here — OS may kill the process immediately.

### FR-08: Save-on-Action Strategy
- No time-based auto-save interval.
- Each UseCase calls `Repository.SaveDataAsync()` immediately after any data-mutating action completes.
- **Confirmed global triggers:** stat change, stage node selection, item add/use, battle start.
- **1st dev note:** No mid-battle save. On resume after forced quit, restore to battle start state.
- Additional triggers are defined per Feature spec.

---

## 3. Non-Functional Requirements

- `GlobalBootstrapper` is a `MonoBehaviour` attached to a single persistent GameObject.
- `GameContext` is a **pure C# class** — no `MonoBehaviour`.
- No game logic may execute before `InitializationTask` completes.
- `GetComponent` / `FindObjectOfType` forbidden
  (except in `GlobalBootstrapper.Awake` for self-reference).

---

## 4. Data Lifetime

| Data | Lifetime | Owner |
|---|---|---|
| MasterData (SO) | Full app lifetime | `GlobalBootstrapper` (via `GameContext`) |
| AccountData | Persists across runs | `CharacterRepository` |
| RunData | Single run — reset on GameOver/Ending | `CharacterRepository`, `StageRepository` |

---

## 5. Resolved Questions

| ID | Question | Resolution | Rationale |
|---|---|---|---|
| OQ-01 | Save timing strategy? | **Save-on-Action** — no time interval. Save on: stat change, stage node selection, item add/use, battle start. Additional triggers per Feature spec. | Mobile roguelite: forced quit is frequent; data loss during a run is critical. |
| OQ-02 | Retry popup before `IPopupManager` is ready? | **Fallback Canvas UI** — GlobalBootstrapper-owned minimal Canvas (Retry/Quit). Force Quit after 3 failures. | Mobile standard pattern. Quit without Retry is perceived as a crash, hurting retention. |
| OQ-03 | MasterData loading strategy? | **`Resources.LoadAll` only** | Constitution §3 explicit requirement. |

---

## Claude Code Implementation Guide

- Read `CLAUDE.md` first before any implementation.
- **Files to create:**
  - `Assets/_Game/App/GlobalBootstrapper.cs`
  - `Assets/_Game/App/GameContext.cs`
  - `Assets/_Game/App/Presentation/Prefabs/FallbackErrorCanvas.prefab` (manual Unity work)
- **Files to reference:**
  - `Assets/_Game/Core/` (IRepository interfaces, IPopupManager, ISceneNavigator)
- **Implementation order:**
  1. `GameContext.cs` (pure C# — no Unity deps)
  2. `GlobalBootstrapper.cs` (MonoBehaviour wrapper)
  3. FallbackErrorCanvas prefab (Unity Editor)
- **DO NOT** create files outside `Assets/_Game/`.
- **DO NOT** use `Manager.Instance` patterns.
- **DO NOT** make `GameContext` a MonoBehaviour.
- **Decisions:** If you encounter a code-level judgment not covered by this Spec,
  make the decision and record it in `Decisions - MD` with the DEC-XX format.
  Do NOT record decisions already explicit in Specify/Plan/Tasks.