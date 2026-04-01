# MaintenanceScene — Patch-001

**Type:** spec-change
**Related:** Specify v1.0.0 → v1.1.0
**Source:** MiniGame Decisions D-02 [SPEC-GAP]

## Background

MiniGame Feature implementation requires modifying MaintenancePresenter's training stat selection flow.

Current MaintenancePresenter.HandleStatSelectedAsync:
1. Calls `ConsumeActionPoint()` (in-memory AP -1)
2. MiniGame scene transition (TODO comment, not connected)

Per MiniGame Specify RQ-04, AP deduction is MiniGame scene's responsibility. Remove `ConsumeActionPoint()` from MaintenancePresenter and only perform scene transition.

## Changes

### MaintenancePresenter.cs
Modify training stat selection handler:

**Before:**
1. `CanPerformAction()` check
2. `ConsumeActionPoint()` (in-memory AP -1)
3. `ISceneNavigator` MiniGame scene transition (TODO not connected)

**After:**
1. `CanPerformAction()` check
2. `GameContext.PendingTrainingStat = selectedStatType`
3. `ISceneNavigator.NavigateToAsync(SceneKey.MiniGame)`

- Remove `ConsumeActionPoint()` call
- Add `GameContext` reference (constructor injection or passed from Bootstrapper)

### MaintenanceSceneBootstrapper.cs
- Modify to pass `GameContext` instance to `MaintenancePresenter` (if GameContext access already available, no additional modification needed — check existing code)

### MaintenanceUseCase.cs
- Do NOT delete `ConsumeActionPoint()` method. May still be used for Exploration (Event) entry.

## Claude Code Implementation Guide

- Read CLAUDE.md first
- Files to modify:
    - `Assets/_Game/Features/MaintenanceScene/Presentation/MaintenancePresenter.cs`
    - `Assets/_Game/Features/MaintenanceScene/Presentation/MaintenanceSceneBootstrapper.cs` (if needed)
- Files to reference:
    - `Assets/_Game/App/GameContext.cs` (PendingTrainingStat property)
    - `Assets/_Game/Core/Navigation/SceneKey.cs` (MiniGame enum value)
    - `Assets/_Game/Features/MiniGame/Presentation/MiniGameSceneBootstrapper.cs` (how PendingTrainingStat is consumed)
- Do NOT delete ConsumeActionPoint() from MaintenanceUseCase
- Record any judgment calls in `.claude/specs/features/maintenance-scene/decisions.md`

## Validation

- Training list stat selection sets `GameContext.PendingTrainingStat` and transitions to MiniGame scene
- `ConsumeActionPoint()` call removed from training flow in MaintenancePresenter
- Insufficient AP popup still works correctly