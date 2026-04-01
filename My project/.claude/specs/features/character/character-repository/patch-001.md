# CharacterRepository — Patch-001

**Type:** spec-change
**Related:** Specify v1.0.0 → v1.1.0
**Source:** MiniGame Decisions D-01 [SPEC-GAP]

## Background

When MiniGameUseCase.ApplyResultAndSave() directly modifies CharacterRunData fields then calls SaveDataAsync(), the CharacterRunRepository internal `_isDirty` flag remains false, causing the save to be skipped.

The Dirty Flag pattern (Constitution §9) only sets dirty when data is changed through Repository's own methods. External direct field modifications do not trigger the dirty flag.

## Changes

### ICharacterRunRepository.cs
- Add `void MarkDirty()` method to the interface

**Before:** No MarkDirty() method
**After:** `void MarkDirty();` added to interface

### CharacterRunRepository.cs
- Implement `MarkDirty()` — sets `_isDirty = true;`

**Before:** No MarkDirty() implementation
**After:** `public void MarkDirty() { _isDirty = true; }` added

## Claude Code Implementation Guide

- Read CLAUDE.md first
- Files to modify:
    - `Assets/_Game/Features/Character/Domain/ICharacterRunRepository.cs`
    - `Assets/_Game/Features/Character/Data/CharacterRunRepository.cs`
- Files to reference:
    - `Assets/_Game/Features/MiniGame/Domain/MiniGameUseCase.cs` (caller of MarkDirty)
- After modification, verify MiniGameUseCase.ApplyResultAndSave() calls MarkDirty() before SaveDataAsync()
- Record any judgment calls in `.claude/specs/features/character-repository/decisions.md`

## Validation

- MiniGameUseCase.ApplyResultAndSave() calls MarkDirty() then SaveDataAsync() → file write actually occurs
- Existing Repository usage patterns (MaintenanceUseCase, etc.) are unaffected