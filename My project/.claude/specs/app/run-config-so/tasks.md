# RunConfigSO — Tasks

**Version:** 1.0.0 | **Date:** 2026-04-14 | **Status:** Confirmed

---

## Task 1: Create RunConfigSO Class

**File:** `Assets/_Game/App/RunConfigSO.cs` (NEW)

**Implementation:**
- Inherit from `ScriptableObject`
- Add `[CreateAssetMenu(fileName = "NewRunConfig", menuName = "Samsara/RunConfigSO")]` attribute
- All fields use `[SerializeField] private` + public getter property (Constitution §8)
- Include `private readonly string _logClass = $"[{nameof(RunConfigSO)}]";` (Constitution §8 LogTag)

**Field List:**

Character section:
- `_defaultEvolutionNodeId` (int, default 1)
- `_initialGold` (int, default 0)
- `_initialDay` (int, default 1)
- `_initialActionPoints` (int, default 3)
- `_initialMaxActionPoints` (int, default 3)

Stage section:
- `_startStageId` (int, default 1)
- `_startNodeIndex` (int, default 0)

Shop section:
- `_initialMerchantAvailable` (bool, default false)

Inventory section:
- `_inventorySlotCount` (int, default 3)

**Constitution Checks:**
- §7 Reset(): Not needed — SO is not a MonoBehaviour View
- §8 Naming: `_camelCase` private fields, `PascalCase` public getters
- §8 SerializeField: Use `[SerializeField] private`, no `public` fields
- §8 LogTag: Included
- §8 SafeCleanup: No OnDestroy needed for SO
- §8 GC: No Update() — N/A

---

## Task 2: Modify GameContext

**File:** `Assets/_Game/App/GameContext.cs` (MODIFY)

**Read the current file before making any changes.**

**Changes:**
- Add `RunConfigSO runConfig` parameter to constructor
- Add `public RunConfigSO RunConfig { get; }` property
- Assign `RunConfig = runConfig;` inside constructor

**Constitution Checks:**
- §6: GameContext remains a pure C# class. RunConfigSO is a read-only reference.
- §2: Not a Singleton. Constructor injection.

---

## Task 3: Modify GlobalBootstrapper

**File:** `Assets/_Game/App/GlobalBootstrapper.cs` (MODIFY)

**Read the current file before making any changes.**

**Changes:**
- Inside `InitializeAsync()`, after MasterData load phase, add RunConfigSO load:

    var runConfig = Resources.Load<RunConfigSO>("MasterData/DefaultRunConfig");
    if (runConfig == null)
        throw new InvalidOperationException(
            $"{_logClass} DefaultRunConfig not found in Resources/MasterData/");

- `Resources.Load` MUST be called on the main thread (cannot be inside UniTask.RunOnThreadPool)
- Add `runConfig` parameter to GameContext constructor call

**Constitution Checks:**
- §7 Fail Fast: Throw InvalidOperationException on null
- §3: GlobalBootstrapper is the only Singleton. Maintain existing structure.

---

## Task 4: Modify CharacterRunRepository

**File:** `Assets/_Game/Features/Character/Data/CharacterRunRepository.cs` (MODIFY)

**Read the current file before making any changes.**

**Changes:**
- Change `InitializeNewRun()` signature to `InitializeNewRun(RunConfigSO config)`
- Replace hardcoded initial values with `config.DefaultEvolutionNodeId`, `config.InitialGold`, `config.InitialDay`, `config.InitialActionPoints`, `config.InitialMaxActionPoints`
- Update interface (ICharacterRunRepository) signature accordingly

**Constitution Checks:**
- §9 Save Strategy: InitializeNewRun is a data change — SaveDataAsync() needed after call. However, the caller (Phase 6) is responsible. This Task only changes the signature.
- §8 SafeCleanup: Maintain existing OnDestroy pattern

---

## Task 5: Modify StageRunRepository

**File:** `Assets/_Game/Features/Stage/Data/StageRunRepository.cs` (MODIFY)

**Read the current file before making any changes.**

**Changes:**
- Change `InitializeNewRun()` signature to `InitializeNewRun(RunConfigSO config)`
- Replace hardcoded initial values with `config.StartStageId`, `config.StartNodeIndex`
- Update interface (IStageRunRepository) signature accordingly

---

## Task 6: Modify ShopRepository

**File:** ShopRepository related files (MODIFY)

**Read the current file before making any changes.**

**Changes:**
- Change `InitializeNewRun()` signature to `InitializeNewRun(RunConfigSO config)`
- Replace hardcoded initial values with `config.InitialMerchantAvailable`
- Update interface signature accordingly

---

## Task 7: Update InitializeNewRun Call Sites

**Search for all existing call sites before making changes.**

If any code currently calls `InitializeNewRun()` (e.g., Dev/ debug tools), update to pass `GameContext.RunConfig` as the config parameter with the new signature `InitializeNewRun(config)`.

---

## Manual Tasks

| ID | Task | Description |
|---|---|---|
| M-01 | Create DefaultRunConfig.asset | In Unity Editor, under `Assets/Resources/MasterData/`, Create > Samsara > RunConfigSO. Enter default values per Specify §2 |

---

## Validation

| ID | Item |
|---|---|
| V-01 | All RunConfigSO fields are visible and editable in Inspector |
| V-02 | GlobalBootstrapper loads RunConfigSO successfully during initialization |
| V-03 | InvalidOperationException fires when RunConfigSO asset is missing |
| V-04 | GameContext.RunConfig property is accessible |
| V-05 | Each Repository's InitializeNewRun(config) correctly applies config values |
| V-06 | All previously hardcoded initial values have been removed |
| V-07 | Project compiles without errors |

---

## Claude Code Implementation Guide

- Read CLAUDE.md first before any implementation
- Files to create: `Assets/_Game/App/RunConfigSO.cs`
- Files to modify: `GlobalBootstrapper.cs`, `GameContext.cs`, `CharacterRunRepository.cs` (+ interface), `StageRunRepository.cs` (+ interface), ShopRepository (+ interface), existing InitializeNewRun call sites
- Files to reference: Read each target file's current implementation first
- Implementation order: Task 1 -> Task 2 -> Task 3 -> Task 4 -> Task 5 -> Task 6 -> Task 7
- DO NOT create files outside `Assets/_Game/`
- If you make any judgment calls not covered by the Spec, record them in `.claude/specs/app/run-config-so/decisions.md` with appropriate tags: [DECISION], [BACKLOG], or [SPEC-GAP]