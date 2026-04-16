# EventSystem — Patch-002

**Status:** ✅ Confirmed (Implementation Pending)
**Feature:** EventSystem (external Feature Patch — for EndingScene integration)
**Patch ID:** Patch-002
**Type:** spec-change
**Related Documents:** EndingScene Specify v1.0.0 §7-3, EndingScene Plan v1.0.0 §10-3, EndingScene Patch-001 v1.0.1, Patch-003 v1.0.0 (includes StagePresenter refactor)
**Date:** 2026-04-16

## Version History

| Version | Date | Changes |
|---|---|---|
| v1.0.0 | 2026-04-16 | Initial version |

---

## Background

1. EndingScene, Patch-001 (EndingResolver), and Patch-003 (StageScene integration) are complete, but the path where the run terminates via events is still unconnected. EventPresenter currently navigates to `SceneKey.GameOver` on `EventResultType.Death`, but `GameOver.unity` does not exist, causing scene load failure at runtime.

2. Conceptually, **Death is a sub-classification of Ending** (EndingType.EventDeath). The current `EventResultType.Death` can only trigger EventDeath; there is no way to trigger other ending classifications like EventEnding. The structure must be changed so that "when an event terminates the run, which specific ending type is triggered" is specified in MasterData.

3. The ending entry logic implemented in StagePresenter (Patch-003) and the ending entry logic to be added in EventPresenter share the same structure. Future Scenes (SplashScene/ReplayScene etc.) are likely to need the same logic. Extract this into a **common service (EndingEntryService)**.

4. EventPresenter handles a scene-transition context (`PendingEventContext`) but currently uses individual injection. For consistency with other scene-transition Presenters in the project (Maintenance/Battle/Stage/Ending — 4/4 all inject GameContext wholesale), change EventPresenter to **inject GameContext wholesale**. (Pragmatic unification at the manual-DI stage; a full reorganization is planned when a DI library is introduced.)

---

## Changes

### 1. New — IEndingEntryService (Domain Interface)

Path: Assets/_Game/Features/Ending/Domain/IEndingEntryService.cs

- Common ending entry service interface
- Method: UniTask EnterEndingAsync(EndingType endingType)

### 2. New — EndingEntryService (Domain Implementation)

Path: Assets/_Game/Features/Ending/Domain/EndingEntryService.cs

- Implements IEndingEntryService. Pure C# class.
- Dependencies: IEndingResolver, ICharacterRunRepository, IStageRepository, EvolutionNodeSO[], ISceneNavigator, GameContext (for PendingEndingContext set)
- **EnterEndingAsync(EndingType) logic:**
    1. _endingResolver.Resolve(endingType) → endingId
    2. Build RunSummaryData:
        - TotalDays: from CharacterRunData
        - FinalEvolutionName: EvolutionNodes foreach lookup (same pattern as D-10; returns "Unknown" if no match)
        - StagesCleared: StageRunData.ClearedStageCount (added in Patch-003)
        - FinalGold: from CharacterRunData
    3. Set _gameContext.PendingEndingContext (EndingId, RunSummary)
    4. _sceneNavigator.NavigateToAsync(SceneKey.Ending)
- Includes _logClass

### 3. Modify Existing — GameContext

Path: Assets/_Game/App/GameContext.cs

- Add EndingEntryService (IEndingEntryService) public accessor
- Construct EndingEntryService instance + DI wiring in constructor (EndingResolver, CharacterRunRepo, StageRepo, EvolutionNodes, SceneNavigator, this)

### 4. Modify Existing — EventResultType (enum)

Path: Assets/_Game/Features/Event/MasterData/EventResultType.cs

- **Remove** Death
- **Add** Ending — result type that terminates the run. Specific ending classification specified via EventResult.EndingType.

### 5. Modify Existing — EventResult (Serializable class inside EventSO)

Path: Assets/_Game/Features/Event/MasterData/EventSO.cs

- Add field: _endingType (EndingType?, nullable) — valid only when ResultType == Ending. Specifies which ending type (EventDeath / EventEnding / future additions) to trigger.
- Expose via public read-only property
- Same pattern as existing StatType? / MerchantId?

### 6. Modify Existing — EventPresenter

Path: Assets/_Game/Features/Event/Presentation/EventPresenter.cs

- **Change constructor dependencies**: existing individual injection → **inject GameContext wholesale** (project scene-transition Presenter unified pattern)
    - View/UseCase-type deps (EventUseCase, EventView, ShopUseCase, ISpriteLoader) remain individually injected
    - ISceneNavigator, PendingEventContext accessed via GameContext
- **Modify HandlePostResultAsync**:
    - **Remove** EventResultType.Death case
    - Add EventResultType.Ending case:
        - If result.EndingType is null, throw InvalidOperationException (Fail Fast — MasterData integrity issue)
        - Call _gameContext.EndingEntryService.EnterEndingAsync(result.EndingType.Value)
        - Do NOT set IsCompleted (run terminates, no ReturnScene return needed)
    - Remaining cases (None/HpChange/StatChange/ShopEncounter/Battle) keep existing logic (access paths cleaned up via GameContext)

### 7. Modify Existing — EventSceneBootstrapper

Path: Assets/_Game/Features/Event/Presentation/EventSceneBootstrapper.cs

- Reflect EventPresenter constructor signature change (inject GameContext wholesale)
- Previously individually-injected dependencies (ISceneNavigator, PendingEventContext) now accessed via GameContext

### 8. Modify Existing — StagePresenter (refactor of Patch-003 implementation)

Path: Assets/_Game/Features/Stage/Presentation/StagePresenter.cs

- **Remove** the ending entry helper method added in Patch-003 (e.g., EnterEndingSceneAsync)
- Replace its calls with _gameContext.EndingEntryService.EnterEndingAsync(endingType)
- Preserve BattleDefeat / BossVictory discrimination logic
- Preserve ClearedStageCount increment on BossVictory path (keep D-09, D-12 logic intact)

### 9. Modify Existing — SceneKey (enum)

Path: Assets/_Game/Core/Navigation/SceneKey.cs

- **Remove** GameOver (only EventPresenter referenced it, and that reference is removed in Patch-002)

---

### DO NOT Modify

- Other files in EndingScene Feature (EndingUseCase, EndingPresenter, EndingView, etc.)
- Existing EndingResolver implementation
- Existing EventUseCase logic
- All BattleScene Feature files

---

## Claude Code Implementation Guide

**Files to read (read-only):**
- Assets/_Game/Features/Event/MasterData/EventResultType.cs
- Assets/_Game/Features/Event/MasterData/EventSO.cs (verify EventResult definition location)
- Assets/_Game/Features/Event/Presentation/EventPresenter.cs
- Assets/_Game/Features/Event/Presentation/EventSceneBootstrapper.cs
- Assets/_Game/Features/Stage/Presentation/StagePresenter.cs (Patch-003 implementation — verify ending entry helper pattern + refactor target)
- Assets/_Game/Features/Ending/Domain/IEndingResolver.cs
- Assets/_Game/Features/Ending/Domain/PendingEndingContext.cs
- Assets/_Game/Features/Ending/Domain/RunSummaryData.cs
- Assets/_Game/Features/Ending/MasterData/EndingType.cs
- Assets/_Game/App/GameContext.cs
- Assets/_Game/Core/Navigation/SceneKey.cs
- Assets/_Game/Features/Character/Data/CharacterRunData.cs
- Assets/_Game/Features/Character/MasterData/EvolutionNodeSO.cs
- Assets/_Game/Features/Stage/Data/StageRunData.cs

**Files to modify:**
- New: Assets/_Game/Features/Ending/Domain/IEndingEntryService.cs, Assets/_Game/Features/Ending/Domain/EndingEntryService.cs
- Modify: Assets/_Game/App/GameContext.cs, Assets/_Game/Features/Event/MasterData/EventResultType.cs, Assets/_Game/Features/Event/MasterData/EventSO.cs, Assets/_Game/Features/Event/Presentation/EventPresenter.cs, Assets/_Game/Features/Event/Presentation/EventSceneBootstrapper.cs, Assets/_Game/Features/Stage/Presentation/StagePresenter.cs, Assets/_Game/Core/Navigation/SceneKey.cs

**Implementation order:**
1. Read all pre-implementation files
2. Create IEndingEntryService interface
3. Implement EndingEntryService (DI + EnterEndingAsync logic)
4. Add EndingEntryService accessor and constructor wiring to GameContext
5. EventResultType: remove Death, add Ending
6. Add EndingType? field to EventResult (inside EventSO)
7. Remove SceneKey.GameOver
8. EventPresenter: transition to GameContext wholesale injection + modify HandlePostResultAsync branching (remove Death case, add Ending case)
9. EventSceneBootstrapper: reflect signature changes
10. StagePresenter refactor: remove existing helper method, replace with EndingEntryService call

**Judgment points (record in decisions.md):**
- List of existing EventSO .assets using Death result (Manual Work migration targets)
- Any compatibility issues encountered during EventPresenter GameContext wholesale injection transition

**DO NOT:**
- Modify BattleScene Feature
- Modify existing files in EndingScene Feature (EndingPresenter, EndingView, etc.)
- Create files outside Assets/_Game/ (except decisions.md)

**decisions.md rules:**
- Record any judgment calls not covered by this Patch in .claude/specs/features/ending-scene/decisions.md with appropriate tags: [DECISION], [BACKLOG], or [SPEC-GAP]

---

## Validation

- [ ] No Unity compile errors
- [ ] EventResultType: Death removed, Ending added
- [ ] EventResult.EndingType? field exists
- [ ] SceneKey: GameOver removed
- [ ] IEndingEntryService / EndingEntryService created
- [ ] GameContext.EndingEntryService accessor exists
- [ ] EventPresenter transitioned to GameContext wholesale injection (scene-transition Presenter unified pattern)
- [ ] EventPresenter Ending result (EndingType=EventDeath) → EndingEntryService.EnterEndingAsync called → SceneKey.Ending transition. Resolver returns EventDeath type EndingSO.
- [ ] EventPresenter Ending result (EndingType=EventEnding) → same flow. Resolver returns EventEnding type EndingSO.
- [ ] StagePresenter refactored: existing ending entry helper removed, replaced with EndingEntryService call. BattleDefeat/BossVictory discrimination logic preserved.
- [ ] BossVictory path ClearedStageCount increment preserved (D-09/D-12 logic intact)
- [ ] RunSummaryData all fields populated (TotalDays, FinalEvolutionName, StagesCleared, FinalGold)
- [ ] Remaining EventResultType (None/HpChange/StatChange/ShopEncounter/Battle) existing behavior preserved
- [ ] Existing EventSO .asset load has no compile/deserialization errors (verify impact of Death enum removal)

---

## Manual Work (after Claude Code implementation)

- [ ] **Migrate existing Death result EventSO .assets** — if any asset uses Death result, change to Ending result + set EndingType=EventDeath. Claude Code will provide a list of existing Death-referencing assets discovered during implementation; Hak performs manual migration.
- [ ] Modify/create test EventSOs: Ending result + EndingType=EventDeath, Ending result + EndingType=EventEnding — for transition verification
- [ ] Patch-001 Manual Work (EndingSO .asset per EndingType) must be complete for actual playtest