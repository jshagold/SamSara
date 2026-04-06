# CharacterRepository — Patch-002

**Type:** spec-change
**Related:** EvolutionTreeScene Specify v1.0.0
**Source:** EvolutionTreeScene Specify RQ-10 — crash safety for forced reincarnation requires isReincarnationPending flag

---

## Background

EvolutionTreeScene's forced reincarnation feature requires saving a flag at the moment the user confirms reincarnation, before scene transition begins. If the app is force-closed during scene transition, the flag ensures correct routing to ReplayScene on next app launch. CharacterRunData needs an isReincarnationPending field for this purpose.

---

## Changes

### CharacterRunData.cs

- Add isReincarnationPending (bool) field

**Before:** No isReincarnationPending field
**After:**
- Add [JsonProperty] private bool _isReincarnationPending; field
- Add public bool IsReincarnationPending { get => _isReincarnationPending; set => _isReincarnationPending = value; } property
- Default value: false

> Read existing CharacterRunData field declaration/property patterns and write consistently.

---

## Validation

- Verify IsReincarnationPending field exists on CharacterRunData with working get/set
- Verify backward compatibility with existing save files (run_save.json): loading a file without the field should deserialize with default value false
- No compile errors

---

## Claude Code Implementation Guide

- Read CLAUDE.md first
- Read Assets/_Game/Features/Character/Data/CharacterRunData.cs to understand existing field patterns
- Add the isReincarnationPending field following the same pattern
- DO NOT create files outside Assets/_Game/
- Record any judgment calls in .claude/specs/features/character/character-repository/decisions.md