# EndingScene — Patch-001

**Status:** ✅ Confirmed
**Feature:** EndingScene
**Patch ID:** Patch-001
**Type:** spec-change
**Related Documents:** Specify v1.1.0 pending / Plan v1.1.0 pending (version bump after this Patch applies)
**Date:** 2026-04-16

---

## Background

EndingScene Specify v1.0.0 was designed with the "caller directly specifies ending ID" structure. However, in actual gameplay, endings must branch based on player state (evolution, karma, stats, etc.), and this should be handled via MasterData condition matching rather than hardcoding, in order to be extensible.

For 1st development, only minimal conditions (EvolutionId matching) + fallback endings are implemented, but the structure must support adding various conditions in the future (stats, karma, event completion status, etc.) — an **extensible structure** is required.

---

## Changes

### 1. New — EndingConditionType (enum)

Path: Assets/_Game/Features/Ending/MasterData/EndingConditionType.cs

- Enum representing the type of ending trigger condition
- 1st dev entries:
    - None — no condition (for fallback)
    - EvolutionId — match specific evolution ID
- Future expansion targets (NOT implemented in this Patch, only enum structure prepared):
    - StatRange, KarmaRange, UnlockedEventId, HpRatio, etc.

### 2. New — EndingCondition (Serializable class)

Path: Assets/_Game/Features/Ending/MasterData/EndingCondition.cs

- Serializable class expressing one trigger condition for an ending
- Fields (designed for extensibility, usage depends on Type):
    - Type (EndingConditionType)
    - IntValue (int) — 1st dev: holds EvolutionId here. Reusable for other integer conditions in the future.
    - Future extensions may add StringValue, IntMin, IntMax, FloatValue, etc.
- Expose via public read-only properties

### 3. Modify Existing — EndingSO

Path: Assets/_Game/Features/Ending/MasterData/EndingSO.cs

- Preserve all existing fields
- Additional fields:
    - _conditions (EndingCondition[]) — trigger conditions (AND). Empty array = no condition = fallback candidate.
    - _priority (int) — priority. When multiple EndingSOs match within the same EndingType, higher value wins. Default 0.
- Add public read-only properties

### 4. New — IEndingResolver (interface)

Path: Assets/_Game/Features/Ending/Domain/IEndingResolver.cs

- Ending condition matching service interface
- Method:
    - int Resolve(EndingType type) — return the Id of the EndingSO matching the current player state for the given type
- If no ending matches, throw InvalidOperationException (Fail Fast — fallback EndingSO must be guaranteed to always match via Conditions = empty array + Priority = 0 in Manual Work)

### 5. New — EndingResolver (implementation)

Path: Assets/_Game/Features/Ending/Domain/EndingResolver.cs

- Implements IEndingResolver. Pure C# class.
- Constructor DI: IEndingMasterDataRepository, ICharacterRunRepository, ICharacterAccountRepository
- Resolve(EndingType) logic:
    1. Get all EndingSOs of the given type via _endingMasterDataRepo.GetEndingByType(type)
    2. Evaluate each EndingSO's Conditions against the current player state (AND)
        - Empty array → always pass
        - If any Condition fails → reject
    3. Sort matched EndingSOs by Priority descending
    4. Select the top and return its Id
    5. If no match → throw exception
- Condition evaluation logic (implemented in 1st dev):
    - EndingConditionType.EvolutionId: compare current CharacterRunData's evolution ID to condition.IntValue
    - EndingConditionType.None: always pass
- Condition evaluation logic (future extension):
    - New EndingConditionType additions require only adding a case to EndingResolver's switch — structure remains extensible
- No LINQ (CLAUDE.md compliance — foreach + List<T> pattern)
- Include _logClass

### 6. Modify Existing — GameContext

Path: Assets/_Game/App/GameContext.cs

- Preserve all existing fields/logic
- Add:
    - EndingResolver (IEndingResolver) public accessor
    - In constructor: create EndingResolver instance + DI injection (IEndingMasterDataRepository, ICharacterRunRepository, ICharacterAccountRepository)

---

### DO NOT Modify

- EndingUseCase.cs — keep LoadEnding(int) as-is. Resolver decides the Id externally; UseCase continues to operate on the Id.
- EndingMasterDataRepository.cs — keep existing GetEndingByType etc. as-is.
- PendingEndingContext.cs — as-is.
- EndingPresenter.cs — as-is.
- EndingSceneBootstrapper.cs — as-is.
- EndingView.cs and child Views — as-is.

---

## Claude Code Implementation Guide

- Read CLAUDE.md first.
- Files to read for context:
    - Assets/_Game/Features/Ending/MasterData/EndingSO.cs
    - Assets/_Game/Features/Ending/MasterData/EndingType.cs
    - Assets/_Game/Features/Ending/Data/EndingMasterDataRepository.cs
    - Assets/_Game/Features/Ending/Domain/IEndingMasterDataRepository.cs
    - Assets/_Game/Features/Character/Domain/ICharacterRunRepository.cs
    - Assets/_Game/Features/Character/Data/CharacterRunData.cs
    - Assets/_Game/Features/Character/Domain/ICharacterAccountRepository.cs
    - Assets/_Game/App/GameContext.cs
- Files to create:
    1. Assets/_Game/Features/Ending/MasterData/EndingConditionType.cs
    2. Assets/_Game/Features/Ending/MasterData/EndingCondition.cs
    3. Assets/_Game/Features/Ending/Domain/IEndingResolver.cs
    4. Assets/_Game/Features/Ending/Domain/EndingResolver.cs
- Files to modify:
    5. Assets/_Game/Features/Ending/MasterData/EndingSO.cs — add Conditions, Priority fields + public properties
    6. Assets/_Game/App/GameContext.cs — create EndingResolver instance, add public accessor
- Implementation order:
    1. EndingConditionType enum
    2. EndingCondition serializable class
    3. EndingSO field additions
    4. IEndingResolver interface
    5. EndingResolver implementation
    6. GameContext wiring
- If you make any judgment calls not covered by this Patch, record them in .claude/specs/features/ending-scene/decisions.md with appropriate tags: [DECISION], [BACKLOG], or [SPEC-GAP].
- DO NOT create files outside Assets/_Game/ (except decisions.md).

---

## Validation

- [ ] No Unity compile errors
- [ ] Existing EndingSO .assets load/operate without impact (newly added Conditions, Priority default to empty array / 0)
- [ ] EndingResolver.Resolve(EndingType) — if only one EndingSO exists for that type, it is returned
- [ ] EndingResolver.Resolve(EndingType) — when a conditional EndingSO and a fallback EndingSO coexist, the higher-priority match wins if condition is satisfied
- [ ] EndingResolver.Resolve(EndingType) — when no conditional EndingSO matches, fallback EndingSO is selected
- [ ] EndingResolver.Resolve(EndingType) — if no EndingSO exists for that type or none match, InvalidOperationException is thrown
- [ ] EndingResolver accessible via GameContext

---

## Manual Work (after Claude Code implementation)

- [ ] Create fallback EndingSO .assets for each EndingType (Conditions = empty array, Priority = 0) — BattleDefeat / EventDeath / BossVictory / EventEnding, 4 total
- [ ] Create 1~2 conditional EndingSO .assets for testing (EvolutionId condition + Priority > 0) — for Resolver behavior verification