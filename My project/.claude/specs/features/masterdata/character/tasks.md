# Character MasterData — Tasks

**Version:** 1.0.0 | **Date:** 2026-03-17 | **Status:** Ready for Claude Code
**Based on:** Specify v1.2.0 / Plan v1.0.0

---

## Pre-Implementation Checklist

> Claude Code MUST verify the following before starting any task.

- [ ] Read `CLAUDE.md` first
- [ ] Create `Assets/_Game/Features/Character/MasterData/` folder if missing
- [ ] Create `Assets/_Game/Core/MasterData/` folder if missing
- [ ] Create `Assets/Resources/MasterData/Character/` folder if missing
- [ ] Create `Assets/Resources/MasterData/Shared/` folder if missing
- [ ] DO NOT modify any files not explicitly listed below

---

## TASK-01 — CharacterStatsSO.cs

**Path:** `Assets/_Game/Features/Character/MasterData/CharacterStatsSO.cs`
**Type:** ScriptableObject
**Priority:** First (no dependencies)

### Implementation

    using UnityEngine;
    
    namespace Samsara.Features.Character.MasterData
    {
        [CreateAssetMenu(fileName = "CharacterStatsSO", menuName = "Samsara/Character/CharacterStats")]
        public class CharacterStatsSO : ScriptableObject
        {
            private readonly string _logClass = $"[{nameof(CharacterStatsSO)}]";
    
            [SerializeField] private int _hp;
            [SerializeField] private int _strength;
            [SerializeField] private int _toughness;
            [SerializeField] private int _speed;
    
            public int Hp => _hp;
            public int Strength => _strength;
            public int Toughness => _toughness;
            public int Speed => _speed;
        }
    }

---

## TASK-02 — StatType.cs

**Path:** `Assets/_Game/Features/Character/MasterData/StatType.cs`
**Type:** Enum
**Priority:** After TASK-01

### Implementation

    namespace Samsara.Features.Character.MasterData
    {
        public enum StatType
        {
            Hp,
            Strength,
            Toughness,
            Speed
        }
    }

---

## TASK-03 — EvolutionNodeSO.cs

**Path:** `Assets/_Game/Features/Character/MasterData/EvolutionNodeSO.cs`
**Type:** ScriptableObject
**Priority:** After TASK-01, TASK-02

### Implementation

    using System;
    using UnityEngine;
    
    namespace Samsara.Features.Character.MasterData
    {
        [CreateAssetMenu(fileName = "EvolutionNodeSO", menuName = "Samsara/Character/EvolutionNode")]
        public class EvolutionNodeSO : ScriptableObject
        {
            private readonly string _logClass = $"[{nameof(EvolutionNodeSO)}]";
    
            [SerializeField] private string _nodeId;
            [SerializeField] private string _characterName;
            [SerializeField] private CharacterStatsSO _baseStats;
            [SerializeField] private int _maxActionPoints;
            [SerializeField] private StatCondition[] _unlockConditions;
            [SerializeField] private bool _isHidden;
            [SerializeField] private EvolutionNodeSO[] _nextNodes;
    
            // Sprite fields (Addressables keys)
            [SerializeField] private string _nodeIconSpriteKey;
            [SerializeField] private string _mainStandingSpriteKey;
            [SerializeField] private string _portraitSpriteKey;
            [SerializeField] private string _battleSpriteKeyHp100;
            [SerializeField] private string _battleSpriteKeyHp50;
            [SerializeField] private string _battleSpriteKeyHp0;
            [SerializeField] private string _attackAnimSpriteKey;
            [SerializeField] private string _stageMoveSpriteKey;
    
            public string NodeId => _nodeId;
            public string CharacterName => _characterName;
            public CharacterStatsSO BaseStats => _baseStats;
            public int MaxActionPoints => _maxActionPoints;
            public StatCondition[] UnlockConditions => _unlockConditions;
            public bool IsHidden => _isHidden;
            public EvolutionNodeSO[] NextNodes => _nextNodes;
            public string NodeIconSpriteKey => _nodeIconSpriteKey;
            public string MainStandingSpriteKey => _mainStandingSpriteKey;
            public string PortraitSpriteKey => _portraitSpriteKey;
            public string BattleSpriteKeyHp100 => _battleSpriteKeyHp100;
            public string BattleSpriteKeyHp50 => _battleSpriteKeyHp50;
            public string BattleSpriteKeyHp0 => _battleSpriteKeyHp0;
            public string AttackAnimSpriteKey => _attackAnimSpriteKey;
            public string StageMoveSpriteKey => _stageMoveSpriteKey;
        }
    
        [Serializable]
        public class StatCondition
        {
            [SerializeField] private StatType _statType;
            [SerializeField] private int _requiredValue;
    
            public StatType StatType => _statType;
            public int RequiredValue => _requiredValue;
        }
    }

---

## TASK-04 — CostType.cs

**Path:** `Assets/_Game/Core/MasterData/CostType.cs`
**Type:** Enum
**Priority:** No dependencies (can parallel with TASK-01~03)

### Implementation

    namespace Samsara.Core.MasterData
    {
        public enum CostType
        {
            CoolDown,
            Hp
        }
    }

---

## TASK-05 — EffectType.cs

**Path:** `Assets/_Game/Core/MasterData/EffectType.cs`
**Type:** Enum
**Priority:** No dependencies (can parallel with TASK-01~03)

### Implementation

    namespace Samsara.Core.MasterData
    {
        // Phase 1: Stun only. Extensible in later phases.
        public enum EffectType
        {
            Stun
        }
    }

---

## TASK-06 — SkillSO.cs

**Path:** `Assets/_Game/Core/MasterData/SkillSO.cs`
**Type:** ScriptableObject
**Priority:** After TASK-04, TASK-05

### Implementation

    using System;
    using UnityEngine;
    
    namespace Samsara.Core.MasterData
    {
        [CreateAssetMenu(fileName = "SkillSO", menuName = "Samsara/Core/Skill")]
        public class SkillSO : ScriptableObject
        {
            private readonly string _logClass = $"[{nameof(SkillSO)}]";
    
            [SerializeField] private int _skillId;
            [SerializeField] private string _skillName;
            [SerializeField] private string _description;
            [SerializeField] private float _damage;
            [SerializeField] private int _qtePatternId;
            [SerializeField] private SkillCost[] _costs;
            [SerializeField] private SkillEffect[] _effects;
            [SerializeField] private string _iconSpriteKey;
            [SerializeField] private string _effectSpriteKey;
    
            public int SkillId => _skillId;
            public string SkillName => _skillName;
            public string Description => _description;
            public float Damage => _damage;
            public int QtePatternId => _qtePatternId;
            public SkillCost[] Costs => _costs;
            public SkillEffect[] Effects => _effects;
            public string IconSpriteKey => _iconSpriteKey;
            public string EffectSpriteKey => _effectSpriteKey;
        }
    
        [Serializable]
        public class SkillCost
        {
            [SerializeField] private CostType _costType;
            [SerializeField] private float _value;
    
            public CostType CostType => _costType;
            public float Value => _value;
        }
    
        [Serializable]
        public class SkillEffect
        {
            [SerializeField] private EffectType _effectType;
            [SerializeField] private float _duration;
    
            public EffectType EffectType => _effectType;
            public float Duration => _duration;
        }
    }

---

## TASK-07 — QTEPatternSO.cs

**Path:** `Assets/_Game/Core/MasterData/QTEPatternSO.cs`
**Type:** ScriptableObject
**Priority:** After TASK-04, TASK-05

### Implementation

    using System;
    using UnityEngine;
    
    namespace Samsara.Core.MasterData
    {
        [CreateAssetMenu(fileName = "QTEPatternSO", menuName = "Samsara/Core/QTEPattern")]
        public class QTEPatternSO : ScriptableObject
        {
            private readonly string _logClass = $"[{nameof(QTEPatternSO)}]";
    
            [SerializeField] private int _patternId;
            [SerializeField] private QTEData[] _qteDataList;
    
            public int PatternId => _patternId;
            public QTEData[] QteDataList => _qteDataList;
        }
    
        [Serializable]
        public class QTEData
        {
            [SerializeField] private string _spriteKey;
            [SerializeField] private Vector2 _coordinate;  // ratio value (0-1)
            [SerializeField] private float _duration;
            [SerializeField] private float _intervalToNext;
    
            public string SpriteKey => _spriteKey;
            public Vector2 Coordinate => _coordinate;
            public float Duration => _duration;
            public float IntervalToNext => _intervalToNext;
        }
    }

---

## TASK-08 — Validation

**Priority:** Final — after all tasks complete.

| # | Check | Method |
|---|---|---|
| V-01 | No compile errors in Unity console | Console check |
| V-02 | 4 items appear in `Assets > Create` menu: `Samsara/Character/CharacterStats`, `Samsara/Character/EvolutionNode`, `Samsara/Core/Skill`, `Samsara/Core/QTEPattern` | Editor check |
| V-03 | `CharacterStatsSO` .asset shows Hp, Strength, Toughness, Speed fields in Inspector | Inspector check |
| V-04 | `EvolutionNodeSO` .asset shows base fields + 8 sprite key fields + StatCondition array in Inspector | Inspector check |
| V-05 | `EvolutionNodeSO` `_baseStats` field accepts CharacterStatsSO .asset via drag-and-drop | Inspector check |
| V-06 | `EvolutionNodeSO` `_nextNodes` array accepts other EvolutionNodeSO .asset references | Inspector check |
| V-07 | `SkillSO` SkillCost and SkillEffect array entries show correct fields when added | Inspector check |
| V-08 | `QTEPatternSO` QTEData array entries show SpriteKey, Coordinate (Vector2), Duration, IntervalToNext when added | Inspector check |

---

## Claude Code Implementation Guide

- Read `CLAUDE.md` first before any implementation.
- **Files to create:**
  - `Assets/_Game/Features/Character/MasterData/CharacterStatsSO.cs`
  - `Assets/_Game/Features/Character/MasterData/StatType.cs`
  - `Assets/_Game/Features/Character/MasterData/EvolutionNodeSO.cs`
  - `Assets/_Game/Core/MasterData/CostType.cs`
  - `Assets/_Game/Core/MasterData/EffectType.cs`
  - `Assets/_Game/Core/MasterData/SkillSO.cs`
  - `Assets/_Game/Core/MasterData/QTEPatternSO.cs`
- **Implementation order:** TASK-01 → TASK-02 → TASK-03 → TASK-04 → TASK-05 → TASK-06 → TASK-07 → TASK-08
- **DO NOT** create files outside `Assets/_Game/`.
- **DO NOT** reference or copy patterns from `Assets/_Game/Dev/`.
- If you make any judgment calls not covered by this Spec,
  record them in `.claude/specs/character-masterdata/decisions.md`