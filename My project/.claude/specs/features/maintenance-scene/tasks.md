# MaintenanceScene — Tasks

**Version:** 1.0.0 | **Date:** 2026-03-30 | **Status:** ✅ Confirmed

## 1. Overview

Implementation instructions for Claude Code to build the MaintenanceScene Feature. Creates files in the order defined by the Plan's class structure.

---

## 2. Prerequisites

- Read `CLAUDE.md` first
- Read the following existing files before starting:
    - `Assets/_Game/Features/Character/Data/CharacterRunData.cs`
    - `Assets/_Game/Features/Character/Domain/ICharacterRunRepository.cs`
    - `Assets/_Game/Features/Character/Domain/CharacterUseCase.cs`
    - `Assets/_Game/Features/Character/MasterData/StatType.cs`
    - `Assets/_Game/Features/Event/Domain/EventUseCase.cs`
    - `Assets/_Game/App/GameContext.cs`
    - `Assets/_Game/Core/Navigation/ISceneNavigator.cs`
    - `Assets/_Game/Core/Navigation/SceneKey.cs`
    - `Assets/_Game/Core/Popup/IPopupManager.cs`
    - `Assets/_Game/Features/MainScene/Presentation/MainSceneBootstrapper.cs` (reference existing pattern)
- Create folder: `Assets/_Game/Features/MaintenanceScene/`
- Create folder: `.claude/specs/features/maintenance-scene/`

---

## 3. Files to Create/Modify

| Order | File | Path | Action |
|---|---|---|---|
| 1 | `SceneKey.cs` | `Assets/_Game/Core/Navigation/` | Modify |
| 2 | `MaintenanceUseCase.cs` | `Assets/_Game/Features/MaintenanceScene/Domain/` | Create |
| 3 | `BackButtonView.cs` | `Assets/_Game/Features/MaintenanceScene/Presentation/TopBar/` | Create |
| 4 | `OptionButtonView.cs` | `Assets/_Game/Features/MaintenanceScene/Presentation/TopBar/` | Create |
| 5 | `TopBarView.cs` | `Assets/_Game/Features/MaintenanceScene/Presentation/TopBar/` | Create |
| 6 | `BackgroundView.cs` | `Assets/_Game/Features/MaintenanceScene/Presentation/Main/` | Create |
| 7 | `CharacterSpriteView.cs` | `Assets/_Game/Features/MaintenanceScene/Presentation/Main/` | Create |
| 8 | `HpBarView.cs` | `Assets/_Game/Features/MaintenanceScene/Presentation/Main/` | Create |
| 9 | `ActionPointsView.cs` | `Assets/_Game/Features/MaintenanceScene/Presentation/Main/` | Create |
| 10 | `GoldView.cs` | `Assets/_Game/Features/MaintenanceScene/Presentation/Main/` | Create |
| 11 | `CharacterInfoPanelView.cs` | `Assets/_Game/Features/MaintenanceScene/Presentation/Main/` | Create |
| 12 | `InteractionOptionsView.cs` | `Assets/_Game/Features/MaintenanceScene/Presentation/Main/` | Create |
| 13 | `TrainingListView.cs` | `Assets/_Game/Features/MaintenanceScene/Presentation/Training/` | Create |
| 14 | `ShopView.cs` | `Assets/_Game/Features/MaintenanceScene/Presentation/Shop/` | Create |
| 15 | `MaintenanceView.cs` | `Assets/_Game/Features/MaintenanceScene/Presentation/` | Create |
| 16 | `MaintenancePresenter.cs` | `Assets/_Game/Features/MaintenanceScene/Presentation/` | Create |
| 17 | `MaintenanceSceneBootstrapper.cs` | `Assets/_Game/Features/MaintenanceScene/Presentation/` | Create |
| 18 | `GameContext.cs` | `Assets/_Game/App/` | Modify |
| 19 | `decisions.md` | `.claude/specs/features/maintenance-scene/` | Create (empty) |

---

## 4. Implementation Order and Instructions

### Task 1 — Modify SceneKey.cs

Add `Maintenance` entry to the `SceneKey` enum.

---

### Task 2 — Create MaintenanceUseCase.cs

- Constructor-inject `ICharacterRunRepository` and `EventUseCase`.
- Define `MaintenanceViewModel` class in the same file. Fields: `ActionPoints`, `MaxActionPoints`, `Gold`, `CurrentHp`, `MaxHp`, `Stats` (Dictionary<StatType, int> or individual fields), `CurrentEvolutionNodeId`.
- `GetMaintenanceViewModel()` method: Read RunData and return as `MaintenanceViewModel`.
- `CanPerformAction()` method: Return `actionPoints >= 1`.
- `ConsumeActionPoint()` method: Deduct `actionPoints` by 1 in memory only. Do NOT save.
- `IsMerchantAvailable()` method: Always return `false` until Phase 5. Add TODO comment noting `EventUseCase` integration planned.
- `PurchasePotion(StatType)` method: Stub until Phase 5 (NotImplementedException or empty impl). Add TODO comment.

---

### Task 3~5 — Create TopBar View Files

Each View inherits `MonoBehaviour`. Inspector fields use `[SerializeField] private`.

- `BackButtonView`: Emits `OnBackClicked` event. Uses Button component.
- `OptionButtonView`: Emits `OnOptionClicked` event. Uses Button component.
- `TopBarView`: References `BackButtonView` and `OptionButtonView` via `[SerializeField] private`. Exposes public events forwarding child events.

---

### Task 6 — Create BackgroundView.cs

- Inherits `MonoBehaviour`. Displays background image via `Image` component.
- Placed at lowest Canvas layer. No methods. Background Sprite connected via Inspector.

---

### Task 7~11 — Create CharacterInfoPanel Child View Files

Each View inherits `MonoBehaviour`. Inspector fields use `[SerializeField] private`.

- `CharacterSpriteView`: `SetSprite(string addressableKey)` method. Async load via Addressables. Reference MainScene's CharacterSpriteView but create as independent file.
- `HpBarView`: `SetHp(int current, int max)` method. HP bar via Slider or Image.fillAmount. Text display for numeric values.
- `ActionPointsView`: `SetActionPoints(int current, int max)` method. Manage circles via `List<Image>` (max 5). Active up to current, inactive for the rest.
- `GoldView`: `SetGold(int gold)` method. Display via TMP_Text.
- `CharacterInfoPanelView`: References above 4 Views via `[SerializeField] private`. `SetCharacterInfo(MaintenanceViewModel viewModel)` method to refresh all at once.

---

### Task 12 — Create InteractionOptionsView.cs

- Manages 3 option buttons (Training/Exploration/Shop). Each uses `Button` component.
- Emits `OnTrainingClicked`, `OnExplorationClicked`, `OnShopClicked` events.
- `SetShopButtonVisible(bool visible)` method: Show/hide shop button.
- `SetInteractable(bool interactable)` method: Set Training/Exploration button interactable state and dim effect. Shop button is unaffected.

---

### Task 13 — Create TrainingListView.cs

- Manages 4-stat list UI.
- `Show(TrainingItemData[] items)` method: Display list. Define `TrainingItemData` in same file (StatType, miniGameName, currentValue).
- `Hide()` method: Hide list.
- Each item's entry button tap emits `OnStatSelected(StatType)` event.
- Collapse button tap emits `OnCollapseClicked` event.

---

### Task 14 — Create ShopView.cs

- Stub implementation until Phase 5.
- Implement `Show()` / `Hide()` methods only. Show displays empty panel with placeholder text "Shop coming soon".
- Emits `OnCloseClicked` event.
- Add TODO comments noting Phase 5 implementation for potion list, purchase logic, gold display, etc.

---

### Task 15 — Create MaintenanceView.cs

- References all child Views via `[SerializeField] private`: `TopBarView`, `CharacterInfoPanelView`, `InteractionOptionsView`, `TrainingListView`, `ShopView`, `BackgroundView`.
- Mode switch methods: `ShowDefaultMode()`, `ShowTrainingListMode()`, `ShowShopMode()`.
    - Default: Show InteractionOptionsView, hide TrainingListView/ShopView
    - TrainingList: Show TrainingListView, hide InteractionOptionsView
    - Shop: Show ShopView, hide InteractionOptionsView
- Exposes public events forwarding child View events to Presenter.

---

### Task 16 — Create MaintenancePresenter.cs

- Constructor-inject `MaintenanceUseCase`, `MaintenanceView`, `ISceneNavigator`, `IPopupManager`.
- `Initialize()` method:
    - Query data via `GetMaintenanceViewModel()` → refresh all Views
    - `IsMerchantAvailable()` → show/hide shop button
    - `CanPerformAction()` → dim Training/Exploration
    - Subscribe to all events
- Event handlers:
    - Training button → `ShowTrainingListMode()`
    - Stat selection → `CanPerformAction()` check → insufficient: popup / sufficient: `ConsumeActionPoint()` + scene transition
    - Exploration button → `CanPerformAction()` check → insufficient: popup / sufficient: `ConsumeActionPoint()` + scene transition
    - Shop button → `ShowShopMode()`
    - Back button → `ISceneNavigator.LoadScene(SceneKey.Main)`
    - Training list collapse → `ShowDefaultMode()`
    - Shop close → `ShowDefaultMode()`
- `Dispose()` method: Unsubscribe all events.

---

### Task 17 — Create MaintenanceSceneBootstrapper.cs

- `MonoBehaviour`. Runs async initialization in `Start()`.
- `await GlobalBootstrapper.Instance.InitializationTask`.
- Retrieve `ICharacterRunRepository`, `EventUseCase` from `GameContext`.
- Create `MaintenanceUseCase` instance.
- `MaintenanceView` connected via Inspector.
- Acquire `ISceneNavigator`, `IPopupManager` from `GlobalBootstrapper`.
- Create `MaintenancePresenter` instance with injected dependencies.
- Call `MaintenancePresenter.Initialize()`.
- Call `MaintenancePresenter.Dispose()` in `OnDestroy()`.

---

### Task 18 — Modify GameContext.cs

- Register `MaintenanceUseCase` in GameContext. (If MaintenanceSceneBootstrapper creates it directly following the MainScene pattern, this modification may be unnecessary — refer to MainScene pattern to decide.)

---

### Task 19 — Create decisions.md

Create empty file at `.claude/specs/features/maintenance-scene/decisions.md`.

---

## 5. Validation

| # | Item | How to Verify |
|---|---|---|
| V-01 | No compile errors in Unity console | Check console |
| V-02 | `SceneKey` enum contains `Maintenance` entry | Check file |
| V-03 | No console errors when playing with `MaintenanceSceneBootstrapper` placed in scene | Editor check |
| V-04 | Training button tap expands 4-stat list | Editor Play check |
| V-05 | Training list collapse returns to default screen | Editor Play check |
| V-06 | When AP insufficient, Training/Exploration buttons are dimmed and tap shows popup | Editor Play check |
| V-07 | Back button tap transitions to MainScene | Editor Play check |
| V-08 | CharacterInfoPanelView displays HP bar, AP circles, gold, character sprite | Editor Play check |
| V-09 | Shop button is hidden by default (merchant not present) | Editor Play check |

---

## 6. Manual Tasks (Hak performs after Claude Code implementation)

| Order | Task |
|---|---|
| M-01 | Create `Assets/_Game/Scenes/Maintenance.unity` scene file |
| M-02 | Place Main Camera + Canvas (Screen Space - Camera) |
| M-03 | Place MaintenanceSceneBootstrapper at scene root, connect MaintenanceView in Inspector |
| M-04 | Connect all View [SerializeField] fields in Inspector |
| M-05 | Add Maintenance scene to Build Settings |
| M-06 | Connect background image Sprite to BackgroundView |

---

## 7. Claude Code Delivery Guide

- Run `claude` from project root
- `CLAUDE.md` auto-loads
- Pass `.claude/specs/features/maintenance-scene/tasks.md` for sequential implementation
- Record any judgment calls in `.claude/specs/features/maintenance-scene/decisions.md`
- Do NOT create files outside `Assets/_Game/` (except decisions.md)