# Stage MasterData — Tasks

**Version:** 1.0.0 | **Date:** 2026-03-17 | **Status:** Ready for Claude Code
**Based on:** Specify v1.1.0 / Plan v1.0.0

---

## Pre-Implementation Checklist

> Claude Code MUST verify the following before starting any task.

- [ ] Read `CLAUDE.md` first
- [ ] Create `Assets/_Game/Features/Stage/MasterData/` folder if missing
- [ ] Create `Assets/Resources/MasterData/Stage/` folder if missing
- [ ] DO NOT modify any files not explicitly listed below

---

## TASK-01 — NodeType.cs

**Path:** `Assets/_Game/Features/Stage/MasterData/NodeType.cs`
**Type:** Enum
**Priority:** First (no dependencies)

### Implementation

    namespace Samsara.Features.Stage.MasterData
    {
        public enum NodeType
        {
            Battle,
            Event,
            Boss
        }
    }

---

## TASK-02 — StageSO.cs

**Path:** `Assets/_Game/Features/Stage/MasterData/StageSO.cs`
**Type:** ScriptableObject
**Priority:** After TASK-01

### Implementation

    using System;
    using UnityEngine;
    
    namespace Samsara.Features.Stage.MasterData
    {
        [CreateAssetMenu(fileName = "StageSO", menuName = "Samsara/Stage/Stage")]
        public class StageSO : ScriptableObject
        {
            private readonly string _logClass = $"[{nameof(StageSO)}]";
    
            [SerializeField] private string _stageId;
            [SerializeField] private string _stageName;
            [SerializeField] private string _backgroundSpriteKey;
            [SerializeField] private string _backgroundMusicKey;
            [SerializeField] private bool _isFixed;
            [SerializeField] private StageNodeSO[] _fixedNodes;
            [SerializeField] private StageNodePool _randomNodePool;
    
            public string StageId => _stageId;
            public string StageName => _stageName;
            public string BackgroundSpriteKey => _backgroundSpriteKey;
            public string BackgroundMusicKey => _backgroundMusicKey;
            public bool IsFixed => _isFixed;
            public StageNodeSO[] FixedNodes => _fixedNodes;
            public StageNodePool RandomNodePool => _randomNodePool;
        }
    
        [Serializable]
        public class StageNodePool
        {
            [SerializeField] private int _minBattleNodes;
            [SerializeField] private int _maxBattleNodes;
            [SerializeField] private int _minEventNodes;
            [SerializeField] private int _maxEventNodes;
            [SerializeField] private int _bossNodeCount;
    
            public int MinBattleNodes => _minBattleNodes;
            public int MaxBattleNodes => _maxBattleNodes;
            public int MinEventNodes => _minEventNodes;
            public int MaxEventNodes => _maxEventNodes;
            public int BossNodeCount => _bossNodeCount;
        }
    }

---

## TASK-03 — StageNodeSO.cs

**Path:** `Assets/_Game/Features/Stage/MasterData/StageNodeSO.cs`
**Type:** ScriptableObject
**Priority:** After TASK-01, TASK-02

### Implementation

    using UnityEngine;
    
    namespace Samsara.Features.Stage.MasterData
    {
        [CreateAssetMenu(fileName = "StageNodeSO", menuName = "Samsara/Stage/StageNode")]
        public class StageNodeSO : ScriptableObject
        {
            private readonly string _logClass = $"[{nameof(StageNodeSO)}]";
    
            [SerializeField] private string _nodeId;
            [SerializeField] private NodeType _nodeType;
            [SerializeField] private string _nodeSpriteKey;
            [SerializeField] private BattleNodeDataSO _battleData;
            [SerializeField] private EventNodeDataSO _eventData;
    
            public string NodeId => _nodeId;
            public NodeType NodeType => _nodeType;
            public string NodeSpriteKey => _nodeSpriteKey;
            public BattleNodeDataSO BattleData => _battleData;
            public EventNodeDataSO EventData => _eventData;
        }
    }

---

## TASK-04 — BattleNodeDataSO.cs

**Path:** `Assets/_Game/Features/Stage/MasterData/BattleNodeDataSO.cs`
**Type:** ScriptableObject
**Priority:** After TASK-01

### Implementation

    using System;
    using UnityEngine;
    
    namespace Samsara.Features.Stage.MasterData
    {
        [CreateAssetMenu(fileName = "BattleNodeDataSO", menuName = "Samsara/Stage/BattleNodeData")]
        public class BattleNodeDataSO : ScriptableObject
        {
            private readonly string _logClass = $"[{nameof(BattleNodeDataSO)}]";
    
            [SerializeField] private bool _isBoss;
            [SerializeField] private EnemySpawn[] _enemySpawns;
    
            public bool IsBoss => _isBoss;
            public EnemySpawn[] EnemySpawns => _enemySpawns;
        }
    
        [Serializable]
        public class EnemySpawn
        {
            [SerializeField] private int _enemyId;
            [SerializeField] private int _count;
    
            public int EnemyId => _enemyId;
            public int Count => _count;
        }
    }

---

## TASK-05 — EventNodeDataSO.cs

**Path:** `Assets/_Game/Features/Stage/MasterData/EventNodeDataSO.cs`
**Type:** ScriptableObject
**Priority:** After TASK-01

### Implementation

    using UnityEngine;
    
    namespace Samsara.Features.Stage.MasterData
    {
        [CreateAssetMenu(fileName = "EventNodeDataSO", menuName = "Samsara/Stage/EventNodeData")]
        public class EventNodeDataSO : ScriptableObject
        {
            private readonly string _logClass = $"[{nameof(EventNodeDataSO)}]";
    
            [SerializeField] private int _eventId;
    
            public int EventId => _eventId;
        }
    }

---

## TASK-06 — Validation

**Priority:** Final — after all tasks complete.

| # | Check | Method |
|---|---|---|
| V-01 | No compile errors in Unity console | Console check |
| V-02 | 4 items appear in `Assets > Create` menu: `Samsara/Stage/Stage`, `Samsara/Stage/StageNode`, `Samsara/Stage/BattleNodeData`, `Samsara/Stage/EventNodeData` | Editor check |
| V-03 | `StageSO` .asset shows IsFixed, FixedNodes array, and RandomNodePool fields in Inspector | Inspector check |
| V-04 | `StageSO` RandomNodePool shows MinBattleNodes, MaxBattleNodes, MinEventNodes, MaxEventNodes, BossNodeCount fields | Inspector check |
| V-05 | `StageNodeSO` NodeType enum shows Battle/Event/Boss options | Inspector check |
| V-06 | `StageNodeSO` BattleData field accepts BattleNodeDataSO .asset via drag-and-drop | Inspector check |
| V-07 | `BattleNodeDataSO` EnemySpawns array entries show EnemyId and Count fields | Inspector check |
| V-08 | `EventNodeDataSO` .asset shows EventId field in Inspector | Inspector check |

---

## Claude Code Implementation Guide

- Read `CLAUDE.md` first before any implementation.
- **Files to create:**
  - `Assets/_Game/Features/Stage/MasterData/NodeType.cs`
  - `Assets/_Game/Features/Stage/MasterData/StageSO.cs`
  - `Assets/_Game/Features/Stage/MasterData/StageNodeSO.cs`
  - `Assets/_Game/Features/Stage/MasterData/BattleNodeDataSO.cs`
  - `Assets/_Game/Features/Stage/MasterData/EventNodeDataSO.cs`
- **Implementation order:** TASK-01 → TASK-02 → TASK-03 → TASK-04 → TASK-05 → TASK-06
- **DO NOT** create files outside `Assets/_Game/`.
- **DO NOT** reference or copy patterns from `Assets/_Game/Dev/`.
- If you make any judgment calls not covered by this Spec,
  record them in `.claude/specs/masterdata/stage/decisions.md`