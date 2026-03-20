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
