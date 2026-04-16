# EndingScene + EventSystem + StageScene — Patch-004

**Status:** ✅ Confirmed (Implementation Pending)
**Feature:** EndingScene + EventSystem + StageScene (cross-Feature patch — introduces ending trigger structure and node progression common service)
**Patch ID:** Patch-004
**Type:** spec-change
**Related Documents:** EndingScene Specify v1.0.0, Plan v1.0.0, Patch-001, Patch-002, Patch-003, Decisions D-01~D-12
**Date:** 2026-04-16

> Patches are not versioned. See Claude Project Instructions §3.

> **IMPORTANT: Patch-004 is a large reorganization patch.** Design flaws in Patch-002/003 (Patch-001 principle violation + intra-Feature asymmetry) were identified, and the ending trigger structure is being reorganized end-to-end. After this patch completes, parts of Patch-002/003 implementation are replaced/removed. Implementation order must be followed.

---

## Background

### Patch-001 Principle Integrity Issue

The EndingScene Specify v1.0.0 §3 and Patch-001 established the principle: **"Ending branching is resolved through MasterData condition matching."** However, Patch-002/Patch-003 design/implementation deviated:

- Patch-003 StagePresenter: `EndingEntryService.EnterEndingAsync(EndingType.BossVictory)` / `EndingType.BattleDefeat` hardcoded
- Patch-003 implementation: Boss judgment via `_useCase.IsStageComplete()`, not `BattleNodeDataSO.IsBoss` (spec-impl mismatch)
- In Patch-002 EventResult can specify `EndingType?` via MasterData while Battle side is hardcoded → **intra-Feature asymmetry broken**

### Hak Design Clarification

Correct ending definition:
- Ending = **"a run-termination event that fires when specific player conditions match at a specific trigger point set by the designer"**
- 1st dev trigger points: Battle result / Event result
- A single ending can have multiple category tags (Good/Bad etc.) simultaneously (Flags)
- When no ending matches, the run continues (existing flow)

### Stage Progression Structure Redefinition

- Stages are tree-shaped (inter-stage branching possible)
- Within a stage, nodes are tree-shaped (inter-node branching possible)
- Start node is singular, end nodes can be multiple (="no next node")
- "Stage clear" = "successfully completing an end node and escaping to the next stage"
- On boss victory, attempt ending match → if no match, process as Clear and move to next stage

### Need for StageProgressService Extraction

Current "node completion + end-node judgment + Clear increment" logic lives only in StagePresenter's Victory branch. Event completion returns are unhandled. Extract common service for symmetry + future expansion (maintenance exploration events etc.).

---

## Patch-001 Principle Integrity Verification

- Patch-001 principle: "Ending branching is resolved through MasterData condition matching"
- This patch **extends** the principle (no violation):
    - Existing: EndingType (type) + Conditions matching
    - New: **TriggerKind (trigger) filter + EndingContext-based Conditions matching + Priority**
    - EndingType is redefined as "ending character classification tag" (Flags)
- EndingResolver core logic (Conditions AND + Priority max, D-07 pattern) preserved
- "Ending entry points delegate condition judgment to MasterData" principle strengthened (StagePresenter's direct boss judgment removed)

---

## Changes

### 1. New — EndingTriggerKind (enum)

Path: Assets/_Game/Features/Ending/MasterData/EndingTriggerKind.cs

1st dev entries:
- BattleVictory — battle victory trigger
- BattleDefeat — battle defeat trigger
- EventResult — event result trigger

Future expansion: SpecialCondition (survive N turns, etc.)

### 2. New — EndingContext (struct)

Path: Assets/_Game/Features/Ending/Domain/EndingContext.cs

Runtime trigger information passed to the Resolver. Fields used selectively by trigger type.

- BattleResult? BattleResult
- int? EventId
- EventResultType? EventResultType
- bool IsStageEndNode — whether current node is a stage end node (used for StageCompleteFlag Condition evaluation)

### 3. Modify — EndingType (enum)

Path: Assets/_Game/Features/Ending/MasterData/EndingType.cs

- **Remove all existing entries** (BattleDefeat/EventDeath/BossVictory/EventEnding)
- Add [Flags] attribute
- 1st dev entries: None=0, Good=1, Bad=2
- Role: ending character/content classification tag (planning/UI/codex). Not used for logic matching.

### 4. Modify — EndingConditionType (enum)

Path: Assets/_Game/Features/Ending/MasterData/EndingConditionType.cs

**1st dev entries:**
- None (existing)
- EvolutionId (existing — uses StringValue)
- EventId (new — uses IntValue)
- EventResultType (new — uses IntValue, EventResultType enum stored as int)
- StageCompleteFlag (new — no value fields, directly reads EndingContext.IsStageEndNode)

### 5. Modify — EndingSO

Path: Assets/_Game/Features/Ending/MasterData/EndingSO.cs

- Existing _endingType (EndingType single value) → _categories (EndingType [Flags]) role redefined
- New field _triggerKind (EndingTriggerKind) — trigger point for this ending
- Existing fields preserved: _conditions, _priority, other presentation data
- Expose via public read-only properties

### 6. Modify — IEndingResolver / EndingResolver

Path: Assets/_Game/Features/Ending/Domain/IEndingResolver.cs, EndingResolver.cs

**Signature change:**
- Existing: `int Resolve(EndingType type)` — throws Exception on no match
- New: `int? TryResolve(EndingTriggerKind trigger, EndingContext context)` — returns null on no match

**Logic:**
1. `_endingMasterDataRepo.GetEndingsByTriggerKind(trigger)` to get candidates
2. Evaluate each EndingSO's Conditions against current player state + EndingContext (AND)
3. Select max Priority (D-07 pattern — single foreach pass)
4. Return null on no match

**Condition evaluation (1st dev):**
- None: always pass
- EvolutionId: CharacterRunData.EvolutionNodeId (string) == condition.StringValue
- EventId: EndingContext.EventId.HasValue && EndingContext.EventId.Value == condition.IntValue
- EventResultType: EndingContext.EventResultType.HasValue && (int)EndingContext.EventResultType.Value == condition.IntValue
- StageCompleteFlag: EndingContext.IsStageEndNode == true

### 7. Modify — EndingMasterDataRepository

Path: Assets/_Game/Features/Ending/Data/EndingMasterDataRepository.cs

- **Remove or replace** existing `GetEndingByType(EndingType)` method
- Add new `GetEndingsByTriggerKind(EndingTriggerKind) : IReadOnlyList<EndingSO>`
- Claude Code decides handling after verifying current structure

### 8. Modify — IEndingEntryService / EndingEntryService (Patch-002 implementation adjustment)

Path: Assets/_Game/Features/Ending/Domain/IEndingEntryService.cs, EndingEntryService.cs

**Signature change:**
- Existing (Patch-002): `UniTask EnterEndingAsync(EndingType endingType)`
- New: `UniTask<bool> TryEnterEndingAsync(EndingTriggerKind trigger, EndingContext context)` — returns true after entering ending on match, false on no match

**Logic:**
1. `_endingResolver.TryResolve(trigger, context)` → endingId?
2. null → return false (caller handles run continuation)
3. has value → assemble RunSummaryData + set PendingEndingContext + SceneKey.Ending transition + return true

RunSummaryData assembly logic (TotalDays, FinalEvolutionName, StagesCleared, FinalGold) unchanged from existing.

### 9. New — IStageProgressService / StageProgressService

Path: Assets/_Game/Features/Stage/Domain/IStageProgressService.cs, StageProgressService.cs

**Responsibility:** Unified node completion handling. Called by StagePresenter (including event return handling).

**Method:**
- `UniTask CompleteNodeAsync(NodeCompletionContext context)` — single method + context parameter (Q2 answer: C)

**Logic:**
1. Mark current node as completed
2. Check if current node is an end node (no next node)
3. If end node:
    - StageRunData.ClearedStageCount++
    - save
    - Move to next stage
4. If not end node:
    - Existing MoveToNode logic (move to next node)

**Dependencies:** `IStageSceneUseCase` or related UseCase, `IStageRepository`, other required dependencies.

**GameContext accessor added:** `IStageProgressService StageProgressService`

### 10. New — NodeCompletionContext (struct)

Path: Assets/_Game/Features/Stage/Domain/NodeCompletionContext.cs

- int NodeIndex — completed node index
- NodeType NodeType — Battle / Event / etc.
- Other necessary info (Claude Code adds after checking actual structure)

### 11. New — EventOriginKind (enum)

Path: Assets/_Game/Features/Event/Domain/EventOriginKind.cs (or appropriate location)

1st dev values:
- StageNode — occurred from stage node
- MaintenanceExploration — occurred from maintenance exploration

Supports future expansion.

### 12. Modify — PendingEventContext

Path: Assets/_Game/Features/Event/Domain/PendingEventContext.cs (Claude Code verifies actual path)

**New fields (Q1 answer: B+C):**
- bool IsStageEndNode — true if event occurred at a stage end node. Maintenance exploration events fixed to false.

**New fields (Q3 answer):**
- EventOriginKind Origin — event origin path (StageNode / MaintenanceExploration)

**Existing fields preserved:** IsCompleted, ReturnScene, etc.

**Caller-side changes required:**
- StagePresenter sets IsStageEndNode (whether current node is end node) + Origin=StageNode on PendingEventContext set
- Maintenance exploration sets IsStageEndNode=false + Origin=MaintenanceExploration on event call

### 13. Modify — EventPresenter (Patch-002 implementation adjustment)

Path: Assets/_Game/Features/Event/Presentation/EventPresenter.cs

**HandlePostResultAsync reorganization:**

1. Existing result-type handling (HpChange/StatChange/ShopEncounter/Battle) preserved — this is "result application" logic, not "completion + move" logic
2. **Remove** the `EventResultType.Ending` case added in Patch-002
3. Add common post-processing (after all result handling):
    - Create EndingContext:
        - EventId = current event ID
        - EventResultType = result.ResultType
        - IsStageEndNode = `_pendingEventContext.IsStageEndNode`
    - Call `_gameContext.EndingEntryService.TryEnterEndingAsync(EndingTriggerKind.EventResult, context)`
    - true → ending entered, terminate (no ReturnScene)
    - false → existing ReturnScene return:
        - Set `IsCompleted = true`
        - `_sceneNavigator.NavigateToAsync(_pendingEventContext.ReturnScene)`
        - **EventPresenter does NOT directly call StageProgressService** — it only returns regardless of Origin
        - Node completion is performed by the Presenter of ReturnScene when necessary

**Reason:** Events can be triggered from outside the stage; it would violate Feature boundaries for EventPresenter to directly call StageProgressService. StagePresenter checks Origin=StageNode on return and calls StageProgressService.

### 14. Modify — StagePresenter (Patch-002/003 implementation adjustment)

Path: Assets/_Game/Features/Stage/Presentation/StagePresenter.cs

**HandleBattleResultIfAny reorganization:**

Existing (after Patch-002):
- Victory + IsStageComplete → EnterEndingAsync(BossVictory)
- Victory + not complete → MoveToNode
- Defeat → EnterEndingAsync(BattleDefeat)

New:
- Victory case:
    1. Calculate current node index + end node status
    2. Create EndingContext { BattleResult=Victory, IsStageEndNode=calculated }
    3. Call TryEnterEndingAsync(BattleVictory, context)
    4. true → ending entered, terminate
    5. false → call `StageProgressService.CompleteNodeAsync(...)`
- Defeat case:
    1. Create EndingContext { BattleResult=Defeat, IsStageEndNode=false } (defeat is not Clear)
    2. Call TryEnterEndingAsync(BattleDefeat, context)
    3. true → ending entered, terminate
    4. false → exceptional state. Log + stay at current position. Fallback EndingSO must be guaranteed via Manual Work.

**New responsibility — node completion on event return:**

At StagePresenter scene re-entry (`OnSceneEnter` or similar lifecycle):
- If `_pendingEventContext != null && _pendingEventContext.IsCompleted && _pendingEventContext.Origin == StageNode`:
    - Call `StageProgressService.CompleteNodeAsync(...)` (node completion handling)
    - Clear `_pendingEventContext.IsCompleted = false` or null (prevent duplicate handling)

Add `HandleEventResultIfAny` method or integrate into existing initialization flow. Claude Code decides after verifying current StagePresenter structure.

**Patch-003 direct judgment logic removal:**
- Remove existing `if (_useCase.IsStageComplete(battleNodeIndex))` branch
- Boss/regular distinction removed from StagePresenter itself. Designer expresses "end-node-only match" via EndingSO Conditions using StageCompleteFlag.
- Remove direct `IncrementClearedStageCount()` call → moved inside StageProgressService

**Event node entry — PendingEventContext setup:**
- When StagePresenter creates PendingEventContext before calling EventScene:
    - Origin = StageNode
    - IsStageEndNode = whether current node is end node

### 15. Modify — EventResultType (enum)

Path: Assets/_Game/Features/Event/MasterData/EventResultType.cs

- **Remove** the `Ending` entry added in Patch-002

### 16. Modify — EventResult (inside EventSO)

Path: Assets/_Game/Features/Event/MasterData/EventSO.cs

- **Remove** the `_endingType` field added in Patch-002
- Ending match info migrates to EndingSO-side Conditions (EventId, EventResultType matching)

### 17. Modify — BattleNodeDataSO

Path: Assets/_Game/Features/Stage/MasterData/BattleNodeDataSO.cs

- **Remove** `_isBoss` field (currently unused. Patch-004 removes boss judgment from StagePresenter itself, making this field unnecessary)

### 18. Modify — StageSceneUseCase (Patch-003 implementation adjustment)

Path: Assets/_Game/Features/Stage/Domain/StageSceneUseCase.cs

- Transfer responsibility of `IncrementClearedStageCount()` added in Patch-003 (D-12):
    - Caller changes to StageProgressService
    - Method itself may be kept or removed (Claude Code decides after checking call structure)
- Existing `IsStageComplete()` method may be used internally by StageProgressService, so keep

### 19. Modify — MaintenancePresenter (or exploration call site)

Path: Claude Code identifies maintenance exploration event call site

- On PendingEventContext setup:
    - Origin = MaintenanceExploration
    - IsStageEndNode = false
- Depends on whether exploration event call is already implemented. If not implemented, apply when exploration feature is built (Backlog)

---

### DO NOT Modify

- EndingUseCase, EndingPresenter, EndingView — they only receive endingId, so no change
- All BattleScene Feature files
- EventUseCase, EventView — event scene internal logic
- MainScene, Maintenance scene internal logic (only modify exploration event call site)
- NodeType enum — including Boss value, not modified in this patch (future audit target)
- BattleNodeDataSO._enemySpawns — battle data preserved
- EndingResolver core logic (Conditions AND + Priority max tracking)

---

## Claude Code Implementation Guide

**Files to read (read-only):**
- Assets/_Game/Features/Ending/MasterData/EndingType.cs
- Assets/_Game/Features/Ending/MasterData/EndingCondition.cs
- Assets/_Game/Features/Ending/MasterData/EndingConditionType.cs
- Assets/_Game/Features/Ending/MasterData/EndingSO.cs
- Assets/_Game/Features/Ending/Domain/IEndingResolver.cs, EndingResolver.cs
- Assets/_Game/Features/Ending/Domain/IEndingEntryService.cs, EndingEntryService.cs
- Assets/_Game/Features/Ending/Domain/PendingEndingContext.cs, RunSummaryData.cs
- Assets/_Game/Features/Ending/Data/EndingMasterDataRepository.cs
- Assets/_Game/Features/Event/MasterData/EventResultType.cs, EventSO.cs
- Assets/_Game/Features/Event/Presentation/EventPresenter.cs, EventSceneBootstrapper.cs
- Assets/_Game/Features/Event/Domain/PendingEventContext.cs
- Assets/_Game/Features/Stage/MasterData/BattleNodeDataSO.cs, StageNodeSO.cs, NodeType.cs
- Assets/_Game/Features/Stage/Presentation/StagePresenter.cs (post-Patch-002 state)
- Assets/_Game/Features/Stage/Domain/StageSceneUseCase.cs
- Assets/_Game/Features/Stage/Data/StageRunData.cs, StageRepository.cs
- Assets/_Game/Features/Maintenance/Presentation/MaintenancePresenter.cs (check exploration event call site)
- Assets/_Game/App/GameContext.cs

**Files to modify/create summary:**
- New: EndingTriggerKind (enum), EndingContext (struct), IStageProgressService/StageProgressService, NodeCompletionContext (struct), EventOriginKind (enum)
- Modify: EndingType, EndingConditionType, EndingSO, IEndingResolver, EndingResolver, EndingMasterDataRepository, IEndingEntryService, EndingEntryService, PendingEventContext, EventResultType, EventSO, EventPresenter, EventSceneBootstrapper (if needed), StagePresenter, StageSceneUseCase, BattleNodeDataSO, GameContext, MaintenancePresenter (exploration call site)

**Implementation order:**
1. Read all pre-implementation files
2. Create EndingTriggerKind, EndingContext
3. Redefine EndingType (Good/Bad [Flags])
4. Extend EndingConditionType (EventId, EventResultType, StageCompleteFlag)
5. Reorganize EndingSO fields (TriggerKind, Categories)
6. EndingMasterDataRepository: replace with GetEndingsByTriggerKind
7. IEndingResolver, EndingResolver: TryResolve signature + new Condition evaluation
8. IEndingEntryService, EndingEntryService: TryEnterEndingAsync → bool
9. EventResultType: remove Ending / EventResult: remove _endingType
10. Create EventOriginKind enum
11. Add PendingEventContext IsStageEndNode, Origin fields
12. Create NodeCompletionContext struct
13. Create IStageProgressService, StageProgressService + GameContext accessor
14. EventPresenter: common post-processing (TryEnterEnding → ReturnScene on no match). NO StageProgressService call.
15. StagePresenter: reorganize HandleBattleResultIfAny, add event-return handling (consider new HandleEventResultIfAny), remove boss judgment, setup Origin/IsStageEndNode on PendingEventContext creation
16. Remove BattleNodeDataSO._isBoss
17. Transfer Patch-003 StageSceneUseCase.IncrementClearedStageCount caller
18. At MaintenancePresenter or exploration call site, set Origin=MaintenanceExploration / IsStageEndNode=false on PendingEventContext (if implementation exists)

**Judgment points (record in decisions.md):**
- PendingEventContext setup locations (Origin, IsStageEndNode setup from StagePresenter / Maintenance side)
- StageProgressService's "end node judgment" specifics (StageNodeSO-based vs StageRunData-based)
- List of existing EndingSO .assets requiring migration (Manual Work targets)
- List of existing EventSO .assets using Death/Ending result types
- Whether other references to BattleNodeDataSO._isBoss exist
- StageSceneUseCase.IncrementClearedStageCount method keep/remove decision
- Maintenance exploration event call implementation status and PendingEventContext setup location

**DO NOT:**
- Modify NodeType enum
- Modify BattleScene Feature
- Create boss judgment logic in StagePresenter or elsewhere (migrates to MasterData Condition)
- Call StageProgressService directly from EventPresenter (Feature boundary violation since events fire outside stage)
- Create files outside Assets/_Game/ (except decisions.md)

**decisions.md rules:**
- Record judgments in `.claude/specs/features/ending-scene/decisions.md` with [DECISION], [BACKLOG], [SPEC-GAP] tags

---

## Validation

- [ ] No Unity compile errors
- [ ] EndingTriggerKind, EndingContext, IStageProgressService/StageProgressService, NodeCompletionContext, EventOriginKind created
- [ ] EndingType [Flags] (Good/Bad) redefined
- [ ] EndingConditionType extended (EventId, EventResultType, StageCompleteFlag)
- [ ] EndingSO: TriggerKind + Categories ([Flags]) fields exist, old EndingType field removed
- [ ] EndingResolver.TryResolve: TriggerKind filter + extended Condition evaluation + Priority max + null on no match
- [ ] EndingEntryService.TryEnterEndingAsync: true on ending entry, false on no match
- [ ] EventResultType: Ending removed / EventResult._endingType removed
- [ ] PendingEventContext.IsStageEndNode, Origin fields exist
- [ ] EventPresenter: attempt TryEnterEnding after result handling → ReturnScene on no match (no StageProgressService call)
- [ ] StagePresenter: boss judgment removed, Victory/Defeat each attempt TryEnterEnding → StageProgressService call on no match
- [ ] StagePresenter: on event return, check PendingEventContext.Origin=StageNode then call StageProgressService
- [ ] BattleNodeDataSO._isBoss removed
- [ ] StageProgressService.CompleteNodeAsync: Clear++ + next stage on end node / MoveToNode otherwise
- [ ] Test scenarios:
    - Regular battle victory → no ending match → move to next node
    - Boss battle victory (end node) → ending entry on corresponding EndingSO match / Clear + next stage on no match
    - Battle defeat → enter Fallback EndingSO
    - Event completion (regular) → no ending match → ReturnScene (Stage) → StagePresenter handles node completion
    - Event completion (end node + ending condition match) → ending entry
    - Maintenance exploration event completion → no ending match → maintenance scene return (no StageProgressService call)

---

## Manual Work (after Claude Code implementation)

- [ ] **Full EndingSO .asset reconfiguration** — existing EndingType field redefinition requires re-setting all existing assets in Inspector with TriggerKind + Categories + Conditions. Claude Code records existing asset list in decisions.md.
- [ ] **Recommended 1st-dev Fallback EndingSOs:**
    - TriggerKind=BattleDefeat + Conditions=empty + Priority=0 + Categories=Bad — matches all battle defeats (Fallback)
    - TriggerKind=BattleVictory + Conditions=[StageCompleteFlag] + Priority=0 + Categories=Good — matches end node (boss) victories
    - EventResult trigger Fallback added as needed by planning
- [ ] **Migrate existing EventSO .assets using Death/Ending result types** — change result type to original meaning. Move related endings to EndingSO Conditions (EventId, EventResultType matching).
- [ ] Run full test scenarios