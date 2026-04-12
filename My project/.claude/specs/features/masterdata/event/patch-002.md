# Event MasterData — Patch-002

**Status:** ✅ Confirmed
**Feature:** Event MasterData
**Patch ID:** Patch-002
**Type:** spec-change
**Related:** Specify v1.1.0 → v1.2.0 (EventSystem Plan RQ-P02)

---

## Context

Add EventSource enum field to EventSO to distinguish maintenance events from stage events. EventSource is fully independent from EventType (OneShot/Chained).

---

## Changes Required

### Target and Direction

**1. Assets/_Game/Features/Event/MasterData/EventSource.cs (NEW)**

Create new enum file:
- EventSource { Stage, Maintenance }

**2. Assets/_Game/Features/Event/MasterData/EventSO.cs**

Add field to EventSO class:
- _eventSource (EventSource, SerializeField) — Event origin. Default: Stage.
- Public read-only property: EventSource

---

## Files to Reference

- Event MasterData Specify v1.2.0

---

## Verification

- [ ] EventSource enum shows as dropdown in EventSO Inspector
- [ ] Existing EventSO .asset files load correctly with default value (Stage)

---

## Claude Code Implementation Guide

- Read CLAUDE.md first before any modification
- Files to create: Assets/_Game/Features/Event/MasterData/EventSource.cs
- Files to modify: Assets/_Game/Features/Event/MasterData/EventSO.cs
- DO NOT create files outside Assets/_Game/
- If you make any judgment calls not covered by the Patch, record them in .claude/specs/features/masterdata/event/decisions.md with [DECISION] tag