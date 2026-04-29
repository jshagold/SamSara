> Holds only the finalized MD file content. Ctrl+A → Ctrl+C, then paste directly into the file.
---
## Version History
<table header-row="true">
<tr>
<td>Version</td>
<td>Date</td>
<td>Changes</td>
</tr>
<tr>
<td>v1.0.0</td>
<td>2026-04-21</td>
<td>Initial — Phase 6 ReplayScene new Feature + Core/Tree shared component extraction</td>
</tr>
<tr>
<td>v2.0.0</td>
<td>2026-04-28</td>
<td>Full rewrite. Aligned with Plan v2.0.0. The v1.0.0 structural decisions based on Plan v1.0.0 (Core/Tree extraction / single NodeState enum / Selectable→Evolvable workaround) were retired in Plan v2.0.0, requiring a full re-edit of Tasks. v1.0.0 body archived under the Tasks Archive folder (GBL-002 / 📐 Archive procedure page applied).</td>
</tr>
</table>
> **v1.0.0 body archive:** Tasks folder \> 🗄️ Archive \> 📦 v1.0.0 (tracks GBL-002 — major version up archival policy for prior body).
---
## Current Version: v2.0.0
```markdown
# ReplayScene — Tasks

**Status:** ⏳ v2.0.0 in progress
**Feature:** ReplayScene (Phase 6)
**Base:** Specify v2.1.1 / Plan v2.0.0
**Constitution Reference:** §1 ~ §11 full verification (Tasks authoring focuses on §3 / §4 / §6 / §7 / §8 / §9)
**Work Type:** Full rewrite over partial v1.x.x implementation (mix of Create + Modify + Rename)

---

## Version History

| Version | Date | Changes |
|---|---|---|
| v1.0.0 | 2026-04-21 | Initial (based on Specify v1.1.0 / Plan v1.0.0) |
| v2.0.0 | 2026-04-28 | Full rewrite. Aligned with Plan v2.0.0. The v1.0.0 structural decisions based on Plan v1.0.0 (Core/Tree extraction / single NodeState enum / Selectable→Evolvable workaround) were retired in Plan v2.0.0, requiring a full re-edit of Tasks. v1.0.0 body archived under the Tasks Archive folder (GBL-002 / 📐 Archive procedure page applied). |

> **v1.0.0 body archive:** Tasks folder > 🗄️ Archive > 📦 v1.0.0 (tracks GBL-002 — major version up archival policy for prior body).

---

## Current Version: v2.0.0

# §0. Overview

## 0-1. Scope of these Tasks

These Tasks cover **the ReplayScene Feature area only**.

- New / rewritten files inside the ReplayScene Feature folder (`Assets/_Game/Features/ReplayScene/`)
- ReplayScene-specific enum definition (`Assets/_Game/Core/Tree/ReplayNodeState.cs`)
- GameContext formal change (only adding `PendingReplayContext` auto-property, G-22)
- Replay.unity scene file creation + Build Settings + Inspector wiring (Manual Work)

The following items are **outside the scope of these Tasks** and are handled via a separate Patch / work flow (Plan §11 OQ-X1 · OQ-X2):

- Changes on the EvolutionTreeScene side (full NodeView signature change / NodeState→EvolutionNodeState rename / EvolutionTreePresenter mapping ownership change / NodeDescriptionPopupView Dim missing fix) → **EvolutionTreeScene Patch** (separate issuance pending)
- Changes on the EndingScene side (PendingReplayContext set logic addition) → **EndingScene Patch-006** (separate issuance pending)

## 0-2. Relationship with the partial v1.x.x implementation

ReplayScene already has some code implemented at the v1.x.x stage (Project Status §1 — "🟡 implementation re-verification pending"). These Tasks are not a simple augmentation of that but a **re-edit** based on Plan v2.0.0.

- Already existing files: `ReplaySceneUseCase.cs` / `ReplayScenePresenter.cs` / `ReplaySceneView.cs` / `ReplaySceneBootstrapper.cs` / `Popup/NodeDescriptionPopupView.cs`
- v2.0.0 handling policy: **Rename + rewrite** (compliant with majority naming convention — only Bootstrapper retains the "Scene" suffix; the rest do not include "Scene")

## 0-3. Assumed application state of external prerequisite work

These Tasks are written assuming all of the following external prerequisite work has been applied.

| # | Patch | Status |
|---|---|---|
| 1 | CharacterRepository Patch-003 (LastRunResult enum + InitializeNewRun signature extension) | ✅ Applied (2026-04-21) |
| 2 | InventorySystem Patch-001 (InitializeNewRun(RunConfigSO) signature unification) | ✅ Applied (2026-04-21) |
| 3 | EndingScene Patch-005 (LastRunResult save + HandleRestart SceneKey.Replay restoration) | ✅ Applied (2026-04-21) |
| 4 | EndingScene Patch-006 (PendingReplayContext set logic addition) | 📤 **Issued before Tasks; applied within these Tasks immediately after T16** (Patch-006 code references T2 / T16 outputs, so application earlier than T16 fails to compile) |
| 5 | EvolutionTreeScene Patch (NodeView signature change + EvolutionTreePresenter mapping + Dim fix) | ⏳ **Must be issued and applied before entering these Tasks** |

EvolutionTreeScene Patch (⏳) must be issued and applied before Claude Code implementation of these Tasks begins; these Tasks use its application result as fact (e.g., existence of `Core/Tree/EvolutionNodeState.cs`, `NodeView.SetFrameSprite/SetIconSprite` signatures).

EndingScene Patch-006 (📤) is **issued** before Tasks but **applied within** these Tasks immediately after T16, because its code (`new PendingReplayContext { ... }` + `gameContext.PendingReplayContext = ...`) requires T2 (class definition) and T16 (GameContext property) to compile. Verification therefore occurs after T16 application (§1-2 below).

## 0-4. Plan §11 OQ resolution

| Plan OQ ID | Resolution in these Tasks |
|---|---|
| OQ-P1 | **Patch separation adopted** — EvolutionTreeScene changes go in a separate Patch. Outside the scope of these Tasks. |
| OQ-P2 | **EvolutionTreeScene Patch responsibility** — The handling of `NodeView._originalIcon` is decided in the EvolutionTreeScene Patch. |
| OQ-P3 | **Handled in these Tasks' Manual Work** — Layout of `ReplayNodeDescriptionPopupView` / `ReplaySkillDescriptionPopupView`. |
| OQ-P4 | **Handled in these Tasks' Manual Work** — `Replay.unity` scene file creation + Build Settings registration. |
| OQ-G1 | **Accumulated in Project Backlog GBL-001**. New ReplayScene hardcoding count handled in the post-Tasks follow-up checklist. |
| OQ-G2 | **The Plan v1.0.0 archival incident has been handled separately**. At the Tasks-KR Notion upload time, the Tasks-KR v1.0.0 body is also archived via the same procedure. |
| OQ-G3 ~ OQ-G9 | **Phase 7 / future planning timing handling** — outside the scope of these Tasks. |

---

# §1. Prerequisites

## 1-1. Required reading (Claude Code)

Claude Code must read the following before starting implementation.

- `CLAUDE.md` (project root)
- `.claude/specs/features/replay-scene/specify.md` (Specify v2.1.1)
- `.claude/specs/features/replay-scene/plan.md` (Plan v2.0.0)
- `.claude/specs/features/replay-scene/tasks.md` (the MD version of this document — Tasks v2.0.0)
- `.claude/specs/features/replay-scene/decisions.md` (created empty in T0)

## 1-2. External prerequisite application status check

Before entering these Tasks, the following two items must already be applied.

| Patch | Verification method |
|---|---|
| EndingScene Patch-006 (PendingReplayContext set) | (Verified after T16 application within these Tasks — see §0-3) A line `gameContext.PendingReplayContext = new PendingReplayContext { ... }` exists immediately before the SceneKey.Replay transition inside `HandleRestart()` of `Assets/_Game/Features/Ending/Presentation/EndingPresenter.cs` |
| EvolutionTreeScene Patch | `Assets/_Game/Core/Tree/EvolutionNodeState.cs` exists (5 values) + `NodeView.SetFrameSprite(Sprite)` / `SetIconSprite(Sprite)` signatures + no `NodeState` enum branching switch inside `NodeView` + Dim background applied to EvolutionTreeScene `NodeDescriptionPopupView` |

Starting these Tasks while either of the above two Patches is unapplied causes either compile errors or implementation-assumption mismatches. If unapplied, ask Hak to confirm application status and wait.

## 1-3. Read existing partial ReplayScene implementation files

In the Pre-Implementation step, verify the current state of the following 5 files (Rename + rewrite targets).

- `Assets/_Game/Features/ReplayScene/Domain/ReplaySceneUseCase.cs`
- `Assets/_Game/Features/ReplayScene/Presentation/ReplayScenePresenter.cs`
- `Assets/_Game/Features/ReplayScene/Presentation/ReplaySceneView.cs`
- `Assets/_Game/Features/ReplayScene/Presentation/ReplaySceneBootstrapper.cs`
- `Assets/_Game/Features/ReplayScene/Presentation/Popup/NodeDescriptionPopupView.cs`

These files are fully rewritten with the new signatures and responsibilities defined by these Tasks. The existing bodies do not match Plan v2.0.0, so use them only as reference.

## 1-4. External dependency type confirmation (Zero Guessing — Constitution §2)

Finalized values based on Plan §5-0 pre-decisions + code investigation. **No guessing — use these as-is in code.**

| Target | Final value |
|---|---|
| GlobalBootstrapper.Instance | `static`, Singleton |
| GlobalBootstrapper init wait | `await GlobalBootstrapper.Instance.InitializationTask;` |
| GameContext access | `var gameContext = GlobalBootstrapper.Instance.GameContext;` |
| GameContext.CharacterAccountRepo | `ICharacterAccountRepository` |
| GameContext.SkillMasterDataRepo | `ISkillMasterDataRepository` |
| GameContext.EvolutionNodes | `EvolutionNodeSO[]` |
| GameContext.SceneNavigator | `ISceneNavigator` |
| GameContext.PopupManager | `IPopupManager` |
| GameContext.SpriteLoader | `ISpriteLoader` |
| GameContext.ResetRunForReplayAsync | `(int) → UniTask` (do not modify) |
| GameContext.PendingReplayContext | **Newly added auto-property in T16 of these Tasks** (G-22) |
| EvolutionNodeSO.NodeId | `string` |
| EvolutionNodeSO.CharacterName | `string` |
| EvolutionNodeSO.BaseStats | `CharacterStatsSO` |
| EvolutionNodeSO.IsHidden | `bool` |
| EvolutionNodeSO.SkillIds | `int[]` |
| EvolutionNodeSO.NodeIconSpriteKey | `string` |
| EvolutionNodeSO.UnlockConditions | `StatCondition[]` (plural) |
| ICharacterAccountRepository.AccountData | `CharacterAccountData` |
| CharacterAccountData.UnlockedEvolutionNodeIds | `List<string>` (direct string comparison) |
| PopupRequest constructor | `new PopupRequest(string title, string message, string confirmText, string cancelText)` |
| IPopupManager.ShowYesNoAsync | `UniTask<bool> ShowYesNoAsync(PopupRequest, CancellationToken = default)` |
| ISceneNavigator.NavigateToAsync | `UniTask NavigateToAsync(SceneKey, CancellationToken = default)` |
| SceneKey.Replay / SceneKey.Main | enum values exist |
| LastRunResult | `enum { None, GameOver, Ending }` (defined by prerequisite Patch-003) |
| SkillSO.SkillName | `string` |
| SkillSO.Description | `string` |
| SkillSO.Damage | `float` |
| SkillSO.IconSpriteKey | `string` |

## 1-5. Global search for impact scope assessment

Claude Code performs a global search to verify whether the following symbols are referenced outside the ReplayScene Feature.

- `ReplaySceneUseCase` (class name — renamed to `ReplayUseCase` in T8)
- `ReplayScenePresenter` (class name — renamed to `ReplayPresenter` in T14)
- `ReplaySceneView` (class name — renamed to `ReplayView` in T13)
- `Features/ReplayScene/Presentation/Popup/NodeDescriptionPopupView` references (renamed to `ReplayNodeDescriptionPopupView` in T12)

If any code outside ReplayScene references the symbols above (e.g., a call site in GameContext.cs), update them together within the scope of these Tasks.

## 1-6. Constitution core article mapping

Mapping of which Constitution article each Task is directly tied to.

| Constitution article | Application location in these Tasks |
|---|---|
| §3 Bootstrapper Hierarchy | T15 (Start() / GlobalBootstrapper wait / `new` usage location) |
| §4 Clean Architecture | T2 ~ T8 (Domain Layer pure C# / no external dependencies) / T12 ~ T14 (Presentation Layer / no direct View manipulation) |
| §6 GameContext / Data Lifetime / SceneKey | T16 (PendingReplayContext addition — G-22) |
| §7 Reset() Auto-Assignment / Fail Fast | T10 / T11 / T12 / T12-X1 / T12-X2 / T13 (View classes require Reset(), no null-guard) |
| §8 Naming / LogTag / SafeCleanup / GC | All Tasks in common (`_camelCase` / `_logClass` / `?.` SafeCleanup / no LINQ) |
| §9 Save Strategy | T8 (UseCase calls GameContext.ResetRunForReplayAsync — guarantees internal SaveAllDataSync) |

## 1-7. UI text inline literals (accumulated in Project Backlog GBL-001)

The ReplayScene Feature in these Tasks **writes UI exposure text as Korean C# inline literals** under the system prompt §2 grace policy. To be cleaned up in bulk when the Phase 7 global UI text management system is built.

- Every UI text hardcoding line must include the comment `// UI text hardcoded: tracked in Project Backlog GBL-001. Subject to Phase 7 global cleanup.` immediately above (Phase 7 grep target).
- After completion of these Tasks' implementation, in the follow-up checklist step, register the new ReplayScene occurrence count summed into the Project Backlog GBL-001 table.

## 1-8. Task ID system

| Category | ID range | Meaning |
|---|---|---|
| Implementation Task | T0 ~ T16 | Code work performed directly by Claude Code |
| Manual Work | M-XX | Performed by Hak in the Unity Editor (Claude Code cannot do) |
| Validation | V-XX | Code-automated verification (static analysis + console) |
| Runtime Validation | R-XX | Hak verifies scenarios in Play mode |

---

# §2. Implementation Tasks

## 2-0. Task list + dependency order

Total 19 items + Manual Work.

| # | Task | Location | Action |
|---|---|---|---|
| T0 | decisions.md | `.claude/specs/replay-scene/` | Create (empty file) |
| T1 | ReplayNodeState.cs | `Assets/_Game/Core/Tree/` | Create |
| T2 | PendingReplayContext.cs | `Assets/_Game/Features/ReplayScene/Domain/` | Create |
| T3 | IReplayUseCase.cs | same | Create |
| T4 | IRestartFlow.cs | same | Create |
| T5 | DefaultRestartFlow.cs | same | Create |
| T6 | IPostRestartSceneRouter.cs | same | Create |
| T7 | DefaultPostRestartSceneRouter.cs | same | Create |
| T8 | ReplaySceneUseCase.cs → ReplayUseCase.cs | same | **Rename + rewrite** |
| T9 | ReplayNodeDescriptionData (top-level struct inside the T12 file) | `Assets/_Game/Features/ReplayScene/Presentation/Popup/` | Create (same file as T12) |
| T10 | OptionButtonView.cs | `Assets/_Game/Features/ReplayScene/Presentation/` | Create |
| T11 | ReplayGuidanceView.cs | same | Create |
| T12 | NodeDescriptionPopupView.cs → ReplayNodeDescriptionPopupView.cs | `Assets/_Game/Features/ReplayScene/Presentation/Popup/` | **Rename + rewrite** |
| T12-A | ReplaySkillDescriptionPopupView.cs | same | Create (new skill description popup) |
| T12-B | ReplaySkillDescriptionData (top-level struct inside the T12-A file) | same | Create (same file as T12-A) |
| T12-X1 | ReplaySkillSlotView.cs | same | Create (single skill slot) |
| T12-X2 | ReplaySkillListView.cs | same | Create (slot container) |
| T13 | ReplaySceneView.cs → ReplayView.cs | `Assets/_Game/Features/ReplayScene/Presentation/` | **Rename + rewrite** |
| T14 | ReplayScenePresenter.cs → ReplayPresenter.cs | same | **Rename + rewrite** |
| T15 | ReplaySceneBootstrapper.cs | same | **Rewrite (name retained)** |
| T16 | GameContext.cs (only add PendingReplayContext auto-property) | `Assets/_Game/App/` | Modify (G-22) |

**Dependency order:**

T0 → T1 ~ T7 (Domain new) → T8 (Domain Rename) → T10 / T11 (simple components) → T12-X1 → T12-X2 (slot container — depends on T12-X1) → T9 / T12 (node info popup + data struct — depends on T12-X2) → T12-B / T12-A (skill description popup + data struct) → T13 (View — depends on T10 / T11 / T12 / T12-X2) → T14 (Presenter — depends on T8 / T13 / T12-A) → T15 (Bootstrapper — depends on T13 / T14) → T16 (GameContext PendingReplayContext addition)

---

## 2-1. T0 ~ T7 (Domain new)

### Task 0: Create empty decisions.md

**Path:** `.claude/specs/replay-scene/decisions.md`

**Work:**

- Create as a new empty file.
- Whenever a judgment not specified in Spec / Plan / Tasks is required during these Tasks' implementation, Claude Code records it directly in this file.
- Recording format: tag each entry with one of `[DECISION]`, `[BACKLOG]`, `[SPEC-GAP]`.

**Constitution satisfied:** §2 (traceability of judgments not in the Spec).

---

### Task 1: Create Core/Tree/ReplayNodeState.cs

**Path:** `Assets/_Game/Core/Tree/ReplayNodeState.cs`

**Background:** ReplayScene-specific node state classification enum. A separate enum from `EvolutionNodeState` already separated by the EvolutionTreeScene Patch.

**Work:**

- Create a new file. Pure C# enum (no Unity reference, Constitution §1 Core/ rule).
- namespace: `Samsara.Core.Tree`
- 3 enum values (Plan §5-2):
    - `Selectable` — nodes contained in AccountData.UnlockedEvolutionNodeIds
    - `Locked` — unlocked + IsHidden == false nodes
    - `Hidden` — unlocked + IsHidden == true nodes

**Example code:**

    namespace Samsara.Core.Tree
    {
        public enum ReplayNodeState
        {
            Selectable,
            Locked,
            Hidden,
        }
    }

**RQ satisfied:** RQ-09, RQ-13, RQ-14.

**Constitution:** §1 (Core/ is engine-independent), §4 (Domain data type), §8 (PascalCase enum values).

---

### Task 2: Create PendingReplayContext.cs

**Path:** `Assets/_Game/Features/ReplayScene/Domain/PendingReplayContext.cs`

**Background:** Inter-scene data transfer container. EndingScene sets it; ReplaySceneBootstrapper reads it immediately after entry (Plan §6-1, G-22).

**Work:**

- Pure C# class (no Unity reference, Constitution §4 Domain rule).
- namespace: `Samsara.Features.ReplayScene.Domain`
- 1 mutable auto-property:
    - `LastRunResult PreviousRunResult { get; set; }`
- Add `using Samsara.Features.Character.MasterData;` (or the actual namespace where LastRunResult is defined) — Claude Code verifies the LastRunResult definition location and applies it consistently.

**Example code:**

    using Samsara.Features.Character.MasterData;  // confirm actual LastRunResult namespace and apply consistently

    namespace Samsara.Features.ReplayScene.Domain
    {
        public class PendingReplayContext
        {
            public LastRunResult PreviousRunResult { get; set; }
        }
    }

**Design rationale (Plan §5-1):**

- Consistent with the other scenes' PendingContext patterns (PendingEndingContext / PendingEventContext) — mutable auto-property.
- The 1st-development field is `PreviousRunResult` only. Future expansions (reached stage, accumulated karma, etc.) are handled by adding new fields (RQ-04).
- No `_logClass` field — data-only class (Constitution §2 Data/Logic Separation).

**RQ satisfied:** RQ-03, RQ-04.

**Constitution:** §4 (Domain Layer pure C#), §6 (Data Lifetime — RunData-scope one-time container).

---

### Task 3: Create IReplayUseCase.cs

**Path:** `Assets/_Game/Features/ReplayScene/Domain/IReplayUseCase.cs`

**Background:** Core behavioral interface of the ReplayScene Domain Layer (Plan §5-3).

**Work:**

- Pure C# interface.
- namespace: `Samsara.Features.ReplayScene.Domain`
- 3 methods:
    - `IReadOnlyList<EvolutionNodeSO> GetAllNodes()`
    - `ReplayNodeState ClassifyNode(EvolutionNodeSO node)`
    - `UniTask ExecuteRestartAsync(string selectedNodeId, CancellationToken ct = default)`

**Example code:**

    using System.Collections.Generic;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using Samsara.Core.Tree;
    using Samsara.Features.Character.MasterData;  // confirm actual EvolutionNodeSO namespace and apply consistently

    namespace Samsara.Features.ReplayScene.Domain
    {
        public interface IReplayUseCase
        {
            IReadOnlyList<EvolutionNodeSO> GetAllNodes();
            ReplayNodeState ClassifyNode(EvolutionNodeSO node);
            UniTask ExecuteRestartAsync(string selectedNodeId, CancellationToken ct = default);
        }
    }

**RQ satisfied:** Constitution §3 (interface separation), §4 (Domain Layer interface).

---

### Task 4: Create IRestartFlow.cs

**Path:** `Assets/_Game/Features/ReplayScene/Domain/IRestartFlow.cs`

**Background:** Restart execution extension point interface (Plan §5-4, RQ-07, Specify §5-3).

**Work:**

- Pure C# interface.
- namespace: `Samsara.Features.ReplayScene.Domain`
- 1 method:
    - `UniTask ExecuteAsync(string selectedNodeId, PendingReplayContext pendingContext, CancellationToken ct = default)`

**Example code:**

    using System.Threading;
    using Cysharp.Threading.Tasks;

    namespace Samsara.Features.ReplayScene.Domain
    {
        public interface IRestartFlow
        {
            UniTask ExecuteAsync(string selectedNodeId, PendingReplayContext pendingContext, CancellationToken ct = default);
        }
    }

**Design rationale:**

- The `pendingContext` parameter is unused in the 1st-development. Included in the signature so that future IRestartFlow implementations can add `PreviousRunResult`-based branching (Plan §5-4 / §8-2).

**RQ satisfied:** RQ-07, Specify §5-3 (extension structure).

---

### Task 5: Create DefaultRestartFlow.cs

**Path:** `Assets/_Game/Features/ReplayScene/Domain/DefaultRestartFlow.cs`

**Background:** 1st-development implementation of IRestartFlow. A thin wrapper of GameContext.ResetRunForReplayAsync (Plan §5-4).

**Work:**

- Pure C# class.
- namespace: `Samsara.Features.ReplayScene.Domain`
- `_logClass = $"[{nameof(DefaultRestartFlow)}]";` field (Constitution §8 Log Tag).
- Ctor dependency: `GameContext _gameContext` — target of `ResetRunForReplayAsync(int)` call.
- Body operation (Plan §5-4):
    1. Convert `selectedNodeId` from string to int (Plan §5-0 — conversion responsibility resides in this class).
    2. On conversion failure, propagate the natural throw exception as-is (Constitution §7 Fail Fast — no try-catch).
    3. `await _gameContext.ResetRunForReplayAsync(intNodeId)`
    4. `pendingContext` is unused in the 1st implementation.

**Example code:**

    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using Samsara.App;  // confirm actual GameContext namespace and apply consistently

    namespace Samsara.Features.ReplayScene.Domain
    {
        public class DefaultRestartFlow : IRestartFlow
        {
            private readonly string _logClass = $"[{nameof(DefaultRestartFlow)}]";
            private readonly GameContext _gameContext;

            public DefaultRestartFlow(GameContext gameContext)
            {
                _gameContext = gameContext;
            }

            public async UniTask ExecuteAsync(string selectedNodeId, PendingReplayContext pendingContext, CancellationToken ct = default)
            {
                if (string.IsNullOrEmpty(selectedNodeId))
                    throw new InvalidOperationException($"{_logClass} ExecuteAsync: selectedNodeId is null or empty");

                // Fail Fast — string → int conversion failure naturally throws FormatException / OverflowException (Constitution §7)
                int intNodeId = int.Parse(selectedNodeId);

                await _gameContext.ResetRunForReplayAsync(intNodeId);
            }
        }
    }

**Design rationale (Plan §5-4):**

- Direct GameContext dependency: the core responsibility of this class is calling `ResetRunForReplayAsync`. This does not violate Constitution §3 "no `new` inside Logic classes + no Singleton.Instance access" — holding GameContext via constructor injection is the formal pattern.
- RQ-16 consistency guarantee is already provided by GameContext (Plan §10-1). This class only delegates the call without an additional mechanism.
- Uses `int.Parse` instead of `int.TryParse` — Fail Fast principle (Constitution §7). Natural throw on conversion failure.
- `pendingContext` unused for future extension preparation (Plan §8-2).

**RQ satisfied:** RQ-07, RQ-16 (indirectly — leverages GameContext guarantee).

**Constitution:** §3 (constructor injection), §7 (Fail Fast), §8 (`_logClass`).

---

### Task 6: Create IPostRestartSceneRouter.cs

**Path:** `Assets/_Game/Features/ReplayScene/Domain/IPostRestartSceneRouter.cs`

**Background:** Extension point interface for determining the post-restart scene transition target (Plan §5-5, RQ-08, Specify §6).

**Work:**

- Pure C# interface.
- namespace: `Samsara.Features.ReplayScene.Domain`
- 1 synchronous method:
    - `SceneKey ResolveNextSceneKey(string selectedNodeId, PendingReplayContext pendingContext)`

**Example code:**

    using Samsara.Core.Navigation;  // confirm actual SceneKey namespace and apply consistently

    namespace Samsara.Features.ReplayScene.Domain
    {
        public interface IPostRestartSceneRouter
        {
            SceneKey ResolveNextSceneKey(string selectedNodeId, PendingReplayContext pendingContext);
        }
    }

**Design rationale (Plan §5-5):**

- Synchronous method — only responsible for "deciding" the scene transition target. Actual transition is performed by ReplayUseCase via ISceneNavigator.
- The `selectedNodeId` / `pendingContext` parameters are unused in the 1st-development. Used for future branching extensions.

**RQ satisfied:** RQ-08.

---

### Task 7: Create DefaultPostRestartSceneRouter.cs

**Path:** `Assets/_Game/Features/ReplayScene/Domain/DefaultPostRestartSceneRouter.cs`

**Background:** 1st-development implementation of IPostRestartSceneRouter. Returns SceneKey.Main fixed (Plan §5-5).

**Work:**

- Pure C# class.
- namespace: `Samsara.Features.ReplayScene.Domain`
- `_logClass = $"[{nameof(DefaultPostRestartSceneRouter)}]";` field.
- Ctor dependency: none.
- Body operation:
    1. Ignore `selectedNodeId` / `pendingContext`.
    2. Return `SceneKey.Main` fixed.

**Example code:**

    using Samsara.Core.Navigation;

    namespace Samsara.Features.ReplayScene.Domain
    {
        public class DefaultPostRestartSceneRouter : IPostRestartSceneRouter
        {
            private readonly string _logClass = $"[{nameof(DefaultPostRestartSceneRouter)}]";

            public SceneKey ResolveNextSceneKey(string selectedNodeId, PendingReplayContext pendingContext)
            {
                return SceneKey.Main;
            }
        }
    }

**RQ satisfied:** RQ-08.

**Constitution:** §6 (SceneKey usage — no string), §8 (`_logClass`).

---

## 2-2. T8 (Domain Rename + rewrite)

### Task 8: ReplaySceneUseCase.cs → ReplayUseCase.cs (Rename + rewrite)

**Path:** `Assets/_Game/Features/ReplayScene/Domain/ReplayUseCase.cs` (after rename)

**Background:**

- Existing file: `ReplaySceneUseCase.cs`. Partial implementation exists from the v1.x.x stage.
- Majority naming convention — only Bootstrapper retains the "Scene" suffix; Presenter / UseCase / View have no "Scene" suffix (Plan §4-1; same pattern across 4 Features: EvolutionTreeScene / Ending / Event / CharacterInfoScene).
- The v1.x.x body does not match Plan v2.0.0, so fully rewrite.

**Work:**

**File handling:**

- Rename the existing `ReplaySceneUseCase.cs` to `ReplayUseCase.cs` (rename in Unity Editor Project view — GUID preserved).
- Change the class name from `ReplaySceneUseCase` to `ReplayUseCase`.
- Fully rewrite the body (see specification below).

**Class definition:**

- Pure C# class.
- namespace: `Samsara.Features.ReplayScene.Domain`
- Implements `IReplayUseCase`.
- `_logClass = $"[{nameof(ReplayUseCase)}]";` field.

**Ctor dependencies (Plan §5-3):**

| Type | Field name | Source |
|---|---|---|
| `ICharacterAccountRepository` | `_accountRepo` | GameContext.CharacterAccountRepo |
| `EvolutionNodeSO[]` | `_evolutionNodes` | GameContext.EvolutionNodes |
| `IRestartFlow` | `_restartFlow` | Instantiated in T15 Bootstrapper |
| `IPostRestartSceneRouter` | `_sceneRouter` | Instantiated in T15 Bootstrapper |
| `ISceneNavigator` | `_sceneNavigator` | GlobalBootstrapper.Instance.SceneNavigator (majority pattern) |
| `PendingReplayContext` | `_pendingContext` | GameContext.PendingReplayContext (read by Bootstrapper and injected) |

**Per-method body (Plan §5-3):**

**GetAllNodes():**

    public IReadOnlyList<EvolutionNodeSO> GetAllNodes()
    {
        return _evolutionNodes;
    }

Exposes the constructor-injected EvolutionNodeSO[] as IReadOnlyList<EvolutionNodeSO> directly.

**ClassifyNode(node):**

    public ReplayNodeState ClassifyNode(EvolutionNodeSO node)
    {
        if (node == null)
            throw new InvalidOperationException($"{_logClass} ClassifyNode: node is null");

        // List.Contains efficiency is sufficient for the 1st-development node count (~tens).
        // Consider HashSet caching for future node count growth (Plan §11 OQ-G4).
        if (_accountRepo.AccountData.UnlockedEvolutionNodeIds.Contains(node.NodeId))
            return ReplayNodeState.Selectable;

        if (node.IsHidden)
            return ReplayNodeState.Hidden;

        return ReplayNodeState.Locked;
    }

`UnlockedEvolutionNodeIds` is `List<string>`; `EvolutionNodeSO.NodeId` is also `string`. Direct comparison (Plan §5-0).

**ExecuteRestartAsync(selectedNodeId, ct):**

    public async UniTask ExecuteRestartAsync(string selectedNodeId, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(selectedNodeId))
            throw new InvalidOperationException($"{_logClass} ExecuteRestartAsync: selectedNodeId is null or empty");

        await _restartFlow.ExecuteAsync(selectedNodeId, _pendingContext, ct);

        var nextSceneKey = _sceneRouter.ResolveNextSceneKey(selectedNodeId, _pendingContext);

        await _sceneNavigator.NavigateToAsync(nextSceneKey, ct);
    }

Order: restart execution → next scene determination → scene transition. Reversing the order risks the next scene entering before the new RunData is persisted (Plan §5-3 design rationale).

**Required usings:**

    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using Samsara.Core.Navigation;
    using Samsara.Core.Tree;
    using Samsara.Features.Character.Data;        // confirm actual ICharacterAccountRepository namespace
    using Samsara.Features.Character.MasterData;  // confirm actual EvolutionNodeSO namespace

**Design rationale (Plan §5-3):**

- AccountData exposure avoidance — expose only EvolutionNodeSO[] via `GetAllNodes()`. AccountData not directly exposed (RQ-05 — read-only + minimal exposure).
- Restart execution → scene transition order: IPostRestartSceneRouter determines the next scene immediately after IRestartFlow completes RunData reset.
- Reason for holding `_pendingContext`: future expansions allow IRestartFlow / IPostRestartSceneRouter to add `PreviousRunResult`-based branching. In the 1st-development it is just passed through.
- No try-catch — Constitution §7 Fail Fast. Exceptions at any stage propagate to the caller (Plan §10-4).

**Differences from the existing v1.x.x file body:**

- The v1.x.x body's structure of directly calling GameContext.ResetRunForReplayAsync → reorganized to delegate via IRestartFlow.
- The v1.x.x body's structure of directly calling ISceneNavigator.NavigateToAsync(SceneKey.Main) → reorganized to first determine via IPostRestartSceneRouter, then call NavigateToAsync.
- The node classification logic that used a `Selectable → Evolvable` workaround → use the new ReplayNodeState directly.

**Global search impact (§1-5 results applied):**

- If any external code references the `ReplaySceneUseCase` class name (e.g., a call site in GameContext.cs), change all to `ReplayUseCase`.
- If GameContext.cs at the v1.x.x stage instantiates `ReplaySceneUseCase`, transfer the instantiation responsibility to T15 Bootstrapper in these Tasks. Remove the corresponding code in GameContext (T16 handling area — specified in T16).

**RQ satisfied:** RQ-05, RQ-09, RQ-13, RQ-16 (indirect).

**Constitution:** §3 (Logic class — constructor injection), §4 (Domain Layer pure C#), §7 (Fail Fast), §8 (`_logClass`, `_camelCase`, no LINQ — `Contains` is a List method, not LINQ).

---

## 2-3. T9 ~ T15 (Presentation Layer)

### (1) Standard pattern decision — commonly applied to all Presentation Tasks in these Tasks

All MonoBehaviour components in these Tasks (T10 OptionButtonView, T11 ReplayGuidanceView, T12 ReplayNodeDescriptionPopupView, T12-A ReplaySkillDescriptionPopupView, T12-X1 ReplaySkillSlotView, T12-X2 ReplaySkillListView, T13 ReplayView, T15 ReplaySceneBootstrapper) consistently apply the following standard pattern. Each Task does not restate this section but references it.

#### 1-1. Lifecycle pattern — Awake/OnDestroy + RemoveListener

| Item | Decision |
|---|---|
| Event registration timing | `Awake()` |
| Event unregistration timing | `OnDestroy()` |
| Listener removal method | `RemoveListener(HandleClick)` (precisely removes the self-registered handler) |
| RemoveAllListeners exception allowed | **Only when a lambda capture is used** (e.g., Skill slot index capture — lambda cannot be identified by RemoveListener) |
| Safe Cleanup | In OnDestroy: `_button?.onClick.RemoveListener(HandleClick)` (Constitution §8 — `?.` null-conditional) |

**Rationale:**

- The Unity official ScriptReference Button.onClick example registers in Start() (verified). These Tasks use Awake() — the Awake/OnDestroy pair sits precisely at the two ends of the GameObject creation/destruction lifecycle, and the ReplayScene component toggle frequency is 0 (active once on scene entry / destroyed on scene exit), making it more suitable for this context than the OnEnable/OnDisable pair.
- Button.onClick is not invoked on inactive GameObjects, so the "zombie scenario" does not apply to this context.
- Aligned with the project majority (13 ButtonViews use Awake/OnDestroy + RemoveAllListeners — Main / Maintenance / CharacterInfo / Ending / Battle). Consciously separated from the EvolutionTreeScene-side OnEnable/OnDisable + RemoveListener minority pattern.
- RemoveListener(HandleClick) precisely removes only the self-registered handler — preserving Inspector registrations / other code's dynamic registrations (an aspect superior to the majority's RemoveAllListeners).

**Project follow-up work (handled after Tasks Notion upload):**

Majority 13 ButtonViews use RemoveAllListeners → bulk transition to RemoveListener. Project Backlog **GBL-003 new registration** scheduled.

#### 1-2. namespace convention

| Area | namespace |
|---|---|
| Feature area | `Samsara.Features.{FeatureName}.{Layer}[.{SubFolder}]` |
| Core area | `Samsara.Core.{Module}` |

Applied in these Tasks:

- `Samsara.Features.ReplayScene.Domain` (T2 ~ T8)
- `Samsara.Features.ReplayScene.Presentation` (T10, T11, T13, T14, T15)
- `Samsara.Features.ReplayScene.Presentation.Popup` (T9, T12, T12-A, T12-B, T12-X1, T12-X2)
- `Samsara.Core.Tree` (T1)

#### 1-3. SerializeField / Reset() / Fail Fast / Log Tag — common rules

Common to all Presentation Tasks (Constitution §7, §8 — not restated per Task):

- All dependent components are declared as `[SerializeField] private` fields. **No null-guard** (Fail Fast). NullReferenceException fires immediately on missing reference.
- MonoBehaviour View classes implement the `Reset()` method (Constitution §7 Reset() Auto-Assignment). Use `GetComponentInChildren<T>()` / `GetComponentsInChildren<T>()`. **Transform / RectTransform are excluded from auto-assignment**.
- Every class holds a `private readonly string _logClass = $"[{nameof(ClassName)}]";` field (Constitution §8 Log Tag).
- In OnDestroy, use `?.` null-conditional (Constitution §8 Safe Cleanup — prevents Exception Masking).
- No LINQ / `new` allocation (Constitution §8 GC Optimization). No Update() hot path in these Tasks.
- All private fields are `_camelCase` (Constitution §8 Naming).

#### 1-4. UI text inline literals

§1-7 applied. Every Korean UI text hardcoding line must include the following comment immediately above:

    // UI text hardcoded: tracked in Project Backlog GBL-001. Subject to Phase 7 global cleanup.

---

### (2) T9 ~ T15 body

#### Task 10: Create OptionButtonView.cs

**Path:** `Assets/_Game/Features/ReplayScene/Presentation/OptionButtonView.cs`

**Background:** Top-right option button. G-05 global decision — the option button is included individually in each scene's View. ReplayScene has no back button per RQ-12 (Plan §4-1 / §5-11).

**Work:**

- Inherits MonoBehaviour.
- namespace: `Samsara.Features.ReplayScene.Presentation`
- Apply standard pattern §2-3 (1): Awake/OnDestroy + RemoveListener, `_logClass`, Fail Fast, `[SerializeField] private`.

**SerializeField:**

| Type | Name | Description |
|---|---|---|
| `Button` | `_button` | Unity Button component |

**Public event:**

    public event Action OnTapped;

**Method body:**

    private void Awake()
    {
        _button.onClick.AddListener(HandleClick);
    }

    private void OnDestroy()
    {
        _button?.onClick.RemoveListener(HandleClick);
    }

    private void HandleClick()
    {
        OnTapped?.Invoke();
    }

    private void Reset()
    {
        _button = GetComponentInChildren<Button>();
    }

**Required usings:**

    using System;
    using UnityEngine;
    using UnityEngine.UI;

**Design rationale:**

- Simple button + event emission. Displaying the option menu itself is handled by ReplayPresenter (Plan §5-7 OnOptionButtonTapped — 1st-development empty method, OQ-G8).
- Uses the named method HandleClick — no lambda, so RemoveListener(HandleClick) precisely removes.

**RQ satisfied:** G-05.

**Constitution:** §3 (View — area where direct UI manipulation is allowed), §7 (Reset), §8 (`_logClass`, Safe Cleanup).

---

#### Task 11: Create ReplayGuidanceView.cs

**Path:** `Assets/_Game/Features/ReplayScene/Presentation/ReplayGuidanceView.cs`

**Background:** Top-fixed guidance UI that cannot be closed. Specify §3-1 (Plan §5-10).

**Work:**

- Inherits MonoBehaviour.
- namespace: `Samsara.Features.ReplayScene.Presentation`
- Apply standard pattern §2-3 (1).

**SerializeField:**

| Type | Name | Description |
|---|---|---|
| `TMP_Text` | `_guidanceText` | Display the guidance text |

**Public method:**

    public void SetGuidanceText(string text)
    {
        _guidanceText.text = text;
    }

**Reset():**

    private void Reset()
    {
        _guidanceText = GetComponentInChildren<TMP_Text>();
    }

**Required usings:**

    using TMPro;
    using UnityEngine;

**Design rationale (Plan §5-10):**

- Specify §3-1 — same visual form as a popup but with no close action. Simple text container.
- The guidance text itself is undecided in OQ-01 (Specify §10). In the 1st-development, ReplayPresenter passes a temporary Korean inline literal via SetGuidanceText. Accumulated in GBL-001.
- No lifecycle events held — Awake/OnDestroy unnecessary. The standard pattern applies only to components with event registration.

**RQ satisfied:** Specify §3-1.

**Constitution:** §3 (View), §7 (Reset, Fail Fast), §8 (`_logClass`).

---

#### Task 12-X1: Create ReplaySkillSlotView.cs (single skill slot)

**Path:** `Assets/_Game/Features/ReplayScene/Presentation/Popup/ReplaySkillSlotView.cs`

**Background:** A single slot that displays one skill inside ReplayNodeDescriptionPopupView. Emits the skill index on click. Borrows the CharacterInfoScene SkillSlotView pattern.

**Cross-feature direct reference forbidden:**

CharacterInfoScene's SkillSlotView is an internal component of the CharacterInfoScene Feature. These Tasks create a ReplayScene-specific new component. Avoid direct dependencies between Features (Constitution §4 — Cross-Feature only via Core/ interfaces).

**Work:**

- Inherits MonoBehaviour.
- namespace: `Samsara.Features.ReplayScene.Presentation.Popup`
- Apply standard pattern §2-3 (1).

**SerializeField:**

| Type | Name | Description |
|---|---|---|
| `Button` | `_button` | Button for slot click |
| `Image` | `_skillIcon` | Image to display the skill icon |

**Public property:**

    public int SkillIndex { get; private set; }

**Public event:**

    public event Action<int> OnSkillSlotClicked;

**Public method:**

    public void Setup(int index, Sprite icon)
    {
        SkillIndex = index;
        _skillIcon.sprite = icon;
    }

**Lifecycle (standard pattern — lambda capture exception allowed):**

This component requires index capture, so a lambda is used. Lambda cannot be identified → use RemoveAllListeners (§2-3 (1) standard pattern lambda exception).

    private void Awake()
    {
        _button.onClick.AddListener(() => OnSkillSlotClicked?.Invoke(SkillIndex));
    }

    private void OnDestroy()
    {
        _button?.onClick.RemoveAllListeners();
    }

**Reset():**

    private void Reset()
    {
        _button = GetComponentInChildren<Button>();
        _skillIcon = GetComponentInChildren<Image>();
    }

**Required usings:**

    using System;
    using UnityEngine;
    using UnityEngine.UI;

**Design rationale:**

- Borrows CharacterInfoScene SkillSlotView — `_button` / `_skillIcon` SerializeField + `SkillIndex` private setter property + index capture via lambda + Awake registration / OnDestroy RemoveAllListeners.
- ReplayScene-specific new component (Cross-feature direct reference forbidden).
- Naming: `ReplaySkillSlotView`.
- The `_effectIndicator` (GameObject) field is not used in ReplayScene — the skill display in these Tasks is icon-only with no effect indicator (Q2 decision — effects excluded).
- Lambda usage is the lambda capture exception (§2-3 (1) standard pattern) — RemoveAllListeners allowed.

**Constitution:** §3 (View), §7 (Reset, Fail Fast), §8 (`_logClass`, Safe Cleanup).

---

#### Task 12-X2: Create ReplaySkillListView.cs (slot container)

**Path:** `Assets/_Game/Features/ReplayScene/Presentation/Popup/ReplaySkillListView.cs`

**Background:** Slot container holding an array of ReplaySkillSlotView. Relays child slots' OnSkillSlotClicked events to its own OnSkillSlotClicked. Borrows the CharacterInfoScene SkillListView pattern.

**SerializeField unification effect:**

Unifies T12 ReplayNodeDescriptionPopupView's 3 SerializeField candidates `_skillIconContainer` / `_skillIcons` (Image[]) / `_skillSlotButtons` into this single container. T12 holds only a single SerializeField `_skillListView`.

**Work:**

- Inherits MonoBehaviour.
- namespace: `Samsara.Features.ReplayScene.Presentation.Popup`
- Apply standard pattern §2-3 (1).

**Top-level struct in the same file (Plan §5-9 same pattern):**

    namespace Samsara.Features.ReplayScene.Presentation.Popup
    {
        public struct ReplaySkillDisplayData
        {
            public Sprite Icon;
        }
    }

The `HasEffect` field is not used in ReplayScene (Q2 — effects excluded). A separate struct from CharacterInfoScene SkillDisplayData.

**SerializeField:**

| Type | Name | Description |
|---|---|---|
| `ReplaySkillSlotView[]` | `_skillSlots` | Pre-placed slot array (placed directly in the scene during Manual Work + Inspector assigned) |

**Public event:**

    public event Action<int> OnSkillSlotClicked;

**Public method:**

    public void SetSkills(ReplaySkillDisplayData[] skills)
    {
        for (int i = 0; i < _skillSlots.Length; i++)
        {
            if (i < skills.Length)
            {
                _skillSlots[i].Setup(i, skills[i].Icon);
                _skillSlots[i].gameObject.SetActive(true);
            }
            else
            {
                _skillSlots[i].gameObject.SetActive(false);
            }
        }
    }

**Lifecycle (standard pattern — RemoveListener precise removal):**

Relays each slot's OnSkillSlotClicked event to its own OnSkillSlotClicked. Uses a named method → RemoveListener possible.

    private void Awake()
    {
        for (int i = 0; i < _skillSlots.Length; i++)
        {
            _skillSlots[i].OnSkillSlotClicked += HandleSkillSlotClicked;
        }
    }

    private void OnDestroy()
    {
        if (_skillSlots == null) return;
        for (int i = 0; i < _skillSlots.Length; i++)
        {
            if (_skillSlots[i] != null)
                _skillSlots[i].OnSkillSlotClicked -= HandleSkillSlotClicked;
        }
    }

    private void HandleSkillSlotClicked(int index)
    {
        OnSkillSlotClicked?.Invoke(index);
    }

**Reset():**

    private void Reset()
    {
        _skillSlots = GetComponentsInChildren<ReplaySkillSlotView>();
    }

**Required usings:**

    using System;
    using UnityEngine;

**Design rationale:**

- Borrows CharacterInfoScene SkillListView — `_skillSlots` array SerializeField + Awake subscribes to child slot events + relays via own OnSkillSlotClicked + Reset uses GetComponentsInChildren.
- Naming: `ReplaySkillListView`.
- The struct `ReplaySkillDisplayData` is top-level in this file (same pattern as CharacterInfoScene SkillDisplayData). No `[Serializable]`, public field, struct (value type).
- The `SetSkills` body iterates as far as the slot array length, activating + Setup-ing slots within the index range and deactivating slots outside the range (handles a variable 0~3 skill count).
- Uses `for` loops — Constitution §8 no LINQ.
- T12 holds only a single instance of this container as SerializeField. The effect of consolidating 3 SerializeFields into 1.

**Constitution:** §3 (View — container), §7 (Reset, Fail Fast), §8 (`_logClass`, Safe Cleanup, no LINQ).

---

#### Task 12: NodeDescriptionPopupView.cs → ReplayNodeDescriptionPopupView.cs (Rename + rewrite) + Task 9 (top-level struct in the file)

**Path:** `Assets/_Game/Features/ReplayScene/Presentation/Popup/ReplayNodeDescriptionPopupView.cs` (after rename)

**Background:**

- Existing file: `Features/ReplayScene/Presentation/Popup/NodeDescriptionPopupView.cs` (v1.x.x). Has identifiability defects since it shares the same class name and file name as the EvolutionTreeScene-side same-name class.
- v2.0.0 — to satisfy RQ-15 separated operation explicitly, both class name and file name use the `Replay` prefix to strengthen identifiability (Plan §5-9, RQ-15).
- ReplayScene-specific node info popup. Placed directly in the scene + ReplayPresenter calls Show/Hide directly via ReplayView (no PopupManager). Same pattern as the EvolutionTreeScene-side NodeDescriptionPopupView (Plan §5-9 display method).

**Work:**

**File handling:**

- Rename the existing `NodeDescriptionPopupView.cs` to `ReplayNodeDescriptionPopupView.cs` (rename in Unity Editor Project view — GUID preserved).
- Change the class name from `NodeDescriptionPopupView` to `ReplayNodeDescriptionPopupView`.
- Fully rewrite the body (see specification below).

**Class definition:**

- Inherits MonoBehaviour.
- namespace: `Samsara.Features.ReplayScene.Presentation.Popup`
- Apply standard pattern §2-3 (1): Awake/OnDestroy + RemoveListener, `_logClass`, Fail Fast, `[SerializeField] private`, Reset().

#### Task 9 — `ReplayNodeDescriptionData` (top-level struct in the T12 file)

Borrows the EvolutionTreeScene-side same-name pattern (top-level struct in the same file, no `[Serializable]`, public field, struct value type).

The ReplayScene side of these Tasks reflects semantic differences in the data structure: `CanEvolve` → `CanRestart`, `OnEvolveClicked` → `OnRestartClicked` (Plan §5-9 design rationale).

**struct definition (top-level in the file):**

| Type | Name | Description |
|---|---|---|
| `Sprite` | `NodeIcon` | Node icon. Presenter loads via ISpriteLoader and passes |
| `string` | `CharacterName` | EvolutionNodeSO.CharacterName directly. On Hidden, the popup side displays as "???" |
| `string` | `StatsText` | Composed by Presenter from EvolutionNodeSO.BaseStats / MaxActionPoints. On Hidden, the popup side displays as "???" |
| `string` | `ConditionsText` | Composed by Presenter from EvolutionNodeSO.UnlockConditions. Always displayed (also for Hidden nodes) |
| `Sprite[]` | `SkillIcons` | Presenter iterates EvolutionNodeSO.SkillIds → SkillMasterData → loads Sprites and passes. Not displayed on Hidden / Locked |
| `bool` | `CanRestart` | true only when in Selectable state. Restart button visibility |
| `bool` | `IsHiddenLocked` | Same meaning as the EvolutionTreeScene-side same-name field. Triggers the CharacterName / StatsText / SkillIcons non-display branch |

**Example code (top-level struct in the file):**

    public struct ReplayNodeDescriptionData
    {
        public Sprite NodeIcon;
        public string CharacterName;
        public string StatsText;
        public string ConditionsText;
        public Sprite[] SkillIcons;
        public bool CanRestart;
        public bool IsHiddenLocked;
    }

#### T12 main — `ReplayNodeDescriptionPopupView`

**SerializeField — display / modal mechanism (Plan §5-9):**

| Type | Name | Description |
|---|---|---|
| `GameObject` | `_popupRoot` | Popup body GameObject. SetActive(true) on Show, SetActive(false) on Hide |
| `Image` | `_dimBackground` | Dim background Image. Child of _popupRoot. RectTransform covering the full screen. raycastTarget=true to block background input |
| `Button` | `_dimButton` | Button attached to the _dimBackground GameObject (outside-region tap → OnCloseClicked) |

**SerializeField — content:**

| Type | Name | Description |
|---|---|---|
| `Image` | `_nodeIconImage` | Display the node icon |
| `TMP_Text` | `_characterNameText` | Character name. "???" on IsHiddenLocked |
| `TMP_Text` | `_statsText` | Stats text (Presenter composition). "???" on IsHiddenLocked |
| `TMP_Text` | `_conditionsText` | Unlock conditions text (Presenter composition). Always displayed |
| `ReplaySkillListView` | `_skillListView` | Skill slot container (T12-X2). Not displayed on IsHiddenLocked / Locked |
| `Button` | `_restartButton` | Restart button. Active only when CanRestart=true |
| `Button` | `_closeButton` | X button |

**Part where 3 SerializeFields are unified into a single one:**

Among T12 candidate SerializeFields, `_skillIconContainer` (Transform) / `_skillIcons` (Image[]) / `_skillSlotButtons` (Button[]) 3 are unified into `_skillListView` (ReplaySkillListView) single SerializeField. Slot operations are handled by the ReplaySkillSlotView array owned by ReplaySkillListView itself.

**Public methods:**

    public void Show(ReplayNodeDescriptionData data)
    public void Hide()

**Public events:**

    public event Action OnRestartClicked;
    public event Action OnCloseClicked;
    public event Action<int> OnSkillSlotClicked;

`OnSkillSlotClicked` is relayed by this class from ReplaySkillListView's same-name event. ReplayPresenter subscribes to this event → invokes the skill description popup (T12-A).

**Method body:**

**Show(data):**

    public void Show(ReplayNodeDescriptionData data)
    {
        _popupRoot.SetActive(true);
        _nodeIconImage.sprite = data.NodeIcon;

        if (data.IsHiddenLocked)
        {
            // UI text hardcoded: tracked in Project Backlog GBL-001. Subject to Phase 7 global cleanup.
            _characterNameText.text = "???";
            // UI text hardcoded: tracked in Project Backlog GBL-001. Subject to Phase 7 global cleanup.
            _statsText.text = "???";
            _conditionsText.text = data.ConditionsText;
            _skillListView.gameObject.SetActive(false);
            _restartButton.gameObject.SetActive(false);
        }
        else
        {
            _characterNameText.text = data.CharacterName;
            _statsText.text = data.StatsText;
            _conditionsText.text = data.ConditionsText;

            if (data.SkillIcons != null && data.SkillIcons.Length > 0)
            {
                _skillListView.gameObject.SetActive(true);
                var skillData = new ReplaySkillDisplayData[data.SkillIcons.Length];
                for (int i = 0; i < data.SkillIcons.Length; i++)
                {
                    skillData[i] = new ReplaySkillDisplayData { Icon = data.SkillIcons[i] };
                }
                _skillListView.SetSkills(skillData);
            }
            else
            {
                _skillListView.gameObject.SetActive(false);
            }

            _restartButton.gameObject.SetActive(data.CanRestart);
        }
    }

**Hide():**

    public void Hide()
    {
        _popupRoot.SetActive(false);
    }

**Lifecycle (standard pattern §2-3 (1)):**

    private void Awake()
    {
        _restartButton.onClick.AddListener(HandleRestartClicked);
        _closeButton.onClick.AddListener(HandleCloseClicked);
        _dimButton.onClick.AddListener(HandleCloseClicked);
        _skillListView.OnSkillSlotClicked += HandleSkillSlotClicked;
        _popupRoot.SetActive(false);
    }

    private void OnDestroy()
    {
        _restartButton?.onClick.RemoveListener(HandleRestartClicked);
        _closeButton?.onClick.RemoveListener(HandleCloseClicked);
        _dimButton?.onClick.RemoveListener(HandleCloseClicked);
        if (_skillListView != null)
            _skillListView.OnSkillSlotClicked -= HandleSkillSlotClicked;
    }

    private void HandleRestartClicked()
    {
        OnRestartClicked?.Invoke();
    }

    private void HandleCloseClicked()
    {
        OnCloseClicked?.Invoke();
    }

    private void HandleSkillSlotClicked(int index)
    {
        OnSkillSlotClicked?.Invoke(index);
    }

**Popup close triggers (Plan §5-9):**

- X button tap → emit `OnCloseClicked` (HandleCloseClicked)
- _dimBackground Button outside-region tap → same handler → emit `OnCloseClicked`
- No automatic transition when tapping another node — _dimBackground's raycastTarget=true blocks tree input (Specify §4-2 naturally satisfied)

**Modal input blocking (Plan §5-9):**

- _dimBackground is a direct child of _popupRoot
- _dimBackground's raycastTarget=true intercepts all input from the tree area / option button / guidance UI
- Popup content is placed above _dimBackground in sibling order so user clicks remain possible
- Button component attached to _dimBackground GameObject → outside-region tap closes simultaneously

**Reset():**

    private void Reset()
    {
        _popupRoot = gameObject;
        _nodeIconImage = GetComponentInChildren<Image>();
        // Manually assign the rest of the SerializeFields (GetComponentsInChildren cannot disambiguate)
    }

ReplaySkillListView / multiple TMP_Text / multiple Button / multiple Image exist in the same GameObject tree → Reset() auto-assignment cannot identify them. Hak directly assigns in the Inspector during Manual Work.

**Required usings:**

    using System;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

**Design rationale (Plan §5-9 / RQ-15):**

- RQ-15 / Specify §4-4 — separate from the EvolutionTreeScene-side NodeDescriptionPopupView in class / file / namespace. Identifiability strengthened with the Replay prefix.
- Data structure also separated — `NodeDescriptionData` (EvolutionTreeScene) and `ReplayNodeDescriptionData` (ReplayScene) defined separately. Primary action difference (Evolve vs Restart) reflected in the field (`CanEvolve` vs `CanRestart`) and event (`OnEvolveClicked` vs `OnRestartClicked`).
- The View only displays after receiving data. Sprite loading / text composition is the Presenter's job (Plan §5-7).
- Modal pattern unified — the Dim + raycastTarget pattern that PopupManager applies to CommonPopupView is also applied to scene-specific popups identically. Specify §4-2 automatic transition blocking naturally satisfied. The same defect on the EvolutionTreeScene side will be fixed in a separate Patch (Plan §11 OQ-X2).
- _dimButton usage — outside tap close handled by attaching a Button component without a separate EventTrigger. Consistent with the event system.
- IsHiddenLocked branching — same as the EvolutionTreeScene-side same-name field branching pattern.

**RQ satisfied:** RQ-15, Specify §4-2 (automatic transition blocking naturally satisfied), §4-1 (per-state popup content).

**Constitution:** §3 (View — direct UI manipulation), §7 (Reset, Fail Fast), §8 (`_logClass`, Safe Cleanup, no LINQ — `for` loop).

---

#### Task 12-A: Create ReplaySkillDescriptionPopupView.cs + Task 12-B (top-level struct in the file)

**Path:** `Assets/_Game/Features/ReplayScene/Presentation/Popup/ReplaySkillDescriptionPopupView.cs`

**Background:** Skill description popup that appears when a skill slot inside the node info popup is clicked (Q1 ~ Q4). ReplayScene-specific new component (Q3 — Cross-feature direct reference forbidden).

**Work:**

- Inherits MonoBehaviour.
- namespace: `Samsara.Features.ReplayScene.Presentation.Popup`
- Apply standard pattern §2-3 (1).

#### Task 12-B — `ReplaySkillDescriptionData` (top-level struct in the T12-A file)

CharacterInfoScene-side SkillDescriptionPopupView uses a multi-parameter pattern Show(Sprite icon, string skillName, string description, float damage, string effectDescription). These Tasks unify with struct DTO consistent with the T9 pattern.

**struct definition (top-level in the file):**

| Type | Name | Description |
|---|---|---|
| `Sprite` | `Icon` | Skill icon. Presenter loads via ISpriteLoader and passes |
| `string` | `SkillName` | SkillSO.SkillName directly |
| `string` | `Description` | SkillSO.Description directly |
| `float` | `Damage` | SkillSO.Damage directly. Display formatting is this View's responsibility |

No effect (Q2 decision — effects excluded). No EffectDescription field.

**Example code (top-level struct in the file):**

    public struct ReplaySkillDescriptionData
    {
        public Sprite Icon;
        public string SkillName;
        public string Description;
        public float Damage;
    }

#### T12-A main — `ReplaySkillDescriptionPopupView`

**SerializeField — display / modal mechanism:**

| Type | Name | Description |
|---|---|---|
| `GameObject` | `_popupRoot` | Popup body GameObject |
| `Image` | `_dimBackground` | Dim background Image. Child of _popupRoot. raycastTarget=true |
| `Button` | `_dimButton` | Button attached to _dimBackground (outside-region tap → OnCloseClicked) |

**Z-order handling (Q4):**

This popup is displayed above ReplayNodeDescriptionPopupView (skill description popup Dim is on top). During Manual Work, Hak guarantees Z-order via Canvas Hierarchy or sortingOrder. This View is not responsible for Z-order — scene placement responsibility.

**SerializeField — content:**

| Type | Name | Description |
|---|---|---|
| `Image` | `_skillIconImage` | Display the skill icon |
| `TMP_Text` | `_skillNameText` | Skill name |
| `TMP_Text` | `_skillDescriptionText` | Skill description |
| `TMP_Text` | `_damageText` | Damage text (formatted by View) |
| `Button` | `_closeButton` | X button |

**Public methods:**

    public void Show(ReplaySkillDescriptionData data)
    public void Hide()

**Public event:**

    public event Action OnCloseClicked;

**Method body:**

**Show(data):**

    public void Show(ReplaySkillDescriptionData data)
    {
        _popupRoot.SetActive(true);
        _skillIconImage.sprite = data.Icon;
        _skillNameText.text = data.SkillName;
        _skillDescriptionText.text = data.Description;
        // UI text hardcoded: tracked in Project Backlog GBL-001. Subject to Phase 7 global cleanup.
        _damageText.text = $"데미지: {data.Damage:F1}";
    }

The damage format `데미지: {damage:F1}` is the same Korean format as CharacterInfoScene SkillDescriptionPopupView (Q6 decision — same expression adopted as-is, accumulated in GBL-001).

**Hide():**

    public void Hide()
    {
        _popupRoot.SetActive(false);
    }

**Lifecycle (standard pattern §2-3 (1)):**

    private void Awake()
    {
        _closeButton.onClick.AddListener(HandleCloseClicked);
        _dimButton.onClick.AddListener(HandleCloseClicked);
        _popupRoot.SetActive(false);
    }

    private void OnDestroy()
    {
        _closeButton?.onClick.RemoveListener(HandleCloseClicked);
        _dimButton?.onClick.RemoveListener(HandleCloseClicked);
    }

    private void HandleCloseClicked()
    {
        OnCloseClicked?.Invoke();
    }

**Popup close triggers (Q4 outside tap closes):**

- X button tap → emit `OnCloseClicked`
- _dimBackground outside-region tap → same handler → emit `OnCloseClicked`

**Reset():**

    private void Reset()
    {
        _popupRoot = gameObject;
        // Manually assign the rest of the SerializeFields
    }

**Required usings:**

    using System;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

**Design rationale (Q1 ~ Q4 / Q7):**

- Q1 — click only (no long-press). The slot component (T12-X1) uses only one Button.onClick — standard pattern. This View itself has no input handling.
- Q2 — icon / name / description / damage only (effects excluded). `ReplaySkillDescriptionData` has no EffectDescription / `_effectText` field.
- Q3 — ReplayScene-specific new class. CharacterInfoScene-side SkillDescriptionPopupView direct reference forbidden (Cross-feature). Same folder pattern (`Presentation/Popup/`).
- Q4 modal mechanism — holds its own Dim. Placed above the node info popup Dim by Z-order (Manual Work responsibility). Closed by outside tap.
- Q7 SkillSO field usage — `SkillName` / `Description` / `Damage` / `IconSpriteKey`. `Effects` not used. `IconSpriteKey` is loaded by Presenter via ISpriteLoader, converted to Sprite, and passed to this View (Plan §5-7).

**RQ satisfied:** Specify §4-1 (Selectable state popup's owned skill display), Q1 ~ Q5 decisions.

**Constitution:** §3 (View), §7 (Reset, Fail Fast), §8 (`_logClass`, Safe Cleanup).

---

#### Task 13: ReplaySceneView.cs → ReplayView.cs (Rename + rewrite)

**Path:** `Assets/_Game/Features/ReplayScene/Presentation/ReplayView.cs` (after rename)

**Background:**

- Existing file: `ReplaySceneView.cs` (v1.x.x).
- Majority naming convention — only Bootstrapper retains the "Scene" suffix; View has no "Scene" suffix (Plan §4-1).
- Composition of tree area / top guidance area / top-right option button / node info popup + event emission (Plan §5-8).

**Work:**

**File handling:**

- Rename the existing `ReplaySceneView.cs` to `ReplayView.cs` (rename in Unity Editor Project view — GUID preserved).
- Change the class name from `ReplaySceneView` to `ReplayView`.
- Fully rewrite the body.

**Class definition:**

- Inherits MonoBehaviour.
- namespace: `Samsara.Features.ReplayScene.Presentation`
- Apply standard pattern §2-3 (1): Awake/OnDestroy + RemoveListener, `_logClass`, Fail Fast, `[SerializeField] private`, Reset().

**SerializeField (Plan §5-8):**

| Type | Name | Description |
|---|---|---|
| `TreeScrollView` | `_treeScrollView` | Core/Tree shared tree container |
| `ReplayGuidanceView` | `_guidanceView` | Top-fixed guidance UI (T11) |
| `OptionButtonView` | `_optionButton` | Top-right option button (T10) |
| `ReplayNodeDescriptionPopupView` | `_nodeDescriptionPopupView` | Node info popup placed directly in the scene (T12). Does not go through PopupManager |
| `ReplaySkillDescriptionPopupView` | `_skillDescriptionPopupView` | Skill description popup placed directly in the scene (T12-A). Does not go through PopupManager |

**Public properties (for Presenter access):**

| Type | Name | Description |
|---|---|---|
| `TreeScrollView` | `TreeScrollView` | For Presenter access to tree node build commands |
| `ReplayGuidanceView` | `GuidanceView` | For Presenter access to guidance UI text setting |
| `ReplayNodeDescriptionPopupView` | `NodeDescriptionPopupView` | For Presenter to call popup Show/Hide directly |
| `ReplaySkillDescriptionPopupView` | `SkillDescriptionPopupView` | For Presenter to call skill description popup Show/Hide directly |

**Public events:**

    public event Action<EvolutionNodeSO> OnNodeTapped;
    public event Action OnOptionButtonTapped;

**Lifecycle (standard pattern §2-3 (1)):**

ReplayView itself relays child component events as its own events. Subscribes to child component emission events (C# event) via named methods → RemoveListener precise removal.

    private void Awake()
    {
        _optionButton.OnTapped += HandleOptionButtonTapped;
        _treeScrollView.OnNodeTapped += HandleTreeNodeTapped;
    }

    private void OnDestroy()
    {
        if (_optionButton != null)
            _optionButton.OnTapped -= HandleOptionButtonTapped;
        if (_treeScrollView != null)
            _treeScrollView.OnNodeTapped -= HandleTreeNodeTapped;
    }

    private void HandleOptionButtonTapped()
    {
        OnOptionButtonTapped?.Invoke();
    }

    private void HandleTreeNodeTapped(EvolutionNodeSO node)
    {
        OnNodeTapped?.Invoke(node);
    }

**TreeScrollView.OnNodeTapped signature assumption:**

- Per Plan §9 NodeView signature change, TreeScrollView is assumed to expose `event Action<EvolutionNodeSO> OnNodeTapped` or an equivalent signature (Plan §9 NodeView signature change accompanies — TreeScrollView relays NodeView's click events as node-entity-unit events).
- On signature mismatch, Claude Code first reads the EvolutionTreeScene Patch application result code to confirm the precise signature, then applies consistently. If the signature is `event Action<string nodeId>` form, ReplayView matches nodeId in the `_evolutionNodes` array, converts to EvolutionNodeSO, and emits its own event — it is more appropriate to place this conversion responsibility in ReplayPresenter. ReplayView only emits what it received as-is.
- These Tasks assume TreeScrollView exposes EvolutionNodeSO-unit events; if the EvolutionTreeScene Patch result signature differs, record `[SPEC-GAP]` in decisions.md and confirm with Hak.

**Reset() Auto-Assignment (Constitution §7):**

    private void Reset()
    {
        _treeScrollView = GetComponentInChildren<TreeScrollView>();
        _guidanceView = GetComponentInChildren<ReplayGuidanceView>();
        _optionButton = GetComponentInChildren<OptionButtonView>();
        _nodeDescriptionPopupView = GetComponentInChildren<ReplayNodeDescriptionPopupView>();
        _skillDescriptionPopupView = GetComponentInChildren<ReplaySkillDescriptionPopupView>();
    }

**Required usings:**

    using System;
    using Samsara.Core.Tree;
    using Samsara.Features.Character.MasterData;  // confirm actual EvolutionNodeSO namespace and apply consistently
    using Samsara.Features.ReplayScene.Presentation.Popup;
    using UnityEngine;

**Design rationale (Plan §5-8):**

- View is a composition container + event relay role. Holds child components (TreeScrollView, ReplayGuidanceView, OptionButtonView, two popups) as SerializeField + exposes externally.
- No back button (RQ-12). No component like EvolutionTreeScene's BackButtonView.
- ReplayNodeDescriptionPopupView and ReplaySkillDescriptionPopupView are held by ReplayView as SerializeField. Placed directly in the scene + Presenter calls Show/Hide directly (no PopupManager). This pattern is the same as the EvolutionTreeScene-side NodeDescriptionPopupView (Plan §5-9). Only simple message popups (restart confirmation, etc.) go through PopupManager.
- Self-methods minimal — composition container role.
- View emits OnNodeTapped event in EvolutionNodeSO units → Presenter receives as-is and calls ClassifyNode. EvolutionNodeSO-unit events separate the nodeId string conversion / search responsibility from the View.

**RQ satisfied:** RQ-12, Constitution §3 (View — area where direct UI manipulation is allowed), §7.

---

#### Task 14: ReplayScenePresenter.cs → ReplayPresenter.cs (Rename + rewrite)

**Path:** `Assets/_Game/Features/ReplayScene/Presentation/ReplayPresenter.cs` (after rename)

**Background:**

- Existing file: `ReplayScenePresenter.cs` (v1.x.x).
- Majority naming convention — Presenter has no "Scene" suffix (Plan §4-1).
- View ↔ UseCase relay. No direct UI manipulation (Constitution §3).

**Work:**

**File handling:**

- Rename the existing `ReplayScenePresenter.cs` to `ReplayPresenter.cs` (rename in Unity Editor Project view — GUID preserved).
- Change the class name from `ReplayScenePresenter` to `ReplayPresenter`.
- Fully rewrite the body.

**Class definition:**

- Pure C# class (not Unity MonoBehaviour, Constitution §4).
- namespace: `Samsara.Features.ReplayScene.Presentation`
- `_logClass = $"[{nameof(ReplayPresenter)}]";` field.
- Implements IDisposable — Bootstrapper.OnDestroy calls Dispose.

**Ctor dependencies (Plan §5-7):**

| Type | Field name | Source |
|---|---|---|
| `IReplayUseCase` | `_useCase` | Instantiated in Bootstrapper |
| `ReplayView` | `_view` | Bootstrapper SerializeField |
| `IPopupManager` | `_popupManager` | GameContext.PopupManager |
| `ISpriteLoader` | `_spriteLoader` | GameContext.SpriteLoader |
| `ISkillMasterDataRepository` | `_skillMasterDataRepo` | GameContext.SkillMasterDataRepo |
| `StateVisualConfig` | `_stateVisualConfig` | Bootstrapper SerializeField |

**Nested type — StateVisualConfig (Plan §5-7 / §9-5):**

Serializable data container. Bootstrapper receives via SerializeField and injects into Presenter Ctor.

| Type | Name | Description |
|---|---|---|
| `string` | `SelectableFrameSpriteKey` | Frame Sprite Key for Selectable nodes |
| `string` | `LockedFrameSpriteKey` | Frame Sprite Key for Locked nodes |
| `string` | `HiddenFrameSpriteKey` | Frame Sprite Key for Hidden nodes |
| `string` | `QuestionMarkSpriteKey` | Sprite Key for Hidden node Icon override |

**Example code (separate file or inside Presenter):**

    [Serializable]
    public class StateVisualConfig
    {
        public string SelectableFrameSpriteKey;
        public string LockedFrameSpriteKey;
        public string HiddenFrameSpriteKey;
        public string QuestionMarkSpriteKey;
    }

**Internal state fields:**

    private EvolutionNodeSO _currentNodeForPopup;  // node currently displayed by the node info popup. Used to look up SkillIds when a skill slot is clicked
    private Sprite[] _currentSkillIcons;           // skill icon array currently displayed by the node info popup. Used for skill index → Sprite mapping

**Public methods:**

    public void Initialize()
    public void Dispose()

#### Initialize() — tree build + View event subscription

    public void Initialize()
    {
        _view.OnNodeTapped += HandleNodeTapped;
        _view.OnOptionButtonTapped += HandleOptionButtonTapped;
        _view.NodeDescriptionPopupView.OnRestartClicked += HandleRestartClicked;
        _view.NodeDescriptionPopupView.OnCloseClicked += HandleNodeDescriptionPopupCloseClicked;
        _view.NodeDescriptionPopupView.OnSkillSlotClicked += HandleSkillSlotClickedAsync;
        _view.SkillDescriptionPopupView.OnCloseClicked += HandleSkillDescriptionPopupCloseClicked;

        // UI text hardcoded: tracked in Project Backlog GBL-001. Subject to Phase 7 global cleanup.
        _view.GuidanceView.SetGuidanceText("재시작할 진화체를 선택하세요.");

        BuildTreeAsync().Forget();
    }

#### BuildTreeAsync() — tree build flow (Plan §7-1, §9-5)

    private async UniTask BuildTreeAsync()
    {
        // 1. Async-load 4 StateVisualConfig SpriteKeys
        var selectableFrame = await _spriteLoader.LoadSpriteAsync(_stateVisualConfig.SelectableFrameSpriteKey);
        var lockedFrame     = await _spriteLoader.LoadSpriteAsync(_stateVisualConfig.LockedFrameSpriteKey);
        var hiddenFrame     = await _spriteLoader.LoadSpriteAsync(_stateVisualConfig.HiddenFrameSpriteKey);
        var questionMark    = await _spriteLoader.LoadSpriteAsync(_stateVisualConfig.QuestionMarkSpriteKey);

        // 2. Get all nodes
        var nodes = _useCase.GetAllNodes();

        // 3. Tree build is performed by TreeScrollView. Per-node state classification → Frame/Icon Sprite mapping result is injected into NodeView.
        //    Actual NodeView instantiation / SetFrameSprite / SetIconSprite calls follow the build method signature of TreeScrollView or the EvolutionTreeScene Patch application result.
        //    (TreeScrollView's exact build signature is checked from EvolutionTreeScene Patch result code and applied consistently)
        for (int i = 0; i < nodes.Count; i++)
        {
            var node = nodes[i];
            var state = _useCase.ClassifyNode(node);

            Sprite frameSprite;
            Sprite iconOverride = null;

            switch (state)
            {
                case ReplayNodeState.Selectable:
                    frameSprite = selectableFrame;
                    break;
                case ReplayNodeState.Locked:
                    frameSprite = lockedFrame;
                    break;
                case ReplayNodeState.Hidden:
                    frameSprite = hiddenFrame;
                    iconOverride = questionMark;
                    break;
                default:
                    throw new InvalidOperationException($"{_logClass} BuildTreeAsync: unexpected ReplayNodeState [{state}]");
            }

            // TreeScrollView build method call — inject frameSprite + (on Hidden) iconOverride into each NodeView
            // Apply the precise method name / signature consistent with the EvolutionTreeScene Patch result
        }
    }

**TreeScrollView build method assumption:**

- Per Plan §9-3 NodeView signature change, NodeView has signatures `SetFrameSprite(Sprite)` / `SetIconSprite(Sprite)` / `Setup(string nodeId, Sprite icon)`.
- The signature of TreeScrollView's method that takes a node collection and builds NodeView instances follows the EvolutionTreeScene Patch application result. After Patch application, Claude Code first reads the TreeScrollView code to confirm the precise signature and applies consistently in this BuildTreeAsync body. On signature mismatch or build flow difference, record `[SPEC-GAP]` in decisions.md and confirm with Hak.

#### HandleNodeTapped(node) — show info popup on node tap (Plan §7-2)

    private async void HandleNodeTapped(EvolutionNodeSO node)
    {
        if (node == null)
        {
            Debug.LogWarning($"{_logClass} HandleNodeTapped: node is null");
            return;
        }

        var state = _useCase.ClassifyNode(node);
        var data = new ReplayNodeDescriptionData
        {
            ConditionsText = ComposeConditionsText(node.UnlockConditions),
        };

        if (state == ReplayNodeState.Hidden)
        {
            data.IsHiddenLocked = true;
            data.CanRestart = false;
            // CharacterName / StatsText / SkillIcons / NodeIcon not set (popup side displays ??? + non-display)
        }
        else if (state == ReplayNodeState.Locked)
        {
            data.IsHiddenLocked = false;
            data.CanRestart = false;
            data.CharacterName = node.CharacterName;
            data.NodeIcon = await _spriteLoader.LoadSpriteAsync(node.NodeIconSpriteKey);
            // StatsText / SkillIcons not set (Locked state shows only character name + conditions)
        }
        else  // Selectable
        {
            data.IsHiddenLocked = false;
            data.CanRestart = true;
            data.CharacterName = node.CharacterName;
            data.NodeIcon = await _spriteLoader.LoadSpriteAsync(node.NodeIconSpriteKey);
            data.StatsText = ComposeStatsText(node.BaseStats);
            data.SkillIcons = await LoadSkillIconsAsync(node.SkillIds);
        }

        _currentNodeForPopup = node;
        _currentSkillIcons = data.SkillIcons;
        _view.NodeDescriptionPopupView.Show(data);
    }

**Selectable state popup content branching precision (Specify §4-1):**

- Selectable: icon / name / description / stats / owned skills / restart button
- Locked: unlock conditions only
- Hidden: minimal information at the ??? level

This branching applies the Specify §4-1 table as-is. Follows the Plan §5-7 OnNodeTapped step 2 specification.

#### LoadSkillIconsAsync — SkillIds → Sprite[] load (Plan §5-7)

    private async UniTask<Sprite[]> LoadSkillIconsAsync(int[] skillIds)
    {
        if (skillIds == null || skillIds.Length == 0)
            return Array.Empty<Sprite>();

        var skills = _skillMasterDataRepo.GetSkillsByIds(skillIds);
        var sprites = new Sprite[skills.Length];
        for (int i = 0; i < skills.Length; i++)
        {
            sprites[i] = await _spriteLoader.LoadSpriteAsync(skills[i].IconSpriteKey);
        }
        return sprites;
    }

`ISkillMasterDataRepository.GetSkillsByIds(int[])` is the same pattern as EvolutionTreePresenter.HandleNodeClickedAsync. Claude Code checks the precise method name from the ISkillMasterDataRepository code and applies consistently. Record `[SPEC-GAP]` on signature mismatch.

#### ComposeStatsText — Presenter helper (Plan §5-7)

    private string ComposeStatsText(CharacterStatsSO stats)
    {
        if (stats == null) return string.Empty;
        // UI text hardcoded: tracked in Project Backlog GBL-001. Subject to Phase 7 global cleanup.
        return $"체력 {stats.Hp}\n힘 {stats.Strength}\n강인함 {stats.Toughness}\n민첩 {stats.Agility}";
    }

The stats 4 + MaxActionPoints specification is per Plan §5-7. The precise format / whether to include MaxActionPoints will be finalized at the OQ-01 (guidance text / popup messages) bulk cleanup time. The 1st-development indicates 4 stats by default.

#### ComposeConditionsText — Presenter helper

    private string ComposeConditionsText(StatCondition[] conditions)
    {
        if (conditions == null || conditions.Length == 0)
            // UI text hardcoded: tracked in Project Backlog GBL-001. Subject to Phase 7 global cleanup.
            return "해금 조건 없음";

        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < conditions.Length; i++)
        {
            // UI text hardcoded: tracked in Project Backlog GBL-001. Subject to Phase 7 global cleanup.
            sb.AppendLine($"- {conditions[i]}");
        }
        return sb.ToString();
    }

The display conversion method of `StatCondition` (`ToString()` or a separate formatter) is checked by Claude Code from the StatCondition definition and applied consistently. If an EvolutionTreeScene same pattern exists, borrow it. These Tasks default to a `- {condition}` per-line form.

#### HandleSkillSlotClickedAsync(index) — show skill description popup on skill slot click

    private async void HandleSkillSlotClickedAsync(int index)
    {
        if (_currentNodeForPopup == null || _currentNodeForPopup.SkillIds == null)
            return;

        if (index < 0 || index >= _currentNodeForPopup.SkillIds.Length)
        {
            Debug.LogWarning($"{_logClass} HandleSkillSlotClickedAsync: index [{index}] out of range");
            return;
        }

        var skillId = _currentNodeForPopup.SkillIds[index];
        var skills = _skillMasterDataRepo.GetSkillsByIds(new[] { skillId });
        if (skills == null || skills.Length == 0)
        {
            Debug.LogWarning($"{_logClass} HandleSkillSlotClickedAsync: skill not found for id [{skillId}]");
            return;
        }

        var skill = skills[0];
        var iconSprite = (_currentSkillIcons != null && index < _currentSkillIcons.Length)
            ? _currentSkillIcons[index]
            : await _spriteLoader.LoadSpriteAsync(skill.IconSpriteKey);

        var data = new ReplaySkillDescriptionData
        {
            Icon = iconSprite,
            SkillName = skill.SkillName,
            Description = skill.Description,
            Damage = skill.Damage,
        };

        _view.SkillDescriptionPopupView.Show(data);
    }

**Z-order (Q4):** At this moment ReplayNodeDescriptionPopupView remains active. The skill description popup is additionally activated above it. Both popups hold their own Dim + the skill description popup is on top by Z-order. Z-order is guaranteed by Hak via Hierarchy or Canvas sortingOrder during Manual Work.

#### HandleNodeDescriptionPopupCloseClicked — close node info popup (Plan §7-3)

    private void HandleNodeDescriptionPopupCloseClicked()
    {
        _view.NodeDescriptionPopupView.Hide();
        _currentNodeForPopup = null;
        _currentSkillIcons = null;
    }

#### HandleSkillDescriptionPopupCloseClicked — close skill description popup

    private void HandleSkillDescriptionPopupCloseClicked()
    {
        _view.SkillDescriptionPopupView.Hide();
    }

#### HandleRestartClicked — restart confirmation flow (Plan §7-4)

    private async void HandleRestartClicked()
    {
        if (_currentNodeForPopup == null)
        {
            Debug.LogWarning($"{_logClass} HandleRestartClicked: _currentNodeForPopup is null");
            return;
        }

        var selectedNode = _currentNodeForPopup;

        // UI text hardcoded: tracked in Project Backlog GBL-001. Subject to Phase 7 global cleanup.
        var request = new PopupRequest(
            "재시작 확정",
            $"{selectedNode.CharacterName}(으)로 새 런을 시작합니다.",
            "재시작",
            "취소");

        bool confirmed = await _popupManager.ShowYesNoAsync(request);

        if (!confirmed)
        {
            // Cancel → node info popup remains open (user can re-select another node)
            return;
        }

        // Restart confirmed → UseCase handles RunData reset + scene transition (§7-4 steps 1~8)
        await _useCase.ExecuteRestartAsync(selectedNode.NodeId);
    }

#### HandleOptionButtonTapped — option menu (Plan §7-5)

    private void HandleOptionButtonTapped()
    {
        // Empty method in 1st-development — option menu spec not finalized (Plan §11 OQ-G8).
        // Body to be filled when option menu spec is finalized.
    }

#### Dispose() — Constitution §8 Safe Cleanup

    public void Dispose()
    {
        if (_view != null)
        {
            _view.OnNodeTapped -= HandleNodeTapped;
            _view.OnOptionButtonTapped -= HandleOptionButtonTapped;

            if (_view.NodeDescriptionPopupView != null)
            {
                _view.NodeDescriptionPopupView.OnRestartClicked -= HandleRestartClicked;
                _view.NodeDescriptionPopupView.OnCloseClicked -= HandleNodeDescriptionPopupCloseClicked;
                _view.NodeDescriptionPopupView.OnSkillSlotClicked -= HandleSkillSlotClickedAsync;
            }

            if (_view.SkillDescriptionPopupView != null)
            {
                _view.SkillDescriptionPopupView.OnCloseClicked -= HandleSkillDescriptionPopupCloseClicked;
            }
        }
    }

**Required usings:**

    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using Samsara.Core.Navigation;
    using Samsara.Core.Popup;
    using Samsara.Core.Sprites;
    using Samsara.Core.Tree;
    using Samsara.Features.Character.MasterData;       // confirm actual EvolutionNodeSO / CharacterStatsSO / StatCondition namespace
    using Samsara.Features.ReplayScene.Domain;
    using Samsara.Features.ReplayScene.Presentation.Popup;
    using Samsara.Features.Skill.Data;                  // confirm actual ISkillMasterDataRepository namespace
    using UnityEngine;

**Design rationale (Plan §5-7 / §7-2 / §7-4):**

- No direct View manipulation — all UI changes are delegated via ReplayView method calls (Constitution §3).
- StateVisualConfig — Plan §9-5 fix result. Sprite Key based. The 4 SpriteKeys are async-loaded via ISpriteLoader and mapped to Frame/Icon.
- No ISceneNavigator held — scene transition is the ReplayUseCase responsibility (Plan §3-2 ReplayUseCase responsibility (4)).
- The restart confirmation popup directly calls IPopupManager.ShowYesNoAsync (a simple message, so CommonPopupView reused). The node info popup and skill description popup are the two popup Views held by ReplayView as SerializeField, called Show/Hide directly by ReplayPresenter — no PopupManager (RQ-15 separated operation).
- StatsText / ConditionsText composition responsibility resides in Presenter — same pattern as the EvolutionTreeScene-side NodeDescriptionPopupView. Data (SO) ↔ display (View) separation (Constitution §2 Data/Logic Separation).
- ISkillMasterDataRepository dependency — used to load owned skill icons. EvolutionTreePresenter applies the same pattern.
- Skill slot click → skill description popup flow occurs inside the node info popup. The node info popup remains active + the skill description popup is additionally activated above it (Q4).
- async void usage is only for Unity event handler exception cases (HandleNodeTapped / HandleSkillSlotClickedAsync / HandleRestartClicked). UniTaskVoid or .Forget pattern applies to Initialize's BuildTreeAsync().Forget().
- No try-catch — Constitution §7 Fail Fast (Plan §10-4).
- No LINQ — for loops (Constitution §8).

**RQ satisfied:** Constitution §3, RQ-15 (popup separation), RQ-13 (visual mapping), Specify §4-1 (per-state popup content), Specify §4-3 (2-step confirmation).

---

#### Task 15: ReplaySceneBootstrapper.cs (rewrite, name retained)

**Path:** `Assets/_Game/Features/ReplayScene/Presentation/ReplaySceneBootstrapper.cs`

**Background:**

- File name retained — Bootstrapper retains the "Scene" suffix (Plan §4-1, majority naming convention).
- Body rewritten per Plan v2.0.0.
- Core responsibilities: GlobalBootstrapper wait → acquire GameContext → assemble dependency graph → inject ReplayPresenter → call Initialize → Dispose in OnDestroy.

**Work:**

**Class definition:**

- Inherits MonoBehaviour.
- namespace: `Samsara.Features.ReplayScene.Presentation`
- `_logClass = $"[{nameof(ReplaySceneBootstrapper)}]";` field.

**SerializeField:**

| Type | Name | Description |
|---|---|---|
| `ReplayView` | `_view` | ReplayView placed at the child UI tree root |
| `StateVisualConfig` | `_stateVisualConfig` | 4 Sprite Key settings (Inline Inspector) |

**Internal field:**

    private ReplayPresenter _presenter;

**Lifecycle method:**

    private void Start()
    {
        InitializeAsync().Forget();
    }

    private async UniTaskVoid InitializeAsync()
    {
        // 1. Wait for GlobalBootstrapper init (Constitution §3 Bootstrapper Hierarchy)
        await GlobalBootstrapper.Instance.InitializationTask;

        // 2. Acquire GameContext
        var gameContext = GlobalBootstrapper.Instance.GameContext;

        // 3. Acquire PendingReplayContext (G-22 — value set by EndingScene)
        var pendingContext = gameContext.PendingReplayContext;
        // null allowed in 1st-development — before EndingScene Patch-006 application or on direct entry.
        // ReplayUseCase only receives PendingReplayContext via constructor injection and does not use its content in the 1st implementation (Plan §8-2).

        // 4. Assemble dependency graph (Constitution §3 — Bootstrapper is the area where new is allowed)
        IRestartFlow restartFlow = new DefaultRestartFlow(gameContext);
        IPostRestartSceneRouter sceneRouter = new DefaultPostRestartSceneRouter();

        IReplayUseCase useCase = new ReplayUseCase(
            gameContext.CharacterAccountRepo,
            gameContext.EvolutionNodes,
            restartFlow,
            sceneRouter,
            gameContext.SceneNavigator,
            pendingContext);

        _presenter = new ReplayPresenter(
            useCase,
            _view,
            gameContext.PopupManager,
            gameContext.SpriteLoader,
            gameContext.SkillMasterDataRepo,
            _stateVisualConfig);

        // 5. Initialize Presenter → start tree build
        _presenter.Initialize();
    }

    private void OnDestroy()
    {
        _presenter?.Dispose();
    }

**Reset() Auto-Assignment (Constitution §7):**

    private void Reset()
    {
        _view = GetComponentInChildren<ReplayView>();
    }

`_stateVisualConfig` is an Inline Serializable class — Hak directly inputs the 4 SpriteKey strings in the Inspector (Manual Work).

**Required usings:**

    using Cysharp.Threading.Tasks;
    using Samsara.App;  // confirm actual GlobalBootstrapper / GameContext namespace and apply consistently
    using Samsara.Features.ReplayScene.Domain;
    using UnityEngine;

**Design rationale (Plan §5-6 / Constitution §3):**

- Use `Start()` — GlobalBootstrapper.InitializationTask wait is deterministic and avoids Awake order dependency (Plan §5-6 / Project Backlog I-02). To thoroughly resolve I-02, GlobalBootstrapper init pattern bulk cleanup is scheduled for Phase 7. These Tasks implement with the Start() pattern — Plan decision takes precedence.
- IDisposable pattern — events subscribed by ReplayPresenter are safely unregistered in OnDestroy.
- new usage location — only inside Bootstrapper allowed (Constitution §3). DefaultRestartFlow / DefaultPostRestartSceneRouter / ReplayUseCase / ReplayPresenter instantiation all concentrated in this class.
- GameContext.PendingReplayContext is simply read and injected into ReplayUseCase — read-only, no write (Specify §5-1 read-only ironclad rule).
- I-02 recurrence caution — do not use Bootstrapper Awake() pattern (Project Backlog I-02 tracked).

**Global search impact (§1-5 results applied):**

- If existing ReplayScenePresenter / ReplaySceneUseCase / ReplaySceneView creation logic exists in GameContext.cs or GlobalBootstrapper.cs, transfer to Bootstrapper in T15 of these Tasks. Removal of code such as ReplayPresenter creation inside GameContext is explicitly handled in T16.

**RQ satisfied:** Constitution §3 (Bootstrapper Hierarchy / new location), §7 (Reset, Fail Fast — no SerializeField dummy injection).

---

## 2-4. T16 (GameContext PendingReplayContext auto-property addition)

### Task 16: GameContext.cs Modify (G-22)

**Path:** `Assets/_Game/App/GameContext.cs`

**Background:**

- A GameContext property for ReplayScene to receive a one-time context from EndingScene (Plan §6-1, G-22).
- Same as the other PendingContext patterns — mutable auto-property.

**Work:**

**Property to add:**

    public PendingReplayContext PendingReplayContext { get; set; }

**Required using to add:**

    using Samsara.Features.ReplayScene.Domain;

**Add location:**

- Place adjacent to other PendingContext properties (e.g., PendingEndingContext / PendingEventContext) inside GameContext.cs.
- If an existing PendingContext pattern group exists, add it in the same area.

**Removal of existing GameContext code related to ReplaySceneUseCase / ReplayScenePresenter / ReplaySceneView:**

- If creation/injection code related to the v1.x.x ReplayScene Feature exists in GameContext.cs per the §1-5 global search results, remove appropriately.
    - Reason 1: T15 Bootstrapper takes the dependency graph assembly responsibility, so related code in GameContext is duplicated/conflicting.
    - Reason 2: If a renamed (ReplaySceneUseCase → ReplayUseCase, etc.) symbol is explicitly referenced in GameContext, a compile error occurs.
- Removing such code aligns with Constitution §6 (GameContext responsibility: holds SOs / Repositories / MasterData + Run-scope data). GameContext does not hold the ReplayScene Feature internal concrete classes (UseCase / Presenter / View) (Bootstrapper responsibility).

**Modification scope restriction (Plan §6-1 / G-22):**

- These Tasks only handle adding the PendingReplayContext auto-property in GameContext + removing ReplayScene-related concrete class creation/holding code (if present).
- **Do NOT modify any other GameContext methods (e.g., ResetRunForReplayAsync) or other properties at all**.
- Design changes / signature changes / consistency-guarantee logic additions to ResetRunForReplayAsync are outside the scope of these Tasks (changes to GameContext itself for RQ-16 guarantee are handled separately as Patches).

**Design rationale (Plan §6-1 / G-22):**

- G-22 — Pending Context follows the GameContext auto-property pattern. Simplifies write/read; separated from ICharacterAccountRepository usage (Specify §5-1).
- The 1st implementation has only the PreviousRunResult field. Future addition of reached stage / accumulated karma fields is handled by extending only the PendingReplayContext class.

**External setting responsibility (handled outside these Tasks):**

- EndingScene sets `gameContext.PendingReplayContext = new PendingReplayContext { PreviousRunResult = ... }` immediately before SceneKey.Replay transition — handled in **EndingScene Patch-006** (§1-2 table).
- These Tasks are written assuming that set logic is already applied.

**Verification:**

- Compile success — PendingReplayContext class (T2) is exported and compiles normally.
- No change to existing GameContext methods / properties — add-only.
- ReplaySceneBootstrapper (T15) can read `gameContext.PendingReplayContext`.

**RQ satisfied:** RQ-03 (PendingReplayContext transfer), G-22 (write/read responsibility separation).

**Constitution:** §6 (GameContext / Run-scope data holding), §2 (Data/Logic Separation — data container auto-property).

---

# §3. Manual Work (Hak performs in the Unity Editor)

Work performed by Hak directly in the Unity Editor after Claude Code completes code implementation. Items unverifiable in the code compile step.

| ID | Work | Description |
|---|---|---|
| M-01 | Replay.unity scene file creation + Build Settings registration | Create `Assets/_Game/Scenes/Replay.unity`. Register in Build Settings at the SceneKey.Replay index. (Plan §11 OQ-P4) |
| M-02 | Replay.unity GameObject hierarchy composition | Canvas + UI root + ReplaySceneBootstrapper (top) + ReplayView (child) + TreeScrollView + ReplayGuidanceView + OptionButtonView + ReplayNodeDescriptionPopupView + ReplaySkillDescriptionPopupView. Reference the EvolutionTreeScene Hierarchy same pattern for the hierarchy structure. |
| M-03 | ReplayNodeDescriptionPopupView GameObject composition | _popupRoot / _dimBackground (RectTransform covering full screen, raycastTarget=true) / _dimButton (Button on _dimBackground) / internal content hierarchy. Place _dimBackground as a child of _popupRoot. (Plan §11 OQ-P3) |
| M-04 | ReplaySkillDescriptionPopupView GameObject composition | _popupRoot / _dimBackground / _dimButton / internal content hierarchy. Same pattern as ReplayNodeDescriptionPopupView. |
| M-05 | Guarantee Z-order between the two popups (Q4) | Set Hierarchy sibling order or Canvas sortingOrder so that ReplaySkillDescriptionPopupView renders above ReplayNodeDescriptionPopupView. The skill description popup's Dim should cover the node info popup so it sits on top when active. |
| M-06 | Create ReplaySkillSlotView prefab + pre-place inside ReplaySkillListView | Create 1 slot prefab and pre-place N as children of ReplaySkillListView (max skill count basis — N=4 currently advised). Inspector-assign to the _skillSlots array. |
| M-07 | Inspector input for 4 StateVisualConfig SpriteKeys | In ReplaySceneBootstrapper Inspector, directly enter the 4 strings _stateVisualConfig.SelectableFrameSpriteKey / LockedFrameSpriteKey / HiddenFrameSpriteKey / QuestionMarkSpriteKey. Follow the SpriteLoader-recognized key convention. |
| M-08 | Manually assign all SerializeFields in Inspector (items Reset cannot auto-identify) | T12 ReplayNodeDescriptionPopupView SerializeFields other than _popupRoot, T12-A ReplaySkillDescriptionPopupView SerializeFields other than _popupRoot, T13 ReplayView's 5 child components, T15 ReplaySceneBootstrapper's _view. Multiple Image / TMP_Text / Button exist in the same GameObject, so Reset() auto-assignment cannot identify them. |
| M-09 | Reuse EvolutionTreeScene NodeView prefab (TreeScrollView setup for Replay.unity) | Reused NodeView prefab + TreeScrollView composition. Follows the NodeView signature (SetFrameSprite/SetIconSprite externally injected) post the EvolutionTreeScene Patch application. |
| M-10 | Set up test condition scenarios (optional) | To perform Runtime Validation, configure 3+ EvolutionNodeSO in Selectable / Locked / Hidden states. Register 1+ in AccountData.UnlockedEvolutionNodeIds. One should have IsHidden=true. |

**Detailed spec — M-03 / M-04 (common to both popups):**

Popup close trigger and modal input blocking are different mechanisms — Manual Work guarantees both simultaneously.

- Close trigger: X button + _dimBackground outside-region tap.
- Modal input blocking: provided by _dimBackground's raycastTarget=true.

When _popupRoot SetActive(false), input blocking is released. When _popupRoot SetActive(true), input blocking is active. No additional logic needed.

The _dimBackground RectTransform is full-screen size (Anchor stretch full / Offset 0). Adopt a structure where content is a child of _popupRoot (not a sibling of _dimBackground), but in sibling order below _dimBackground (rendered above). Then content remains clickable and only outside _dimBackground is input-blocked.

**Detailed spec — M-05 (Z-order guarantee):**

Both popups hold their own Dim. Activated together when opened (node info popup remains open + skill description popup additionally active). At this time, the skill description popup's Dim must cover the node info popup.

Choose one guarantee method:

- Method 1: Place the ReplaySkillDescriptionPopupView GameObject below the ReplayNodeDescriptionPopupView GameObject in sibling order (uses Hierarchy rendering order).
- Method 2: Add a separate Canvas + high sortingOrder value to the ReplaySkillDescriptionPopupView root.

Hak's judgment — Method 1 if the child structure is simple, Method 2 if complex or if other UI hierarchies overlap.

---

# §4. Validation (code-automated verification)

Application checklist after Claude Code completes implementation. Verified at the static analysis (compile) + console log + code review level. Each item is derived from Plan v2.0.0.

## 4-1. Compile / dependency verification

| ID | Verification item |
|---|---|
| V-01 | Full source compile success. Symbols ReplaySceneUseCase / ReplayScenePresenter / ReplaySceneView / NodeDescriptionPopupView (old name) do not remain anywhere in the source. |
| V-02 | Application status of the 2 external Patches (EndingScene Patch-006 / EvolutionTreeScene Patch) confirmed. PendingReplayContext set line exists, NodeView.SetFrameSprite/SetIconSprite signatures, EvolutionNodeState.cs exists. |
| V-03 | namespace consistency. ReplayScene Domain Layer 8 files (excluding T1, 7 files) all `Samsara.Features.ReplayScene.Domain`. Presentation 4 files (T10/T11/T13/T14) all `Samsara.Features.ReplayScene.Presentation`. Popup 6 files (T9 struct/T12/T12-A/T12-B struct/T12-X1/T12-X2) all `Samsara.Features.ReplayScene.Presentation.Popup`. T1 is `Samsara.Core.Tree`. |
| V-04 | Constitution §1 — Core/Tree/ReplayNodeState.cs is a pure enum with no Unity reference (no `using UnityEngine`). |
| V-05 | Constitution §4 — No `using UnityEngine` / `using TMPro` in any ReplayScene Domain Layer (T2~T8) file. Pure C#. |
| V-06 | Constitution §3 — `new` is not used in Logic classes (DefaultRestartFlow / DefaultPostRestartSceneRouter / ReplayUseCase / ReplayPresenter) for transferred class (excluding DTO/struct) creation. `new` is used only in ReplaySceneBootstrapper. |
| V-07 | Constitution §3 — No `GlobalBootstrapper.Instance` or other Singleton access inside Logic classes. Exists only in ReplaySceneBootstrapper.InitializeAsync. |
| V-08 | Constitution §8 — `_logClass = $"[{nameof(ClassName)}]";` field exists on every class. enum excluded. |
| V-09 | Constitution §8 — All private fields are `_camelCase`. No PascalCase / missing prefix. |
| V-10 | Constitution §8 — Event unsubscription uses `?.` null-conditional (Safe Cleanup). Child event unsubscription uses `if (X != null)` or `?.` (T12-X2 ReplaySkillListView OnDestroy / T13 ReplayView OnDestroy / T14 ReplayPresenter Dispose). |
| V-11 | Constitution §8 — No LINQ usage. Only for loops. ReplayUseCase.ClassifyNode's List.Contains is a List method, not LINQ — allowed. |
| V-12 | Constitution §7 — No SerializeField null-guard (Fail Fast). No `if (_field == null) return;` pattern. Exception: Safe Cleanup usage inside OnDestroy / Dispose. |
| V-13 | Constitution §7 — MonoBehaviour View classes (T10/T11/T12/T12-A/T12-X1/T12-X2/T13) have a Reset() method. T15 ReplaySceneBootstrapper same. |

## 4-2. Naming / file structure verification

| ID | Verification item |
|---|---|
| V-14 | RQ-15 / Plan §4-1 — class name + file name + namespace separation. ReplayNodeDescriptionPopupView (file name same) / ReplaySkillDescriptionPopupView (new) / ReplayPresenter / ReplayUseCase / ReplayView / ReplaySceneBootstrapper (suffix retained). Distinguished from EvolutionTreeScene-side same-name classes. |
| V-15 | T9 / T12-B structs each defined as top-level inside the T12 / T12-A files. Not nested classes. No `[Serializable]`. value type. |
| V-16 | T15 ReplaySceneBootstrapper.InitializeAsync internal new assembly order — DefaultRestartFlow → DefaultPostRestartSceneRouter → ReplayUseCase → ReplayPresenter. |

## 4-3. Lifecycle / listener pattern verification

| ID | Verification item |
|---|---|
| V-17 | Standard pattern (§2-3 (1)) applied — T10 OptionButtonView, T11 ReplayGuidanceView (no events, so absence of Awake/OnDestroy is normal), T12 ReplayNodeDescriptionPopupView, T12-A ReplaySkillDescriptionPopupView, T12-X1 ReplaySkillSlotView (lambda exception RemoveAllListeners), T12-X2 ReplaySkillListView, T13 ReplayView — all use Awake/OnDestroy + RemoveListener (or RemoveAllListeners) pattern. |
| V-18 | OnEnable/OnDisable pattern is not used in any Presentation Layer component of these Tasks. |
| V-19 | Uses RemoveListener(HandleClick) named methods. No lambda registration code. Exception: T12-X1 ReplaySkillSlotView (lambda used for index capture + RemoveAllListeners allowed). |
| V-20 | T12-X2 ReplaySkillListView OnDestroy applies single `_skillSlots` null-check branch + per-child null check. |

## 4-4. Wrong type verification

| ID | Verification item |
|---|---|
| V-21 | UnlockedEvolutionNodeIds is List<string>. EvolutionNodeSO.NodeId is string. ReplayUseCase.ClassifyNode uses Contains directly without int conversion. |
| V-22 | string → int conversion only in DefaultRestartFlow.ExecuteAsync. Uses int.Parse, not int.TryParse (Fail Fast). |
| V-23 | EvolutionNodeSO.UnlockConditions name (plural). Not the singular UnlockCondition. |
| V-24 | SkillSO field access names — SkillName / Description / Damage / IconSpriteKey. Effects not used. |
| V-25 | ISpriteLoader method name — LoadSpriteAsync (returns UniTask<Sprite>). Matches the common pattern across other Features. |
| V-26 | IPopupManager.ShowYesNoAsync signature — (PopupRequest, CancellationToken = default) → UniTask<bool>. |

## 4-5. UI text hardcoding tracking verification

| ID | Verification item |
|---|---|
| V-27 | Each newly added UI text hardcoding line in the ReplayScene Feature in these Tasks includes the comment `// UI text hardcoded: tracked in Project Backlog GBL-001. Subject to Phase 7 global cleanup.` immediately above. greppable. |
| V-28 | Count of new hardcoding lines in the ReplayScene Feature aggregated (derived via general console output grep). The number is used during Project Backlog GBL-001 cumulative registration in the follow-up checklist. |

---

# §5. Runtime Validation (Hak Play mode)

Behavioral verification performed by Hak in Unity Play mode after Manual Work completes. Each item is derived from Specify v2.1.1 / Plan v2.0.0 decisions.

## 5-1. Entry flow

| ID | Verification item | Expected result |
|---|---|---|
| R-01 | Enter Replay.unity from EndingScene (RQ-01 / RQ-02 / RQ-03) | ReplayScene displays without disruption. PendingReplayContext.PreviousRunResult read success (no console warnings / errors). |
| R-02 | Direct Play of Replay.unity (PendingReplayContext null state) | ReplayScene displays without disruption. No exception (Plan §8-2 1st implementation content not used). |
| R-03 | State immediately after entry | Full tree displayed, guidance UI text "재시작할 진화체를 선택하세요." displayed, top-right option button fixed display, no back button. |

## 5-2. Node state mapping

| ID | Verification item | Expected result |
|---|---|---|
| R-04 | Display of nodes contained in UnlockedEvolutionNodeIds (Selectable) | StateVisualConfig.SelectableFrameSpriteKey loaded frame. Icon = EvolutionNodeSO.NodeIconSpriteKey original. (RQ-09 / RQ-13) |
| R-05 | Display of unlocked + IsHidden=false nodes (Locked) | LockedFrameSpriteKey loaded frame. Icon = NodeIconSpriteKey original. (RQ-09 / RQ-13) |
| R-06 | Display of unlocked + IsHidden=true nodes (Hidden) | HiddenFrameSpriteKey loaded frame. Icon = QuestionMarkSpriteKey (NodeIconSpriteKey not used). (RQ-09 / RQ-13 / RQ-14) |

## 5-3. Node info popup

| ID | Verification item | Expected result |
|---|---|---|
| R-07 | Tap a Selectable node | Node info popup opens with its own Dim. Character name / icon / stats / owned skill icons / restart button / unlock conditions displayed. (Specify §4-1 / Specify §4-2) |
| R-08 | Tap a Locked node | Node info popup opens. Only character name / icon / unlock conditions displayed. Stats / skills / restart button not shown. |
| R-09 | Tap a Hidden node | Node info popup opens. "???" displayed — character name / stats are "???". Unlock conditions displayed. Icon / skills / restart button not shown. (Q re-confirmation — absence of icon display in Hidden node popup) |

## 5-4. Popup close / modal input blocking

| ID | Verification item | Expected result |
|---|---|---|
| R-10 | Node info popup X tap | Popup closes. Tree display retained. (Plan §7-3) |
| R-11 | Tap Dim region (outside) of node info popup | Popup closes. (Plan §7-3) |
| R-12 | Try tapping another node while node info popup is active | Cannot tap another node (popup Dim blocks input). Must close the popup and tap again. (Specify §4-2) |
| R-13 | Try tapping the top-right option button while node info popup is active | Cannot tap option button (popup Dim blocks input). |

## 5-5. Skill description popup

| ID | Verification item | Expected result |
|---|---|---|
| R-14 | Tap an owned skill icon in the Selectable node info popup | Skill description popup opens floating above the node info popup. Popup's Dim covers the node info popup (M-05 Z-order). Icon / name / description / "데미지: 3.5" format displayed. (Q1 ~ Q4 / Q6) |
| R-15 | Skill description popup X tap | Popup closes. Node info popup remains open. |
| R-16 | Tap Dim region (outside) of skill description popup | Popup closes. Node info popup remains open. |

## 5-6. Restart flow

| ID | Verification item | Expected result |
|---|---|---|
| R-17 | Selectable node info popup → tap restart button → confirmation popup "재시작" | Routine fields (currentNodeId / lastRunResult) initialized + reset. Auto-transition to SceneKey.Main. (RQ-08 / RQ-16 / Specify §4-3) |
| R-18 | Tap "취소" in restart confirmation popup | Confirmation popup closes. Node info popup remains open (Plan §7-4 cancel handling). Can re-select another node. |
| R-19 | Verify new run state after MainScene entry | RunData fully initialized (Wave 1 / no carry-over state / explicit Day notation). MainScene operates in the receiving state. (Specify §6 / RQ-16) |

---

# §6. Claude Code Handoff Guide

Operational rules and application order to apply when handing off this Tasks document to Claude Code.

## 6-1. Required reading before implementation

Claude Code reads the following documents **in order** before starting code authoring.

1. `CLAUDE.md` — project root (Constitution + global operational rules).
2. `.claude/specs/features/replay-scene/specify.md` — ReplayScene Specify v2.1.1.
3. `.claude/specs/features/replay-scene/plan.md` — ReplayScene Plan v2.0.0.
4. `.claude/specs/features/replay-scene/tasks.md` — these Tasks v2.0.0 (the MD version of this document).
5. `.claude/specs/features/replay-scene/decisions.md` — created empty in T0. Claude Code records judgments during implementation.

## 6-2. Implementation order

Follow these Tasks' §2-0 "Dependency order".

    T0 → T1 ~ T7 → T8 → T10 → T11 → T12-X1 → T12-X2 → T9/T12 → T12-B/T12-A → T13 → T14 → T15 → T16

Move to the next Task immediately upon completing each Task. Mid-implementation reporting of intermediate results to Hak at small group / completion timings is welcome.

## 6-3. SPEC-GAP handling

When a decision not specified in Spec / Plan / Tasks is needed during implementation:

1. **Do not request immediate Spec / Plan / Tasks changes**. Claude Code does not modify the Spec.
2. Record in `decisions.md` with one of the following tags:
    - `[DECISION]` — code-level judgment (e.g., int.Parse selection rationale, ConditionsText format details).
    - `[BACKLOG]` — temporary implementation (e.g., TreeScrollView signature substitute implementation, scheduled to sync upon future EvolutionTreeScene Patch application).
    - `[SPEC-GAP]` — item not defined in Spec (e.g., TreeScrollView actual signature differs from assumption, ISkillMasterDataRepository.GetSkillsByIds method name differs).
3. Implementation proceeds. Code is authored under explicit assumptions + rationale specified in decisions.md.
4. After implementation completion, Hak checks decisions.md. If a SPEC-GAP requires Spec / Plan modification, the Patch authoring procedure proceeds (details in §7 follow-up checklist).

## 6-4. External code reading guidance

The implementation of these Tasks depends on the following external code:

- `Assets/_Game/App/GameContext.cs` (T16 modify target, read by T15)
- `Assets/_Game/App/GlobalBootstrapper.cs` (T15 reads InitializationTask)
- `Assets/_Game/Core/Tree/EvolutionNodeState.cs` (file adjacent to T1 — already created by EvolutionTreeScene Patch)
- `Assets/_Game/Core/Tree/TreeScrollView.cs` (T14 calls build method — reads signature from EvolutionTreeScene Patch application result)
- `Assets/_Game/Core/Tree/NodeView.cs` (T14 reads SetFrameSprite/SetIconSprite spec)
- `Assets/_Game/Features/Character/Data/CharacterAccountRepository.cs` (T8 reads AccountData access path)
- `Assets/_Game/Features/Character/MasterData/EvolutionNodeSO.cs` (no modify — fields provided only)
- `Assets/_Game/Features/Skill/Data/SkillMasterDataRepository.cs` (T14 reads GetSkillsByIds name / signature)
- `Assets/_Game/Features/Skill/MasterData/SkillSO.cs` (no modify)

External code is **read-only**. Modify only files explicitly specified for modification by these Tasks (T16 GameContext.cs).

## 6-5. File rename caution (T8 / T12 / T13 / T14)

Rename via Unity Editor Project view to **preserve the GUID**. The purpose is to keep the component script identifier already configured in the .meta file and the scene file readable. Claude Code execution caution:

- If you perform path move and class name change simultaneously with `mv` class, the .meta file may not detach and follow along. Investigate with a single normal git mv.
- If rename is performed while Unity Editor is open, the .meta auto-syncs, but Claude Code works outside the IDE / Editor. So **Hak proceeds work with Unity Editor open**, or after Claude Code work, Hak restarts Unity to confirm .meta sync.
- Even after rename, the .meta file's GUID for the class name does not change, so .unity / .prefab / Inspector configurations are retained as-is.

## 6-6. Deliverables Claude Code provides after implementation completion

1. **Implemented code** — 19 Tasks + GameContext.cs modification.
2. **Final decisions.md** — includes `[DECISION]` / `[BACKLOG]` / `[SPEC-GAP]` tags.
3. **Validation execution log** — PASS / FAIL / partial PASS notation per §4 V-01 ~ V-28 item.
4. **High-detail implementation summary** — explanation of implementation flow and key decisions (for Hak's review, separate from decisions.md).
5. **Manual Work item list** — explicitly indicate to Hak which items in M-01 ~ M-10 Claude Code cannot perform.

## 6-7. Specific signature mismatch handling

These Tasks make the following assumptions — may differ from actual code:

| Assumption | Handling if actual differs |
|---|---|
| `EvolutionNodeSO.SkillIds` is `int[]` | If a different type, apply consistently with the implementation type + decisions.md `[DECISION]`. |
| `ISkillMasterDataRepository.GetSkillsByIds(int[])` method exists | If method name differs (e.g., GetSkill / FindSkills), apply actual name + decisions.md `[SPEC-GAP]`. |
| TreeScrollView exposes `event Action<EvolutionNodeSO> OnNodeTapped` | If signature is per nodeId string, add conversion responsibility to ReplayPresenter + decisions.md `[SPEC-GAP]`. |
| Bootstrapper holds StateVisualConfig as SerializeField | Match. If a separate SO structure is required, decisions.md `[SPEC-GAP]`. |

Do not stop implementation due to insufficient injection. Apply matching the actual code + specify in decisions.md + reflect during Hak review.

---

# §7. Follow-up Checklist

Application items + work order to perform after Tasks completion.

## 7-1. Tasks document family finalization

- ✅ Tasks-KR v2.0.0 Notion upload (already done by Hak 2026-04-28)
- ⏳ Tasks-MD v2.0.0 English translation + Notion upload (current task — in progress)
- ⏳ Tasks-Changelog v2.0.0 entry add (Initial v2.0.0 — full rewrite)
- ⏳ Tasks-KR/MD v1.0.0 body archive completion verification (must be in Archive folder per GBL-002 / 📐 archive procedure page)

## 7-2. ReplayScene Status update

- ⏳ ReplayScene Status page Tasks v2.0.0 completion notation
- ⏳ Timeline entry add: "2026-04-28 — Tasks v2.0.0 completed (full rewrite per Plan v2.0.0)"
- ⏳ Status field update — "Tasks v2.0.0 completed / Implementation pending" state (Claude Code implementation handoff timing)

## 7-3. External prerequisite Patch issuance

The two ⏳ Patches in the §1-2 table must be issued as separate Patches before entering implementation.

- ⏳ EndingScene Patch-006 issuance (PendingReplayContext set logic add)
    - Specify: only what to add (gameContext.PendingReplayContext = new PendingReplayContext { PreviousRunResult = ... }; line immediately before SceneKey.Replay transition)
    - Direction: Patch only (no Specify version up; pure addition)
- ⏳ EvolutionTreeScene Patch issuance (NodeView signature change + EvolutionTreePresenter mapping ownership change + NodeDescriptionPopupView Dim addition + NodeState→EvolutionNodeState rename + ReplayNodeState file location)
    - Comprehensive Patch issuance — many small changes consolidated
    - Pre-decisions: see Plan §11 OQ-X1 / OQ-X2 / OQ-P2

## 7-4. Project Backlog update

- ⏳ GBL-001 (UI text hardcoding) cumulative count update
    - In ReplayScene Feature in these Tasks: Korean inline literal count `// UI text hardcoded:` comment basis grep aggregation
    - Number registration in Project Backlog GBL-001 page table
- ⏳ GBL-003 new registration (majority 13 ButtonView RemoveAllListeners → RemoveListener bulk transition)
    - These Tasks consciously choose RemoveListener — opportunity to align project-wide majority pattern
    - Backlog candidate scope: 13 ButtonView occurrences identified in Main / Maintenance / CharacterInfo / Ending / Battle

## 7-5. Claude Code implementation handoff

- ⏳ Hak provides handoff to Claude Code by passing the following 3 items together:
    1. Approved English MD body of these Tasks (`.claude/specs/features/replay-scene/tasks.md`)
    2. Confirmation that the 2 external Patches (EndingScene Patch-006 / EvolutionTreeScene Patch) have been applied
    3. Empty `decisions.md` template
- ⏳ Implementation completion deliverables Claude Code provides as listed in §6-6 (5 items)
- ⏳ After implementation completion, Hak reviews + applies decisions.md

## 7-6. Manual Work

- ⏳ Hak performs M-01 ~ M-10 (§3) directly in Unity Editor
    - Replay.unity scene file creation + Build Settings registration (M-01)
    - GameObject hierarchy composition (M-02)
    - Two popups internal hierarchy + Z-order (M-03 / M-04 / M-05)
    - ReplaySkillSlotView prefab + ReplaySkillListView pre-placement (M-06)
    - StateVisualConfig 4 SpriteKey input (M-07)
    - All SerializeField Inspector assignment (M-08)
    - NodeView prefab + TreeScrollView setup (M-09)
    - Test condition scenarios (M-10)
- ⏳ R-01 ~ R-19 Runtime Validation execution

## 7-7. Reference document update

- ⏳ Project Roadmap update — Phase 6 ReplayScene v2.0.0 completion mark (after implementation completion)
- ⏳ Project Status update — RunData / GameContext / Feature lists' ReplayScene-related entries
- ⏳ Constitution review — confirm whether new patterns introduced in these Tasks (Reset() Auto-Assignment + lambda exception RemoveAllListeners + popup own Dim pattern + StateVisualConfig SerializeField pattern) need to be reflected. Constitution amendment is handled in a separate work batch.

## 7-8. Next Phase entry

- ⏳ After ReplayScene v2.0.0 implementation completion, the Phase 7 candidate list (UI text management system / Bootstrapper Awake-Start standardization / etc. — all in Project Backlog) is reviewed for prioritization.

---

## End-of-document marker

This document is **ReplayScene Tasks v2.0.0**. Authored 2026-04-28. Aligned with Specify v2.1.1 / Plan v2.0.0.

Total 19 Implementation Tasks (T0 ~ T16, including T9 / T12-A / T12-B / T12-X1 / T12-X2 sub-IDs) + 10 Manual Work + 28 Validation + 19 Runtime Validation.

Handoff target: Claude Code (`.claude/specs/features/replay-scene/tasks.md`).
```