# CLAUDE.md — Samsara Project

**Version:** 1.2.0 | **Date:** 2026-03-17

This file is automatically read by Claude Code on startup.
Read this before any Spec files. Apply all rules here to every file you create.

---

## Constitution (Architecture Bible)

**IMPORTANT: Read `constitution.md` before any implementation.**

- All rules in this CLAUDE.md are derived from the Constitution.
- In case of conflict between this file and the Constitution, **Constitution takes precedence.**
- Every architectural decision must comply with the Constitution.

### Reference Documents
- Constitution: `.claude/constitution.md`

---

## Project Overview

- **Project:** Samsara
- **Platform:** Mobile (Android / iOS)
- **Engine:** Unity 6.2
- **Architecture:** Feature-based Modular Architecture (DDD + Clean Architecture)

---

## Path Rules (Non-Negotiable)

All scripts MUST be inside `Assets/_Game/`. `Assets/Scripts/` does NOT exist.

| Folder | Purpose |
|---|---|
| `Assets/_Game/App/` | GlobalBootstrapper, GameContext, top-level wiring |
| `Assets/_Game/Core/` | Shared interfaces, base classes, utilities |
| `Assets/_Game/Features/` | Independent game modules (Inventory, Battle, etc.) |
| `Assets/_Game/Scenes/` | Scene files and SceneBootstrappers |
| `Assets/Resources/MasterData/` | ScriptableObject .asset files ONLY |

---

## Architecture Rules

### Dependency Injection
- Logic classes (Presenter, UseCase, Repository) MUST use constructor injection.
- `new` keyword is FORBIDDEN inside Logic classes.
- `Singleton.Instance` access is FORBIDDEN inside Logic classes.
- `new` is ONLY allowed inside `*Bootstrapper.cs` files.

### Bootstrapper Hierarchy
1. **GlobalBootstrapper** — Core system init, the ONLY allowed Singleton (`DontDestroyOnLoad`)
2. **SceneBootstrapper** — Scene orchestrator. Initializes FeatureBootstrappers from `Start()`.
3. **FeatureBootstrapper** — Passive. NO `Start()`/`Awake()` logic. Waits for `.Initialize()` from SceneBootstrapper.

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

## Libraries

| Library | Purpose | Import |
|---|---|---|
| UniTask | All async/await | `using Cysharp.Threading.Tasks;` |
| DOTween | Tween animation | `using DG.Tweening;` |
| Newtonsoft.Json | Serialization | `using Newtonsoft.Json;` |
| TextMeshPro | UI text | `using TMPro;` |

- Standard C# `Task` and Coroutines are FORBIDDEN. Use UniTask only.
- Fire-and-Forget: use `.Forget()`.

---

## Coding Standards

### Naming
- Public / Methods: `PascalCase`
- Private fields: `_camelCase` (underscore prefix)
- Suffixes: `*View`, `*Presenter`, `*UseCase`, `*Repository`, `*SO`

### Log Tag
Add to every class:

    private readonly string _logClass = $"[{nameof(ClassName)}]";

### Fail Fast
- NO null-guard on `[SerializeField]` UI components. NullReferenceException must fire immediately.
- On data integrity failure: throw `InvalidOperationException` immediately.

### Safe Cleanup
- In `OnDestroy()` / `Dispose()`: use `?.` null-conditional operator, NOT null checks.
- Reason: prevents Exception Masking.

### Inspector Variables
- Declare as `[SerializeField] private`. Never use `public` fields for internal state.

---

## Data Persistence Rules

- **Save Strategy:** Save-on-Action (save immediately on data change, no timer-based saves)
- **Async save:** `UniTask SaveDataAsync()` — for normal gameplay
- **Sync save:** `void SaveDataSync()` — ONLY for `OnApplicationPause` / `OnApplicationQuit`. async is STRICTLY FORBIDDEN here.
- **I/O:** MUST be offloaded to ThreadPool via `UniTask.RunOnThreadPool`
- **Serialization:** Use Newtonsoft.Json

---

## Project Status (Notion)

Project-wide editor state, scene configurations, global decisions, and manual task completion are tracked in the Notion **Project Status** document.

**Notion Page ID:** `32652975d2df812f93abd1eecf7dd634`

Before any implementation, use Notion MCP to fetch this page and review:
- Current scene configurations (cameras, objects, Canvas setup)
- Global decisions (e.g., Bootstrap scene has no camera)
- Manual tasks that must be completed in Unity Editor
- Known issues

---

## Spec Document Locations

All Spec documents are in Notion.

| Feature | Location |
|---|---|
| Project Status | `문서 > Project Status` |
| GlobalBootstrapper + GameContext | `문서 > [App] > GlobalBootstrapper + GameContext` |
| SceneNavigator | `문서 > [Core] > SceneNavigator` |
| PopupManager | `문서 > [Core] > PopupManager` |

When implementing, use the **Tasks-MD** file of the target Feature as your primary guide.

---

## Pre-Implementation Checklist

- [ ] Read this entire CLAUDE.md
- [ ] Read `constitution.md` (project root)
- [ ] Fetch **Project Status** from Notion (Page ID: `32652975d2df812f93abd1eecf7dd634`) and review current state
- [ ] Open the Tasks-MD for the target Feature
- [ ] Verify `Assets/_Game/` path rules
- [ ] Confirm all referenced files in Tasks exist
  - If missing → create empty interface stubs first, then proceed