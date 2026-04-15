# SpriteLoader — Tasks

**Version:** 1.0.0 | **Date:** 2026-04-15 | **Status:** Confirmed
**Feature:** SpriteLoader (Phase X — Asset Loading System)
**Constitution Ref:** §1~§11 (all sections)
**Based on:** Specify v1.0.0, Plan v1.0.0

---

## 1. Overview

Implementation instructions for SpriteLoader infrastructure + full migration of existing Features.
Proceed in order: Plan Part A (infrastructure) then Part B (migration).

---

## 2. Prerequisites

- Read CLAUDE.md first before any implementation
- Read the following files to understand current structure:
  - Assets/_Game/App/GameContext.cs
  - Assets/_Game/App/GlobalBootstrapper.cs
- Create folder: Assets/_Game/Core/AssetLoading/
- Create folder: .claude/specs/core/sprite-loader/

---

## 3. Files to Create/Modify

**Part A — Infrastructure (in order)**

| Order | File | Path | Action |
|---|---|---|---|
| 1 | ISpriteLoader.cs | Assets/_Game/Core/AssetLoading/ | Create |
| 2 | AddressableSpriteLoader.cs | Assets/_Game/App/ | Create |
| 3 | GameContext.cs | Assets/_Game/App/ | Modify |
| 4 | GlobalBootstrapper.cs | Assets/_Game/App/ | Modify |

**Part B — SO Modifications**

| Order | File | Path | Action |
|---|---|---|---|
| 5 | PotionSO.cs | Assets/_Game/Features/Shop/MasterData/ | Modify |
| 6 | MerchantSO.cs | Assets/_Game/Features/Shop/MasterData/ | Modify |
| 7 | EvolutionNodeSO.cs | Assets/_Game/Features/Character/MasterData/ | Modify |
| 8 | EnemySO.cs | Assets/_Game/Core/MasterData/ | Modify |

**Part B — View Modifications**

| Order | File | Path | Action |
|---|---|---|---|
| 9 | CharacterSpriteView.cs | Assets/_Game/Features/MainScene/Presentation/Main/ | Modify |
| 10 | CharacterSpriteView.cs | MaintenanceScene (verify actual path) | Modify |
| 11 | BackgroundView.cs | Assets/_Game/Features/MainScene/Presentation/Main/ | Modify |
| 12 | BackgroundView.cs | MaintenanceScene (verify actual path) | Modify |
| 13 | CharacterUnitView.cs | Assets/_Game/Features/BattleScene/Presentation/Field/ | Modify |
| 14 | NodeMapView.cs | StageScene (verify actual path) | Modify |
| 15 | EvolutionNodeView.cs | EvolutionTreeScene (verify actual path) | Modify |

**Part B — Presenter/Bootstrapper Modifications (per Feature)**

| Order | File | Path | Action |
|---|---|---|---|
| 16 | MainPresenter.cs + MainSceneBootstrapper.cs | MainScene Presentation/ | Modify |
| 17 | MaintenancePresenter.cs + MaintenanceSceneBootstrapper.cs | MaintenanceScene Presentation/ | Modify |
| 18 | CharacterInfoPresenter.cs + CharacterInfoSceneBootstrapper.cs | CharacterInfoScene Presentation/ | Modify |
| 19 | EvolutionTreePresenter.cs + EvolutionTreeSceneBootstrapper.cs | EvolutionTreeScene Presentation/ | Modify |
| 20 | BattlePresenter.cs + BattleSceneBootstrapper.cs | BattleScene Presentation/ | Modify |
| 21 | StagePresenter.cs + StageSceneBootstrapper.cs | StageScene Presentation/ | Modify |
| 22 | EventPresenter.cs + EventSceneBootstrapper.cs | EventSystem Presentation/ (verify actual path) | Modify |

**decisions.md**

| Order | File | Path | Action |
|---|---|---|---|
| 23 | decisions.md | .claude/specs/core/sprite-loader/ | Create (empty) |

> **IMPORTANT:** Files marked "verify actual path" must have their actual paths confirmed before proceeding. If path differs, record in decisions.md.

---

## 4. Implementation Instructions

### Task 1 — Create ISpriteLoader.cs

Read: None (new file)

Create Assets/_Game/Core/AssetLoading/ISpriteLoader.cs.

- namespace: Samsara.Core.AssetLoading
- using Cysharp.Threading.Tasks; (Constitution §11)
- using UnityEngine;
- Interface methods (4):
  - UniTask<Sprite> LoadSpriteAsync(string key) — async load single Sprite
  - UniTask PreloadSpritesAsync(string[] keys) — bulk preload multiple Sprites
  - void ReleaseSpriteAsync(string key) — release specific Sprite (Phase 1: no-op)
  - void ReleaseAllAsync() — release all cached Sprites (Phase 1: no-op)

---

### Task 2 — Create AddressableSpriteLoader.cs

Read: ISpriteLoader.cs from Task 1

Create Assets/_Game/App/AddressableSpriteLoader.cs.

- namespace: Samsara.App
- using Samsara.Core.AssetLoading;
- using Cysharp.Threading.Tasks;
- using UnityEngine;
- using UnityEngine.AddressableAssets;
- using System.Collections.Generic;
- Pure C# class (not MonoBehaviour). Implements ISpriteLoader.
- Constitution §8: private readonly string _logClass = $"[{nameof(AddressableSpriteLoader)}]";
- private readonly Dictionary<string, Sprite> _cache = new Dictionary<string, Sprite>();
- Constructor: no parameters.

**LoadSpriteAsync implementation:**
1. If key is null or empty: Debug.LogWarning($"{_logClass} LoadSpriteAsync: key is null or empty") + return null
2. _cache.TryGetValue(key, out var cached) -> if found, return cached
3. try-catch: Addressables.LoadAssetAsync<Sprite>(key), await via .ToUniTask()
4. On success: _cache[key] = sprite -> return sprite
5. On failure: Debug.LogError($"{_logClass} LoadSpriteAsync failed for key: {key} -- {e.Message}") -> return null

**PreloadSpritesAsync implementation:**
1. If keys is null or empty, return immediately
2. Create UniTask array calling LoadSpriteAsync(key) for each key
3. await UniTask.WhenAll(tasks)

**ReleaseSpriteAsync, ReleaseAllAsync:**
- Empty methods (no-op). Comment: "Phase 1: no-op. Future implementation for reference counting."

---

### Task 3 — Modify GameContext.cs

Read: Assets/_Game/App/GameContext.cs (read entire current content)

**Changes:**
1. Add using Samsara.Core.AssetLoading;
2. Add ISpriteLoader spriteLoader parameter to constructor (as last parameter)
3. Add public ISpriteLoader SpriteLoader { get; } property
4. Assign SpriteLoader = spriteLoader; in constructor body

Do NOT modify any existing code. Parameter addition and property addition only.

---

### Task 4 — Modify GlobalBootstrapper.cs

Read: Assets/_Game/App/GlobalBootstrapper.cs (read entire current content)

**Changes:**
1. Add using Samsara.Core.AssetLoading;
2. Verify using for AddressableSpriteLoader access (same namespace may not need it)
3. Add var spriteLoader = new AddressableSpriteLoader(); just before GameContext creation
4. Add spriteLoader parameter to GameContext creation call

Do NOT modify other init steps.

---

### Task 5 — Modify PotionSO.cs

Read: Assets/_Game/Features/Shop/MasterData/PotionSO.cs (read entire current content)

**Changes:**
1. [SerializeField] private Sprite _sprite; -> [SerializeField] private string _spriteKey;
2. Public getter change: public Sprite Sprite => _sprite; -> public string SpriteKey => _spriteKey;

---

### Task 6 — Modify MerchantSO.cs

Read: Assets/_Game/Features/Shop/MasterData/MerchantSO.cs (read entire current content)

**Changes:**
1. [SerializeField] private Sprite _portrait; -> [SerializeField] private string _portraitKey;
2. [SerializeField] private Sprite _shopSprite; -> [SerializeField] private string _shopSpriteKey;
3. Public getter changes: Portrait -> PortraitKey, ShopSprite -> ShopSpriteKey (return type string)

---

### Task 7 — Modify EvolutionNodeSO.cs

Read: Assets/_Game/Features/Character/MasterData/EvolutionNodeSO.cs (read entire current content)

**Changes:**
1. Delete [SerializeField] private Sprite _nodeIconSprite; field
2. Delete public Sprite NodeIconSprite => _nodeIconSprite; property
3. Keep existing _nodeIconSpriteKey (string) field and getter — this becomes the canonical reference
4. Other sprite key fields (7) are already string — no change

---

### Task 8 — Modify EnemySO.cs

Read: Assets/_Game/Core/MasterData/EnemySO.cs (read entire current content)

**Changes:**
1. Add [SerializeField] private string _portraitSpriteKey; field
2. Add public string PortraitSpriteKey => _portraitSpriteKey; getter
3. Existing battleSpriteKey fields unchanged

---

### Task 9 — Modify MainScene CharacterSpriteView.cs

Read: Assets/_Game/Features/MainScene/Presentation/Main/CharacterSpriteView.cs (read entire current content)

**Changes:**
1. Remove entire internal Addressables call code from existing SetSprite(string addressableKey) method
2. Delete _spriteHandle field if present
3. Delete Addressables.Release calls in OnDestroy()/Dispose() if present
4. New method: public void SetSprite(Sprite sprite) — _characterImage.sprite = sprite; only
5. Remove using UnityEngine.AddressableAssets;
6. Remove using Cysharp.Threading.Tasks; if not used elsewhere
7. Constitution §7 Reset(): keep existing. Modify only if needed.
8. Constitution §8 Safe Cleanup: verify ?. usage in OnDestroy().

---

### Task 10 — Modify MaintenanceScene CharacterSpriteView.cs

Read: Verify actual file path first (explore MaintenanceScene folder structure)

Apply same change pattern as Task 9. Read file content and process identically.

---

### Task 11 — Modify MainScene BackgroundView.cs

Read: Assets/_Game/Features/MainScene/Presentation/Main/BackgroundView.cs (read entire current content)

**Changes:**
1. Add public void SetBackground(Sprite sprite) method
2. Internal: _backgroundImage.sprite = sprite; (verify existing _backgroundImage field)
3. Do not modify existing fields/methods. Addition only.

---

### Task 12 — Modify MaintenanceScene BackgroundView.cs

Read: Verify actual file path first

Apply same change pattern as Task 11.

---

### Task 13 — Modify BattleScene CharacterUnitView.cs

Read: Assets/_Game/Features/BattleScene/Presentation/Field/CharacterUnitView.cs (read entire current content)

**Changes:**
1. Remove entire #if UNITY_EDITOR ... #endif block with AssetDatabase.LoadAssetAtPath
2. Remove using UnityEditor;
3. Replace existing sprite load method with: public void SetSprite(Sprite sprite) sync setter
4. Internal: _characterSprite.sprite = sprite;
5. Keep existing null check logic for sprite if any

---

### Task 14 — Modify StageScene NodeMapView.cs

Read: Verify actual file path first. Read NodeMapView.cs entire content.

**Changes:**
1. [SerializeField] private Sprite[] _nodeTypeIcons; -> [SerializeField] private string[] _nodeTypeIconKeys;
2. Change _nodeTypeIcons to private Sprite[] (non-serialized)
3. Add public void SetNodeTypeIcons(Sprite[] icons) method — _nodeTypeIcons = icons;
4. Keep existing NodeType index access logic
5. Add public string[] NodeTypeIconKeys => _nodeTypeIconKeys; getter

---

### Task 15 — Modify EvolutionTreeScene EvolutionNodeView.cs

Read: Verify actual file path first. Read EvolutionNodeView.cs entire content.

**Changes:**
1. Remove 5 frame Sprite SerializeField (actual field names determined from code)
2. Remove question mark Sprite SerializeField (actual field name from code)
3. Change above 6 to non-serialized private fields
4. Add public void SetUISprites(...) method for external injection — parameters match existing code usage pattern
5. Keep existing Setup() method logic using these Sprites. Only change: SerializeField -> injection

> **IMPORTANT:** Actual field names and usage patterns determined after reading code. If different from Decisions-based estimates, record in decisions.md.

---

### Task 16 — Modify MainScene Presenter/Bootstrapper

Read:
- Assets/_Game/Features/MainScene/Presentation/MainPresenter.cs
- Assets/_Game/Features/MainScene/Presentation/MainSceneBootstrapper.cs

**MainSceneBootstrapper changes:**
1. Pass gameContext.SpriteLoader to MainPresenter constructor

**MainPresenter changes:**
1. Add ISpriteLoader spriteLoader parameter to constructor
2. Add private readonly ISpriteLoader _spriteLoader; field
3. Add using Samsara.Core.AssetLoading;
4. In Initialize() or InitializeAsync():
   - Find current character's EvolutionNodeSO from GameContext.EvolutionNodes (may already exist in code)
   - _spriteLoader.LoadSpriteAsync(evolutionNode.MainStandingSpriteKey) -> _view.CharacterSpriteView.SetSprite(sprite)
   - Background: read existing code for background handling, pass result to _view.BackgroundView.SetBackground(sprite)
   - CharacterStatusView portrait: _spriteLoader.LoadSpriteAsync(evolutionNode.PortraitSpriteKey) -> call View SetPortrait method

> **NOTE:** Read existing Presenter code first. If sprite-related code already exists, replace with SpriteLoader. If adding new load logic, record in decisions.md.

---

### Task 17 — Modify MaintenanceScene Presenter/Bootstrapper

Read: MaintenancePresenter.cs, MaintenanceSceneBootstrapper.cs (verify actual paths)

Apply same pattern as Task 16. Additionally:
- Merchant sprites: MerchantSO PortraitKey, ShopSpriteKey via SpriteLoader
- ShopItemSlotView: PotionSO.Sprite direct reference -> PotionSO.SpriteKey + SpriteLoader
- Read existing code first to determine change scope

---

### Task 18 — Modify CharacterInfoScene Presenter/Bootstrapper

Read: CharacterInfoPresenter.cs, CharacterInfoSceneBootstrapper.cs (verify actual paths)

**Changes:**
1. Bootstrapper: inject gameContext.SpriteLoader to Presenter
2. Presenter: replace null placeholders with actual SpriteLoader calls:
   - Character sprite: evolutionNode.MainStandingSpriteKey -> LoadSpriteAsync -> View.SetSprite()
   - Skill icons: each SkillSO IconSpriteKey -> LoadSpriteAsync -> pass to View
3. Replace existing SetSprite(null) calls with actual loads

---

### Task 19 — Modify EvolutionTreeScene Presenter/Bootstrapper

Read: EvolutionTreePresenter.cs, EvolutionTreeSceneBootstrapper.cs (verify actual paths)

**Changes:**
1. Bootstrapper: inject gameContext.SpriteLoader to Presenter
2. Bootstrapper: define 6 UI sprite keys as constants or [SerializeField] private string[]:
   - 5 frame keys + 1 question mark key (verify actual filenames in Assets/_Game/Art/Sprites/)
3. Presenter InitializeAsync():
   - _spriteLoader.PreloadSpritesAsync(uiSpriteKeys) for 6 UI sprites
   - After preload, pass to each EvolutionNodeView via SetUISprites()
   - Node icons: _spriteLoader.LoadSpriteAsync(node.NodeIconSpriteKey) -> Setup()
   - Skill icons: each SkillSO IconSpriteKey -> LoadSpriteAsync

---

### Task 20 — Modify BattleScene Presenter/Bootstrapper

Read: BattlePresenter.cs, BattleSceneBootstrapper.cs

**Changes:**
1. Bootstrapper: inject gameContext.SpriteLoader to Presenter
2. Presenter:
   - Ally sprite: EvolutionNodeSO BattleSpriteKeyHp100 -> LoadSpriteAsync -> CharacterUnitView.SetSprite()
   - Enemy sprite: EnemySO BattleSpriteKeyHp100 -> LoadSpriteAsync -> CharacterUnitView.SetSprite()
   - HP-based sprite swap: load Hp100/Hp50/Hp0 keys via SpriteLoader -> SetSprite()
   - Action order portraits: ally EvolutionNodeSO.PortraitSpriteKey, enemy EnemySO.PortraitSpriteKey -> LoadSpriteAsync
   - Skill icons: SkillSO.IconSpriteKey -> LoadSpriteAsync
3. Remove #if UNITY_EDITOR related code in Presenter if any

---

### Task 21 — Modify StageScene Presenter/Bootstrapper

Read: StagePresenter.cs, StageSceneBootstrapper.cs (verify actual paths)

**Changes:**
1. Bootstrapper: inject gameContext.SpriteLoader to Presenter
2. Presenter InitializeAsync():
   - Background: StageSO.BackgroundSpriteKey -> _spriteLoader.LoadSpriteAsync() -> _view.SetBackground(sprite)
   - Replace existing null or missing background code with actual load
   - Node type icons: read NodeMapView.NodeTypeIconKeys (string[]), _spriteLoader.PreloadSpritesAsync() -> pass loaded Sprite[] to NodeMapView.SetNodeTypeIcons(icons)
   - Node icons: StageNodeSO NodeSpriteKey -> LoadSpriteAsync -> NodeView

---

### Task 22 — Modify EventScene Presenter/Bootstrapper

Read: EventPresenter.cs, EventSceneBootstrapper.cs (verify actual paths)

**Changes:**
1. Bootstrapper: inject gameContext.SpriteLoader to Presenter
2. Presenter:
   - Background: replace Resources.Load<Sprite>(key) -> await _spriteLoader.LoadSpriteAsync(key)
   - Remove Resources-related code (keep if used elsewhere)
   - Portraits: EventDialogue PortraitSpriteKey -> _spriteLoader.LoadSpriteAsync() -> PortraitView.SetPortrait()
   - Check how existing dialogue loop handles portraits before applying changes

---

### Task 23 — Create decisions.md

Create empty file at .claude/specs/core/sprite-loader/decisions.md.

---

## 5. Full Code Inspection Directive

> **CRITICAL:** The Task list above is based on Decisions records and Spec/Plan analysis. When executing each Task, you MUST **read the target file and related files first** and verify:
>
> 1. Are there additional direct Sprite references (SerializeField Sprite, Resources.Load, AssetDatabase) not described above?
> 2. If found, migrate immediately using the same pattern and record in decisions.md with [DECISION] tag
> 3. If SO Sprite fields differ from Spec definitions, record with [SPEC-GAP] tag
> 4. If migration scope expands significantly (e.g., new interface changes needed), stop work, record in decisions.md, and request confirmation from Hak

---

## 6. Validation

| # | Item | Method |
|---|---|---|
| V-01 | No compile errors in Unity console | Console check |
| V-02 | ISpriteLoader interface exists in Core/AssetLoading | File check |
| V-03 | AddressableSpriteLoader exists in App/ and implements ISpriteLoader | File check |
| V-04 | GameContext.SpriteLoader property accessible | Code check |
| V-05 | PotionSO, MerchantSO have no direct Sprite fields | Code check |
| V-06 | EvolutionNodeSO has no _nodeIconSprite field | Code check |
| V-07 | EnemySO has _portraitSpriteKey field | Code check |
| V-08 | CharacterSpriteView (both) have no direct Addressables calls | Code check |
| V-09 | BattleScene CharacterUnitView has no #if UNITY_EDITOR AssetDatabase code | Code check |
| V-10 | EventPresenter has no Resources.Load calls | Code check |
| V-11 | All Presenters load Sprites through ISpriteLoader | Code check |
| V-12 | No direct Sprite references/Resources.Load/AssetDatabase usage in entire project (grep check) | grep check |

---

## 7. Manual Tasks (Hak — after Claude Code implementation)

| Order | Task |
|---|---|
| M-01 | Install Unity Addressables package (Package Manager) |
| M-02 | Create Addressables Settings (Window > Asset Management > Addressables > Groups) |
| M-03 | Mark all Sprite files under Assets/_Game/Art/Sprites/ as Addressable |
| M-04 | Set each Sprite's Addressable Address to filename without extension |
| M-05 | Create Sprites group, configure local mode |
| M-06 | PotionSO .asset x4: enter Addressable addresses in _spriteKey field |
| M-07 | MerchantSO .asset: enter addresses in _portraitKey, _shopSpriteKey fields |
| M-08 | EvolutionNodeSO .asset all: verify/enter addresses in _nodeIconSpriteKey field |
| M-09 | EnemySO .asset all: enter addresses in new _portraitSpriteKey field |
| M-10 | EvolutionTreeScene: enter UI sprite key constants/SerializeField values in EvolutionTreeSceneBootstrapper |
| M-11 | StageScene: enter 4 keys in NodeMapView _nodeTypeIconKeys string[] Inspector |
| M-12 | Other .asset files: enter values in any empty string key fields |
| M-13 | Play test: verify Sprites display correctly in each scene |

---

## 8. Claude Code Delivery Guide

- Run claude from project root
- CLAUDE.md loads automatically
- Deliver .claude/specs/core/sprite-loader/tasks.md for sequential implementation
- Each Task MUST read target files first before modifying
- Record judgment calls in .claude/specs/core/sprite-loader/decisions.md with [DECISION], [BACKLOG], or [SPEC-GAP] tags
- DO NOT create files outside Assets/_Game/ (except decisions.md)