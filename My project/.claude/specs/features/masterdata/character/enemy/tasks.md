# Enemy MasterData — Tasks

**Version:** 1.0.0 | **Date:** 2026-03-17 | **Status:** Ready for Claude Code
**Based on:** Specify v1.0.0 / Plan v1.0.0

---

## Pre-Implementation Checklist

> Claude Code MUST verify the following before starting any task.

- [ ] Read `CLAUDE.md` first
- [ ] Confirm `Assets/_Game/Core/MasterData/` folder exists (already contains CharacterStatsSO, SkillSO, etc.)
- [ ] Create `Assets/Resources/MasterData/Character/Enemy/` folder if missing
- [ ] Confirm `Assets/_Game/Features/Character/MasterData/CharacterStatsSO.cs` exists
- [ ] DO NOT modify any files not explicitly listed below

---

## TASK-01 — EnemySO.cs

**Path:** `Assets/_Game/Core/MasterData/EnemySO.cs`
**Type:** ScriptableObject
**Priority:** First (no dependencies)

### Implementation

    using UnityEngine;
    using Samsara.Features.Character.MasterData;
    
    namespace Samsara.Core.MasterData
    {
        [CreateAssetMenu(fileName = "EnemySO", menuName = "Samsara/Core/Enemy")]
        public class EnemySO : ScriptableObject
        {
            private readonly string _logClass = $"[{nameof(EnemySO)}]";
    
            [SerializeField] private int _enemyId;
            [SerializeField] private string _enemyName;
            [SerializeField] private CharacterStatsSO _baseStats;
            [SerializeField] private int[] _skillIds;  // SkillSO ID references
    
            // Sprite fields (Addressables keys)
            [SerializeField] private string _battleSpriteKeyHp100;
            [SerializeField] private string _battleSpriteKeyHp50;
            [SerializeField] private string _battleSpriteKeyHp0;
    
            public int EnemyId => _enemyId;
            public string EnemyName => _enemyName;
            public CharacterStatsSO BaseStats => _baseStats;
            public int[] SkillIds => _skillIds;
            public string BattleSpriteKeyHp100 => _battleSpriteKeyHp100;
            public string BattleSpriteKeyHp50 => _battleSpriteKeyHp50;
            public string BattleSpriteKeyHp0 => _battleSpriteKeyHp0;
        }
    }

---

## TASK-02 — Validation

**Priority:** Final — after all tasks complete.

| # | Check | Method |
|---|---|---|
| V-01 | No compile errors in Unity console | Console check |
| V-02 | `Assets > Create` menu shows `Samsara/Core/Enemy` | Editor check |
| V-03 | `EnemySO` .asset shows EnemyId, EnemyName, BaseStats, SkillIds, and 3 sprite key fields in Inspector | Inspector check |
| V-04 | `EnemySO` BaseStats field accepts CharacterStatsSO .asset via drag-and-drop | Inspector check |
| V-05 | `EnemySO` SkillIds array accepts int values | Inspector check |

---

## Claude Code Implementation Guide

- Read `CLAUDE.md` first before any implementation.
- **Files to create:**
  - `Assets/_Game/Core/MasterData/EnemySO.cs`
- **Files to reference:**
  - `Assets/_Game/Features/Character/MasterData/CharacterStatsSO.cs`
- **Implementation order:** TASK-01 → TASK-02
- **DO NOT** create files outside `Assets/_Game/`.
- **DO NOT** reference or copy patterns from `Assets/_Game/Dev/`.
- If you make any judgment calls not covered by this Spec,
  record them in `.claude/specs/features/masterdata/character/enemy/decisions.md`