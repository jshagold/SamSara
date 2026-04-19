# Stage MasterData — Patch-002

**Status:** ⏳ Pending Implementation
**Feature:** Stage MasterData
**Patch ID:** Patch-002
**Type:** spec-change
**Related Documents:** EndingScene Specify v2.0.0 / Plan v2.0.0 / Tasks v2.0.0
**Date:** 2026-04-19

> Patches are not versioned. See Claude Project Instructions §3.

---

## Background

Under the EndingScene v2.0.0 redesign, a **node-local ending candidate slot structure** is introduced. Previously, ending evaluation used the `EndingTriggerKind`-based global iteration approach, but from v2.0.0 onward each battle node declares directly which ending candidate slots to attempt on victory and on defeat. To support this, `_victoryEndings` and `_defeatEndings` fields (both `EndingCandidateSlot`) are added to `BattleNodeDataSO`.

---

## Changes (Summary)

- **File**: `Assets/_Game/Features/Stage/MasterData/BattleNodeDataSO.cs`
- **Fields to add**: `_victoryEndings: EndingCandidateSlot`, `_defeatEndings: EndingCandidateSlot` (both `[SerializeField] private`)
- **Properties to add**: `VictoryEndings`, `DefeatEndings` (read-only)
- **Fields to remove (if present)**: Any boss-flag-style field should be removed (boss-ness is now expressed structurally by "which ending is placed in which slot" — Specify v2.0.0 §4-5). Claude Code must first read the file to verify whether such a field actually exists before removing.
- **using to add**: `using Samsara.Features.Ending.MasterData;` (for `EndingCandidateSlot` reference)

---

## Implementation Reference

**This Patch is reference-only.** Follow the actual implementation instructions in the Tasks listed below, from EndingScene Tasks-MD v2.0.0:

- **T12**: Modify `BattleNodeDataSO.cs` (add fields, remove boss flag, Inspector verification)
- **T1**: Create `EndingCandidateSlot.cs` (prerequisite — created within the EndingScene Feature)

EndingScene Tasks-MD v2.0.0: https://www.notion.so/34452975d2df81ea9679fcdd8d619d25

---

## Claude Code Implementation Guide

**This Patch is reference-only. Follow EndingScene Tasks-MD v2.0.0 for the actual implementation.**
Claude Code does not need to open this Patch document directly. This Patch is issued solely to track the change history for the Stage MasterData Feature.

---

## Validation

- [ ] `BattleNodeDataSO` Inspector exposes `_victoryEndings` and `_defeatEndings` slots (Validation V-08)
- [ ] The two existing BattleNodeDataSO .asset files (Boss_Test, Test) load correctly and allow slot configuration (Manual Work M-02)
- [ ] For full Validation, refer to the Validation / Runtime Validation sections of EndingScene Tasks-MD v2.0.0