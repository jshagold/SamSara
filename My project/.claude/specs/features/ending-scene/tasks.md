# EndingScene — Tasks

**Version:** 2.0.0 | **Date:** 2026-04-18 | **Feature:** EndingScene (Phase 6)
**Base:** Specify v2.0.0 / Plan v2.0.0
**Constitution Reference:** §1~§11 (full compliance verified)
**Work Type:** Full redesign on top of v1.0.0 + Patch-001~003 implementation (mixed Create / Modify / Delete)

---

## Version History

| Version | Date | Changes |
|---|---|---|
| v1.0.0 | 2026-04-16 | Initial — 0→1 greenfield Tasks (22 file creation) |
| v2.0.0 | 2026-04-18 | Full rewrite — node-local ending candidate slot structure introduced. Redesign Tasks on top of existing implementation. v1.0.0 full text preserved as Archive page under Changelog. |

---

## Current Version: v2.0.0

### ⚠️ Post-Task Checklist (must be performed after Tasks confirmation)

After Tasks-KR confirmation → Tasks-MD translation → Notion upload, proceed through the following **without omission**.

1. ✅ Tasks-MD v2.0.0 Notion upload
2. ⏳ EndingScene Status page refresh (mark Tasks v2.0.0 ✅, update next-step items)
3. ⏳ **Issue 3 external Feature Patch documents** (reference-only, lightweight 5-10 line structure)
    - Stage Feature Patch — BattleNodeDataSO field additions, StageProgressService/NodeCompletionContext cleanup
    - StageScene Feature Patch — StagePresenter.HandleBattleResultIfAny restructure
    - Event Feature Patch — EventSO.EventResult field addition, PendingEventContext.IsStageEndNode removal, EventPresenter restructure
    - Each Patch document body: Status + Type + "Refer to EndingScene Tasks-MD v2.0.0" notation + 3-5 line summary
4. ⏳ Add Patch History entries to each external Feature's Status page
5. ⏳ Hand off to Claude Code (only EndingScene Tasks-MD v2.0.0 is handed off; external Patches are reference-only)

---

### Constitution Checklist

During the v2.0.0 redesign, verify each article's compliance state. Articles marked "no change" maintain v1.0.0 compliance as-is.

| Article | Applied | v2.0.0 Reflection |
|---|---|---|
| §1 Folder Structure | ✅ | Features/Ending/, Features/Stage/, Features/Event/, Features/StageScene/ with Data/Domain/Presentation/MasterData. .asset files under Resources/MasterData/. Existing paths kept, files modified in place. |
| §2 Zero Guessing | ✅ | All paths/type names specified per Plan. No Singletons (GameContext injection retained). Data/Logic separation: EndingCandidateSlot is pure [Serializable] data; Resolver owns the matching logic. |
| §3 Bootstrapper | ✅ | No change. EndingSceneBootstrapper retained as in v1.0.0 (Start() initialization). `new` keyword is used only in Bootstrapper/Composition Root sites, such as the EndingResolver constructor call in GameContext. |
| §4 Clean Architecture | ✅ | **v2.0.0 core reflection point.** ① EndingCandidateSlot in the Domain layer is pure [Serializable] data, no logic. ② NodeCompletionContext shrinks to a pure DTO (IsStageEndNode removed); judgment logic moves into StageProgressService via IStageSceneUseCase (data/judgment responsibility split). ③ EndingConditionType switch evaluation lives in Resolver or a dedicated evaluator, not in the Condition data class. |
| §5 Async/Popup | ✅ | EndingEntryService.TryEnterEndingAsync keeps its UniTask return. All branches in StagePresenter/EventPresenter remain UniTask-based. Coroutine/Task forbidden rule upheld. |
| §6 GameContext | ✅ | Accessor composition unchanged (Plan §7). Only the IEndingMasterDataRepository argument is removed from EndingResolver's constructor. All other ctor arguments and public properties stay the same. |
| §7 UI Standards | ✅ | No View-layer changes in v2.0.0. v1.0.0 compliance maintained (Fail Fast, Reset() Auto-Assignment, Component Caching). |
| §8 Coding Standards | ✅ | Every new/modified class keeps its _logClass. _camelCase, suffix rules, [SerializeField] private, SafeCleanup (?.) retained. **EndingResolver matching logic uses a single foreach pass with no List/LINQ (Decisions D-07 pattern inherited, GC optimization).** |
| §9 Data Persistence | ✅ | EndingUseCase.CompleteEnding() Save-on-Action retained (no change). Dual-Mode Saving, Dirty Flag, ThreadPool I/O, Newtonsoft.Json retained. |
| §10 Wrapper Pattern | ✅ | Core/App separation retained. IEndingResolver/IEndingEntryService/IEndingMasterDataRepository live under Domain (Features/Ending/Domain/); implementations live under Data/Domain/ respectively. |
| §11 Libraries | ✅ | UniTask, DOTween, Newtonsoft.Json, TMP usage retained. No change. |

---

### Pre-Implementation Checklist

Claude Code must perform the following before starting implementation.

**1. Required reading**

- `CLAUDE.md` (project root)
- `.claude/specs/features/ending-scene/specify.md` (Specify v2.0.0)
- `.claude/specs/features/ending-scene/plan.md` (Plan v2.0.0)
- `.claude/specs/features/ending-scene/tasks.md` (MD version of this document)
- `.claude/specs/features/ending-scene/decisions.md` (existing v1.x decisions for reference; v2.0.0 decisions start fresh in this file — see Task 0)

**2. Read all existing implementation files (v1.0.0 + Patch-001~003 outputs)**

EndingScene Feature files:

- `Assets/_Game/Features/Ending/MasterData/EndingType.cs`
- `Assets/_Game/Features/Ending/MasterData/EndingDialogue.cs`
- `Assets/_Game/Features/Ending/MasterData/EndingSO.cs`
- `Assets/_Game/Features/Ending/MasterData/EndingCondition.cs`
- `Assets/_Game/Features/Ending/MasterData/EndingConditionType.cs`
- `Assets/_Game/Features/Ending/MasterData/EndingTriggerKind.cs`
- `Assets/_Game/Features/Ending/Domain/IEndingMasterDataRepository.cs`
- `Assets/_Game/Features/Ending/Domain/EndingContext.cs`
- `Assets/_Game/Features/Ending/Domain/IEndingResolver.cs`
- `Assets/_Game/Features/Ending/Domain/EndingResolver.cs`
- `Assets/_Game/Features/Ending/Domain/IEndingEntryService.cs`
- `Assets/_Game/Features/Ending/Domain/EndingEntryService.cs`
- `Assets/_Game/Features/Ending/Domain/EndingUseCase.cs`
- `Assets/_Game/Features/Ending/Domain/PendingEndingContext.cs`
- `Assets/_Game/Features/Ending/Domain/RunSummaryData.cs`
- `Assets/_Game/Features/Ending/Data/EndingMasterDataRepository.cs`
- `Assets/_Game/Features/Ending/Presentation/` entire subtree (no changes, but read for structural awareness)

External Feature files:

- `Assets/_Game/Features/Stage/MasterData/BattleNodeDataSO.cs`
- `Assets/_Game/Features/Stage/Domain/StageProgressService.cs`
- `Assets/_Game/Features/Stage/Domain/IStageProgressService.cs`
- `Assets/_Game/Features/Stage/Domain/NodeCompletionContext.cs`
- `Assets/_Game/Features/StageScene/Domain/StageSceneUseCase.cs` (or the corresponding UseCase file)
- `Assets/_Game/Features/StageScene/Presentation/StagePresenter.cs`
- `Assets/_Game/Features/Event/MasterData/EventSO.cs`
- `Assets/_Game/Features/Event/MasterData/EventResultType.cs`
- `Assets/_Game/Features/Event/Domain/PendingEventContext.cs`
- `Assets/_Game/Features/Event/Domain/EventOriginKind.cs`
- `Assets/_Game/Features/Event/Presentation/EventPresenter.cs`
- `Assets/_Game/App/GameContext.cs`

**3. Re-confirm architectural principles**

- Constitution §2 (Zero Guessing), §4 (Clean Architecture), §8 (Naming/LogTag/GC)
- Plan §5-6 (StageProgressService data/judgment split)
- Plan §6-3 (EventPresenter "node action model" perspective)
- Plan §9 RQ-P04 (Resolver uses foreach single pass, no LINQ)

**4. Global search to assess impact scope**

Enumerate every occurrence of the following symbols across the project before starting work.

- `EndingTriggerKind` (enum and all references)
- `StageCompleteFlag` (EndingConditionType value and all usages)
- `PendingEventContext.IsStageEndNode` (all setter/getter sites)
- `NodeCompletionContext.IsStageEndNode` (all setter/getter sites)

---

### Files to Create / Modify / Delete

v2.0.0 is a mixed structure of 1 new creation, many modifications, and 1 deletion. The Action column makes this distinction explicit.

| # | File | Path | Action |
|---|---|---|---|
| T0 | decisions.md | `.claude/specs/features/ending-scene/` | Reset (empty the file and begin fresh v2.0.0 records) |
| T1 | EndingCandidateSlot.cs | `Assets/_Game/Features/Ending/MasterData/` | Create |
| T2 | EndingSO.cs | `Assets/_Game/Features/Ending/MasterData/` | Modify (remove `_triggerKind` field) |
| T3 | EndingConditionType.cs | `Assets/_Game/Features/Ending/MasterData/` | Modify (remove `StageCompleteFlag` value) |
| T4 | EndingConditionType switch evaluation code | (Claude Code locates) | Modify (remove `StageCompleteFlag` case; verify Clean Architecture compliance) |
| T5 | EndingContext.cs | `Assets/_Game/Features/Ending/Domain/` | Modify (remove `IsStageEndNode` field) |
| T6 | IEndingResolver.cs | `Assets/_Game/Features/Ending/Domain/` | Modify (signature change) |
| T7 | EndingResolver.cs | `Assets/_Game/Features/Ending/Domain/` | Modify (signature + internal logic + ctor dependency adjustment) |
| T8 | IEndingMasterDataRepository.cs | `Assets/_Game/Features/Ending/Domain/` | Modify (remove `GetEndingsByTriggerKind`) |
| T9 | EndingMasterDataRepository.cs | `Assets/_Game/Features/Ending/Data/` | Modify (same method removal) |
| T10 | IEndingEntryService.cs | `Assets/_Game/Features/Ending/Domain/` | Modify (signature change) |
| T11 | EndingEntryService.cs | `Assets/_Game/Features/Ending/Domain/` | Modify (signature + internal logic) |
| T12 | BattleNodeDataSO.cs | `Assets/_Game/Features/Stage/MasterData/` | Modify (add `_victoryEndings`, `_defeatEndings`) |
| T13 | EventSO.cs (nested EventResult class) | `Assets/_Game/Features/Event/MasterData/` | Modify (add `_endingSlot`) |
| T14 | PendingEventContext.cs | `Assets/_Game/Features/Event/Domain/` | Modify (remove `IsStageEndNode`) |
| T15 | NodeCompletionContext.cs | `Assets/_Game/Features/Stage/Domain/` | Modify (remove `IsStageEndNode`) |
| T16 | StageProgressService.cs | `Assets/_Game/Features/Stage/Domain/` | Modify (switch to direct IStageSceneUseCase invocation) |
| T17 | IStageProgressService.cs | `Assets/_Game/Features/Stage/Domain/` | Verify (assess signature impact) |
| T18 | StagePresenter.cs | `Assets/_Game/Features/StageScene/Presentation/` | Modify (restructure `HandleBattleResultIfAny` + remove IsStageEndNode setter code) |
| T19 | EventPresenter.cs | `Assets/_Game/Features/Event/Presentation/` | Modify (restructure `HandlePostResultAsync`) |
| T20 | GameContext.cs | `Assets/_Game/App/` | Modify (EndingResolver ctor argument adjustment only) |
| T21 | MaintenanceScene-related code | (Claude Code locates) | Modify (remove `IsStageEndNode = false` setter code) |
| T22 | EndingTriggerKind.cs | `Assets/_Game/Features/Ending/MasterData/` | **Delete** (perform last, after all references are removed) |

**Unchanged files (reference):**

- `EndingType.cs`, `EndingDialogue.cs`, `EndingCondition.cs` data classes (for T4, Claude Code verifies whether the evaluation code lives in EndingCondition.cs; if it lives elsewhere, modify that location instead)
- `EndingUseCase.cs`, `PendingEndingContext.cs`, `RunSummaryData.cs`
- `EndingScene` entire Presentation layer (`EndingPresenter.cs`, `EndingSceneBootstrapper.cs`, `EndingView.cs`, all sub-Views)
- `SceneKey.cs`, `SceneNavigator.cs`, `CharacterAccountData.cs`
- `StageRunData.cs`, `StageSceneUseCase.cs`
- `EventResultType.cs`, `EventOriginKind.cs`
- `StageNodeSO.cs`, `EventNodeDataSO.cs`

---

### Implementation Tasks

#### Task 0: decisions.md Reset (§2)

**Target file:** `.claude/specs/features/ending-scene/decisions.md`

**Work:**

- Empty the file entirely (remove v1.0.0~v1.2.0 content, D-01~D-12).
- Keep the file empty. During v2.0.0 implementation, start recording from scratch whenever a decision outside the Spec is required.
- Record format follows the previous convention: attach one of `[DECISION]`, `[BACKLOG]`, `[SPEC-GAP]` tags per entry.

**Background:**

- v1.0.0 Decisions were judgments for v1.0.0 + Patch-001~003 implementation (D-06/D-07 for Patch-001 Resolver matching logic, D-08~D-12 for Patch-003 StagePresenter branching), which diverge from v2.0.0 redesign outcomes.
- v1.x content is already preserved in the Notion Decisions page up to v1.2.0 and will remain accessible via the Archive under the Changelog, so recoverability is guaranteed.

---

#### Task 1: Create EndingCandidateSlot.cs (§1, §4, §8)

**Path:** `Assets/_Game/Features/Ending/MasterData/EndingCandidateSlot.cs`

**Work:**

- Create as a new `[Serializable]` class. NOT a ScriptableObject.
- namespace: `Samsara.Features.Ending.MasterData` (follows existing MasterData namespace convention)

**Fields (private, [SerializeField]):**

- `_candidates: EndingSO[]` — conditional candidate ending array. Each ending carries its own Conditions and Priority.
- `_fallback: EndingSO` — default ending selected when no candidate passes condition matching. `null` allowed.

**Public properties (read-only):**

- `Candidates` → `EndingSO[]` (`_candidates`)
- `Fallback` → `EndingSO` (`_fallback`)
- `IsEmpty` → `bool` — true when `_candidates == null || _candidates.Length == 0` AND `_fallback == null`

**Design principle compliance:**

- §4 Clean Architecture: pure data class. Logic (matching algorithm) lives in EndingResolver. Do NOT add Evaluate/Resolve-like methods here.
- §8 Naming: `_camelCase` fields, `PascalCase` properties.
- Plan §4-1 RQ-P01: implemented as class (not struct). Aligns with Unity SerializeField convention and natural composition with nested SO fields.

**Inspector exposure verification:**

- Since `BattleNodeDataSO`'s `_victoryEndings`/`_defeatEndings` fields and `EventSO.EventResult`'s `_endingSlot` field all declare this type with `[SerializeField]`, it must be editable in Inspector as dropdown + array.

---

#### Task 2: Modify EndingSO.cs (§2, §4, §8)

**Path:** `Assets/_Game/Features/Ending/MasterData/EndingSO.cs`

**Work:**

**Fields to remove:**

- Remove `_triggerKind: EndingTriggerKind` field entirely (declaration, SerializeField, and the associated public `TriggerKind` property)

**Fields to keep (leave untouched):**

- `_id`, `_categories` (EndingType [Flags]), `_isGameOver`, `_title`
- `_dialogues` (EndingDialogue[])
- `_backgroundSpriteKeys` (string[])
- `_resultText`
- `_unlocksMainBgKey`, `_unlocksMainBgmKey`
- `_conditions` (EndingCondition[])
- `_priority` (int)

**Properties to keep:** All public read-only properties corresponding to the above fields remain. Only `TriggerKind` property is removed.

**Impact scope:**

- After this change, every remaining `EndingSO._triggerKind` reference triggers compile errors → removed incrementally by subsequent Tasks.

---

#### Task 3: Modify EndingConditionType.cs (§2)

**Path:** `Assets/_Game/Features/Ending/MasterData/EndingConditionType.cs`

**Work:**

**Enum values to remove:**

- `StageCompleteFlag`

**Enum values to keep:**

- `None`, `EvolutionId`, `EventId`, `EventResultType`

**Extensibility principle:**

- Open/Closed Principle compliance: when a new enum value is added, only the single evaluation switch block needs extending. This file contains only the pure enum definition.

**Impact scope:**

- After this change, any switch case code handling `EndingConditionType.StageCompleteFlag` triggers compile errors → handled in Task 4.

---

#### Task 4: Remove StageCompleteFlag switch evaluation (§2, §4)

**Target file:** Claude Code locates by search.

**Search method:**

- Global search: `EndingConditionType.StageCompleteFlag` or `case StageCompleteFlag` or `StageCompleteFlag =>`
- Identify which file contains it, then remove the case from that file.

**Work:**

- Remove the `StageCompleteFlag` case from the switch block.
- Leave all other cases untouched.

**Compliance principle (Clean Architecture verification):**

- §4 Data/Logic separation: If the Condition data class (`EndingCondition.cs`) contains evaluation logic, that violates the principle. Data classes should hold only fields/properties; evaluation belongs to Resolver or a dedicated evaluator.
- If the current code violates this principle:
    1. Record in `decisions.md` with `[SPEC-GAP]` tag. Example format: `[SPEC-GAP] EndingCondition.cs contains evaluation logic. Clean Architecture violation.`
    2. Do NOT attempt the refactoring (moving evaluation logic to Resolver) yourself (to prevent v2.0.0 redesign scope creep). Record only that the refactoring is queued for a separate Patch.
    3. For this task, only remove the `StageCompleteFlag` case at its current location.

**Extensibility principle:**

- Preserve the switch block structure. Adding a new EndingConditionType value should require only adding a single case line.

---

#### Task 5: Modify EndingContext.cs (§4, §8)

**Path:** `Assets/_Game/Features/Ending/Domain/EndingContext.cs`

**Work:**

**Fields to remove:**

- Remove `IsStageEndNode: bool` field entirely

**Fields to keep:**

- `BattleResult?` (nullable)
- `int? EventId`
- `EventResultType? EventResultType`

**Type preservation:**

- Keep as struct (Plan §4-5 RQ-P02: runtime matching single-use data, GC pressure minimization)

**Impact scope:**

- Code that reads from `PendingEventContext.IsStageEndNode` to populate `EndingContext` triggers compile errors → handled in Task 18 (StagePresenter).

---

#### Task 6: Modify IEndingResolver.cs signature (§4, §10)

**Path:** `Assets/_Game/Features/Ending/Domain/IEndingResolver.cs`

**Work:**

**Existing signature to remove:**

- Remove `int? TryResolve(EndingTriggerKind triggerKind, EndingContext context)` (or Patch-004-era signature) entirely.

**New signature to add:**

- `int? TryResolve(EndingCandidateSlot slot, EndingContext context)`
- Return: matched EndingSO's Id (nullable int). null means no match (run continues).

**using cleanup:**

- Remove `EndingTriggerKind` reference and any using statements that become unused.
- Add `EndingCandidateSlot` using if not in the same namespace.

---

#### Task 7: Modify EndingResolver.cs (§4, §8)

**Path:** `Assets/_Game/Features/Ending/Domain/EndingResolver.cs`

**Work:**

**Constructor dependency changes:**

- **Remove:** `IEndingMasterDataRepository` parameter (Resolver no longer iterates the global repository — Plan §5-1 RQ-P03)
- **Keep:** `ICharacterRunRepository`, `ICharacterAccountRepository`

**Example ctor signature:**

    public EndingResolver(
        ICharacterRunRepository characterRunRepo,
        ICharacterAccountRepository characterAccountRepo)

**TryResolve method reimplementation:**

In order:

1. `slot == null` or `slot.IsEmpty` → `return null;`
2. Matching iteration (**single foreach pass, no List/LINQ** — §8 GC optimization, Decisions D-07 pattern inherited):
    - Declare local variables: `EndingSO matchedEnding = null;`, `int matchedPriority = int.MinValue;`
    - `foreach (var ending in slot.Candidates)`:
        - skip if ending is null (continue)
        - Evaluate all Conditions as AND (every Condition must pass for the ending to match)
        - On pass + `ending.Priority > matchedPriority` → `matchedEnding = ending; matchedPriority = ending.Priority;`
        - (Earlier array index wins on equal Priority, achieved automatically via `>` comparison)
3. `matchedEnding != null` → `return matchedEnding.Id;`
4. `matchedEnding == null && slot.Fallback != null` → **no condition check** → `return slot.Fallback.Id;`
5. Neither → `return null;`

**Conditions AND evaluation (internal private method or inline):**

For each Condition:

| EndingConditionType | Evaluation |
|---|---|
| `None` | always true |
| `EvolutionId` | `_characterRunRepo.GetRunData().EvolutionNodeId == condition.StringValue` |
| `EventId` | `context.EventId.HasValue && context.EventId.Value == condition.IntValue` |
| `EventResultType` | `context.EventResultType.HasValue && (int)context.EventResultType.Value == condition.IntValue` |

- If any Condition evaluates false, the whole expression is false (AND). May exit early via return/break.
- Empty Conditions array is treated as unconditional pass (Specify §4-3).

**Coding rules (§8):**

- Keep `_logClass = $"[{nameof(EndingResolver)}]";` field
- LINQ (`Where`, `Select`, `OrderBy`, `FirstOrDefault`, etc.) forbidden
- No `new List<>` allocation. Do not create temporary collections during matching.
- Log one line each for match failure/success (debugging aid).

**using cleanup:**

- Remove `EndingTriggerKind` reference
- Remove `IEndingMasterDataRepository` using if not used elsewhere

---

#### Task 8: Modify IEndingMasterDataRepository.cs (§4, §10)

**Path:** `Assets/_Game/Features/Ending/Domain/IEndingMasterDataRepository.cs`

**Work:**

**Method to remove:**

- Remove `GetEndingsByTriggerKind(EndingTriggerKind triggerKind) → EndingSO[]` entirely

**Methods to keep:**

- `EndingSO GetEnding(int id)` — lookup EndingSO by PendingEndingContext.EndingId (used by EndingPresenter)
- `EndingSO[] GetAllEndings()` — retained for future codex/statistics features

**using cleanup:**

- Remove `EndingTriggerKind`-related using statements

---

#### Task 9: Modify EndingMasterDataRepository.cs (§4, §9, §10)

**Path:** `Assets/_Game/Features/Ending/Data/EndingMasterDataRepository.cs`

**Work:**

**Method to remove:**

- Remove `GetEndingsByTriggerKind(EndingTriggerKind)` implementation entirely

**Structure to keep:**

- `Dictionary<int, EndingSO>` cache
- `Resources.LoadAll<EndingSO>("MasterData/Ending")` loading in constructor
- `GetEnding(int id)`, `GetAllEndings()` methods
- `_logClass` field

**using cleanup:**

- Remove `EndingTriggerKind`-related using statements

---

#### Task 10: Modify IEndingEntryService.cs signature (§5, §10)

**Path:** `Assets/_Game/Features/Ending/Domain/IEndingEntryService.cs`

**Work:**

**Existing signature to remove:**

- Remove `UniTask<bool> TryEnterEndingAsync(EndingTriggerKind triggerKind, EndingContext context)` (or Patch-004-era signature) entirely

**New signature to add:**

- `UniTask<bool> TryEnterEndingAsync(EndingCandidateSlot slot, EndingContext context)`
- Return: true → ending match success + scene transition initiated (caller does no further work). false → match failure (caller continues existing flow).

**using cleanup:**

- Remove `EndingTriggerKind` using
- Add `EndingCandidateSlot` using (if different namespace)
- Keep UniTask using (`using Cysharp.Threading.Tasks;`)

---

#### Task 11: Modify EndingEntryService.cs (§4, §5, §8)

**Path:** `Assets/_Game/Features/Ending/Domain/EndingEntryService.cs`

**Work:**

**Constructor dependencies:**

- **No change.** Keep existing ctor parameters (`IEndingResolver`, `ICharacterRunRepository`, `IStageRepository`, `EvolutionNodeSO[]`, `ISceneNavigator`, `GameContext`) as-is.

**TryEnterEndingAsync method reimplementation:**

In order:

1. `int? endingId = _endingResolver.TryResolve(slot, context);`
2. `endingId == null` → `return false;`
3. `endingId.HasValue` case:
    - **Build RunSummaryData** (inherit existing Decisions D-10 pattern):
        - `TotalDays` ← `_characterRunRepo.GetRunData().Day`
        - `FinalEvolutionName` ← iterate `EvolutionNodes` array with foreach, find the EvolutionNodeSO whose ID matches `CharacterRunData.EvolutionNodeId` (foreach, no LINQ)
        - `StagesCleared` ← `_stageRepo.GetRunData().ClearedStageCount`
        - `FinalGold` ← `_characterRunRepo.GetRunData().Gold`
    - `_gameContext.PendingEndingContext = new PendingEndingContext { EndingId = endingId.Value, RunSummary = runSummary };`
    - `await _sceneNavigator.NavigateToAsync(SceneKey.Ending);`
    - `return true;`

**Coding rules (§8):**

- Keep `_logClass` field
- No LINQ, use foreach
- One log line each for match success/failure

**using cleanup:**

- Remove `EndingTriggerKind`-related using statements

---

#### Task 12: Modify BattleNodeDataSO.cs (§1, §2, §8)

**Path:** `Assets/_Game/Features/Stage/MasterData/BattleNodeDataSO.cs`

**Work:**

**Fields to add (private, [SerializeField]):**

- `_victoryEndings: EndingCandidateSlot` — candidate slot attempted for matching on battle victory
- `_defeatEndings: EndingCandidateSlot` — candidate slot attempted for matching on battle defeat

**Public properties to add (read-only):**

- `VictoryEndings` → `EndingCandidateSlot` (`_victoryEndings`)
- `DefeatEndings` → `EndingCandidateSlot` (`_defeatEndings`)

**Fields to keep:**

- `_enemySpawns: EnemySpawn[]` — enemy composition (leave as-is)
- All other existing fields kept

**Fields to remove (if present):**

- If a boss-flag-style field exists, remove it (Specify §4-5: boss-ness is expressed structurally by "which ending is placed in which slot"). **However, Claude Code must first read the file to verify whether such a field actually exists.** If not present, skip this removal step.

**using cleanup:**

- Add `EndingCandidateSlot` namespace using (`using Samsara.Features.Ending.MasterData;`)

**Inspector perspective:**

- Both slots must accept a default of empty slot (`_candidates = empty, _fallback = null`). Not every battle node ends in an ending.
- Hak must be able to set candidate endings per-node directly in the Inspector.

---

#### Task 13: Modify EventSO.cs — nested EventResult class (§1, §8)

**Path:** `Assets/_Game/Features/Event/MasterData/EventSO.cs`

**Target:** the `EventResult` class (`[Serializable]` class) nested inside the file

**Field to add (private, [SerializeField]):**

- `_endingSlot: EndingCandidateSlot` — candidate slot attempted for matching when this result is applied

**Public property to add (read-only):**

- `EndingSlot` → `EndingCandidateSlot` (`_endingSlot`)

**Fields to keep (inside EventResult, leave as-is):**

- `_resultType`, `_value`, `_useStatType`, `_statType`, `_hasMerchantId`, `_merchantId`
- All other existing fields kept

**Structure to keep (EventSO itself):**

- The outer EventSO class's field composition is unchanged. Only the nested EventResult class is modified.

**Empty-slot determination principle:**

- EventPresenter checks `eventResult.EndingSlot.IsEmpty` to decide "skip ending attempt". When both Candidates and Fallback are empty in Inspector, the ending matching step is skipped entirely (Specify §4-5).

**using cleanup:**

- Add `EndingCandidateSlot` namespace using

---

#### Task 14: Modify PendingEventContext.cs (§4, §8)

**Path:** `Assets/_Game/Features/Event/Domain/PendingEventContext.cs`

**Work:**

**Field to remove:**

- Remove `IsStageEndNode: bool` field entirely (declaration + get/set + any constructor parameters or initialization code using it)

**Fields to keep:**

- `EventId`, `ReturnScene`, `BackgroundSpriteKey`, `IsReturningFromBattle`, `IsCompleted`, `Origin` (EventOriginKind)

**Impact scope:**

- Code setting `PendingEventContext.IsStageEndNode` (in StagePresenter, MaintenanceScene-related code) triggers compile errors → handled in Tasks 18, 21.

---

#### Task 15: Modify NodeCompletionContext.cs (§4, §8)

**Path:** `Assets/_Game/Features/Stage/Domain/NodeCompletionContext.cs`

**Work:**

**Field to remove:**

- Remove `IsStageEndNode: bool` field entirely

**Fields to keep:**

- `NodeIndex: int`
- `NodeType: NodeType` (or existing type)

**Design principle (§4 Clean Architecture):**

- NodeCompletionContext shrinks to a **pure DTO (Data Transfer Object)**. It carries only event metadata (which node completed) — NOT pre-computed judgment results like "is this node a stage end node".
- Judgment is performed by StageProgressService directly via IStageSceneUseCase (see Task 16).

**Impact scope:**

- Callers (e.g., StagePresenter) that construct NodeCompletionContext with `IsStageEndNode = ...` must remove that setter code → handled in Task 18.

---

#### Task 16: Modify StageProgressService.cs (§4, §8)

**Path:** `Assets/_Game/Features/Stage/Domain/StageProgressService.cs`

**Work:**

**Constructor dependency verification:**

- Claude Code first checks current ctor parameters.
- Verify whether `IStageSceneUseCase` (or an equivalent interface/class exposed by StageSceneUseCase) is injected in the ctor.
- **If not injected, add it.** The IStageSceneUseCase reference is needed to call `IsStageComplete(nodeIndex)`.
- Also update the StageProgressService creation site inside `GameContext`'s constructor (see Task 20).

**CompleteNodeAsync method reimplementation:**

Remove the existing logic that branched on `context.IsStageEndNode` reads, and switch to direct judgment.

In order:

1. `bool isEndNode = _stageSceneUseCase.IsStageComplete(context.NodeIndex);`
2. `if (isEndNode)`:
    - `_stageSceneUseCase.IncrementClearedStageCount();`
    - `_stageSceneUseCase.SelectNextStage(...);` (keep existing logic; Claude Code verifies the signature)
    - Keep other end-node handling logic
3. `else`:
    - `_stageSceneUseCase.MoveToNode(context.NodeIndex + 1);` (keep existing logic)
    - Keep other regular-node handling logic

**Design principle (§4 Clean Architecture):**

- **Data/judgment responsibility split:** Context carries only "which node completed" as pure data. The judgment "is that node a stage end node" is owned by the service (UseCase) that holds the judgment information.
- Having the caller (StagePresenter) pre-compute the judgment and pass it via Context leaks responsibility across the Clean Architecture boundary.

**Coding rules (§8):**

- Keep `_logClass` field
- No LINQ/new allocation (even outside Update, minimize GC in service methods)

**using cleanup:**

- Add `IStageSceneUseCase`-related using if necessary

---

#### Task 17: Verify IStageProgressService.cs (§4, §10)

**Path:** `Assets/_Game/Features/Stage/Domain/IStageProgressService.cs`

**Work:**

- Claude Code first reads the current interface signatures.
- The `CompleteNodeAsync(NodeCompletionContext)` signature itself is **not changed** (parameter type remains NodeCompletionContext; only NodeCompletionContext's internal fields change per Task 15).
- Verify whether the interface method list needs any additions/removals. If not, no file modification required.

**Verification result reporting:**

- No separate decisions.md entry needed (if no change).
- If interface changes are found to be needed, record with `[DECISION]` tag and rationale.

---

#### Task 18: Modify StagePresenter.cs (§4, §5, §8)

**Path:** `Assets/_Game/Features/StageScene/Presentation/StagePresenter.cs`

**Work 1 — Restructure `HandleBattleResultIfAny` method:**

**Existing logic (Patch-004 era):**

- Determined Victory/Defeat from TriggerKind → called Resolver globally → called StageProgressService on match failure

**v2.0.0 logic:**

In order:

1. Look up current battle node: `BattleNodeDataSO battleNode = ...;` (keep existing approach; path verified by Claude Code)
2. Select slot based on battle result:
    - Victory → `EndingCandidateSlot slot = battleNode.VictoryEndings;`
    - Defeat → `EndingCandidateSlot slot = battleNode.DefeatEndings;`
3. Build EndingContext:
    - `BattleResult = Victory or Defeat` (keep existing BattleResult type)
    - `EventId = null`
    - `EventResultType = null`
4. `bool entered = await _gameContext.EndingEntryService.TryEnterEndingAsync(slot, context);`
5. `entered == true` → return (scene transition already initiated)
6. `entered == false` branch:
    - **Victory branch:** `NodeCompletionContext nodeContext = new NodeCompletionContext { NodeIndex = currentNodeIndex, NodeType = NodeType.Battle };` → `await _gameContext.StageProgressService.CompleteNodeAsync(nodeContext);`
    - **Defeat branch:** **Output warning log only, stay on current scene.**
        - Example: `Debug.LogWarning($"{_logClass} Battle defeat matched no ending and no Fallback. Designer should review defeat slot for this node. NodeIndex={currentNodeIndex}");`
        - No scene transition, no RunData reset, no hardcoded return policy
        - Reason: Design data verification is the designer/developer's responsibility. Hardcoding a safety net in code hides missing data (Plan §9 RQ-P06)

**Work 2 — Minor adjustment to `HandleEventResultIfAny` method:**

- Keep `PendingEventContext.Origin == EventOriginKind.StageNode` check logic
- When constructing NodeCompletionContext, **only remove the `IsStageEndNode = ...` setter line** (the field itself is gone, so the line would trigger a compile error)
- Keep remaining logic

**Work 3 — Adjust PendingEventContext construction code when entering event nodes:**

- Claude Code locates the site in StagePresenter where PendingEventContext is created/populated on event-node entry.
- Remove `IsStageEndNode = ...` calculation and setter line (field removed).
- **Keep** `Origin = EventOriginKind.StageNode` setter (EventPresenter uses this for return-handling branch).

**Coding rules (§8):**

- Keep `_logClass`
- Warning log message should include debugging-helpful context (NodeIndex, etc.)

**ctor dependencies:**

- No change. Whole-GameContext injection pattern retained (Decisions D-11).

**using cleanup:**

- Add/keep `EndingCandidateSlot`, `EndingContext` using as needed

---

#### Task 19: Modify EventPresenter.cs (§4, §5, §8)

**Path:** `Assets/_Game/Features/Event/Presentation/EventPresenter.cs`

**Background — Node action model (Plan §6-3):**

Samsara's node flow is "node movement → node action → return to Stage → player selects next action". In an event node, "node action" is event progression (choices → result application), and upon completion, control returns to Stage by default. Endings are a **selective termination branch** of a node action; matching is attempted only when the designer has set ending candidates on a specific EventResult.

**Work — Restructure `HandlePostResultAsync` method:**

In order:

1. **Keep result application logic** (HpChange/StatChange/ShopEncounter/Battle branches as-is)
2. **Ending matching branch (node action termination handling):**
    - `EndingCandidateSlot slot = currentEventResult.EndingSlot;`
    - `if (slot.IsEmpty)` → skip ending matching step, proceed directly to step 3 (ReturnScene)
    - `else`:
        - Build EndingContext:
            - `BattleResult = null`
            - `EventId = _pendingEventContext.EventId`
            - `EventResultType = currentEventResult.ResultType`
        - `bool entered = await _gameContext.EndingEntryService.TryEnterEndingAsync(slot, context);`
        - `entered == true` → return (no Stage return; ending scene transition ends the run)
        - `entered == false` → proceed to step 3 (ReturnScene)
3. **ReturnScene return (common to ending skip or match failure):**
    - `_pendingEventContext.IsCompleted = true;`
    - `await _sceneNavigator.NavigateToAsync(_pendingEventContext.ReturnScene);`

**Responsibility boundary preservation (Plan §6-3):**

- EventPresenter does **NOT call StageProgressService directly**.
- Node completion is performed by the ReturnScene's Presenter (StagePresenter) on re-entry via `Origin` check (see Task 18 Work 2).
- This boundary must be preserved because "events can originate outside Stage (e.g., maintenance exploration)". Calling StageProgressService from EventPresenter violates the Feature boundary.

**Coding rules (§8):**

- Keep `_logClass`
- One log line each for ending matching success/failure/skip

**ctor dependencies:**

- No change

**using cleanup:**

- Add/keep `EndingCandidateSlot`, `EndingContext` using

---

#### Task 20: Modify GameContext.cs (§6)

**Path:** `Assets/_Game/App/GameContext.cs`

**Work:**

**Accessor composition unchanged:**

- Keep all existing public properties:
    - `EndingMasterDataRepo`, `EndingUseCase`, `EndingResolver`, `EndingEntryService`
    - `StageProgressService`
    - `PendingEndingContext`, `EvolutionNodes`

**Constructor internal adjustments:**

1. **EndingResolver constructor call adjustment (linked to Task 7):**
    - Before: `new EndingResolver(characterRunRepo, characterAccountRepo, endingMasterDataRepo)` (or Patch-004-era signature)
    - After: `new EndingResolver(characterRunRepo, characterAccountRepo)` — `IEndingMasterDataRepository` argument removed
2. **StageProgressService constructor call adjustment (linked to Task 16):**
    - If Task 16 added `IStageSceneUseCase` injection to StageProgressService, reflect it in GameContext.
    - Claude Code verifies the existing StageProgressService ctor call site and passes the required dependency.
    - Verify whether the injection target instance (IStageSceneUseCase) already exists inside GameContext. If not, creation order must be adjusted (so `StageSceneUseCase` is created first, then StageProgressService).

**Other Features' accessors:**

- No change

**using cleanup:**

- Remove `EndingTriggerKind`-related using (if referenced here)

---

#### Task 21: Remove IsStageEndNode setter code in MaintenanceScene (§4)

**Target file:** Claude Code locates by search.

**Search method:**

- Global search: `IsStageEndNode` setter lines under Features/Maintenance/ or related paths
- Sites in MaintenanceScene where PendingEventContext is created/populated when invoking exploration events

**Work:**

- Remove `IsStageEndNode = false;` (or equivalent setter) line
- **Keep** `Origin = EventOriginKind.MaintenanceExploration` setter (Origin-based return branching introduced in Patch-004)
- Keep other PendingEventContext construction logic as-is

**Compliance principle (§4 Clean Architecture):**

- MaintenanceScene is not part of Stage, so the "stage end node" concept never fit cleanly in the first place. v1.0.0~Patch-004 explicitly passed `IsStageEndNode = false` for compatibility, but since the field is removed in v2.0.0, this is resolved naturally.

---

#### Task 22: Delete EndingTriggerKind.cs file (§2)

**Path:** `Assets/_Game/Features/Ending/MasterData/EndingTriggerKind.cs`

**Work:**

**Precondition check (mandatory before deletion):**

- Claude Code confirms via global search that `EndingTriggerKind` references in the project are **zero**.
- If any reference remains, fix that site first, then re-search. Proceed with deletion only after 0-reference state is achieved.

**Deletion targets:**

- File body: `EndingTriggerKind.cs`
- Unity meta file: `EndingTriggerKind.cs.meta` (should be auto-deleted when the file is deleted. If the file was removed directly from the filesystem rather than through Unity Editor, manual cleanup may be required)

**Post-deletion verification:**

- Confirm project recompile succeeds
- Confirm no MissingReference or ScriptNotFound errors in Unity Editor console

**Why this Task is last:**

- Tasks are ordered so that the project stays partially compilable during Claude Code's sequential execution. Compile errors arise mid-process while `EndingTriggerKind` references still exist, but as each Task completes, related references are incrementally removed, allowing safe deletion at the end.
- Deletion is only safe after all references are removed (Task 2: EndingSO, Task 6~7: Resolver, Task 8~9: Repository, Task 10~11: EntryService, Task 20: GameContext).

---

### Validation

Claude Code verifies the following items sequentially after implementation completion.

| # | Item | Verification Method |
|---|---|---|
| V-01 | No compile errors in Unity console | Console check |
| V-02 | `EndingTriggerKind` references: 0 project-wide | Project-wide global search |
| V-03 | `StageCompleteFlag` references: 0 project-wide | Project-wide global search |
| V-04 | `PendingEventContext.IsStageEndNode` references: 0 project-wide | Project-wide global search |
| V-05 | `NodeCompletionContext.IsStageEndNode` references: 0 project-wide | Project-wide global search |
| V-06 | `EndingTriggerKind.cs` file and `.meta` file deleted | Filesystem check |
| V-07 | `EndingCandidateSlot.cs` file created (`Features/Ending/MasterData/`) | Filesystem check |
| V-08 | `BattleNodeDataSO` Inspector exposes VictoryEndings and DefeatEndings (2 slots) | Inspector check |
| V-09 | `EventSO.EventResult` Inspector exposes EndingSlot | Inspector check |
| V-10 | `GameContext` initialization: EndingResolver ctor has no IEndingMasterDataRepository argument | File check |
| V-11 | `StageProgressService` ctor has IStageSceneUseCase injected (if applicable) | File check |
| V-12 | decisions.md is in v2.0.0 fresh-record state (v1.x content removed) | File check |
| V-13 | EndingScene entire Presentation layer unchanged (identical to v1.0.0) | File comparison |

---

### Runtime Validation (performed after Hak's manual work)

After manual work (M-01~M-04) completion, verify the following scenarios sequentially in Play mode.

| # | Scenario | Expected Behavior |
|---|---|---|
| R-01 | Normal battle victory (VictoryEndings slot empty) | Ending match fails → StageProgressService.CompleteNodeAsync called → move to next node (normal progression) |
| R-02 | Boss battle victory (VictoryEndings slot populated with boss ending, end node) | Ending match success → EndingScene transition → story playback → result summary popup → restart |
| R-03 | Boss battle victory but ending condition unmet (no Fallback set) | Match fails → CompleteNodeAsync called → end-node judgment → ClearedStageCount++ → next stage selected |
| R-04 | Battle defeat (DefeatEndings slot with Fallback set) | Fallback ending match success → EndingScene transition (game-over ending) |
| R-05 | Battle defeat (DefeatEndings slot completely empty) | Match fails → **warning log only, stay on current scene** (no scene transition). Confirm warning in console. |
| R-06 | Event complete (EventResult.EndingSlot completely empty) | Ending matching skipped → Stage return (existing flow) → StageNode re-entry triggers Origin check → CompleteNodeAsync called |
| R-07 | Event complete (EndingSlot populated, condition matches) | Ending match success → EndingScene transition (no Stage return) |
| R-08 | Event complete (EndingSlot populated, condition mismatch, no Fallback) | Match fails → Stage return (existing flow) |
| R-09 | Event complete (Fallback set, condition mismatch) | Fallback ending match → EndingScene transition |
| R-10 | Maintenance-stage exploration event complete (EndingSlot empty) | Ending matching skipped → Maintenance scene return (Origin == MaintenanceExploration preserved) |

---

### Hak's Manual Work (after Claude Code implementation completion)

After Claude Code completes the code implementation, Hak performs the following directly in Unity Editor. (Plan §11 reflection)

| # | Work | Target | Notes |
|---|---|---|---|
| M-01 | Reconfigure 3 existing EndingSO .asset files | `Assets/Resources/MasterData/Ending/Ending_Battle_Defeat_001.asset`, `Ending_Battle_Victory_001.asset`, `Ending_Event_001.asset` | Verify `_triggerKind` field auto-removed. If necessary, reconfigure Conditions (if any Condition used StageCompleteFlag type, replace with another condition or remove) |
| M-02 | Configure ending slots on 2 existing BattleNodeDataSO .asset files | `Assets/Resources/MasterData/Stage/BattleNodeData_Boss_Test.asset`, `BattleNodeData_Test.asset` | Inspector newly exposes `_victoryEndings`, `_defeatEndings`. **Boss_Test**: recommend setting at least 1 boss victory ending Candidate or Fallback in Victory slot. **Test**: empty slots can remain as default (regular battle, no ending applied) |
| M-03 | Configure ending slot on existing EventSO .asset | `Assets/Resources/MasterData/Event/Event_Test_Ending_Death.asset` (priority target) | Configure `_endingSlot` on that event's EventResult (ending candidate or Fallback). Events unrelated to endings (`Event_00`, `Event_Chain_01`, etc.) can leave empty slots |
| M-04 | Compile and runtime verification | — | Sequential verification of Validation V-01~V-13 and Runtime Validation R-01~R-10 |

**Work Claude Code cannot perform:**

- Setting Inspector field values on .asset files (SO instance data is editor-only)
- Runtime Play-mode testing

---

### Claude Code Handoff Guide

**Handoff method:**

- Run `claude` from project root
- `CLAUDE.md` auto-loads
- Hand off `.claude/specs/features/ending-scene/tasks.md` (Tasks-MD v2.0.0)
- Make `.claude/specs/features/ending-scene/specify.md` and `plan.md` accessible for reference

**Implementation principles:**

- Execute Tasks sequentially from Task 0 to Task 22. **No reordering** (designed by dependency).
- For "locate" instructions (T4, T21), find the file via global search before working on it.
- Adhere to each Task's compliance principles (Clean Architecture, Open/Closed, §8 GC optimization, etc.).
- When a decision outside the Spec is required mid-task, **record it in decisions.md with a tag**.
    - `[DECISION]`: code-level judgment
    - `[BACKLOG]`: temporary handling, real implementation required later
    - `[SPEC-GAP]`: definition missing from Spec
- After recording, continue that Task's implementation (do NOT wait for Hak's approval).

**Coding rules:**

- No file creation outside `Assets/_Game/` (decisions.md exception)
- Use all paths exactly as specified in Tasks
- Constitution §2 Zero Guessing: **never guess** file paths/class names/API signatures; **read existing files first** to verify
- Uphold Constitution §8 log tag, _camelCase, SafeCleanup, no-LINQ rules

**External Feature Patch documents:**

- Claude Code **receives only the EndingScene Tasks-MD v2.0.0 document** and works from it.
- External Patch documents for Stage / StageScene / Event Features are issued separately by Claude Web, but they are **reference-only** and Claude Code does not need to open them. All external Feature change instructions are integrated into this Tasks body (T12~T21).

**Post-implementation report items:**

- Full list of files created / modified / deleted
- decisions.md record summary (entry count per tag, key items)
- Self-verification results for Validation V-01~V-13
- Guidance for Hak's manual work (M-01~M-04), noting it has not yet been performed