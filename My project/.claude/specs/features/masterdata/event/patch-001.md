# Event MasterData — Patch-001

**Status:** ✅ Confirmed
**Feature:** Event MasterData
**Patch ID:** Patch-001
**Type:** spec-change
**Related:** Specify v1.0.0 → v1.1.0 (EventSystem Phase 5-A support)

---

## Context

Field additions to existing EventSO-related classes are required for EventSystem (Phase 5-A) implementation.
- Speaker left/right placement and name display in dialogue UI
- Specify which stat to change for StatChange results
- Chained event stage indicator UI support

---

## Changes Required

### Target and Direction

**1. Assets/_Game/Features/Event/MasterData/EventSO.cs**

Add fields to EventSO class:
- _chainTotalSteps (int, SerializeField) — Total steps in chained sequence. 0 for one-shot.
- _chainStep (int, SerializeField) — This event's step in the chain. 0 for one-shot.
- Public read-only properties: ChainTotalSteps, ChainStep

Add fields to EventDialogue struct/class:
- _speakerName (string, SerializeField) — Speaker name for dialogue box
- _speakerPosition (SpeakerPosition, SerializeField) — Portrait position
- Public read-only properties: SpeakerName, SpeakerPosition

Add fields to EventResult struct/class:
- _statType (StatType?, SerializeField) — Which stat to change for StatChange. null if not StatChange.
- Public read-only property: StatType

**2. Assets/_Game/Features/Event/MasterData/SpeakerPosition.cs (NEW)**

Create new enum file:
- SpeakerPosition { Left, Right }

---

## Files to Reference

- Event MasterData Specify v1.1.0 (Specify-KR / Specify-MD)
- Assets/_Game/Features/Character/MasterData/StatType.cs (StatType enum reference)

---

## Verification

- [ ] New fields appear correctly in EventSO Inspector
- [ ] Existing EventSO .asset files load correctly with new field defaults
- [ ] SpeakerPosition enum shows as dropdown in EventDialogue Inspector
- [ ] StatType nullable field displays correctly in EventResult Inspector

---

## Claude Code Implementation Guide

- Read CLAUDE.md first before any modification
- Files to modify: Assets/_Game/Features/Event/MasterData/EventSO.cs
- Files to create: Assets/_Game/Features/Event/MasterData/SpeakerPosition.cs
- Files to reference: Assets/_Game/Features/Character/MasterData/StatType.cs
- DO NOT create files outside Assets/_Game/
- If you make any judgment calls not covered by the Patch, record them in .claude/specs/features/masterdata/event/decisions.md with [DECISION] tag