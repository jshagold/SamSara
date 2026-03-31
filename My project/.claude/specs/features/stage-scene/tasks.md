# StageScene — Tasks

**Version:** 1.0.0 | **Date:** 2026-03-31 | **Status:** ✅ Confirmed

## 1. Overview

Implementation guide for Claude Code to build the StageScene Feature. Covers file creation order and method based on the class structure defined in Plan.

**Note:** Patch work (Stage MasterData Patch-001, StageRepository Patch-001) must be completed before StageScene implementation.

---

## 2. Prerequisites

- Read `CLAUDE.md` first
- Read the following existing files first:
  - `Assets/_Game/Features/Stage/Data/StageRunData.cs`
  - `Assets/_Game/Features/Stage/Data/StageRepository.cs`
  - `Assets/_Game/Features/Stage/Data/StageMasterDataRepository.cs`
  - `Assets/_Game/Features/Stage/Domain/IStageRepository.cs`
  - `Assets/_Game/Features/Stage/Domain/IStageMasterDataRepository.cs`
  - `Assets/_Game/Features/Stage/Domain/StageUseCase.cs`
  - `Assets/_Game/Features/Stage/MasterData/StageSO.cs`
  - `Assets/_Game/Features/Stage/MasterData/StageNodeSO.cs`
  - `Assets/_Game/Features/Stage/MasterData/NodeType.cs`
  - `Assets/_Game/Features/Character/Data/CharacterRunData.cs`
  - `Assets/_Game/Features/Character/Domain/ICharacterRunRepository.cs`
  - `Assets/_Game/App/GameContext.cs`
  - `Assets/_Game/Core/Navigation/ISceneNavigator.cs`
  - `Assets/_Game/Core/Navigation/SceneKey.cs`
  - `Assets/_Game/Core/Popup/IPopupManager.cs`
  - `Assets/_Game/Features/MainScene/Presentation/MainSceneBootstrapper.cs` (existing pattern reference)
- Create `Assets/_Game/Features/StageScene/` folder
- Create `.claude/specs/features/stage-scene/` folder

---

## 3. Files to Create/Modify

| Order | File | Path | Action |
|---|---|---|---|
| 1 | `SceneKey.cs` | `Assets/_Game/Core/Navigation/` | Modify |
| 2 | `StageSceneUseCase.cs` | `Assets/_Game/Features/StageScene/Domain/` | Create |
| 3 | `DayView.cs` | `Assets/_Game/Features/StageScene/Presentation/TopBar/` | Create |
| 4 | `BackButtonView.cs` | `Assets/_Game/Features/StageScene/Presentation/TopBar/` | Create |
| 5 | `OptionButtonView.cs` | `Assets/_Game/Features/StageScene/Presentation/TopBar/` | Create |
| 6 | `BackgroundView.cs` | `Assets/_Game/Features/StageScene/Presentation/Background/` | Create |
| 7 | `NodeView.cs` | `Assets/_Game/Features/StageScene/Presentation/Map/` | Create |
| 8 | `NodeMapView.cs` | `Assets/_Game/Features/StageScene/Presentation/Map/` | Create |
| 9 | `CharacterMarkerView.cs` | `Assets/_Game/Features/StageScene/Presentation/Map/` | Create |
| 10 | `StageCompletePopupView.cs` | `Assets/_Game/Features/StageScene/Presentation/Popup/` | Create |
| 11 | `StageView.cs` | `Assets/_Game/Features/StageScene/Presentation/` | Create |
| 12 | `StagePresenter.cs` | `Assets/_Game/Features/StageScene/Presentation/` | Create |
| 13 | `StageSceneBootstrapper.cs` | `Assets/_Game/Features/StageScene/Presentation/` | Create |
| 14 | `GameContext.cs` | `Assets/_Game/App/` | Modify |
| 15 | `decisions.md` | `.claude/specs/features/stage-scene/` | Create (empty) |

---

## 4. Implementation Order & Instructions

### Task 1 — SceneKey.cs Modification

Add `Stage` entry to the `SceneKey` enum.

---

### Task 2 — StageSceneUseCase.cs Creation

- Receives `IStageRepository`, `IStageMasterDataRepository`, `ICharacterRunRepository` via constructor injection.
- Define `StageSceneViewModel` class in the same file. Fields: `CurrentStageId`, `CurrentNodeIndex`, `CompletedNodeIndices`, `Day`, `CurrentEvolutionNodeId`, `BiomeSpriteKey`, `CanReturnToMain`.
- `GetStageSceneViewModel()` method: Reads data from StageRunData + CharacterRunData and returns as `StageSceneViewModel`.
- `GetCurrentStageNodes()` method: Returns node list for current stage. From StageSO node list for fixed stages, from StageRunData.generatedNodeIds via StageMasterDataRepository for random stages.
- `MoveToNode(int targetIndex)` method: CharacterRunData.day +1, StageRunData.currentNodeIndex update, add previous node to completedNodeIndices, call `StageRepository.SaveAsync()` + `CharacterRunRepository.SaveDataAsync()`.
- `GetNodeType(int index)` method: Returns NodeType for the given node.
- `CanReturnToMain()` method: Returns current node's StageNodeSO.canReturnToMain flag.
- `IsStageComplete(int nodeIndex)` method: Checks if the node is a boss node (BattleNodeDataSO.isBoss).
- `GetNextStageOptions()` method: Returns next stage StageSO list from StageSO.nextStageIds.
- `SelectNextStage(string stageId)` method: Updates StageRunData (change currentStageId, currentNodeIndex = 0, clear completedNodeIndices, generate new generatedNodeIds for random stages), call `StageRepository.SaveAsync()`.
- `GetBiomeBackground()` method: Returns background sprite key from StageSO biome info.

---

### Task 3 — DayView.cs Creation

- Inherits `MonoBehaviour`. References `TMP_Text` via `[SerializeField] private`.
- `SetDay(int day)` method: Sets "Day {day}" text + DOTween punch scale animation on change.

---

### Task 4 — BackButtonView.cs Creation

- Inherits `MonoBehaviour`. Uses `Button` component.
- `OnBackClicked` event emission.
- `SetInteractable(bool interactable)` method: Button enable/disable + dim via CanvasGroup.alpha.

---

### Task 5 — OptionButtonView.cs Creation

- Inherits `MonoBehaviour`. Uses `Button` component.
- `OnOptionClicked` event emission.

---

### Task 6 — BackgroundView.cs Creation

- Inherits `MonoBehaviour`. Displays background image via `Image` component.
- `SetBackground(Sprite sprite)` method: Sets background sprite.
- `FadeToBackground(Sprite sprite, float duration)` method: Fade transition via DOTween.

---

### Task 7 — NodeView.cs Creation

- Inherits `MonoBehaviour`. Manages individual node UI.
- `[SerializeField] private Image _nodeIcon`: Node type icon.
- `[SerializeField] private Image _highlightEffect`: Highlight effect.
- `[SerializeField] private Image _completedMark`: Completion mark.
- `[SerializeField] private Button _button`: Touch input.
- `NodeIndex` property (int): This node's index.
- `Setup(int index, NodeType type, Sprite icon)` method: Initial node setup.
- `SetState(NodeViewState state)` method: Change visual representation based on state (Current / Completed / Active / Unvisited).
- Define `NodeViewState` enum in the same file.
- `OnNodeClicked(int index)` event emission.

---

### Task 8 — NodeMapView.cs Creation

- Inherits `MonoBehaviour`. Manages entire node map.
- `[SerializeField] private NodeView _nodeViewPrefab`: Node prefab reference.
- `[SerializeField] private Transform _nodeContainer`: Node placement parent Transform.
- `[SerializeField] private float _nodeSpacing`: Spacing between nodes.
- `RenderNodes(StageNodeSO[] nodes)` method: Create node prefab instances, arrange in vertical straight line.
- `HighlightNode(int index)` method: Set target node to Current state, next node to Active state.
- `MarkCompleted(int index)` method: Set target node to Completed state.
- `GetNodeWorldPosition(int index)` method: Return World Position for target node (used for camera tracking, character movement).
- `FocusOnNode(int index)` method: Move camera (or parent Transform) to center on target node (DOTween).
- `OnNodeClicked(int index)` event: Aggregate NodeView touch events and relay to Presenter.
- Viewport restriction: Deactivate (SetActive(false)) nodes outside a certain range from current node.
- `ClearNodes()` method: Destroy all existing node instances (used on stage transition).

---

### Task 9 — CharacterMarkerView.cs Creation

- Inherits `MonoBehaviour`. Character marker on current node.
- `[SerializeField] private Image _characterSprite`: Character image.
- `SetSprite(Sprite sprite)` method: Set character sprite.
- `MoveTo(Vector3 worldPosition, float duration)` method: Movement animation via DOTween.
- `SetPosition(Vector3 worldPosition)` method: Immediate position set (for initial placement).

---

### Task 10 — StageCompletePopupView.cs Creation

- Inherits `MonoBehaviour`. Stage completion dedicated popup.
- `[SerializeField] private TMP_Text _clearMessageText`: Clear message.
- `[SerializeField] private Transform _buttonContainer`: Button placement parent.
- `[SerializeField] private Button _buttonPrefab`: Option button prefab.
- `[SerializeField] private Button _returnToMainButton`: MainScene return button.
- `Show(string clearMessage, List<StageOptionData> options)` method: Display popup. Define `StageOptionData` in same file (stageId, stageName).
- Dynamically generate option buttons (based on nextStageIds).
- `OnStageSelected(string stageId)` event emission.
- `OnReturnToMainClicked` event emission.
- `Hide()` method: Hide popup.

---

### Task 11 — StageView.cs Creation

- References all child Views via `[SerializeField] private`: `DayView`, `BackButtonView`, `OptionButtonView`, `BackgroundView`, `NodeMapView`, `CharacterMarkerView`, `StageCompletePopupView`.
- Expose public events relaying child View events to Presenter.
- `ShowStageCompletePopup(string message, List<StageOptionData> options)` / `HideStageCompletePopup()` methods.

---

### Task 12 — StagePresenter.cs Creation

- Receives `StageSceneUseCase`, `StageView`, `ISceneNavigator`, `IPopupManager` via constructor injection.
- `Initialize()` method:
  - Query data via `GetStageSceneViewModel()`
  - Query node list via `GetCurrentStageNodes()`
  - `NodeMapView.RenderNodes()` → `HighlightNode()` → `FocusOnNode()`
  - `CharacterMarkerView.SetPosition()` at current node position
  - `BackgroundView.SetBackground()` biome background
  - `DayView.SetDay()`
  - `BackButtonView.SetInteractable(canReturnToMain)`
  - Subscribe to all events
- Event handlers:
  - Node touch → verify movable (only currentIndex + 1 allowed) → `MoveToNode()` → DayView refresh → CharacterMarkerView move → NodeMapView refresh → BackButtonView refresh → node type branching
  - Battle node → `ISceneNavigator.LoadScene(SceneKey.Battle)` (Phase 2: stub if SceneKey missing — TODO comment)
  - Event node → `ISceneNavigator.LoadScene(SceneKey.Event)` (Phase 2: same stub)
  - Boss node completion → `StageCompletePopupView.Show()` → next stage selection / MainScene return
  - Next stage selection → `SelectNextStage()` → re-render node map (ClearNodes + RenderNodes)
  - MainScene return → `ISceneNavigator.LoadScene(SceneKey.Main)`
  - Back button → `CanReturnToMain()` check → MainScene return
- `Dispose()` method: Unsubscribe all events.

---

### Task 13 — StageSceneBootstrapper.cs Creation

- MonoBehaviour. Runs async initialization in `Start()`.
- Await `GlobalBootstrapper.Instance.InitializationTask`.
- Retrieve `IStageRepository`, `IStageMasterDataRepository`, `ICharacterRunRepository` from GameContext.
- Create `StageSceneUseCase` instance.
- `StageView` connected via Inspector.
- Acquire `ISceneNavigator`, `IPopupManager` from GlobalBootstrapper.
- Create and inject `StagePresenter` instance.
- Call `StagePresenter.Initialize()`.
- Call `StagePresenter.Dispose()` in `OnDestroy()`.

---

### Task 14 — GameContext.cs Modification

- Register `StageSceneUseCase` in GameContext. (Or no modification needed if using pattern where StageSceneBootstrapper creates directly — refer to MainScene/MaintenanceScene pattern to decide)

---

### Task 15 — decisions.md Creation

Create empty file at `.claude/specs/features/stage-scene/decisions.md`.

---

## 5. Validation

| # | Item | Verification Method |
|---|---|---|
| V-01 | No compile errors in Unity console | Console check |
| V-02 | `Stage` entry exists in `SceneKey` enum | File check |
| V-03 | No console errors when placing `StageSceneBootstrapper` in scene and pressing Play | Editor check |
| V-04 | Node map renders in vertical straight line | Editor Play check |
| V-05 | Current node shows highlight, only next node is touchable | Editor Play check |
| V-06 | Character marker moves with animation on node touch | Editor Play check |
| V-07 | Day increments by +1 on node movement and reflects in DayView | Editor Play check |
| V-08 | Back button is disabled on nodes where `canReturnToMain` is false | Editor Play check |
| V-09 | StageCompletePopupView displays on boss node completion with next stage selection | Editor Play check |
| V-10 | Back button tap transitions to MainScene | Editor Play check |

---

## 6. Manual Tasks (Performed by Hak after Claude Code implementation)

| Order | Task |
|---|---|
| M-01 | Create `Assets/_Game/Scenes/Stage.unity` scene file |
| M-02 | Place Main Camera + Canvas (Screen Space - Camera) |
| M-03 | Place StageSceneBootstrapper at scene root, connect StageView in Inspector |
| M-04 | Connect each View's [SerializeField] fields in Inspector |
| M-05 | Add Stage scene to Build Settings |
| M-06 | Create NodeView prefab + connect _nodeViewPrefab in NodeMapView |
| M-07 | Create StageCompletePopupView button prefab + connect _buttonPrefab |
| M-08 | Connect biome background Sprite to BackgroundView |
| M-09 | Prepare and connect node type icon Sprites |

---

## 7. Claude Code Delivery Guide

- Run `claude` from project root
- `CLAUDE.md` loads automatically
- **Apply Patch first:** Deliver `.claude/specs/features/stage-masterdata/patch-001.md` first to complete existing SO changes before proceeding with StageScene Tasks
- Deliver `.claude/specs/features/stage-scene/tasks.md` for sequential implementation
- Record any judgment calls in `.claude/specs/features/stage-scene/decisions.md`
- Do NOT create files outside `Assets/_Game/` (except decisions.md)