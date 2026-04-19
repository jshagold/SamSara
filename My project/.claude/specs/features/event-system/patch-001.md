# EventSystem — Patch-001

**Status:** ⏳ Pending Implementation
**Feature:** EventSystem
**Patch ID:** Patch-001
**Type:** spec-change
**Related Documents:** EndingScene Specify v2.0.0 / Plan v2.0.0 / Tasks v2.0.0
**Date:** 2026-04-19

> Patches are not versioned. See Claude Project Instructions §3.

---

## Background

Under the EndingScene v2.0.0 redesign, event results also follow the node-local ending candidate slot structure. Previously, entering an ending from an event used the `EndingTriggerKind.EventFinish`-based global iteration approach, but from v2.0.0 onward each `EventSO.EventResult` declares directly which ending candidate slot to attempt on application. In addition, following Clean Architecture §4 data/judgment separation, the `PendingEventContext.IsStageEndNode` pre-computed judgment field is removed, and EventPresenter's post-result handling is restructured from a "node action model" perspective.

---

## Changes (Summary)

- **`EventSO.cs` (nested `EventResult` class)**: Add `_endingSlot: EndingCandidateSlot` field (`[SerializeField] private`) + `EndingSlot` property. The candidate slot to attempt when this result is applied. Ending matching is skipped entirely when `IsEmpty` is true.
- **`PendingEventContext.cs`**: Remove `IsStageEndNode: bool` field (declaration + getter/setter + ctor parameter). Judgment is performed directly by the return-scene Presenter based on `Origin`.
- **`EventPresenter.cs`**: Restructure `HandlePostResultAsync` — keep result application logic → if `EventResult.EndingSlot.IsEmpty`, skip ending matching → else construct `EndingContext` and call `EndingEntryService.TryEnterEndingAsync(slot, context)` → on success, return; on failure/skip, navigate to `ReturnScene`. EventPresenter does NOT call `StageProgressService` directly (Feature boundary preserved).
- **using to add**: `using Samsara.Features.Ending.MasterData;` (for `EndingCandidateSlot` reference), using for `EndingContext`.

---

## Implementation Reference

**This Patch is reference-only.** Follow the actual implementation instructions in the Tasks listed below, from EndingScene Tasks-MD v2.0.0:

- **T13**: Modify `EventSO.cs` nested `EventResult` class (add `_endingSlot` field)
- **T14**: Modify `PendingEventContext.cs` (remove `IsStageEndNode`)
- **T19**: Modify `EventPresenter.cs` (restructure `HandlePostResultAsync` from node action model perspective)

EndingScene Tasks-MD v2.0.0: https://www.notion.so/34452975d2df81ea9679fcdd8d619d25

---

## Claude Code Implementation Guide

**This Patch is reference-only. Follow EndingScene Tasks-MD v2.0.0 for the actual implementation.**
Claude Code does not need to open this Patch document directly. This Patch is issued solely to track the change history for the EventSystem Feature.

---

## Decisions Ownership

- Judgments related to this Patch (T13, T14, T19) belong to the EventSystem Feature (EventSO / PendingEventContext / EventPresenter).
- These judgments are recorded on the EventSystem Decisions page.
- Entries tagged `[BACKLOG]` / `[SPEC-GAP]` are additionally registered on the EventSystem Backlog page.

---

## Validation

- [ ] `PendingEventContext.IsStageEndNode` references: 0 project-wide (Validation V-04)
- [ ] `EventSO.EventResult` Inspector exposes `EndingSlot` field (Validation V-09)
- [ ] Runtime R-06 (EndingSlot empty → matching skipped → Stage return → StageNode Origin check → CompleteNodeAsync called), R-07 (EndingSlot populated + condition matches → EndingScene transition), R-10 (Maintenance exploration event + empty slot → Maintenance return) scenarios pass
- [ ] For full Validation, refer to the Validation / Runtime Validation sections of EndingScene Tasks-MD v2.0.0