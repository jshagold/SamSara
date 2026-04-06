# CharacterInfoScene — Patch-001

**Status:** Confirmed
**Feature:** CharacterInfoScene
**Patch ID:** Patch-001
**Type:** refactor
**Related:** D-01 [SPEC-GAP] — EvolutionNodeSO[] GameContext access undefined
**Version:** 1.0.0
**Date:** 2026-04-06

---

## Context

D-01 [SPEC-GAP]: `EvolutionNodeSO[]` access method is not defined in `GameContext`. `CharacterInfoSceneBootstrapper` directly calls `Resources.LoadAll<EvolutionNodeSO>("MasterData")`. Per Constitution §6, GameContext should serve as the Service Container providing MasterData access. Each Bootstrapper performing direct Resources loading breaks structural consistency.

---

## Changes Required

### 1. Modify `Assets/_Game/App/GameContext.cs`
- Add `EvolutionNodeSO[]` cache field (`private readonly EvolutionNodeSO[] _evolutionNodes`)
- Store `Resources.LoadAll<EvolutionNodeSO>("MasterData")` result in constructor
- Expose public read-only property `EvolutionNodes`

### 2. Modify `Assets/_Game/Features/CharacterInfoScene/Presentation/CharacterInfoSceneBootstrapper.cs`
- Remove direct `Resources.LoadAll<EvolutionNodeSO>("MasterData")` call
- Acquire `EvolutionNodeSO[]` via `GameContext.EvolutionNodes` property
- Pass to `CharacterInfoUseCase` constructor

---

## Files to Reference
- `Assets/_Game/App/GameContext.cs` — current constructor structure and MasterData load approach
- `Assets/_Game/Features/CharacterInfoScene/Presentation/CharacterInfoSceneBootstrapper.cs` — current Resources.LoadAll call location

---

## Verification
- [ ] GameContext has `EvolutionNodes` property that returns a non-null array
- [ ] CharacterInfoSceneBootstrapper has no direct `Resources.LoadAll` call
- [ ] CharacterInfoScene Play works identically to before (stats/skills/evolution info display)
- [ ] No compile errors in Unity console

---

## Claude Code Implementation Guide
- Read CLAUDE.md first before any modification
- Files to modify:
    - `Assets/_Game/App/GameContext.cs`
    - `Assets/_Game/Features/CharacterInfoScene/Presentation/CharacterInfoSceneBootstrapper.cs`
- Files to reference:
    - `Assets/_Game/Features/Character/MasterData/EvolutionNodeSO.cs`
- Modification order: GameContext first, then CharacterInfoSceneBootstrapper
- DO NOT create files outside Assets/_Game/
- If you make any judgment calls not covered by the Patch, record them in `.claude/specs/features/character-info-scene/decisions.md` with [DECISION] tag