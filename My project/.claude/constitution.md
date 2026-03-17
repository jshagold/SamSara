# Samsara Project Constitution

**Version:** 1.2.1 | **Ratification Date:** 2026-02-10 | **Last Amended:** 2026-03-17
**Status:** ✅ Active (Read-only — spec writing in progress)

> This document is the **Architecture Bible** of the Samsara Project.
> All design decisions, Spec documents, and code implementations MUST comply with this Constitution.
> Once spec/plan writing has begun, this document is **read-only**.
> Any modification requires explicit developer approval and must be recorded in the Constitution Changelog before taking effect.

---

## §1 — Project Overview

- **Project:** Samsara
- **Platform:** Mobile (Android / iOS)
- **Engine:** Unity 6.2
- **Architecture:** Feature-based Modular Architecture (DDD + Clean Architecture)

### Folder Structure

| Folder | Purpose |
|---|---|
| `Assets/_Game/App/` | GlobalBootstrapper, GameContext, top-level wiring, Core interface implementations |
| `Assets/_Game/Core/` | Engine/platform-independent interfaces, base classes, utilities |
| `Assets/_Game/Features/` | Independent game modules (each Feature is self-contained) |
| `Assets/_Game/Scenes/` | Scene files and SceneBootstrappers |
| `Assets/_Game/Dev/` | Development sandbox — relaxed rules for prototyping |
| `Assets/_Game/Art/` | Shared visual assets (fonts, textures, sprites not tied to a specific Feature) |
| `Assets/Resources/MasterData/` | ScriptableObject .asset files ONLY |

### Dev/ Graduation Checklist

Before moving any code from `Dev/` to `Features/`, ALL of the following must be satisfied:

1. Has a completed Specify + Plan + Tasks Spec set
2. Follows all Constitution rules (DI, naming, path, etc.)
3. No `Dev/`-specific shortcuts or hardcoded values remain
4. All dependencies are injected via constructor (no `FindObjectOfType`, no `GetComponent` in logic)
5. Reviewed and approved by the developer

---

## §2 — Zero Guessing Principle

- NEVER guess file paths, class names, API versions, or method signatures.
- If uncertain, ASK first.
- All paths are relative to `Assets/_Game/`. `Assets/Scripts/` does NOT exist.

### Singleton Rule

- `GlobalBootstrapper` is the ONLY permitted Singleton in the entire project.
- All other classes MUST use constructor injection via `GameContext`.
- `Manager.Instance` pattern is STRICTLY FORBIDDEN.

### Data/Logic Separation

- Data classes (Model, SO, SaveData) contain NO logic.
- Logic classes (UseCase, Repository) contain NO Unity UI references.
- Presenter bridges Domain and View — it does NOT directly manipulate UI elements.

---

## §3 — Bootstrapper Hierarchy

### Three-Tier Bootstrapper System

1. **GlobalBootstrapper** (MonoBehaviour, `DontDestroyOnLoad`)
   - The ONLY Singleton. Created once, persists across scene loads.
   - Composition Root: creates all Core systems and `GameContext`.
   - Owns `ISceneNavigator`, `IPopupManager`, and all global infrastructure.

2. **SceneBootstrapper** (MonoBehaviour)
   - One per scene. Orchestrates Feature initialization.
   - MUST `await GlobalBootstrapper.Instance.InitializationTask` before self-init.
   - Creates FeatureBootstrappers from `Start()`.

3. **FeatureBootstrapper** (MonoBehaviour)
   - Passive. NO `Start()`/`Awake()` logic.
   - Waits for SceneBootstrapper to call `.Initialize()`.
   - Creates Presenter, View, and wires dependencies.

### Instantiation Rules

- `new` keyword is ONLY allowed inside `*Bootstrapper.cs` files.
- Logic classes (Presenter, UseCase, Repository) MUST use constructor injection.
- `Singleton.Instance` access is FORBIDDEN inside Logic classes.

---

## §4 — Feature Module Structure

Each Feature follows Clean Architecture layers:

    Assets/_Game/Features/[FeatureName]/
    ├── Data/           # Repository implementations, SaveData
    ├── Domain/         # UseCase, Model, Interface definitions
    ├── Presentation/   # Bootstrapper, Presenter, View
    └── MasterData/     # ScriptableObject definitions (*SO.cs)

### Layer Rules

- **Data Layer** → May depend on Domain Layer interfaces only.
- **Domain Layer** → ZERO external dependencies. Pure C# only.
- **Presentation Layer** → Depends on Domain Layer. May reference Unity UI.
- **Cross-Feature** → Features communicate ONLY through shared interfaces in `Core/`.

---

## §5 — Async UI & Popup System

### UniTask-based Async Pattern

- ALL async operations use UniTask. Standard C# `Task` and Coroutines are FORBIDDEN.
- Fire-and-Forget: use `.Forget()`.
- Popup methods return `UniTask<T>` so Presenters can `await` the result.

### PopupManager Ownership

- Owned by `GlobalBootstrapper`, registered in `GameContext` as `IPopupManager`.
- Injected via Constructor — NEVER accessed as Singleton.
- **Domain layer (UseCase/Repository) may NOT call PopupManager.** Only Presentation layer.

### Object Pooling

- Use `UnityEngine.Pool.ObjectPool<T>` for frequently created/destroyed objects.
- Popup Views, VFX, and projectiles are primary candidates.
- Avoid `Instantiate`/`Destroy` in hot paths.

---

## §6 — Global Architecture

### GameContext

- Pure C# class (NOT MonoBehaviour).
- Service Container: owns all Repository and UseCase instances.
- Constructor performs Manual DI wiring.
- `LoadAllDataAsync()`: loads all Repository data via `UniTask.WhenAll`.
- `SaveAllDataSync()`: synchronous save for `OnApplicationPause`/`OnApplicationQuit`.

### Data Lifetime Separation

| Data Type | Lifetime | Description |
|---|---|---|
| **AccountData** | Permanent across runs | Unlocked evolution nodes, Codex, Gems |
| **RunData** | Single run (resets on reincarnation) | Character stats, Gold, Karma, current Day |

- `GameContext` holds `AccountRepository` and `RunRepository` as separate instances.
- NEVER mix both lifetimes in the same Repository class.

### Initialization Sequence

MasterData: Must be initialized **Asynchronously** before GameContext setup.
- Use await with a loading screen. Do NOT block the main thread.
- Must complete before any GameContext wiring begins.

**Order:** MasterData load → GameContext wiring → RuntimeData load → Scene ready.

### Scene Transition Management

- `ISceneNavigator` interface in `Core/Navigation/`, implementation in `App/`.
- `SceneKey` enum in `Core/Navigation/` — raw string scene names FORBIDDEN.
- ALL scene transitions go through `ISceneNavigator`. Direct `SceneManager.LoadScene*` calls outside `SceneNavigator` are FORBIDDEN.
- **Exception:** Main ↔ Maintenance transition uses camera movement, not scene load.

---

## §7 — Unity UI Standards

### Fail Fast

- NO null-guard on `[SerializeField]` UI components.
- `NullReferenceException` must fire immediately if a reference is missing.
- On data integrity failure: throw `InvalidOperationException` immediately.

### Reset() Auto-Assignment

- MonoBehaviour View classes implement `Reset()` for editor-time auto-assignment.
- Use `GetComponentInChildren<T>()` / `GetComponentsInChildren<T>()`.
- **Exception:** `Transform` and `RectTransform` are NOT auto-assigned.

### Component Caching

- Cache frequently accessed components in `Awake()` or constructor.
- Do NOT call `GetComponent<T>()` in `Update()` or hot paths.

---

## §8 — Coding Standards

### Naming Conventions

- Public / Methods: `PascalCase`
- Private fields: `_camelCase` (underscore prefix)
- Suffixes: `*View`, `*Presenter`, `*UseCase`, `*Repository`, `*SO`

### Log Tag

Every class MUST include:

    private readonly string _logClass = $"[{nameof(ClassName)}]";

### Inspector Variables

- Declare as `[SerializeField] private`. NEVER use `public` fields for internal state.

### Safe Cleanup

- In `OnDestroy()` / `Dispose()`: use `?.` null-conditional operator, NOT null checks.
- Reason: prevents Exception Masking.

### GC Optimization

- NO `LINQ` or `new` allocations inside `Update()`.
- Use pre-allocated arrays and cached values.
- Object Pooling for frequently created/destroyed objects.

### Forbidden Patterns

| Forbidden | Correct Alternative |
|---|---|
| `Manager.Instance` Singleton | DI via GameContext |
| `new` inside Logic classes | Instantiate in Bootstrapper only |
| Direct UI manipulation in Presenter | Delegate to View methods |
| `Assets/Scripts/` path | Use `Assets/_Game/` |
| Textures/Audio in `Resources/` | Direct Reference or Addressables |
| Hardcoded game data | MasterData (ScriptableObject) |
| LINQ / `new` inside `Update()` | Pre-allocated arrays / cached values |

---

## §9 — Data Persistence

### Save Strategy: Save-on-Action

- Save immediately on data change. NO timer-based auto-save.
- Each UseCase calls `Repository.SaveDataAsync()` after completing a data-changing action.

### Dual-Mode Saving

| Mode | Method | When |
|---|---|---|
| Async | `UniTask SaveDataAsync()` | Normal gameplay |
| Sync | `void SaveDataSync()` | `OnApplicationPause` / `OnApplicationQuit` ONLY |

- **async is STRICTLY FORBIDDEN** in `OnApplicationPause` / `OnApplicationQuit`.
- OS may terminate the process immediately — sync save is the only safe option.

### I/O Rules

- File I/O MUST be offloaded to ThreadPool via `UniTask.RunOnThreadPool`.
- Serialization: Use Newtonsoft.Json.

### Dirty Flag Pattern

- Repository tracks whether data has changed via a dirty flag.
- `SaveDataAsync()` skips write if not dirty.
- Reduces unnecessary I/O operations.

---

## §10 — Wrapper Pattern

### Interface/Implementation Separation

- **Interfaces** (`I*`) reside in `Assets/_Game/Core/` — engine/platform independent.
- **Implementations** reside in `Assets/_Game/App/` — may depend on Unity.
- This enables mock injection for testing and future engine migration.

### Examples

| Interface (Core) | Implementation (App) |
|---|---|
| `ISceneNavigator` | `SceneNavigator` |
| `IPopupManager` | `PopupManager` |

---

## §11 — Libraries

| Library | Purpose | Import |
|---|---|---|
| UniTask | All async/await | `using Cysharp.Threading.Tasks;` |
| DOTween | Tween animation | `using DG.Tweening;` |
| Newtonsoft.Json | Serialization | `using Newtonsoft.Json;` |
| TextMeshPro | UI text | `using TMPro;` |

- Standard C# `Task` and Coroutines are FORBIDDEN. Use UniTask only.
- Fire-and-Forget: use `.Forget()`.

---

## Governance

### Amendment Procedure

1. Propose change with rationale.
2. Record in Constitution Changelog with before/after values.
3. Obtain explicit developer approval.
4. Update this document.
5. Propagate changes to dependent documents (CLAUDE.md, Spec templates).

### Versioning Policy

- **MAJOR:** Backward-incompatible principle removals or redefinitions.
- **MINOR:** New principle/section added or materially expanded.
- **PATCH:** Clarifications, wording, typo fixes.

### Compliance

- All Spec documents (Specify, Plan, Tasks) MUST reference applicable Constitution sections.
- Code reviews should verify Constitution compliance.
- Any violation must be flagged immediately — no silent exceptions.