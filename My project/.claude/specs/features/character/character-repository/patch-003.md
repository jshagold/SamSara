# CharacterRepository — Patch-003

**Type:** spec-change
**Related:** ReplayScene Specify v1.1.0 / Plan v1.0.0 (preceding work request)
**Source:** ReplayScene Plan-MD §4-2, §4-3, §11-6 — preceding patch sequence

---

## ⚠️ Retroactive Correction (2026-05-04)

**This patch was originally applied in commit `aac7142` (2026-04-21) but was incomplete.**

The "Claude Code Implementation Guide" → "Files to modify" section below
(line 121-123 in original) **omitted `Assets/_Game/Features/Character/Domain/ICharacterRunRepository.cs`**.

§3 of this patch changes the public method signature on `CharacterRunRepository`,
which implements `ICharacterRunRepository`. C# requires the interface declaration
to match the implementation; an optional parameter does NOT satisfy a no-parameter
interface contract — they are distinct overloads. CS0535 was the inevitable consequence
once a two-argument caller (`GameContext.ResetRunForReplayAsync`) was added.

**Corrective actions taken on 2026-05-04:**
1. `ICharacterRunRepository.cs:9` updated:
   `void InitializeNewRun(RunConfigSO config, int? overrideEvolutionNodeId = null);`
2. SPEC-GAP recorded as **D-04** in `.claude/specs/features/character/character-repository/decisions.md`.
3. The "Files to modify" list below should be read as if it ALSO included
   `Assets/_Game/Features/Character/Domain/ICharacterRunRepository.cs`.

**Future patches:** When changing a class's public method signature, the matching
interface file (if any) MUST be included in "Files to modify".

---

## Background

Three CharacterRepository-side changes are required as preceding work for ReplayScene implementation.

1. **Introduce LastRunResult enum**: EndingScene needs to record the run-end type (GameOver/Ending) into RunData at completion time, and ReplayScene needs to reference this on entry. A new enum type `LastRunResult { None, GameOver, Ending }` is required.

2. **Add evolutionNodeId override parameter to InitializeNewRun**: A fresh run uses RunConfigSO's default evolutionNodeId, but reincarnation through ReplayScene must start with the user-selected evolution node. The signature must be extended so that GameContext.ResetRunForReplayAsync can pass an override value at call time.

3. **Explicit initialization of new fields inside InitializeNewRun**: At new-run start, LastRunResult must be set to None and IsReincarnationPending must be set to false. They must be assigned explicitly inside the `new CharacterRunData{}` initializer so that previous values do not persist after reincarnation.

Additionally, a comment typo was found in CharacterRunRepository.cs:49 — "See GlobalBootstrapper Step 4-E" should be "See GlobalBootstrapper Step 4-A". Fixed in this Patch.

---

## Changes

### 1) LastRunResult.cs (new file)

Path: `Assets/_Game/Features/Character/MasterData/LastRunResult.cs`

- enum definition: `None`, `GameOver`, `Ending`
- Placement rationale: depends on Character Feature's runtime data, so placed under Character/MasterData/ (follows existing enum placement pattern)

**Before:** No file
**After:**

    public enum LastRunResult
    {
        None,
        GameOver,
        Ending
    }

### 2) CharacterRunData.cs (add field)

Path: `Assets/_Game/Features/Character/Data/CharacterRunData.cs`

- Add field: `public LastRunResult LastRunResult;`
- Declaration pattern: same public direct declaration style as existing CharacterRunData fields (per Decisions D-03)
- Serialization: auto-handled by JsonUtility / Newtonsoft.Json. Backward compatibility with existing save files: if key missing, deserializes to default (None).

**Before:** No LastRunResult field
**After:** `public LastRunResult LastRunResult;` added (follow existing field declaration order/style)

> Read existing CharacterRunData field declaration patterns and write consistently per D-03.

### 3) CharacterRunRepository.cs (signature change + body update)

Path: `Assets/_Game/Features/Character/Data/CharacterRunRepository.cs`

**Signature change:**

Before:

    public void InitializeNewRun(RunConfigSO config)

After:

    public void InitializeNewRun(RunConfigSO config, int? overrideEvolutionNodeId = null)

**Body update direction:**

    _runData = new CharacterRunData
    {
        EvolutionNodeId        = (overrideEvolutionNodeId ?? config.DefaultEvolutionNodeId).ToString(),
        Day                    = config.InitialDay,
        Gold                   = config.InitialGold,
        ActionPoints           = config.InitialActionPoints,
        MaxActionPoints        = config.InitialMaxActionPoints,
        LastRunResult          = LastRunResult.None,   // explicit assignment
        IsReincarnationPending = false                  // explicit assignment
        // Hp/MaxHp/Strength/Toughness/Agility are set separately by the caller
        // (GlobalBootstrapper Step 4-A or GameContext.ResetRunForReplayAsync.ApplyStatsFromEvolutionNode)
    };
    _isDirty = true;

> Actual field names / RunConfigSO property names must be confirmed by Claude Code reading the code. The pseudocode above is per ReplayScene Plan-MD §4-3.

**Call-site impact:**
- GlobalBootstrapper's existing call site omits overrideEvolutionNodeId → optional parameter preserves existing behavior
- New call site GameContext.ResetRunForReplayAsync passes the override value (this method will be added to GameContext during ReplayScene Tasks)

### 4) Comment typo fix

Path: `Assets/_Game/Features/Character/Data/CharacterRunRepository.cs:49`

- Before: `See GlobalBootstrapper Step 4-E`
- After: `See GlobalBootstrapper Step 4-A`

> Actual line number must be confirmed by Claude Code via grep for "Step 4-E".

---

## Validation

- `LastRunResult.cs` file created with enum values None / GameOver / Ending defined
- `CharacterRunData.LastRunResult` public field added, get/set behavior confirmed
- `InitializeNewRun(RunConfigSO, int?)` signature change compiles without errors
- GlobalBootstrapper existing call site compiles and runs normally (compatible via optional parameter)
- After InitializeNewRun call: RunData.LastRunResult == None, RunData.IsReincarnationPending == false
- InitializeNewRun(config, 5) call: RunData.EvolutionNodeId == "5" (override applied)
- InitializeNewRun(config) call: RunData.EvolutionNodeId == config.DefaultEvolutionNodeId.ToString() (default applied)
- grep for "Step 4-E" in CharacterRunRepository.cs returns 0 matches
- Loading existing save file (run_save.json without LastRunResult key) deserializes LastRunResult as None

---

## Claude Code Implementation Guide

- Read CLAUDE.md first
- Files to create:
    - `Assets/_Game/Features/Character/MasterData/LastRunResult.cs`
- Files to modify:
    - `Assets/_Game/Features/Character/Data/CharacterRunData.cs`
    - `Assets/_Game/Features/Character/Data/CharacterRunRepository.cs`
- Files to reference:
    - `Assets/_Game/App/GlobalBootstrapper.cs` (existing InitializeNewRun call site at ~line 135)
    - `Assets/_Game/App/RunConfigSO.cs` (DefaultEvolutionNodeId / InitialDay / InitialGold / InitialActionPoints / InitialMaxActionPoints field names)
- Verify CharacterRunData existing field declaration pattern (Decisions D-03 — public direct declaration) before adding LastRunResult field
- After signature change, verify GlobalBootstrapper.cs call site (~line 135) still compiles without changes
- DO NOT create files outside `Assets/_Game/`
- If you make any judgment calls not covered by this Patch, record them in `.claude/specs/character/character-repository/decisions.md` with appropriate tags ([DECISION], [BACKLOG], [SPEC-GAP])