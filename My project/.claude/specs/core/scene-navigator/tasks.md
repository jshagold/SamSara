# SceneNavigator — Tasks

**Version:** 1.0.0 | **Date:** 2026-03-16 | **Status:** Ready for Claude Code
**Based on:** Specify v1.1.0 / Plan v1.3.0

---

## Pre-Implementation Checklist

> Claude Code MUST verify the following before starting any task.

- [ ] Read `CLAUDE.md` first
- [ ] Confirm `Assets/_Game/Core/Navigation/` folder exists (create if missing)
- [ ] Confirm `Assets/_Game/App/` folder exists
- [ ] DO NOT modify any existing files unless explicitly listed below

---

## TASK-01 — SceneKey.cs

**Path:** `Assets/_Game/Core/Navigation/SceneKey.cs`
**Type:** Pure C# enum
**Priority:** First (no dependencies)

### Full Implementation

    namespace Samsara.Core.Navigation
    {
        public enum SceneKey
        {
            Bootstrap,
            Splash,
            Main,
            CharacterInfo,
            EvolutionTree,
            Maintenance,
            Stage,
            Battle,
            ActionEvent,
            Ending,
            Replay,
            GameOver
        }
    }

### Rules
- No numeric values assigned.
- No UnityEngine import.
- Values must exactly match Unity Build Settings scene names.
- The scene list may grow or shrink as the Roadmap evolves.
  Add or remove enum entries and keep in sync with Build Settings.

---

## TASK-02 — ISceneNavigator.cs

**Path:** `Assets/_Game/Core/Navigation/ISceneNavigator.cs`
**Type:** Pure C# interface
**Priority:** After TASK-01

### Full Implementation

    using System.Threading;
    using Cysharp.Threading.Tasks;

    namespace Samsara.Core.Navigation
    {
        public interface ISceneNavigator
        {
            UniTask NavigateToAsync(SceneKey key);
            UniTask NavigateToAsync(SceneKey key, CancellationToken ct);
        }
    }

### Rules
- NO UnityEngine namespace imports — strictly forbidden.
- UniTask is the only allowed external dependency.
- No implementation logic — interface declaration only.

---

## TASK-03 — SceneNavigator.cs

**Path:** `Assets/_Game/App/SceneNavigator.cs`
**Type:** Pure C# class (NOT MonoBehaviour)
**Priority:** After TASK-02

### Full Implementation

    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using Samsara.Core.Navigation;

    namespace Samsara.App
    {
        public class SceneNavigator : ISceneNavigator
        {
            private readonly string _logClass = $"[{nameof(SceneNavigator)}]";

            public UniTask NavigateToAsync(SceneKey key)
                => NavigateToAsync(key, CancellationToken.None);

            public async UniTask NavigateToAsync(SceneKey key, CancellationToken ct)
            {
                var sceneName = key.ToString();

                if (SceneManager.GetActiveScene().name == sceneName)
                {
                    Debug.Log($"{_logClass} Already at: {key}, skipping");
                    return;
                }

                Debug.Log($"{_logClass} Navigating to: {key}");
                await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single)
                    .ToUniTask(cancellationToken: ct);
                Debug.Log($"{_logClass} Arrived: {key}");
            }
        }
    }

### Rules
- Must NOT extend MonoBehaviour.
- `new` keyword is ONLY used by GlobalBootstrapper to instantiate this class.
- Do NOT call SceneManager anywhere else in the project.

---

## TASK-04 — GlobalBootstrapper Integration Check

**Priority:** After TASK-03
**Action:** No new files — verification only.

If `Assets/_Game/App/GlobalBootstrapper.cs` exists:
- Confirm it contains `_sceneNavigator = new SceneNavigator();`
- If missing, record in `decisions.md` and proceed.

If `GlobalBootstrapper.cs` does not yet exist:
- Record in `decisions.md` and proceed.
  (GlobalBootstrapper is implemented under a separate Spec.)

---

## TASK-05 — Validation

**Priority:** Final — after all tasks complete.

| # | Check | Method |
|---|---|---|
| V-01 | SceneKey values match Unity Build Settings scene names | Manual check in Editor |
| V-02 | ISceneNavigator has zero UnityEngine imports | Code review |
| V-03 | SceneNavigator does NOT extend MonoBehaviour | Code review |
| V-04 | Duplicate guard: navigating to active scene does nothing | Play Mode test |
| V-05 | NavigateToAsync loads the correct scene | Play Mode test (Bootstrap -> Main) |
| V-06 | No SceneManager calls exist outside SceneNavigator.cs | Search codebase |

---

## Claude Code Implementation Guide

- Read `CLAUDE.md` first before any implementation.
- **Files to create:**
  - `Assets/_Game/Core/Navigation/SceneKey.cs`
  - `Assets/_Game/Core/Navigation/ISceneNavigator.cs`
  - `Assets/_Game/App/SceneNavigator.cs`
- **Files to reference:**
  - `Assets/_Game/App/GlobalBootstrapper.cs` (if exists — verify SceneNavigator instantiation)
- **Implementation order:**
  1. `SceneKey.cs`
  2. `ISceneNavigator.cs`
  3. `SceneNavigator.cs`
  4. GlobalBootstrapper integration check
  5. Validation
- **DO NOT** create files outside `Assets/_Game/`.
- **DO NOT** call `SceneManager.LoadScene*` anywhere except `SceneNavigator.cs`.
- **DO NOT** make `SceneNavigator` a MonoBehaviour.
- **DO NOT** reference or copy patterns from `Assets/_Game/Dev/`.
- If you make any judgment calls not covered by this Spec,
  record them in `.claude/specs/core/scene-navigator/decisions.md`
  (the file is pre-created and empty)