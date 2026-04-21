# InventorySystem — Patch-001

**Type:** refactor
**Related:** ReplayScene Specify v1.1.0 / Plan v1.0.0 (preceding work request), InventorySystem Decisions D-02 [SPEC-GAP]
**Source:** ReplayScene Plan-MD §4-4, §11-6 — preceding patch sequence

---

## Background

For ReplayScene implementation, `GameContext.ResetRunForReplayAsync` must call the initialization method of every RunData Repository with a consistent signature.

Currently only InventoryRepository uses the `ResetRunData()` signature, while all other RunData Repositories (CharacterRunRepository, StageRepository, ShopRepository) use the `InitializeNewRun(RunConfigSO config)` pattern. This asymmetry forces inconsistent call-site code in `GameContext.ResetRunForReplayAsync`.

This Patch unifies the InventoryRepository signature to match other RunData Repositories: `InitializeNewRun(RunConfigSO config)`.

This work resolves the connectability gap noted in InventorySystem Decisions **D-02 [SPEC-GAP]** ("GameContext has no ResetAllRunData() or Reincarnation method. Connection needed when such method is added.") — by the time ReplayScene is implemented, GameContext.ResetRunForReplayAsync will be able to invoke InventoryRepository through the unified signature.

---

## Changes

### 1) IInventoryRepository.cs (signature change)

Path: `Assets/_Game/Features/Inventory/Domain/IInventoryRepository.cs`

**Signature change:**

    // Before
    void ResetRunData();

    // After
    void InitializeNewRun(RunConfigSO config);

> Add `using` for RunConfigSO. Exact namespace must be confirmed by Claude Code based on RunConfigSO's actual location.

### 2) InventoryRepository.cs (implementation signature + body update)

Path: `Assets/_Game/Features/Inventory/Data/InventoryRepository.cs`

**Signature change:**

    // Before
    public void ResetRunData()

    // After
    public void InitializeNewRun(RunConfigSO config)

**Body update direction:**

- Preserve the existing `ResetRunData()` body logic (clear all slots: ItemId=-1, Quantity=0)
- Use RunConfigSO parameter where applicable
- If RunConfigSO defines slot count (e.g., `config.InventorySlotCount`), use it
- If RunConfigSO has no inventory-related fields, keep the existing hardcoded 3-slot behavior; only unify the signature so the structure is consistent for future extension
- Maintain `_isDirty = true` setting

> Actual RunConfigSO field names and the existence of inventory-related initialization fields must be determined by Claude Code after inspecting RunConfigSO and the existing `InventoryRepository.ResetRunData()` body. If RunConfigSO has no inventory slot count field, keep existing hardcoded values.

### 3) Existing call-site cleanup

**Scope:** All `ResetRunData()` call sites

**Action:**
- If any callers exist, update them to `InitializeNewRun(_runConfig)` form
- If no callers exist (likely, since GameContext currently has no ResetAllRunData method), no further changes needed

> Claude Code must grep for `ResetRunData(` across the codebase and update all call sites uniformly.

---

## Validation

- `IInventoryRepository.InitializeNewRun(RunConfigSO)` signature change complete
- `IInventoryRepository` no longer declares `ResetRunData()` method
- `InventoryRepository.InitializeNewRun(RunConfigSO)` implementation signature change complete
- After calling new `InitializeNewRun`, all slots are empty (ItemId=-1, Quantity=0)
- `_isDirty = true` is set
- grep for `ResetRunData(` across codebase returns 0 matches (Inventory scope)
- No compile errors

---

## Claude Code Implementation Guide

- Read CLAUDE.md first
- Files to modify:
    - `Assets/_Game/Features/Inventory/Domain/IInventoryRepository.cs`
    - `Assets/_Game/Features/Inventory/Data/InventoryRepository.cs`
- Files to reference:
    - `Assets/_Game/App/RunConfigSO.cs` (verify: any inventory-related fields, namespace)
    - `Assets/_Game/Features/Character/Data/CharacterRunRepository.cs` (InitializeNewRun pattern reference)
- Verify current `InventoryRepository.ResetRunData()` body before signature change to preserve existing reset logic
- Check RunConfigSO for any inventory-related fields (e.g., InventorySlotCount). If absent, keep existing hardcoded slot count and just unify the signature.
- Grep for `ResetRunData(` across codebase and update all callers (likely none, since GameContext has no such call yet)
- DO NOT create files outside `Assets/_Game/`
- If you make any judgment calls not covered by this Patch, record them in `.claude/specs/features/inventory-system/decisions.md` with appropriate tags ([DECISION], [BACKLOG], [SPEC-GAP])