# Battle Test Data Creator

**Purpose:** Dev-only editor script to auto-create test SO assets for BattleScene testing.
**Location:** Assets/_Game/Dev/Features/Battle/BattleTestDataCreator.cs
**This is NOT a formal Spec. No Notion documentation required.**

---

## What to Create

An editor-only script (`#if UNITY_EDITOR`) that adds a Unity menu item:
**Samsara > Dev > Create Test Battle Data**

When clicked, creates the following SO assets under `Assets/Resources/MasterData/`:

### QTEPatternSO (2)

**qte_test_single** (patternId = 9001)
- QTEData[1]: spriteKey="qte_tap", coordinate=(0.5, 0.5), duration=1.0, intervalToNext=0

**qte_test_triple** (patternId = 9002)
- QTEData[3]:
  - spriteKey="qte_tap", coordinate=(0.3, 0.5), duration=0.8, intervalToNext=0.3
  - spriteKey="qte_tap", coordinate=(0.5, 0.5), duration=0.7, intervalToNext=0.3
  - spriteKey="qte_tap", coordinate=(0.7, 0.5), duration=0.6, intervalToNext=0

### SkillSO (3)

**skill_test_basic** (skillId = 9001)
- Basic attack. No QTE pattern (qtePatternId = 0 or -1, check SkillSO field).
- CoolDown = 0. No HP cost. Damage multiplier = 1.0.

**skill_test_heavy** (skillId = 9002)
- Heavy attack with QTE. qtePatternId = 9001 (qte_test_single).
- CoolDown = 2. No HP cost. Damage multiplier = 1.8.

**skill_test_drain** (skillId = 9003)
- HP drain attack with QTE. qtePatternId = 9002 (qte_test_triple).
- CoolDown = 3. HP cost (check CostType field). Damage multiplier = 2.5.

### EnemySO (3)

**enemy_test_weak** (enemyId = 9001)
- Stats: HP=50, Strength=8, Toughness=5, Agility=20
- SkillIds = [9001] (basic only)

**enemy_test_normal** (enemyId = 9002)
- Stats: HP=80, Strength=12, Toughness=8, Agility=35
- SkillIds = [9001, 9002] (basic + heavy)

**enemy_test_boss** (enemyId = 9003)
- Stats: HP=200, Strength=20, Toughness=15, Agility=50
- SkillIds = [9001, 9002, 9003] (all three)

### BattleNodeDataSO (2)

**battlenode_test_normal** (battleNodeId = 9001)
- Enemy list: [enemy_test_weak, enemy_test_normal]
- isBoss = false

**battlenode_test_boss** (battleNodeId = 9002)
- Enemy list: [enemy_test_boss]
- isBoss = true

### EvolutionNodeSO Update

Find the existing test EvolutionNodeSO asset (check _evolutionNodeId used in DebugRunDataEditor).
Set its _skillIds = [9001, 9002, 9003].
If no test EvolutionNodeSO exists, log a warning.

---

## Implementation Notes

- Wrap entire class in `#if UNITY_EDITOR` / `#endif`
- Use `using UnityEditor;`
- Use `AssetDatabase.CreateAsset()` + `AssetDatabase.SaveAssets()` + `AssetDatabase.Refresh()`
- If asset with same name already exists at target path, skip and log: "[BattleTestDataCreator] Skipped: {path} already exists"
- Read actual SO field names from source code before setting values (DO NOT guess field names)
- Must read: SkillSO.cs, QTEPatternSO.cs, EnemySO.cs, BattleNodeDataSO.cs, EvolutionNodeSO.cs
- All test asset IDs use 9000+ range to avoid collision with real data