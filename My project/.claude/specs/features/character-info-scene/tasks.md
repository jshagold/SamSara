# CharacterInfoScene — Tasks

**Feature:** CharacterInfoScene
**Phase:** 4 — Character System
**Version:** 1.0.0
**Date:** 2026-04-06
**Status:** Confirmed
**Constitution Reference:** §2, §3, §4, §5, §7, §8, §10, §11

---

## 1. Overview

Implementation guide for Claude Code to build the CharacterInfoScene Feature. Based on the class structure defined in the Plan, this document covers the order and method for creating actual files.

**Note:** SceneKey enum must have `CharacterInfo` and `EvolutionTree` values added first (handled in Task 1).

---

## 2. Prerequisites

- Read `CLAUDE.md` first
- Read the following existing files before implementation:
    - `Assets/_Game/Features/Character/Data/CharacterRunData.cs`
    - `Assets/_Game/Features/Character/Domain/ICharacterRunRepository.cs`
    - `Assets/_Game/Features/Character/MasterData/CharacterStatsSO.cs`
    - `Assets/_Game/Features/Character/MasterData/EvolutionNodeSO.cs`
    - `Assets/_Game/Features/Character/MasterData/StatType.cs`
    - `Assets/_Game/Features/Skill/Domain/ISkillMasterDataRepository.cs`
    - `Assets/_Game/Features/Skill/Domain/SkillUseCase.cs`
    - `Assets/_Game/Core/MasterData/SkillSO.cs`
    - `Assets/_Game/App/GameContext.cs`
    - `Assets/_Game/Core/Navigation/ISceneNavigator.cs`
    - `Assets/_Game/Core/Navigation/SceneKey.cs`
    - `Assets/_Game/Core/Popup/IPopupManager.cs`
    - `Assets/_Game/Features/MainScene/Presentation/MainSceneBootstrapper.cs` (reference existing pattern)
- Create folder: `Assets/_Game/Features/CharacterInfoScene/`
- Create folder: `.claude/specs/features/character-info-scene/`

---

## 3. Files to Create/Modify

| Order | File | Path | Action |
|---|---|---|---|
| 1 | SceneKey.cs | Assets/_Game/Core/Navigation/ | Modify |
| 2 | CharacterInfoUseCase.cs | Assets/_Game/Features/CharacterInfoScene/Domain/ | Create |
| 3 | BackButtonView.cs | Assets/_Game/Features/CharacterInfoScene/Presentation/TopBar/ | Create |
| 4 | OptionButtonView.cs | Assets/_Game/Features/CharacterInfoScene/Presentation/TopBar/ | Create |
| 5 | CharacterSpriteView.cs | Assets/_Game/Features/CharacterInfoScene/Presentation/CharacterArea/ | Create |
| 6 | EvolutionStageButtonView.cs | Assets/_Game/Features/CharacterInfoScene/Presentation/CharacterArea/ | Create |
| 7 | CharacterNameView.cs | Assets/_Game/Features/CharacterInfoScene/Presentation/InfoScroll/ | Create |
| 8 | StatListView.cs | Assets/_Game/Features/CharacterInfoScene/Presentation/InfoScroll/ | Create |
| 9 | SkillSlotView.cs | Assets/_Game/Features/CharacterInfoScene/Presentation/InfoScroll/ | Create |
| 10 | SkillListView.cs | Assets/_Game/Features/CharacterInfoScene/Presentation/InfoScroll/ | Create |
| 11 | InventoryView.cs | Assets/_Game/Features/CharacterInfoScene/Presentation/InfoScroll/ | Create |
| 12 | InfoScrollView.cs | Assets/_Game/Features/CharacterInfoScene/Presentation/InfoScroll/ | Create |
| 13 | SkillDescriptionPopupView.cs | Assets/_Game/Features/CharacterInfoScene/Presentation/Popup/ | Create |
| 14 | CharacterInfoView.cs | Assets/_Game/Features/CharacterInfoScene/Presentation/ | Create |
| 15 | CharacterInfoPresenter.cs | Assets/_Game/Features/CharacterInfoScene/Presentation/ | Create |
| 16 | CharacterInfoSceneBootstrapper.cs | Assets/_Game/Features/CharacterInfoScene/Presentation/ | Create |
| 17 | decisions.md | .claude/specs/features/character-info-scene/ | Create (empty) |

---

## 4. Implementation Order and Instructions

### Task 1 — Modify SceneKey.cs
Add `CharacterInfo` and `EvolutionTree` entries to the `SceneKey` enum.

---

### Task 2 — Create CharacterInfoUseCase.cs
- Namespace: `Samsara.Features.CharacterInfoScene.Domain`
- Pure C# class (not MonoBehaviour). §2 Data/Logic separation.
- Constructor injection: `ICharacterRunRepository`, `ISkillMasterDataRepository`, `EvolutionNodeSO[]` (MasterData cache array).
- `_logClass` required (§8).
- `GetCurrentStats()`: Returns HP, Strength, Toughness, Agility from `CharacterRunData`. Define `CharacterInfoStatsData` struct in the same file (int hp, int strength, int toughness, int agility).
- `GetCurrentEvolutionNode()`: Searches `EvolutionNodeSO[]` array using `CharacterRunData.currentEvolutionNodeId`. Throws `InvalidOperationException` if not found (§7 Fail Fast).
- `GetSkills()`: Returns `SkillSO[]` from `ISkillMasterDataRepository` using `GetCurrentEvolutionNode().skillIds`.

---

### Task 3 — Create BackButtonView.cs
- Inherits `MonoBehaviour`. Uses `Button` component.
- `[SerializeField] private Button _button`.
- `OnBackClicked` event (System.Action).
- `Reset()`: `_button = GetComponentInChildren<Button>()` (§7).
- `_logClass` required (§8).

---

### Task 4 — Create OptionButtonView.cs
- Inherits `MonoBehaviour`. Uses `Button` component.
- `[SerializeField] private Button _button`.
- `OnOptionClicked` event (System.Action).
- `Reset()`: `_button = GetComponentInChildren<Button>()` (§7).
- `_logClass` required (§8).

---

### Task 5 — Create CharacterSpriteView.cs
- Inherits `MonoBehaviour`. Uses `Image` component.
- `[SerializeField] private Image _characterImage`.
- `SetSprite(Sprite sprite)` method.
- `Reset()`: `_characterImage = GetComponentInChildren<Image>()` (§7).
- `_logClass` required (§8).

---

### Task 6 — Create EvolutionStageButtonView.cs
- Inherits `MonoBehaviour`. Button combining sprite + text.
- `[SerializeField] private Button _button`.
- `[SerializeField] private Image _evolutionIcon`.
- `[SerializeField] private TMP_Text _evolutionNameText`.
- `SetEvolutionInfo(Sprite icon, string name)` method.
- `OnEvolutionStageClicked` event (System.Action).
- `Reset()`: Auto-assign child components (§7).
- `_logClass` required (§8).

---

### Task 7 — Create CharacterNameView.cs
- Inherits `MonoBehaviour`. Uses `TMP_Text`.
- `[SerializeField] private TMP_Text _nameText`.
- `SetName(string name)` method.
- `Reset()`: `_nameText = GetComponentInChildren<TMP_Text>()` (§7).
- `_logClass` required (§8).

---

### Task 8 — Create StatListView.cs
- Inherits `MonoBehaviour`. 4-stat text list.
- `[SerializeField] private TMP_Text _hpText`.
- `[SerializeField] private TMP_Text _strengthText`.
- `[SerializeField] private TMP_Text _toughnessText`.
- `[SerializeField] private TMP_Text _agilityText`.
- `SetStats(int hp, int strength, int toughness, int agility)` method: Set each text (e.g., "HP: 120").
- `Reset()`: Assign via `GetComponentsInChildren<TMP_Text>()` or manual — use judgment and record in `decisions.md` (§7).
- `_logClass` required (§8).

---

### Task 9 — Create SkillSlotView.cs
- Inherits `MonoBehaviour`. Individual skill icon UI.
- `[SerializeField] private Button _button`.
- `[SerializeField] private Image _skillIcon`.
- `[SerializeField] private GameObject _effectIndicator`: Status effect icon object.
- `SkillIndex` property (int): This slot's skill index.
- `Setup(int index, Sprite icon, bool hasEffect)` method: Initial skill setup. `_effectIndicator.SetActive(hasEffect)`.
- `OnSkillSlotClicked(int index)` event (System.Action<int>).
- `Reset()`: Auto-assign child components (§7).
- `_logClass` required (§8).

---

### Task 10 — Create SkillListView.cs
- Inherits `MonoBehaviour`. Manages skill slot list.
- `[SerializeField] private SkillSlotView[] _skillSlots`: Max 3 slots (pre-placed in Inspector).
- `SetSkills(SkillDisplayData[] skills)` method: Activate slots matching skill count, deactivate rest. Define `SkillDisplayData` in same file (Sprite icon, bool hasEffect).
- `OnSkillSlotClicked(int index)` event: Aggregates child `SkillSlotView` events.
- `Reset()`: `_skillSlots = GetComponentsInChildren<SkillSlotView>()` (§7).
- `_logClass` required (§8).

---

### Task 11 — Create InventoryView.cs
- Inherits `MonoBehaviour`. Inventory stub.
- `[SerializeField] private GameObject[] _inventorySlots`: 3 empty slot objects.
- Phase 1: Empty slot display only. No interaction.
- `Reset()`: Auto-assign if possible (§7).
- `_logClass` required (§8).

---

### Task 12 — Create InfoScrollView.cs
- Inherits `MonoBehaviour`. ScrollRect container.
- `[SerializeField] private ScrollRect _scrollRect`.
- `[SerializeField] private CharacterNameView _characterNameView`.
- `[SerializeField] private StatListView _statListView`.
- `[SerializeField] private SkillListView _skillListView`.
- `[SerializeField] private InventoryView _inventoryView`.
- Expose child Views as public properties (for Presenter access).
- `Reset()`: Auto-assign child components (§7).
- `_logClass` required (§8).

---

### Task 13 — Create SkillDescriptionPopupView.cs
- Inherits `MonoBehaviour`. Dedicated skill description popup.
- `[SerializeField] private GameObject _popupRoot`: Popup root object.
- `[SerializeField] private Image _skillIconImage`.
- `[SerializeField] private TMP_Text _skillNameText`.
- `[SerializeField] private TMP_Text _skillDescriptionText`.
- `[SerializeField] private TMP_Text _damageText`.
- `[SerializeField] private TMP_Text _effectText`.
- `[SerializeField] private Button _closeButton`.
- `Show(Sprite icon, string skillName, string description, float damage, string effectDescription)` method: Display popup.
- `Hide()` method: Hide popup.
- `OnCloseClicked` event (System.Action).
- `Reset()`: Auto-assign child components (§7).
- `_logClass` required (§8).

---

### Task 14 — Create CharacterInfoView.cs
- Inherits `MonoBehaviour`. Scene root View.
- All child Views as `[SerializeField] private`:
    - `BackButtonView _backButtonView`
    - `OptionButtonView _optionButtonView`
    - `CharacterSpriteView _characterSpriteView`
    - `EvolutionStageButtonView _evolutionStageButtonView`
    - `InfoScrollView _infoScrollView`
    - `SkillDescriptionPopupView _skillDescriptionPopupView`
- Expose child Views as public properties.
- `Reset()`: Auto-assign child components (§7).
- `_logClass` required (§8).

---

### Task 15 — Create CharacterInfoPresenter.cs
- Pure C# class (not MonoBehaviour).
- Constructor injection: `CharacterInfoUseCase`, `CharacterInfoView`, `ISceneNavigator`, `IPopupManager`.
- `_logClass` required (§8).
- `_skillSoCache` (SkillSO[]): Skill data cached during initialization.
- `Initialize()` method:
    - `CharacterInfoUseCase.GetCurrentEvolutionNode()` -> get EvolutionNodeSO
    - `CharacterSpriteView.SetSprite()` (Phase 1: placeholder)
    - `EvolutionStageButtonView.SetEvolutionInfo()`
    - `CharacterInfoUseCase.GetCurrentStats()` -> `StatListView.SetStats()`
    - `CharacterNameView.SetName()`
    - `CharacterInfoUseCase.GetSkills()` -> cache in `_skillSoCache` -> `SkillListView.SetSkills()` (convert SkillSO to SkillDisplayData)
    - Subscribe to events: BackButton, OptionButton, EvolutionStageButton, SkillSlotClicked, SkillPopupClose
- Event handlers:
    - **BackButton tap** -> `ISceneNavigator.LoadScene(SceneKey.Main)`
    - **EvolutionStageButton tap** -> Show "Coming Soon" CommonPopup via `IPopupManager` (Phase 1 stub)
    - **SkillSlot tap(index)** -> Extract data from `_skillSoCache[index]` -> `SkillDescriptionPopupView.Show()`
    - **SkillPopup close** -> `SkillDescriptionPopupView.Hide()`
- `Dispose()` method: Unsubscribe all events. Use `?.` (§8 Safe Cleanup).

---

### Task 16 — Create CharacterInfoSceneBootstrapper.cs
- `MonoBehaviour`. Run async initialization from `Start()` (§3).
- `[SerializeField] private CharacterInfoView _characterInfoView`.
- `await GlobalBootstrapper.Instance.InitializationTask`.
- Acquire `ICharacterRunRepository`, `ISkillMasterDataRepository` from `GameContext`.
- Acquire `EvolutionNodeSO[]` MasterData cache array: Use approach accessible from `GameContext` — reference existing pattern. Claude Code reads existing code and decides, recording in `decisions.md`.
- Create `CharacterInfoUseCase` instance.
- Acquire `ISceneNavigator`, `IPopupManager` from `GlobalBootstrapper`.
- Create `CharacterInfoPresenter` instance with injection.
- Call `CharacterInfoPresenter.Initialize()`.
- `OnDestroy()`: call `_presenter?.Dispose()` (§8 Safe Cleanup).
- `_logClass` required (§8).

---

### Task 17 — Create decisions.md
Create empty file at `.claude/specs/features/character-info-scene/decisions.md`.

---

## 5. Validation

| # | Item | Verification Method |
|---|---|---|
| V-01 | No compile errors in Unity console | Console check |
| V-02 | SceneKey enum contains CharacterInfo and EvolutionTree entries | File check |
| V-03 | No console errors when CharacterInfoSceneBootstrapper is placed in scene and Play is pressed | Editor check |
| V-04 | Fixed region (character sprite + evolution stage button) is not affected by scrolling | Editor Play check |
| V-05 | Scroll region (name/stats/skills/inventory) scrolls vertically | Editor Play check |
| V-06 | 4 stats display correctly as text | Editor Play check |
| V-07 | Skill icons display matching the evolution node's skillIds count | Editor Play check |
| V-08 | Status effect icon shows only on skills with status effects | Editor Play check |
| V-09 | Skill icon tap shows skill description popup and close button works | Editor Play check |
| V-10 | Evolution stage button tap shows "Coming Soon" popup | Editor Play check |
| V-11 | Back button tap transitions to MainScene | Editor Play check |
| V-12 | Inventory area shows 3 empty slots | Editor Play check |

---

## 6. Manual Tasks (Hak performs after Claude Code implementation)

| Order | Task |
|---|---|
| M-01 | Create scene file: Assets/_Game/Scenes/CharacterInfo.unity |
| M-02 | Set up Main Camera + Canvas (Screen Space - Camera) |
| M-03 | Place CharacterInfoSceneBootstrapper at scene root, connect CharacterInfoView in Inspector |
| M-04 | Connect all View [SerializeField] fields in Inspector |
| M-05 | Add CharacterInfo scene to Build Settings |
| M-06 | Configure ScrollRect (Vertical only, place InfoScroll child Views under Content) |
| M-07 | Pre-place 3 SkillSlotViews under SkillListView |
| M-08 | Set up SkillDescriptionPopupView popup UI prefab/object |
| M-09 | Place 3 empty inventory slot objects |
| M-10 | Prepare placeholder Sprites (character, evolution icon, skill icons) |
| M-11 | Create test SO assets (via BattleTestDataCreator editor script or manually) |

---

## 7. Claude Code Handoff Guide

- Run `claude` from project root
- `CLAUDE.md` is auto-loaded
- Hand off `.claude/specs/features/character-info-scene/tasks.md` for sequential implementation
- Record judgment calls in `.claude/specs/features/character-info-scene/decisions.md`
- Do NOT create files outside `Assets/_Game/` (except decisions.md)