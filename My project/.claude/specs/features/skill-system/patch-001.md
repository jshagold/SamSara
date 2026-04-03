# SkillSystem — Patch-001

## Patch ID: Patch-001
## Type: spec-change
## Related: Specify v1.1.0

---

## Context

During BattleScene Specify design, it was determined that combat cooldown management
responsibility should not reside in SkillUseCase. Cooldowns are logic dependent on
combat flow (tick-based action order, action timing), and having SkillUseCase manage
them creates a dependency on battle logic. SkillUseCase should focus on its pure data
service role: skill MasterData queries.

Therefore, all combat cooldown-related responsibilities (Specify §5-2 through §5-5)
and runtime state are removed from SkillUseCase. Cooldown management will be handled
by BattleUseCase when BattleScene is implemented.

---

## Changes Required

### 1. Remove combat runtime features from SkillUseCase

`Assets/_Game/Features/Skill/Domain/SkillUseCase.cs`
- Remove InitializeBattle() method
- Remove EndBattle() method
- Remove CanUseSkill() method (cooldown check + HP cost check are both combat logic)
- Remove ConsumeSkill() method
- Remove TickCooldowns() method
- Remove SkillRuntimeData reference and internal state

### 2. Delete SkillRuntimeData

`Assets/_Game/Features/Skill/Domain/SkillRuntimeData.cs`
- This class was exclusively for combat session cooldown state. Delete it entirely.

### 3. Features that remain unchanged (confirmation only)
- LoadAsync() — MasterData load
- GetSkillById() — single SkillSO lookup by ID
- GetSkillsByIds() — bulk SkillSO lookup by ID array
- GetQTEPatternById() — single QTEPatternSO lookup by ID
- GetSkillsForEvolutionNode() — evolution node skill list query

### 4. GameContext — no change
- GameContext's SkillUseCase accessor is retained (G-11). Used by other Features
  for MasterData queries.

---

## Files to Reference
- SkillSystem Specify-MD v1.1.0 (Notion page)
- BattleScene Specify-KR v1.0.0 §2-3 (cooldown management is BattleUseCase responsibility)

---

## Verification
- [ ] InitializeBattle, EndBattle, CanUseSkill, ConsumeSkill, TickCooldowns methods
      are all removed from SkillUseCase
- [ ] SkillRuntimeData.cs file is deleted
- [ ] SkillUseCase MasterData query features (LoadAsync, GetSkillById, GetSkillsByIds,
      GetQTEPatternById, GetSkillsForEvolutionNode) work correctly
- [ ] GameContext SkillUseCase accessor works correctly
- [ ] No compile errors in Unity build

---

## Claude Code Implementation Guide
- Read CLAUDE.md first before any modification
- Files to modify: Assets/_Game/Features/Skill/Domain/SkillUseCase.cs
- Files to delete: Assets/_Game/Features/Skill/Domain/SkillRuntimeData.cs
- Files to reference: SkillSystem Specify-MD v1.1.0 from Notion
- After removing methods, check for any remaining references to deleted methods
  or SkillRuntimeData across the project (grep for method names and class name)
- DO NOT create files outside Assets/_Game/
- If you make any judgment calls not covered by this Patch,
  record them in .claude/specs/skill-system/decisions.md
  with [DECISION] tag