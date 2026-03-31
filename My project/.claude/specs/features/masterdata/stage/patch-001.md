# Stage MasterData — Patch-001

## Patch ID: Patch-001
## Type: spec-change
## Related: StageScene Specify v1.1.0 (RQ-08)

---

## Context

New fields are required on existing Stage MasterData SOs for StageScene implementation. Defined in StageScene Specify v1.1.0 RQ-08.

---

## Changes Required

### 1. StageNodeSO.cs
`Assets/_Game/Features/Stage/MasterData/StageNodeSO.cs`
- Add `canReturnToMain` (bool) field. [SerializeField] private, default true.
- Controls whether the player can return to MainScene at this node.

### 2. StageSO.cs
`Assets/_Game/Features/Stage/MasterData/StageSO.cs`
- Add `nextStageIds` (List<string>) field. [SerializeField] private.
- List of next stage IDs selectable after completing this stage. Empty list means no next stage (final stage).

### 3. NodeType.cs
`Assets/_Game/Features/Stage/MasterData/NodeType.cs`
- Add `Start` enum value.
- Represents stage start node. Existing values (Battle, Event) remain unchanged.

---

## Files to Reference
- StageScene Specify v1.1.0 section 4 "New Fields Required"

---

## Verification
- [ ] No compile errors in Unity console
- [ ] StageNodeSO Inspector shows canReturnToMain checkbox (default true)
- [ ] StageSO Inspector shows nextStageIds list field
- [ ] NodeType enum contains Start value

---

## Claude Code Implementation Guide
- Read CLAUDE.md first before any modification
- Files to modify:
  - `Assets/_Game/Features/Stage/MasterData/StageNodeSO.cs`
  - `Assets/_Game/Features/Stage/MasterData/StageSO.cs`
  - `Assets/_Game/Features/Stage/MasterData/NodeType.cs`
- Files to reference:
  - Existing StageNodeSO.cs, StageSO.cs, NodeType.cs (read current structure first)
- Modification order: NodeType.cs -> StageNodeSO.cs -> StageSO.cs
- DO NOT create files outside Assets/_Game/
- If you make any judgment calls not covered by this Patch, record them in .claude/specs/features/stage-masterdata/decisions.md with [DECISION] tag