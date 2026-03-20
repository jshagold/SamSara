# StageRepository — Tasks

**Version:** 1.0.0 | **Date:** 2026-03-20 | **Status:** ✅ Confirmed
**Feature:** StageRepository
**Phase:** 1 — Data Layer
**Constitution Ref:** §3 (Bootstrapper Hierarchy), §6 (GameContext / Data Lifetime Separation), §8 (Coding Standards), §9 (Data Persistence)

---

## 1. Overview

Implementation instruction set for StageRepository. Based on the design confirmed in Plan. Claude Code executes these tasks in order.

---

## 2. Prerequisites

- Read `CLAUDE.md` before any implementation
- Verify `Assets/_Game/Features/Stage/Domain/` exists (create if missing)
- Verify `Assets/_Game/Features/Stage/Data/` exists (create if missing)
- Read existing `GameContext.cs` before modifying
- Check `Assets/_Game/Features/Stage/Data/StageRepository.cs` — overwrite if empty

---

## 3. Files to Create / Modify

| Order | File | Path | Action |
| --- | --- | --- | --- |
| 1 | IStageRepository.cs | Features/Stage/Domain/ | Create |
| 2 | IStageMasterDataRepository.cs | Features/Stage/Domain/ | Create |
| 3 | StageRunData.cs | Features/Stage/Data/ | Create |
| 4 | StageRepository.cs | Features/Stage/Data/ | Create (overwrite empty file) |
| 5 | StageMasterDataRepository.cs | Features/Stage/Data/ | Create |
| 6 | GameContext.cs | App/ | Modify |

---

## 4. Implementation Instructions

### Task 1 — IStageRepository.cs

**Path**: `Assets/_Game/Features/Stage/Domain/IStageRepository.cs`
**Namespace**: `Samsara.Features.Stage.Domain`

    public interface IStageRepository
    {
        StageRunData RunData { get; }
        void InitializeRun(string startStageId);
        void CompleteNode(int nodeIndex);
        void TransitionToStage(string stageId);
        void SetGeneratedNodes(List<string> nodeIds);
        UniTask SaveAsync();
        void SaveSync();
        UniTask LoadAsync();
    }

---

### Task 2 — IStageMasterDataRepository.cs

**Path**: `Assets/_Game/Features/Stage/Domain/IStageMasterDataRepository.cs`
**Namespace**: `Samsara.Features.Stage.Domain`

    public interface IStageMasterDataRepository
    {
        void Initialize();
        StageSO GetStageById(string id);
        StageNodeSO GetNodeById(string id);
        IReadOnlyList<StageSO> GetAllStages();
    }

---

### Task 3 — StageRunData.cs

**Path**: `Assets/_Game/Features/Stage/Data/StageRunData.cs`
**Namespace**: `Samsara.Features.Stage.Data`

Plain data class. No logic. Serialized with Newtonsoft.Json.

    public class StageRunData
    {
        public string CurrentStageId;
        public int CurrentNodeIndex;
        public List<string> GeneratedNodeIds = new();
        public List<int> CompletedNodeIndices = new();
    }

---

### Task 4 — StageRepository.cs

**Path**: `Assets/_Game/Features/Stage/Data/StageRepository.cs`
**Namespace**: `Samsara.Features.Stage.Data`

- Include `_logClass` field (Constitution §8)
- Save path constant: `Application.persistentDataPath + "/stage_run_save.json"`
- `_isDirty` bool field for Dirty Flag
- `InitializeRun()`: set CurrentStageId, CurrentNodeIndex=0, clear lists, `_isDirty = true` → call `SaveAsync()`
- `CompleteNode()`: add nodeIndex to CompletedNodeIndices, advance CurrentNodeIndex, `_isDirty = true` → call `SaveAsync()`
- `TransitionToStage()`: update CurrentStageId, CurrentNodeIndex=0, clear GeneratedNodeIds and CompletedNodeIndices, `_isDirty = true` → call `SaveAsync()`
- `SetGeneratedNodes()`: assign GeneratedNodeIds, `_isDirty = true` → call `SaveAsync()`
- `LoadAsync()`: deserialize file if exists; create default `StageRunData` if not
- `SaveAsync()`: check `_isDirty` → serialize on `UniTask.RunOnThreadPool` → write file → `_isDirty = false`
- `SaveSync()`: check `_isDirty` → synchronous file write → `_isDirty = false`

---

### Task 5 — StageMasterDataRepository.cs

**Path**: `Assets/_Game/Features/Stage/Data/StageMasterDataRepository.cs`
**Namespace**: `Samsara.Features.Stage.Data`

- Include `_logClass` field (Constitution §8)
- `_stageCache`: `Dictionary<string, StageSO>`
- `_nodeCache`: `Dictionary<string, StageNodeSO>`
- `Initialize()`: call on main thread only. Load all SOs via `Resources.LoadAll<StageSO>()` and `Resources.LoadAll<StageNodeSO>()`. Store in dictionaries keyed by id field.
- `GetStageById()`: return from `_stageCache`. Throw exception if key not found.
- `GetNodeById()`: return from `_nodeCache`. Throw exception if key not found.
- `GetAllStages()`: return `_stageCache.Values` as `IReadOnlyList<StageSO>`
- No save or serialization logic.

---

### Task 6 — GameContext.cs (Modify)

Add the following to existing `Assets/_Game/App/GameContext.cs`:

- Declare `IStageRepository` and `IStageMasterDataRepository` fields with public properties
- In constructor: instantiate `new StageRepository()` and `new StageMasterDataRepository()`, assign to fields
- In `InitializeAsync()`: call `StageMasterDataRepository.Initialize()` synchronously on the main thread, before `UniTask.WhenAll`
- In `LoadAllDataAsync()`: add `StageRepository.LoadAsync()` to the `UniTask.WhenAll` batch
- In `SaveAllDataSync()`: add `StageRepository.SaveSync()` call (`StageMasterDataRepository` requires no save)

---

## 5. Validation

Verify the following after implementation.

| # | Check | Method |
| --- | --- | --- |
| V-01 | No compile errors in Unity console | Console |
| V-02 | StageRepository and StageMasterDataRepository registered in GameContext | Runtime debug log |
| V-03 | `LoadAsync()` initializes with defaults when no save file exists | Play mode |
| V-04 | `stage_run_save.json` created in `persistentDataPath` after `SaveAsync()` | File system |
| V-05 | Saved values restored correctly on next run | Play mode |
| V-06 | No file I/O occurs when Dirty Flag is false | Log |
| V-07 | `TransitionToStage()` clears GeneratedNodeIds and CompletedNodeIndices | Play mode |
| V-08 | `GetStageById()` throws exception for unknown ID | Play mode |
| V-09 | All StageSOs loaded into cache after `Initialize()` | Log |

---

## 6. Claude Code Handoff

- Run `claude` from project root
- `CLAUDE.md` is loaded automatically
- Pass this Tasks file and request implementation in order
- Record any judgment calls not covered by this spec in `.claude/specs/features/stage/stage-repository/decisions.md`
- DO NOT create files outside `Assets/_Game/`