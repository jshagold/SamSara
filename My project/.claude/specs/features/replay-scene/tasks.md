# ReplayScene — Tasks

**Version:** 1.0.0 | **Date:** 2026-04-21 | **Feature:** ReplayScene (Phase 6)
**Base:** Specify v1.1.0 / Plan v1.0.0
**Constitution Reference:** §1~§11 (full compliance verified)
**Work Type:** New Feature creation + shared Tree component extraction (includes EvolutionTreeScene structural changes)

**Prerequisite Patches Completed (2026-04-21):** CharacterRepository Patch-003 / InventorySystem Patch-001 / EndingScene Patch-005. These Tasks assume all three are applied.

---

## Version History

| Version | Date | Changes |
|---|---|---|
| v1.0.0 | 2026-04-21 | Initial — Phase 6 ReplayScene new Feature + Core/Tree shared component extraction |

---

## Current Version: v1.0.0

### ⚠️ Post-Task Checklist (must be performed after Tasks confirmation)

After Tasks-KR confirmation → Tasks-MD translation → Notion upload, proceed through the following without omission.

1. ✅ Tasks-MD v1.0.0 Notion upload
2. ⏳ ReplayScene Status page refresh (mark Tasks v1.0.0 ✅)
3. ⏳ **Pre-Manual Work (Pre-M-01 ~ Pre-M-02) performed by Hak in Unity Editor** — shared Tree component file/prefab moves (GUID preservation; mandatory before Claude Code starts)
4. ⏳ Hand off to Claude Code (ReplayScene Tasks-MD v1.0.0)
5. ⏳ Receive Claude Code completion report
6. ⏳ **Post-Manual Work (Post-M-01 ~ Post-M-04) performed by Hak in Unity Editor** — Replay.unity scene creation + Inspector wiring + runtime validation
7. ⏳ Decisions page upload (decisions.md content from Claude Code)
8. ⏳ Project Backlog GBL-001 update with new ReplayScene hardcoding count

---

### Constitution Checklist

| Article | Applied | v1.0.0 Reflection |
|---|---|---|
| §1 Folder Structure | ✅ | `Core/Tree/` new folder (shared components), `Features/ReplayScene/` follows Data/Domain/Presentation. All paths under `Assets/_Game/`. |
| §2 Zero Guessing | ✅ | All accessor / property names / signatures finalized from Claude Code investigation (2026-04-21). No Singleton (GameContext injection). Data/Logic separated: NodeState enum is data, classification is in UseCase. |
| §3 Bootstrapper | ✅ | ReplaySceneBootstrapper initializes in Start() (new creation, not subject to I-02, Plan RQ-P08). `new` used only in Bootstrapper and GameContext. Standard pattern: `await GlobalBootstrapper.Instance.InitializationTask;` → `var gameContext = GlobalBootstrapper.Instance.GameContext;` (7/9 Features). |
| §4 Clean Architecture | ✅ | Domain(ReplaySceneUseCase) ↔ Presentation(Presenter/View/Popup) ↔ App(GameContext) separated. UseCase has no Unity references. Scene transition is invoked by Presenter via SceneNavigator (Plan RQ-P04). |
| §5 Async/Popup | ✅ | ExecuteReplayAsync, ResetRunForReplayAsync both return UniTask. PopupManager called only from Presenter. Coroutine/Task forbidden. |
| §6 GameContext | ✅ | ResetRunForReplayAsync added as new method. Existing accessors unchanged. Data Lifetime separation preserved (CharacterAccountData not reset). Uses SceneKey.Replay (no string literal). |
| §7 UI Standards | ✅ | Reset() Auto-Assignment in View class. No null-guards on [SerializeField] UI (Fail Fast). NodeId parse failure throws InvalidOperationException. |
| §8 Coding Standards | ✅ | `_camelCase` fields, `PascalCase` methods, `*View/*Presenter/*UseCase` suffix. Every class has `_logClass`. `[SerializeField] private`. `?.` SafeCleanup in OnDestroy. No LINQ/new in Update (no Update in this Feature). |
| §9 Data Persistence | ✅ | ResetRunForReplayAsync calls SaveAllDataSync() internally — Save-on-Action preserved. Dirty Flag handled per Repository. |
| §10 Wrapper Pattern | ✅ | NodeState/TreeLayoutCalculator/NodeView in Core/Tree/ (engine-independent). ISceneNavigator/IPopupManager already in Core. |
| §11 Libraries | ✅ | UniTask (async/await), TMP (text). No change. |

---

### UI Text Hardcoding Note (Project Backlog GBL-001)

Popup texts in this Feature (restart-confirmation popup title/body/button labels) follow **the existing project convention (C# inline literals, Korean)** — permitted under Project Backlog GBL-001.

- Background: Claude Project Instructions v2.16.0 §2 declares "UI text hardcoding forbidden," but Phase 7 will build a global UI text management system and convert everything in bulk. Grandfathered for now.
- Claude Code investigation (2026-04-21) found C# inline literals are the convention across the entire project. Making ReplayScene the sole exception would break consistency.
- **Every new hardcoded UI string in this Feature must be added to Project Backlog GBL-001 after Tasks completion** (Post-Task Checklist item 8).

---

### Pre-Implementation Checklist

Claude Code MUST complete the following before starting implementation.

**1. Required reading**

- `CLAUDE.md` (project root)
- `.claude/specs/features/replay-scene/specify.md` (Specify v1.1.0)
- `.claude/specs/features/replay-scene/plan.md` (Plan v1.0.0)
- `.claude/specs/features/replay-scene/tasks.md` (this MD document)
- `.claude/specs/features/replay-scene/decisions.md` (created empty in Task 0)

**2. Read all existing implementation files (after prerequisite patches applied)**

EvolutionTreeScene Feature files (move / reference-update targets):

- `Assets/_Game/Features/EvolutionTreeScene/Domain/EvolutionTreeUseCase.cs` (verify embedded NodeState definition + ClassifyNodeState)
- `Assets/_Game/Features/EvolutionTreeScene/Presentation/EvolutionTreeSceneBootstrapper.cs`
- `Assets/_Game/Features/EvolutionTreeScene/Presentation/EvolutionTreePresenter.cs`
- `Assets/_Game/Features/EvolutionTreeScene/Presentation/EvolutionTreeView.cs`
- `Assets/_Game/Features/EvolutionTreeScene/Presentation/Popup/NodeDescriptionPopupView.cs` (reference for ReplayScene Popup View design)

Files expected in Core/Tree/ after Pre-Manual Work (Pre-M-02):

- `Assets/_Game/Core/Tree/TreeLayoutCalculator.cs`
- `Assets/_Game/Core/Tree/TreeScrollView.cs`
- `Assets/_Game/Core/Tree/NodeView.cs` (renamed from EvolutionNodeView.cs)
- `Assets/_Game/Core/Tree/NodeConnectionView.cs`
- `Assets/_Game/Core/Tree/Prefabs/TreeNode.prefab`
- `Assets/_Game/Core/Tree/Prefabs/TreeNodeConnection.prefab`

Files modified by prerequisite patches (read-only, DO NOT modify):

- `Assets/_Game/Features/Character/MasterData/LastRunResult.cs`
- `Assets/_Game/Features/Character/Data/CharacterRunData.cs`
- `Assets/_Game/Features/Character/Data/CharacterRunRepository.cs` (InitializeNewRun signature: `(RunConfigSO config, int? overrideEvolutionNodeId = null)`)
- `Assets/_Game/Features/Inventory/Data/InventoryRepository.cs` (InitializeNewRun(RunConfigSO))
- `Assets/_Game/Features/Ending/Presentation/EndingPresenter.cs` (HandleRestart → SceneKey.Replay applied)

App / Core files:

- `Assets/_Game/App/GameContext.cs` (verify all existing accessors)
- `Assets/_Game/App/GlobalBootstrapper.cs` (locate Step 4-A TODO comment)
- `Assets/_Game/App/RunConfig/RunConfigSO.cs` (confirm path)
- `Assets/_Game/Core/Navigation/SceneKey.cs` (confirm Replay value exists)
- `Assets/_Game/Core/Popup/PopupRequest.cs` (constructor signature: `new PopupRequest(string title, string message, string confirmText, string cancelText)`)
- `Assets/_Game/Core/Popup/IPopupManager.cs` (method signature: `ShowYesNoAsync(PopupRequest, CancellationToken = default) → UniTask<bool>`)

**3. Finalized signatures (Zero Guessing compliance)**

Signatures confirmed by Claude Code investigation (2026-04-21). **No guessing — use exactly as shown.**

| Target | Final Value |
|---|---|
| GlobalBootstrapper access | `GlobalBootstrapper.Instance` (static Singleton) |
| GlobalBootstrapper init wait | `await GlobalBootstrapper.Instance.InitializationTask;` |
| GameContext access | `var gameContext = GlobalBootstrapper.Instance.GameContext;` |
| GameContext.CharacterAccountRepo | `ICharacterAccountRepository` |
| GameContext.SkillMasterDataRepo | `ISkillMasterDataRepository` |
| GameContext.EvolutionNodes | `EvolutionNodeSO[]` |
| GameContext.SceneNavigator | `ISceneNavigator` |
| GameContext.PopupManager | `IPopupManager` |
| GameContext.SpriteLoader | `ISpriteLoader` |
| GameContext.RunConfig | `RunConfigSO` |
| GameContext.CharacterRunRepo | `ICharacterRunRepository` |
| GameContext.StageRepo | `IStageRepository` |
| GameContext.ShopRepo | `IShopRepository` |
| GameContext.InventoryRepo | `IInventoryRepository` |
| EvolutionNodeSO.NodeId | `string` |
| EvolutionNodeSO.CharacterName | `string` |
| EvolutionNodeSO.BaseStats | `CharacterStatsSO` |
| EvolutionNodeSO.IsHidden | `bool` |
| EvolutionNodeSO.SkillIds | `int[]` |
| EvolutionNodeSO.NodeIconSpriteKey | `string` |
| EvolutionNodeSO.UnlockConditions | `StatCondition[]` (plural — note!) |
| CharacterStatsSO.Hp / Strength / Toughness / Agility | `int` |
| ICharacterAccountRepository.AccountData | `CharacterAccountData` |
| CharacterAccountData.UnlockedEvolutionNodeIds | `List<string>` (string comparison — note!) |
| PopupRequest constructor | `new PopupRequest(title, message, confirmText, cancelText)` |
| IPopupManager.ShowYesNoAsync | `UniTask<bool> ShowYesNoAsync(PopupRequest, CancellationToken = default)` |

**4. Global search to assess impact scope**

Enumerate every occurrence of the following across the project before starting work.

- `EvolutionNodeView` (class name — rename all to `NodeView`)
- EvolutionTreeScene TreeArea namespace references — migrate all to `Samsara.Core.Tree`
- `NodeState` embedded enum references under `EvolutionTreeScene.Domain` — migrate to `Core.Tree.NodeState`

**5. Confirm Pre-Manual Work is complete**

Proceed only if Pre-M-01 and Pre-M-02 are complete. Otherwise ask Hak and wait.

---

### Pre-Manual Work (performed by Hak BEFORE Claude Code starts)

Hak performs these in Unity Editor before Claude Code touches any code. Skipping this step results in compile errors because target files remain in old locations.

**Core principle (Plan §6-6, §11-2):**

- All file/prefab moves MUST be done via drag-and-drop in Unity Editor's Project view.
- OS-level file moves (Finder/Explorer) drop the .meta files, breaking GUIDs and all Inspector references in EvolutionTreeScene.
- Unity Editor moves preserve both .cs/.prefab and .meta, keeping GUIDs intact.

| # | Work | Target |
|---|---|---|
| Pre-M-01 | Create `Assets/_Game/Core/Tree/` + `Prefabs/` subfolder | — |
| Pre-M-02 | Drag-and-drop 6 files/prefabs in Unity Editor to Core/Tree/ | see table below |

**Pre-M-02 move list:**

| Source Path | Destination Path | Rename? |
|---|---|---|
| `Features/EvolutionTreeScene/Domain/TreeLayoutCalculator.cs` | `Core/Tree/TreeLayoutCalculator.cs` | — |
| `Features/EvolutionTreeScene/Presentation/TreeArea/TreeScrollView.cs` | `Core/Tree/TreeScrollView.cs` | — |
| `Features/EvolutionTreeScene/Presentation/TreeArea/EvolutionNodeView.cs` | `Core/Tree/NodeView.cs` | **FILE RENAME** |
| `Features/EvolutionTreeScene/Presentation/TreeArea/NodeConnectionView.cs` | `Core/Tree/NodeConnectionView.cs` | — |
| `Features/EvolutionTreeScene/Presentation/Prefabs/EvolutionNode.prefab` | `Core/Tree/Prefabs/TreeNode.prefab` | **FILE RENAME** |
| `Features/EvolutionTreeScene/Presentation/Prefabs/NodeConnectionView.prefab` | `Core/Tree/Prefabs/TreeNodeConnection.prefab` | **FILE RENAME** |

**Empty folder cleanup:**

- Delete `Features/EvolutionTreeScene/Presentation/TreeArea/` if empty after move
- Delete `Features/EvolutionTreeScene/Presentation/Prefabs/` if empty after move

**Project state at this point:**

- Unity Console will show **many compile errors** (EvolutionTreeScene still references old paths/namespaces/class names). **This is expected — Claude Code resolves them sequentially in T1 ~ T9.**
- Do NOT Play EvolutionTreeScene in this state.

---

### Files to Create / Modify

**Scope:**

- New files: 6 (5 ReplayScene Feature files + 1 decisions.md)
- Modified files: 9 (4 moved Core/Tree + 4 EvolutionTreeScene + 2 App)

| # | File | Path | Action |
|---|---|---|---|
| T0 | decisions.md | `.claude/specs/features/replay-scene/` | Create (empty) |
| T1 | NodeState.cs | `Assets/_Game/Core/Tree/` | Create (new) |
| T2 | TreeLayoutCalculator.cs | `Assets/_Game/Core/Tree/` | Modify (namespace only) |
| T3 | TreeScrollView.cs | `Assets/_Game/Core/Tree/` | Modify (namespace only) |
| T4 | NodeView.cs | `Assets/_Game/Core/Tree/` | Modify (namespace + class rename `EvolutionNodeView` → `NodeView`) |
| T5 | NodeConnectionView.cs | `Assets/_Game/Core/Tree/` | Modify (namespace only) |
| T6 | EvolutionTreeUseCase.cs | `Assets/_Game/Features/EvolutionTreeScene/Domain/` | Modify (remove embedded NodeState + update using) |
| T7 | EvolutionTreeSceneBootstrapper.cs | `Assets/_Game/Features/EvolutionTreeScene/Presentation/` | Modify (using + `EvolutionNodeView` → `NodeView`) |
| T8 | EvolutionTreePresenter.cs | `Assets/_Game/Features/EvolutionTreeScene/Presentation/` | Modify (using + `EvolutionNodeView` → `NodeView`) |
| T9 | EvolutionTreeView.cs | `Assets/_Game/Features/EvolutionTreeScene/Presentation/` | Modify (using + `EvolutionNodeView` → `NodeView`) |
| T10 | GameContext.cs | `Assets/_Game/App/` | Modify (add ResetRunForReplayAsync + ApplyStatsFromEvolutionNode) |
| T11 | GlobalBootstrapper.cs | `Assets/_Game/App/` | Modify (fix Step 4-A TODO comment only) |
| T12 | ReplaySceneUseCase.cs | `Assets/_Game/Features/ReplayScene/Domain/` | Create |
| T13 | ReplaySceneView.cs | `Assets/_Game/Features/ReplayScene/Presentation/` | Create |
| T14 | NodeDescriptionPopupView.cs | `Assets/_Game/Features/ReplayScene/Presentation/Popup/` | Create |
| T15 | ReplayScenePresenter.cs | `Assets/_Game/Features/ReplayScene/Presentation/` | Create |
| T16 | ReplaySceneBootstrapper.cs | `Assets/_Game/Features/ReplayScene/Presentation/` | Create |

**Unchanged files (reference):**

- EvolutionTreeScene's `Popup/NodeDescriptionPopupView.cs`, `ReincarnationButtonView.cs`, `TopBar/*` — kept as-is
- Files modified by prerequisite patches (Character/Inventory/Ending) — no further changes
- SceneKey.cs, SceneNavigator.cs — kept as-is (SceneKey.Replay already exists)

---

### Implementation Tasks

#### Task 0: Create empty decisions.md (§2)

**Path:** `.claude/specs/features/replay-scene/decisions.md`

**Work:**

- Create as empty file.
- Start recording here whenever a decision outside the Spec is required during v1.0.0 implementation.
- Format: tag each entry with `[DECISION]`, `[BACKLOG]`, or `[SPEC-GAP]`.

**Backlog Items pre-announced by Plan (MUST record when encountered):**

- `[BACKLOG] BL-03` — Evolution-node stat application logic duplicated in 3 places (GlobalBootstrapper Step 4-A / EvolutionTreeUseCase.ExecuteEvolutionAsync / GameContext.ApplyStatsFromEvolutionNode). Phase 7 review for shared utility extraction. (Plan RQ-P11, RQ-P15)
- `[BACKLOG] BL-04` — NodeDescriptionPopupView shared-extraction opportunity between EvolutionTreeScene and ReplayScene. Post-Phase 7 review. (Plan RQ-P06, RQ-P16)

Both items concern code Claude Code will touch during implementation — record them in decisions.md.

---

#### Task 1: Create Core/Tree/NodeState.cs (§1, §4, §8, §10)

**Path:** `Assets/_Game/Core/Tree/NodeState.cs`

**Background:**

- After Pre-M-02, Hak has moved TreeLayoutCalculator/TreeScrollView/NodeView/NodeConnectionView to Core/Tree/.
- NodeState enum currently lives embedded at the top of EvolutionTreeUseCase.cs (investigation found it around lines 11-18).
- T1 extracts this enum into Core/Tree/NodeState.cs. T6 then removes the embedded definition from EvolutionTreeUseCase.cs.

**Work:**

- Create new file. NOT a file move — copy-create from EvolutionTreeUseCase.cs.
- namespace: `Samsara.Core.Tree` (align with existing Core namespace convention — check patterns like `Samsara.Core.Navigation` first).

**Enum values (Plan §5-2):**

    public enum NodeState
    {
        Current,      // EvolutionTreeScene-only — current evolution
        Evolvable,    // EvolutionTreeScene-only — unlock conditions met, can evolve
        Reachable,    // EvolutionTreeScene-only — next-stage candidate
        Locked,       // shared — not unlocked
        Hidden,       // shared — hidden route + not unlocked
        Selectable    // ReplayScene-only — unlocked, selectable as starting node (reuses Evolvable frame, RQ-14)
    }

**Design compliance:**

- §1: Core/ is engine/platform-independent. enum is pure C#, fits perfectly.
- §4 Clean Architecture: Shared Domain data type referenced by both scenes' UseCases. Belongs in Core/Tree.
- §8 Naming: PascalCase enum values.

**Plan RQ-P02 rationale:**

- Frame SpriteKey mapping is each scene's Presenter responsibility (scenes use different state sets).
- enum enumerates all states; each Presenter handles only what it uses.

---

#### Task 2: Update namespace in Core/Tree/TreeLayoutCalculator.cs (§1, §4)

**Path:** `Assets/_Game/Core/Tree/TreeLayoutCalculator.cs`

**Background:** Pre-M-02 moved the file, but the internal namespace still reflects the old path.

**Work:**

- Change top-level namespace declaration to `Samsara.Core.Tree`.
- Internal logic (calculation algorithms) — **DO NOT modify.**
- Class name (`TreeLayoutCalculator`) — **DO NOT change.**
- Keep `_logClass` field if it exists.

**Design rationale (Plan §5-3):**

- Pure mathematical layout calculation, scene-agnostic. Belongs in Core/Tree.
- Touching the internal logic would risk breaking EvolutionTreeScene's existing behavior.

---

#### Task 3: Update namespace in Core/Tree/TreeScrollView.cs (§1, §4, §8)

**Path:** `Assets/_Game/Core/Tree/TreeScrollView.cs`

**Work:**

- Change top-level namespace declaration to `Samsara.Core.Tree`.
- Internal logic — **DO NOT modify.**
- Class name (`TreeScrollView`) — **DO NOT change.**
- Keep `[SerializeField] private` fields, Reset() Auto-Assignment, `_logClass` exactly as-is.

**Design rationale (Plan §7):**

- ScrollRect wrapper, scene-agnostic. Shared between both scenes.

---

#### Task 4: Update Core/Tree/NodeView.cs — namespace + class rename (§1, §4, §8)

**Path:** `Assets/_Game/Core/Tree/NodeView.cs`

**Background:** Pre-M-02 already renamed the file from `EvolutionNodeView.cs` to `NodeView.cs`. The inner class name is still `EvolutionNodeView`.

**Work:**

- Change top-level namespace declaration to `Samsara.Core.Tree`.
- **Rename class from `EvolutionNodeView` to `NodeView`** (Plan §5-2, RQ-P07).
- Update `_logClass` to use `nameof(NodeView)`.
- Internal logic (Sprite application, click event exposure, Reset(), etc.) — **DO NOT modify.**

**Design rationale (Plan §7-2):**

- Old name `EvolutionNodeView` implied "evolution tree node." Generalized to `NodeView` for shared use.
- File / class / prefab names all renamed consistently.

**GUID note:**

- Unity tracks .cs GUIDs via .meta. Pre-M-02's drag-and-drop moved .meta alongside, preserving GUIDs.
- Class rename alone preserves GUID — existing scene/prefab MonoBehaviour references stay intact.
- Unity warns if filename and class name mismatch. T4 completion unifies both to `NodeView`, resolving this.

---

#### Task 5: Update namespace in Core/Tree/NodeConnectionView.cs (§1, §4, §8)

**Path:** `Assets/_Game/Core/Tree/NodeConnectionView.cs`

**Work:**

- Change top-level namespace declaration to `Samsara.Core.Tree`.
- Internal logic — **DO NOT modify.**
- Class name (`NodeConnectionView`) — **DO NOT change.**
- Keep `[SerializeField] private` fields, Reset(), `_logClass` exactly as-is.

---

#### Task 6: EvolutionTreeUseCase.cs — Remove embedded NodeState + update using (§2, §4, §8)

**Path:** `Assets/_Game/Features/EvolutionTreeScene/Domain/EvolutionTreeUseCase.cs`

**Work:**

**Code to remove:**

- Remove the entire embedded `public enum NodeState { ... }` at the top of the file (around lines 11-18 per investigation).
- The enum values have already been migrated to `Core/Tree/NodeState.cs` in T1.

**Using statements to add:**

- `using Samsara.Core.Tree;` (for NodeState reference)
- If a using for TreeLayoutCalculator existed, replace it with `using Samsara.Core.Tree;`

**Preserve:**

- `ClassifyNodeState` method body — **DO NOT modify.** Classification criteria differ from ReplaySceneUseCase, so no shared extraction (Plan §5-4).
- All of `GetCurrentNodeId`, `GetCurrentStats`, `GetAllNodes`, `GetSkillsForNode`, `FindCurrentNode`, `CheckUnlockConditions`, `ExecuteEvolutionAsync`, `ExecuteReincarnationAsync` — signatures and bodies preserved.
- `_logClass` field preserved.
- All public method signatures unchanged.

**Design rationale (Plan §5-4):**

- NodeState enum moves to Core, but classification logic stays scene-specific. ReplayScene checks "is unlocked?"; EvolutionTreeScene checks "in nextNodes AND unlock conditions met."

---

#### Task 7: EvolutionTreeSceneBootstrapper.cs — using/reference update (§2, §3)

**Path:** `Assets/_Game/Features/EvolutionTreeScene/Presentation/EvolutionTreeSceneBootstrapper.cs`

**Work:**

**Using updates:**

- Migrate all references to TreeLayoutCalculator / TreeScrollView / NodeView (formerly EvolutionNodeView) / NodeConnectionView / NodeState to `Samsara.Core.Tree`.
- Remove any remaining old-namespace usings (TreeArea/Domain subfolders under EvolutionTreeScene).

**Class name update:**

- Replace every `EvolutionNodeView` type reference with `NodeView` (new calls, SerializeField type declarations, method parameters, etc.).

**Preserve:**

- SceneBootstrapper 3-phase structure (Constitution §3) as-is.
- If Awake() is currently used, **keep Awake()** (I-02 Awake/Start mass cleanup is Phase 7 scope, outside this Task).
- GlobalBootstrapper wait + GameContext access pattern as-is.
- `_logClass` as-is.

---

#### Task 8: EvolutionTreePresenter.cs — using/reference update (§2, §4, §8)

**Path:** `Assets/_Game/Features/EvolutionTreeScene/Presentation/EvolutionTreePresenter.cs`

**Work:**

**Using updates:**

- Migrate all references to TreeLayoutCalculator / TreeScrollView / NodeView / NodeConnectionView / NodeState to `Samsara.Core.Tree`.

**Class name update:**

- Replace every `EvolutionNodeView` type reference with `NodeView`.

**Preserve:**

- Do NOT modify any private method body.
- HandleNodeClicked, per-state Sprite mapping, Evolve button callback — all preserved.
- Popup inline text literals (investigation found them around lines 174 / 189) — **DO NOT modify.** Tracked by Project Backlog GBL-001 for Phase 7 cleanup.
- `_logClass` as-is.

---

#### Task 9: EvolutionTreeView.cs — using/reference update (§2, §7, §8)

**Path:** `Assets/_Game/Features/EvolutionTreeScene/Presentation/EvolutionTreeView.cs`

**Work:**

**Using updates:**

- Migrate all references to TreeScrollView / NodeView / NodeConnectionView to `Samsara.Core.Tree`.

**Class name update:**

- `[SerializeField] private TreeScrollView _treeScrollView;` — type name unchanged (only namespace changes).
- If any SerializeField was declared as `EvolutionNodeView`, **rename the type to `NodeView`**. GUID preserved so Inspector references remain intact.

**Preserve:**

- Reset() Auto-Assignment logic (Constitution §7).
- `_logClass` as-is.

---

#### Task 10: GameContext.cs — Add ResetRunForReplayAsync + ApplyStatsFromEvolutionNode (§4, §5, §6, §9)

**Path:** `Assets/_Game/App/GameContext.cs`

**Work:**

**New public method (Plan §8):**

    public async UniTask ResetRunForReplayAsync(int selectedEvolutionNodeId)
    {
        // 1. Reset each RunData Repository (signatures unified by prerequisite patches)
        CharacterRunRepo.InitializeNewRun(RunConfig, selectedEvolutionNodeId);  // optional param added by Patch-003
        StageRepo.InitializeNewRun(RunConfig);
        ShopRepo.InitializeNewRun(RunConfig);
        InventoryRepo.InitializeNewRun(RunConfig);  // after Patch-001

        // 2. Apply evolution-node stats (same pattern as GlobalBootstrapper Step 4-A)
        ApplyStatsFromEvolutionNode(selectedEvolutionNodeId);

        // 3. Batch save (OQ-01 → RQ-P01: single SaveAllDataSync call)
        SaveAllDataSync();

        await UniTask.CompletedTask;
    }

**New private helper:**

    private void ApplyStatsFromEvolutionNode(int nodeId)
    {
        var nodeIdStr = nodeId.ToString();
        foreach (var node in EvolutionNodes)
        {
            if (node.NodeId != nodeIdStr || node.BaseStats == null) continue;

            var d = CharacterRunRepo.RunData;
            d.Hp        = node.BaseStats.Hp;
            d.MaxHp     = node.BaseStats.Hp;
            d.Strength  = node.BaseStats.Strength;
            d.Toughness = node.BaseStats.Toughness;
            d.Agility   = node.BaseStats.Agility;
            return;
        }

        // No match or BaseStats null → Fail Fast (§7)
        throw new InvalidOperationException(
            $"[GameContext] ResetRunForReplayAsync: EvolutionNodeSO not found or BaseStats null for NodeId [{nodeIdStr}]");
    }

**⚠️ Verification before writing:**

- Check whether `_logClass` exists in GameContext.cs. If yes, use `{_logClass}`. If no, use `"[GameContext]"` literal as shown.
- Confirm `CharacterRunRepo.RunData` access pattern (property name / getter) by reading CharacterRunRepository, and use the same pattern.
- Confirm `MaxHp` field exists on CharacterRunData. If missing, record as `[SPEC-GAP]` in decisions.md and ask Hak before proceeding.

**Compliance:**

- §5: UniTask return. Currently synchronous internally but prepared for future async save migration.
- §6: App-layer orchestration across multiple Repositories.
- §7: Fail Fast — throw on node match failure.
- §8: No LINQ, single-pass foreach.
- §9: SaveAllDataSync preserves Save-on-Action.

**Unchanged:** Constructor and all existing public properties. Only 2 new methods added.

**Impact:**

- T12 (ReplaySceneUseCase.ExecuteReplayAsync) calls this method.
- GlobalBootstrapper Step 4-A's new-user init path is preserved (new-user only).

**Required decisions.md entry (Plan pre-announced Backlog):**

- Record `[BACKLOG] BL-03`: stat application duplicated in 3 places (GlobalBootstrapper / EvolutionTreeUseCase.ExecuteEvolutionAsync / GameContext.ApplyStatsFromEvolutionNode). Phase 7 consolidation review.

---

#### Task 11: GlobalBootstrapper.cs — Fix TODO comment (§2, §6)

**Path:** `Assets/_Game/App/GlobalBootstrapper.cs`

**Work (Specify §6-5, Plan §11-6 #6):**

**Existing comment (from RunConfigSO Decisions D-12):**

    // TODO: Phase 6 — Move to SplashScene/ReplayScene

**Replace with (example — Claude Code writes equivalent):**

    // New-user path only: auto-initialize RunData on first app launch.
    // ReplayScene handles restart after an ended run via a separate path
    // and does not inherit this auto-initialization.
    // (Per ReplayScene Specify RQ-01 / RQ-12)

**Compliance:**

- §2 Zero Guessing: make intent explicit.
- Declare that the new-user auto-init path stays in GlobalBootstrapper.

**Unchanged:**

- Step 4-A auto-init body preserved.
- RunConfigSO-based InitializeNewRun calls preserved.

**Prerequisite Patch note:**

- CharacterRunRepository.cs:49 comment "Step 4-E" → "Step 4-A" correction is **already done by prerequisite Patch-003**. Not in scope here.

---

#### Task 12: Create ReplaySceneUseCase.cs (§1, §4, §5, §7, §8)

**Path:** `Assets/_Game/Features/ReplayScene/Domain/ReplaySceneUseCase.cs`

**Work:**

- namespace: `Samsara.Features.ReplayScene.Domain` (align with project namespace convention — check existing Feature patterns first)
- Pure C# class (no Unity reference, Constitution §4 Domain rule)
- `_logClass = $"[{nameof(ReplaySceneUseCase)}]";` field

**Constructor dependencies (Plan §5-1):**

- `ICharacterAccountRepository _characterAccountRepo` — unlock list access
- `ISkillMasterDataRepository _skillMasterDataRepo` — skill lookup
- `EvolutionNodeSO[] _evolutionNodes` — from GameContext.EvolutionNodes
- `GameContext _gameContext` — for calling ResetRunForReplayAsync

**Public methods:**

    public EvolutionNodeSO[] GetAllNodes()
    public bool IsNodeUnlocked(EvolutionNodeSO node)
    public NodeState ClassifyNodeStateForReplay(EvolutionNodeSO node)
    public SkillSO[] GetSkillsForNode(int[] skillIds)
    public async UniTask ExecuteReplayAsync(EvolutionNodeSO selectedNode)

**GetAllNodes:**

- Return `_evolutionNodes` directly (reference return is fine — assume callers don't mutate).

**IsNodeUnlocked (Plan §5-1):**

    public bool IsNodeUnlocked(EvolutionNodeSO node)
    {
        if (node == null) return false;
        var unlocked = _characterAccountRepo.AccountData.UnlockedEvolutionNodeIds;
        foreach (var id in unlocked)
        {
            if (id == node.NodeId) return true;
        }
        return false;
    }

**⚠️ Note (investigation):** `UnlockedEvolutionNodeIds` is `List<string>`, `EvolutionNodeSO.NodeId` is also `string`. Direct string comparison is correct.

**ClassifyNodeStateForReplay (Plan §5-1):**

    public NodeState ClassifyNodeStateForReplay(EvolutionNodeSO node)
    {
        if (node == null)
            throw new InvalidOperationException($"{_logClass} ClassifyNodeStateForReplay: node is null");

        if (!IsNodeUnlocked(node))
            return node.IsHidden ? NodeState.Hidden : NodeState.Locked;

        return NodeState.Selectable;
    }

**GetSkillsForNode:**

- Mirror the pattern in EvolutionTreeUseCase (lookup skillIds via ISkillMasterDataRepository).
- Claude Code reads EvolutionTreeUseCase first and replicates the exact signature and return style.

**ExecuteReplayAsync (Plan §5-1):**

    public async UniTask ExecuteReplayAsync(EvolutionNodeSO selectedNode)
    {
        // Fail Fast validation (§7)
        if (selectedNode == null)
            throw new InvalidOperationException($"{_logClass} ExecuteReplayAsync: selectedNode is null");

        if (!IsNodeUnlocked(selectedNode))
            throw new InvalidOperationException(
                $"{_logClass} ExecuteReplayAsync: selected node [{selectedNode.NodeId}] is not unlocked");

        // NodeId type conversion (Plan RQ-P13: EvolutionNodeSO.NodeId is string, RunConfigSO.DefaultEvolutionNodeId is int)
        if (!int.TryParse(selectedNode.NodeId, out int nodeIdInt))
            throw new InvalidOperationException(
                $"{_logClass} ExecuteReplayAsync: NodeId [{selectedNode.NodeId}] is not parseable as int");

        await _gameContext.ResetRunForReplayAsync(nodeIdInt);
    }

**Compliance:**

- §4: Scene transition (enter Main) is invoked by Presenter after reset completes. UseCase handles reset only — responsibility boundary (Plan RQ-P04).
- §7: Fail Fast — throw on null, not-unlocked, parse failure.
- §8: No LINQ, use foreach. Use `_logClass`.

---

#### Task 13: Create ReplaySceneView.cs (§1, §7, §8)

**Path:** `Assets/_Game/Features/ReplayScene/Presentation/ReplaySceneView.cs`

**Work:**

- Inherit MonoBehaviour
- namespace: `Samsara.Features.ReplayScene.Presentation`
- `_logClass = $"[{nameof(ReplaySceneView)}]";` field

**SerializeField (Plan §6-3):**

    [SerializeField] private OptionButtonView         _optionButtonView;   // G-05: option button only (no back button, RQ-11)
    [SerializeField] private TreeScrollView           _treeScrollView;     // Core/Tree
    [SerializeField] private NodeDescriptionPopupView _popupView;          // ReplayScene-specific

**⚠️ Verification:**

- Confirm where `OptionButtonView` is defined (which namespace) — Claude Code reads first, adds using accordingly.
- Mirror how EvolutionTreeView wires OptionButtonView.

**Public properties (read-only):**

    public TreeScrollView           TreeScrollView => _treeScrollView;
    public NodeDescriptionPopupView PopupView      => _popupView;
    public OptionButtonView         OptionButtonView => _optionButtonView;

**Reset() Auto-Assignment (Constitution §7):**

    private void Reset()
    {
        _optionButtonView = GetComponentInChildren<OptionButtonView>();
        _treeScrollView   = GetComponentInChildren<TreeScrollView>();
        _popupView        = GetComponentInChildren<NodeDescriptionPopupView>();
    }

**Compliance:**

- §7 Fail Fast: No null-guards on the 3 SerializeFields above. Missing reference → NullReferenceException surfaces immediately.
- §8 `[SerializeField] private`.
- No internal public methods — like EvolutionTreeView, just exposes child views via properties (Plan §6-3).

**No back button (Specify §3-1, RQ-11):**

- We're coming off an ended run — no prior scene to return to. Option button only.

---

#### Task 14: Create NodeDescriptionPopupView.cs for ReplayScene (§1, §5, §7, §8)

**Path:** `Assets/_Game/Features/ReplayScene/Presentation/Popup/NodeDescriptionPopupView.cs`

**Work:**

- Inherit MonoBehaviour
- namespace: `Samsara.Features.ReplayScene.Presentation.Popup`
- `_logClass = $"[{nameof(NodeDescriptionPopupView)}]";` field

**Separation from EvolutionTreeScene rationale (Plan §6-4, RQ-P06):**

- EvolutionTree popup: Evolve button + "evolve" semantics.
- Replay popup: Restart button + "select" semantics.
- Same info displayed but primary action and calling context differ.
- v1.0.0 keeps them separate. Extraction opportunity tracked in decisions.md `[BACKLOG] BL-04` (see Task 0).

**Mandatory reference to EvolutionTreeScene's NodeDescriptionPopupView:**

- SerializeField composition, DTO delivery (direct Sprite vs SpriteKey + ISpriteLoader), skill slot rendering — **Claude Code MUST read EvolutionTreeScene's version first and replicate its structure.** Start by viewing `Features/EvolutionTreeScene/Presentation/Popup/NodeDescriptionPopupView.cs`, then copy the structure.
- Only one difference: **Evolve button is replaced by Restart button.** (button GameObject + event handler name)

**SerializeField (replicate EvolutionTree + Restart button):**

    [SerializeField] private GameObject _root;                 // full popup toggle
    [SerializeField] private Image      _iconImage;
    [SerializeField] private TMP_Text   _nameText;
    [SerializeField] private TMP_Text   _hpText;
    [SerializeField] private TMP_Text   _strengthText;
    [SerializeField] private TMP_Text   _toughnessText;
    [SerializeField] private TMP_Text   _agilityText;
    [SerializeField] private TMP_Text   _unlockConditionText;  // shown only when locked/hidden
    [SerializeField] private Transform  _skillSlotContainer;   // parent for skill list
    [SerializeField] private Button     _restartButton;
    [SerializeField] private Button     _closeButton;

**⚠️ Actual field composition must match EvolutionTreeScene's equivalent class.** The list above is illustrative — if EvolutionTree's composition differs, follow that.

**NodeDescriptionData DTO (Plan RQ-P14):**

- Claude Code confirms **what DTO / parameter structure** EvolutionTreeScene's popup uses.
- Apply the same DTO structure (or same parameter style) here.
- DTO placement (nested in same file vs separate file) — follow EvolutionTree's pattern.
- Record mismatches as `[DECISION]` in decisions.md.

**Public API (Plan §6-4):**

    public event Action OnRestartClicked;
    public event Action OnCloseClicked;

    public void Show(/* same parameters as EvolutionTree popup */, bool isRestartEnabled)
    {
        // 1. _root SetActive(true)
        // 2. Populate SerializeField texts/images (same as EvolutionTree)
        // 3. If UnlockConditionText null/empty → _unlockConditionText.gameObject.SetActive(false)
        // 4. Render skill slots (same as EvolutionTree)
        // 5. _restartButton.interactable = isRestartEnabled
    }

    public void Hide()
    {
        // _root SetActive(false)
    }

**Reset() Auto-Assignment:**

    private void Reset()
    {
        _root = gameObject;
        // Other fields recommended to assign manually (GetComponentsInChildren can't disambiguate)
    }

**OnDestroy SafeCleanup (§8):**

    private void OnDestroy()
    {
        _restartButton?.onClick.RemoveAllListeners();
        _closeButton?.onClick.RemoveAllListeners();
    }

**Button event wiring:**

- Wire in Awake or Start: `_restartButton.onClick.AddListener(() => OnRestartClicked?.Invoke());`
- Presenter subscribes/unsubscribes via the event.

---

#### Task 15: Create ReplayScenePresenter.cs (§1, §4, §5, §8)

**Path:** `Assets/_Game/Features/ReplayScene/Presentation/ReplayScenePresenter.cs`

**Work:**

- Pure C# class (NOT MonoBehaviour, Constitution §4)
- namespace: `Samsara.Features.ReplayScene.Presentation`
- `_logClass = $"[{nameof(ReplayScenePresenter)}]";` field
- Implement IDisposable (Bootstrapper calls Dispose on OnDestroy)

**Fields (Plan §6-2):**

    private readonly ReplaySceneUseCase    _useCase;
    private readonly TreeLayoutCalculator  _layoutCalculator;
    private readonly ReplaySceneView       _view;
    private readonly ISceneNavigator       _sceneNavigator;
    private readonly IPopupManager         _popupManager;
    private readonly ISpriteLoader         _spriteLoader;
    private readonly string[]              _frameSpriteKeys;   // [0]=Evolvable(for Selectable), [1]=Locked, [2]=Hidden, [3]=QuestionMark

    private EvolutionNodeSO[]              _allNodes;
    private EvolutionNodeSO                _selectedNode;

**Public methods:**

    public void Initialize()
    public void Dispose()

**Initialize:**

    public void Initialize()
    {
        InitializeAsync().Forget();
    }

    private async UniTask InitializeAsync()
    {
        // 1. Load all nodes
        _allNodes = _useCase.GetAllNodes();

        // 2. Preload frame sprites (_spriteLoader.LoadSpriteAsync)
        //    - Mirror EvolutionTreePresenter's preload pattern
        //    - Load 4 frame SpriteKeys (Evolvable/Locked/Hidden/QuestionMark)

        // 3. Compute layout (_layoutCalculator)

        // 4. Place nodes into View's TreeScrollView
        //    - Call ClassifyNodeStateForReplay per node
        //    - Map frame SpriteKey, inject into NodeView
        //    - For Hidden, override icon with QuestionMark key

        // 5. Subscribe to View events
        //    - Each NodeView click → HandleNodeClicked
        //    - Popup: OnRestartClicked → HandleRestartRequested, OnCloseClicked → HandlePopupCloseRequested
    }

**⚠️ Mandatory reference:** Read EvolutionTreePresenter.Initialize pattern (preload order, node placement flow, event subscription) first and replicate the structure.

**HandleNodeClicked:**

    private void HandleNodeClicked(string nodeId)
    {
        // 1. Find matching node in _allNodes (foreach, no LINQ)
        EvolutionNodeSO clicked = null;
        foreach (var node in _allNodes)
        {
            if (node.NodeId == nodeId) { clicked = node; break; }
        }
        if (clicked == null)
        {
            Debug.LogWarning($"{_logClass} HandleNodeClicked: node not found for id=[{nodeId}]");
            return;
        }

        // 2. Classify state
        var state = _useCase.ClassifyNodeStateForReplay(clicked);

        // 3. Locked / Hidden → no reaction
        if (state == NodeState.Locked || state == NodeState.Hidden) return;

        // 4. Selectable → show info popup
        _selectedNode = clicked;
        // Assemble DTO and call _view.PopupView.Show(data, isRestartEnabled: true)
        //   DTO matches EvolutionTreeScene's NodeDescriptionPopupView parameters
    }

**HandleRestartRequested (async void allowed as Unity event handler exception):**

    private async void HandleRestartRequested()
    {
        // UI text hardcoding: tracked by Project Backlog GBL-001. Phase 7 global cleanup target.
        var request = new PopupRequest(
            "재시작 확정",
            $"{_selectedNode.CharacterName}(으)로 새 런을 시작합니다.",
            "재시작",
            "취소");

        bool confirmed = await _popupManager.ShowYesNoAsync(request);

        if (!confirmed)
        {
            // Cancel → info popup stays open (user can pick another node)
            return;
        }

        // Confirm restart → reset RunData
        await _useCase.ExecuteReplayAsync(_selectedNode);

        // Navigate to MainScene (§4: scene transition is Presentation responsibility, RQ-P04)
        _sceneNavigator.NavigateToAsync(SceneKey.Main).Forget();
    }

**⚠️ Mandatory hardcoding comment:**

- The line immediately above the PopupRequest constructor MUST include the comment: `// UI text hardcoding: tracked by Project Backlog GBL-001. Phase 7 global cleanup target.`
- This comment is the grep target for Phase 7 bulk cleanup.

**HandlePopupCloseRequested:**

    private void HandlePopupCloseRequested()
    {
        _selectedNode = null;
        _view.PopupView.Hide();
    }

**GetFrameKeyForState / GetIconKeyForState (Plan §6-2):**

    private string GetFrameKeyForState(NodeState state)
    {
        switch (state)
        {
            case NodeState.Selectable: return _frameSpriteKeys[0];  // reuse Evolvable frame (RQ-14)
            case NodeState.Locked:     return _frameSpriteKeys[1];
            case NodeState.Hidden:     return _frameSpriteKeys[2];
            default:
                throw new InvalidOperationException(
                    $"{_logClass} GetFrameKeyForState: unexpected NodeState [{state}]");
        }
    }

    private string GetIconKeyForState(NodeState state, string originalIconKey)
    {
        return state == NodeState.Hidden ? _frameSpriteKeys[3] : originalIconKey;
    }

**Dispose (§8 SafeCleanup):**

    public void Dispose()
    {
        // Unsubscribe each node's click event
        // (Adjust per how _view.TreeScrollView manages NodeViews — mirror EvolutionTreePresenter.Dispose)

        // Unsubscribe popup events
        if (_view?.PopupView != null)
        {
            _view.PopupView.OnRestartClicked -= HandleRestartRequested;
            _view.PopupView.OnCloseClicked   -= HandlePopupCloseRequested;
        }
    }

**Compliance:**

- §4: Scene transition in Presenter, reset in UseCase (RQ-P04).
- §5: UniTask, proper Forget. async void only as button-callback exception.
- §8: `_logClass`, `?.` SafeCleanup, foreach (no LINQ), `_camelCase`.

---

#### Task 16: Create ReplaySceneBootstrapper.cs (§1, §3)

**Path:** `Assets/_Game/Features/ReplayScene/Presentation/ReplaySceneBootstrapper.cs`

**Work:**

- Inherit MonoBehaviour
- namespace: `Samsara.Features.ReplayScene.Presentation`
- `_logClass = $"[{nameof(ReplaySceneBootstrapper)}]";` field

**SerializeField (Plan §6-1):**

    [SerializeField] private ReplaySceneView _view;
    [SerializeField] private string _evolvableFrameKey;    // reused for Selectable nodes (RQ-14)
    [SerializeField] private string _lockedFrameKey;
    [SerializeField] private string _hiddenFrameKey;
    [SerializeField] private string _questionMarkKey;

**Start() flow (Constitution §3, Plan RQ-P08, investigation-based standard pattern):**

    private ReplayScenePresenter _presenter;

    private async void Start()
    {
        // 1. Wait for GlobalBootstrapper init (standard pattern, 7/9 Features)
        await GlobalBootstrapper.Instance.InitializationTask;

        // 2. Acquire GameContext
        var gameContext = GlobalBootstrapper.Instance.GameContext;

        // 3. Create Core.Tree.TreeLayoutCalculator (§3: new only in Bootstrapper)
        var layoutCalculator = new TreeLayoutCalculator();

        // 4. Create ReplaySceneUseCase
        var useCase = new ReplaySceneUseCase(
            gameContext.CharacterAccountRepo,
            gameContext.SkillMasterDataRepo,
            gameContext.EvolutionNodes,
            gameContext);

        // 5. Create ReplayScenePresenter
        var frameKeys = new[] { _evolvableFrameKey, _lockedFrameKey, _hiddenFrameKey, _questionMarkKey };
        _presenter = new ReplayScenePresenter(
            useCase,
            layoutCalculator,
            _view,
            gameContext.SceneNavigator,
            gameContext.PopupManager,
            gameContext.SpriteLoader,
            frameKeys);

        // 6. Initialize
        _presenter.Initialize();
    }

    private void OnDestroy()
    {
        _presenter?.Dispose();
    }

**Awake vs Start (Plan RQ-P08):**

- New creation → **use Start()** (standard pattern, 7/9 Features). Not subject to I-02 (Awake/Start mismatch cleanup).

**SerializeField null not allowed (§7 Fail Fast):**

- If `_view`, `_evolvableFrameKey`, etc. are null/empty, let NullReferenceException / runtime exceptions surface immediately.

**Compliance:**

- §3: `new` only in Bootstrapper. Start() waits for GlobalBootstrapper then acts as Feature Bootstrapper.
- §8: `_logClass`, SafeCleanup (`?.`).

---

### Validation

Verify each after Claude Code completes implementation.

| # | Item | Method |
|---|---|---|
| V-01 | No compile errors in Unity Console | Console check |
| V-02 | `Assets/_Game/Core/Tree/NodeState.cs` exists, namespace `Samsara.Core.Tree` | File check |
| V-03 | 0 occurrences of `EvolutionNodeView` class name project-wide (renamed to `NodeView`) | Global search |
| V-04 | `EvolutionTreeUseCase.cs` has no embedded `enum NodeState` | File check |
| V-05 | `Features/EvolutionTreeScene/Presentation/TreeArea/` folder gone | FS check |
| V-06 | `Features/EvolutionTreeScene/Presentation/Prefabs/` folder gone or empty | FS check |
| V-07 | `Features/EvolutionTreeScene/Domain/TreeLayoutCalculator.cs` gone | FS check |
| V-08 | `Assets/_Game/Core/Tree/` has 4 .cs files + Prefabs folder | FS check |
| V-09 | `GameContext.ResetRunForReplayAsync`, `GameContext.ApplyStatsFromEvolutionNode` defined | File check |
| V-10 | `GlobalBootstrapper.cs` TODO comment clearly expresses new-user-path intent | File check |
| V-11 | `Features/ReplayScene/` has 5 new .cs files | FS check |
| V-12 | EvolutionTreeScene Play → all existing behavior intact (node display, evolve, reincarnate) | Play test |
| V-13 | decisions.md records BL-03, BL-04 (2 items) | File check |
| V-14 | ReplayScenePresenter has "Project Backlog GBL-001" comment above PopupRequest constructor | File check |

---

### Post-Manual Work (performed by Hak AFTER Claude Code completes)

| # | Task | Target | Notes |
|---|---|---|---|
| Post-M-01 | Create Replay.unity scene | `Assets/_Game/Scenes/Replay.unity` | Plan §11-1. Main Camera (Screen Space - Camera) + Canvas + ReplaySceneBootstrapper GameObject + ReplaySceneView hierarchy + TreeScrollView + NodeDescriptionPopupView (initially disabled) + OptionButtonView |
| Post-M-02 | Add Replay.unity to Build Settings | Unity > Build Settings | Match SceneKey.Replay's index |
| Post-M-03 | Wire Inspector SerializeFields | ReplaySceneBootstrapper, ReplaySceneView, NodeDescriptionPopupView, TreeScrollView | Full Plan §11-1 "Inspector wiring" section. TreeScrollView prefabs reference the moved `Core/Tree/Prefabs/TreeNode.prefab`, `TreeNodeConnection.prefab`. Fill in the 4 frame SpriteKeys (Evolvable/Locked/Hidden/QuestionMark). |
| Post-M-04 | Runtime Validation R-01 ~ R-08 | Play mode | — |

**Tasks Claude Code cannot perform (hence Post-Manual):**

- Creating .unity scene and arranging GameObject hierarchy
- Wiring Inspector SerializeFields
- Build Settings
- Play mode testing

---

### Runtime Validation (performed during Post-Manual Work)

| # | Scenario | Expected |
|---|---|---|
| R-01 | EndingScene Restart button → enter ReplayScene | Enters cleanly, tree UI loads |
| R-02 | Unlocked nodes show Selectable (Evolvable frame) | Only unlocked nodes get Evolvable frame; tappable |
| R-03 | Unlocked non-hidden nodes show Locked (Dim) | Locked frame + Dim; taps do nothing |
| R-04 | Unlocked hidden nodes show Hidden (question mark) | Hidden frame + question-mark sprite; taps do nothing |
| R-05 | Tap Selectable node → info popup | Icon/name/stats/skills shown, Restart button enabled |
| R-06 | Info popup Restart → confirmation popup (CommonPopup) → Cancel | Info popup remains open |
| R-07 | Info popup Restart → confirmation popup → Confirm → MainScene | RunData reset + selected BaseStats applied + MainScene loads |
| R-08 | After MainScene entry, verify CharacterRunData | `Day=1`, `Gold=0`, `Hp=MaxHp=selected.BaseStats.Hp`, `LastRunResult=None`, `IsReincarnationPending=false`, save file updated |

---

### Claude Code Handoff Guide

**Handoff:**

- Run `claude` at project root.
- `CLAUDE.md` auto-loads.
- Provide `.claude/specs/features/replay-scene/tasks.md` (this MD document).
- Make `specify.md` and `plan.md` accessible for reference.

**Implementation principles:**

- Execute Task 0 through Task 16 **sequentially**. No reordering (designed per dependency).
- **Assume Pre-Manual Work (Pre-M-01, Pre-M-02) is complete.** If not, ask Hak and wait.
- Obey each Task's compliance notes (Constitution §1 ~ §11).
- When a judgment beyond the Spec is needed mid-Task, **record it in decisions.md with a tag**.
    - `[DECISION]`: code-level decision
    - `[BACKLOG]`: temporary handling, follow-up needed
    - `[SPEC-GAP]`: Spec lacks definition
- Continue implementation after recording (no Hak approval wait).

**Pre-announced decisions.md entries (MUST record):**

- `[BACKLOG] BL-03` — ApplyStatsFromEvolutionNode duplicated in 3 places (record in Task 10)
- `[BACKLOG] BL-04` — NodeDescriptionPopupView shared-extraction opportunity (record in Task 14)

**Coding rules:**

- No files outside `Assets/_Game/` (decisions.md is the exception).
- All paths as specified in Tasks.
- Constitution §2 Zero Guessing: **never guess file paths/class names/API signatures — always read existing files first.**
- Where Tasks include "Finalized Signatures" tables, use the values verbatim. On mismatch, record `[SPEC-GAP]` in decisions.md and pause or ask Hak.
- Constitution §8: log tag, `_camelCase`, SafeCleanup, no LINQ.
- **UI text hardcoding is permitted under Project Backlog GBL-001.** Every hardcoded line MUST have a "Project Backlog GBL-001" comment above it.

**Completion report:**

- Full list of created/modified files.
- decisions.md entries summary (count per tag + key items).
- V-01 ~ V-14 self-validation results.
- **Count of NEW UI text hardcoding occurrences in ReplayScene (file:line)** — for GBL-001 update.
- Heads-up that Post-Manual Work (Post-M-01 ~ Post-M-04) remains.