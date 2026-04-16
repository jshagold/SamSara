# StageScene — Patch-003

**Status:** ✅ Confirmed (Implementation Pending)
**Feature:** StageScene (external Feature Patch — for EndingScene integration)
**Patch ID:** Patch-003
**Type:** spec-change
**Related Documents:** EndingScene Specify v1.0.0 §7-2, EndingScene Plan v1.0.0 §10-2, EndingScene Patch-001 v1.0.1 (depends on EndingResolver)
**Date:** 2026-04-16

## Version History

| Version | Date | Changes |
|---|---|---|
| v1.0.0 | 2026-04-16 | Initial version |

---

## Background

EndingScene and Patch-001 (EndingResolver) are ready, but the **battle result handling point (StagePresenter.HandleBattleResultIfAny)** — where the run actually terminates — has no code to enter EndingScene. Current logic stays in Stage regardless of Victory/Defeat, and boss victory is treated the same as regular battle victory.

Both battle defeat (HP 0) and boss battle victory are run termination events, so at this point EndingResolver must be used to determine the appropriate EndingSO and transition to EndingScene.

BattlePresenter must not be touched. The existing architecture has BattlePresenter store the result and return to Stage → StagePresenter handles the result. This structure is preserved.

---

## Changes

### 1. Modify Existing — StagePresenter

Path: Assets/_Game/Features/Stage/Presentation/StagePresenter.cs (Claude Code to confirm actual path)

**Target method:** HandleBattleResultIfAny() (or whichever method handles battle result)

**Change direction:**

Existing flow:
- `_gameContext.LastBattleResult` is Victory → node completion handling
- `_gameContext.LastBattleResult` is Defeat → stay at current position (log only)

Updated flow:
- On Victory:
    - If the completed node is a boss node (`BattleNodeData.IsBoss == true`) → **BossVictory ending entry handling**
    - If not a boss node → existing node completion handling (`_useCase.MoveToNode(...)`)
- On Defeat:
    - **BattleDefeat ending entry handling**
    - Do not stay at current position — transition to Ending scene

---

### Ending Entry Common Logic

Add a private helper method to StagePresenter (e.g., `EnterEndingSceneAsync(EndingType endingType)`) and call it from both Defeat and BossVictory branches.

1. Call `_gameContext.EndingResolver.Resolve(endingType)` → get `endingId`
2. Build RunSummaryData:
    - TotalDays: current Day (from CharacterRunData or StageRunData)
    - FinalEvolutionName: current evolution name (via CharacterRunData.EvolutionNodeId + EvolutionNode MasterData lookup)
    - StagesCleared: cleared stage count (via StageRunData.ClearedStageCount — see "2. StageRunData field addition" below)
    - FinalGold: current gold (from CharacterRunData)
3. `_gameContext.PendingEndingContext = new PendingEndingContext { EndingId = endingId, RunSummary = runSummary }`
4. `_gameContext.LastBattleResult = null` (mark as processed)
5. `_sceneNavigator.NavigateToAsync(SceneKey.Ending)` transition

> ⚠️ Exact field/method names must be verified by Claude Code against actual CharacterRunData, StageRunData, EvolutionNode MasterData before assembly.

---

### 2. SPEC-GAP — Add ClearedStageCount field to StageRunData

Path: Assets/_Game/Features/Stage/Data/StageRunData.cs (Claude Code to confirm actual path)

**Field to add:**
- `ClearedStageCount` (int, default 0) — number of stages cleared in the current run
- Maintain serialization compatibility (Newtonsoft.Json default value handling)

**Increment timing:**
- At the point where a stage is completed (currently determined by `_useCase.MoveToNode` or related logic). Claude Code must inspect the current StageUseCase to find an appropriate location to add `ClearedStageCount++` + dirty flag + save handling.

**Decisions record required:**
- Claude Code must record this field addition in decisions.md with `[SPEC-GAP]` tag
- StageRepository Specify reflection is deferred to a separate cleanup (Backlog follow-up)

---

### Dependency Addition

- StagePresenter's constructor DI needs access to `EndingResolver`
- If StagePresenter already holds GameContext, access via `_gameContext.EndingResolver` (no additional DI needed)
- If GameContext is not held, StageSceneBootstrapper should receive EndingResolver and pass it to StagePresenter's constructor

Claude Code must inspect the actual StagePresenter structure and add the dependency in the appropriate manner.

---

### DO NOT Modify

- BattlePresenter.cs — preserve existing structure (stores result then returns to Stage)
- BattleUseCase.cs
- BattleNodeDataSO.cs
- PendingBattleContext.cs
- All files in EndingScene Feature
- StageRepository.cs existing methods (however, adding ClearedStageCount increment logic is permitted)

---

## Claude Code Implementation Guide

**Files to read (read-only):**
- `Assets/_Game/Features/Stage/Presentation/StagePresenter.cs` — modification target. Verify HandleBattleResultIfAny structure.
- `Assets/_Game/Features/Stage/Presentation/StageSceneBootstrapper.cs` — verify dependency injection structure.
- `Assets/_Game/Features/Stage/Data/StageRunData.cs` — ClearedStageCount addition target.
- `Assets/_Game/Features/Stage/Data/StageRepository.cs` — verify save flow.
- `Assets/_Game/Features/Stage/Domain/StageUseCase.cs` — verify stage completion point (location for ClearedStageCount increment).
- `Assets/_Game/Features/Character/Data/CharacterRunData.cs` — verify Day, Gold, EvolutionNodeId fields.
- `Assets/_Game/Features/Character/MasterData/EvolutionNodeSO.cs` — verify evolution name field (evolution ID → name mapping).
- `Assets/_Game/Features/Ending/Domain/IEndingResolver.cs` — verify Resolve signature.
- `Assets/_Game/Features/Ending/Domain/PendingEndingContext.cs` — verify fields.
- `Assets/_Game/Features/Ending/Domain/RunSummaryData.cs` — verify fields.
- `Assets/_Game/Features/Ending/MasterData/EndingType.cs` — verify BattleDefeat, BossVictory entries.
- `Assets/_Game/App/GameContext.cs` — verify EndingResolver accessor.
- `Assets/_Game/Features/Stage/MasterData/BattleNodeDataSO.cs` — verify IsBoss access.
- `Assets/_Game/Core/Navigation/SceneKey.cs` — verify Ending entry.

**Files to modify:**
- `Assets/_Game/Features/Stage/Presentation/StagePresenter.cs`
- `Assets/_Game/Features/Stage/Data/StageRunData.cs`
- (If needed) `Assets/_Game/Features/Stage/Domain/StageUseCase.cs` — increment ClearedStageCount on stage completion
- (If needed) `Assets/_Game/Features/Stage/Presentation/StageSceneBootstrapper.cs` — expand dependency injection

**Implementation order:**
1. Read all pre-implementation files
2. Add ClearedStageCount field to StageRunData → record in decisions.md with [SPEC-GAP]
3. Increment ClearedStageCount + dirty flag + save at stage completion point
4. Add ending entry helper method to StagePresenter
5. Modify HandleBattleResultIfAny branching logic
6. If needed, expand Bootstrapper dependency injection path

**Judgment points (record in decisions.md):**
- Specific location to increment ClearedStageCount (which method in StageUseCase?)
- Evolution name lookup path (how to get name from EvolutionNodeSO — via CharacterRepository or GameContext)
- Dependency injection approach based on StagePresenter's GameContext ownership

**DO NOT:**
- Modify BattlePresenter
- Modify BattleScene Feature files
- Modify EndingScene Feature files (use Patch-001-implemented files as-is)
- Create files outside `Assets/_Game/` (except decisions.md)

**decisions.md rules:**
- Record any judgment calls not covered by this Patch in `.claude/specs/features/ending-scene/decisions.md` with appropriate tags: [DECISION], [BACKLOG], or [SPEC-GAP]
- ClearedStageCount field addition MUST be recorded with [SPEC-GAP] tag

---

## Validation

- [ ] No Unity compile errors
- [ ] Regular battle victory → existing node completion handling, stay in Stage (no ending entry)
- [ ] Boss battle victory → PendingEndingContext set + SceneKey.Ending transition. Resolver returns BossVictory type EndingSO.
- [ ] Battle defeat → PendingEndingContext set + SceneKey.Ending transition. Resolver returns BattleDefeat type EndingSO.
- [ ] RunSummaryData all fields populated (TotalDays, FinalEvolutionName, StagesCleared, FinalGold)
- [ ] StageRunData.ClearedStageCount increments + saves on stage completion
- [ ] Existing save files load with ClearedStageCount default 0 without issues
- [ ] LastBattleResult is reset to null after handling to prevent duplicate processing

---

## Manual Work (after Claude Code implementation)

None. Patch-001's Manual Work (EndingSO .asset creation) must be complete for actual playtest.