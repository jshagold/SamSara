# StageScene — Patch-004

**Status:** ⏳ Pending Implementation
**Feature:** StageScene
**Patch ID:** Patch-004
**Type:** spec-change
**Related Documents:** EndingScene Specify v2.0.0 / Plan v2.0.0 / Tasks v2.0.0
**Date:** 2026-04-19

> Patches are not versioned. See Claude Project Instructions §3.

---

## Background

Under the EndingScene v2.0.0 redesign, Stage Domain layer responsibilities are reorganized. The `EndingTriggerKind` / `NodeCompletionContext.IsStageEndNode` pre-computed judgment approach introduced in Patch-004 (EndingScene) violated Clean Architecture §4 data/judgment separation. From v2.0.0 onward: ① `NodeCompletionContext` is shrunk to a pure DTO, ② judgment is performed by `StageProgressService` directly via `IStageSceneUseCase`, and ③ `StagePresenter.HandleBattleResultIfAny` is restructured to attempt slot-based ending matching on `BattleNodeDataSO` first, and only fall back to node completion handling on match failure.

---

## Changes (Summary)

- **`NodeCompletionContext.cs`**: Remove `IsStageEndNode` field. Shrink to pure DTO (keep only NodeIndex + NodeType).
- **`StageProgressService.cs`**: Inject `IStageSceneUseCase` in ctor (add if not already injected). In `CompleteNodeAsync`, remove `context.IsStageEndNode` read and switch to direct `_stageSceneUseCase.IsStageComplete(context.NodeIndex)` invocation.
- **`IStageProgressService.cs`**: Verify signature impact (the parameter type remains `NodeCompletionContext`; only the internal field composition changes, so interface modification is likely unnecessary). Claude Code verifies.
- **`StagePresenter.cs`**: Restructure `HandleBattleResultIfAny` — select the current battle node's `VictoryEndings` / `DefeatEndings` slot → call `EndingEntryService.TryEnterEndingAsync(slot, context)` → on match failure, call `CompleteNodeAsync` only in the victory branch; the defeat branch emits a warning log only and stays on the current scene. Remove the `PendingEventContext.IsStageEndNode` setter line when entering event nodes (keep `Origin = EventOriginKind.StageNode`).
- **MaintenanceScene-related code**: Remove `PendingEventContext.IsStageEndNode = false` setter line when invoking exploration events (keep `Origin = EventOriginKind.MaintenanceExploration`).

---

## Implementation Reference

**This Patch is reference-only.** Follow the actual implementation instructions in the Tasks listed below, from EndingScene Tasks-MD v2.0.0:

- **T15**: Modify `NodeCompletionContext.cs` (remove IsStageEndNode, shrink to pure DTO)
- **T16**: Modify `StageProgressService.cs` (switch to direct IStageSceneUseCase invocation)
- **T17**: Verify `IStageProgressService.cs` (assess signature impact)
- **T18**: Modify `StagePresenter.cs` (restructure HandleBattleResultIfAny + remove IsStageEndNode setter)
- **T21**: Remove `IsStageEndNode` setter code in MaintenanceScene

EndingScene Tasks-MD v2.0.0: https://www.notion.so/34452975d2df81ea9679fcdd8d619d25

---

## Claude Code Implementation Guide

**This Patch is reference-only. Follow EndingScene Tasks-MD v2.0.0 for the actual implementation.**
Claude Code does not need to open this Patch document directly. This Patch is issued solely to track the change history for the StageScene Feature.

---

## Validation

- [ ] `NodeCompletionContext.IsStageEndNode` references: 0 project-wide (Validation V-05)
- [ ] `StageProgressService` ctor has `IStageSceneUseCase` injected (Validation V-11)
- [ ] Runtime R-01 (regular battle victory → CompleteNodeAsync called → move to next node) and R-05 (battle defeat + empty slot → warning log only, stay on scene) scenarios pass
- [ ] For full Validation, refer to the Validation / Runtime Validation sections of EndingScene Tasks-MD v2.0.0