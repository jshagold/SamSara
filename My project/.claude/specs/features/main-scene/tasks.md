# MainScene — Tasks

## 1. Overview

Implementation instructions for Claude Code to build the MainScene feature.
Based on the class structure defined in Plan, this document defines the order and method for creating actual files.

---

## 2. Prerequisites

- Read CLAUDE.md first before any implementation
- Read the following existing files first:
  - Assets/_Game/Features/Character/Data/CharacterRunData.cs
  - Assets/_Game/Features/Character/Domain/ICharacterRunRepository.cs
  - Assets/_Game/App/GameContext.cs
  - Assets/_Game/Core/Navigation/ISceneNavigator.cs
  - Assets/_Game/Core/Navigation/SceneKey.cs
- Create folder: Assets/_Game/Features/MainScene/
- Create folder: .claude/specs/features/main-scene/

---

## 3. Files to Create/Modify

| Order | File | Path | Action |
|---|---|---|---|
| 1 | CharacterRunData.cs | Assets/_Game/Features/Character/Data/ | Modify |
| 2 | MainUseCase.cs | Assets/_Game/Features/MainScene/Domain/ | Create |
| 3 | GoldView.cs | Assets/_Game/Features/MainScene/Presentation/Hud/ | Create |
| 4 | DayView.cs | Assets/_Game/Features/MainScene/Presentation/Hud/ | Create |
| 5 | HudToggleButtonView.cs | Assets/_Game/Features/MainScene/Presentation/Hud/ | Create |
| 6 | OptionButtonView.cs | Assets/_Game/Features/MainScene/Presentation/Hud/ | Create |
| 7 | CharacterStatusView.cs | Assets/_Game/Features/MainScene/Presentation/Hud/ | Create |
| 8 | HudView.cs | Assets/_Game/Features/MainScene/Presentation/Hud/ | Create |
| 9 | BackgroundView.cs | Assets/_Game/Features/MainScene/Presentation/Main/ | Create |
| 10 | CharacterSpriteView.cs | Assets/_Game/Features/MainScene/Presentation/Main/ | Create |
| 11 | StageButtonView.cs | Assets/_Game/Features/MainScene/Presentation/Main/ | Create |
| 12 | MaintenanceButtonView.cs | Assets/_Game/Features/MainScene/Presentation/Main/ | Create |
| 13 | CharacterInfoButtonView.cs | Assets/_Game/Features/MainScene/Presentation/Main/ | Create |
| 14 | MerchantButtonView.cs | Assets/_Game/Features/MainScene/Presentation/Main/ | Create |
| 15 | MainView.cs | Assets/_Game/Features/MainScene/Presentation/ | Create |
| 16 | MainPresenter.cs | Assets/_Game/Features/MainScene/Presentation/ | Create |
| 17 | MainSceneBootstrapper.cs | Assets/_Game/Features/MainScene/Presentation/ | Create |
| 18 | decisions.md | .claude/specs/features/main-scene/ | Create (empty) |

---

## 4. Implementation Instructions

### Task 1 — Modify CharacterRunData.cs

Add the following three fields to CharacterRunData:
- actionPoints (int) — current remaining action points
- maxActionPoints (int) — maximum action points
- maxHp (int) — maximum HP

Follow the existing field pattern: JsonProperty, private backing field, public getter.

---

### Task 2 — Create MainUseCase.cs

- Receives ICharacterRunRepository via constructor injection.
- Define MainViewModel class in the same file. Fields: Day, Gold, CurrentHp, MaxHp, ActionPoints, MaxActionPoints, CurrentEvolutionNodeId, IsMerchantActive.
- GetMainViewModel() method: reads the above data from RunData and returns as MainViewModel.
- IsMerchantActive: always returns false in v1. Add comment noting OQ-02 is unresolved.

---

### Task 3~8 — Create HUD View files

Each View inherits MonoBehaviour. Inspector-linked fields declared as [SerializeField] private.

- GoldView: SetGold(int gold) method. Display via TMP_Text.
- DayView: SetDay(int day) method. Display via TMP_Text.
- HudToggleButtonView: Publishes OnToggleClicked event. Uses Button component. Always remains active.
- OptionButtonView: Publishes OnOptionClicked event. Uses Button component.
- CharacterStatusView: SetHp(int current, int max), SetActionPoints(int current, int max), SetPortrait(Sprite sprite) methods. Action point circles managed as List<Image> (max 5).
- HudView: ShowHud(), HideHud() methods. Slide animation via DOTween. On off: TopRightGroup (Gold/Options/Day N) slides out upward, CharacterStatusView slides out leftward simultaneously. On on: reverse directions.

---

### Task 9 — Create BackgroundView.cs

- Inherits MonoBehaviour. Inspector-linked fields declared as [SerializeField] private.
- Displays background image via Image component.
- Placed at the lowest layer in Canvas. No methods required. Background Sprite connected directly via Inspector.

---

### Task 10~14 — Create Main area View files

Each View inherits MonoBehaviour. Inspector-linked fields declared as [SerializeField] private.

- CharacterSpriteView: SetSprite(string addressableKey) method. Async load from Addressables.
- StageButtonView / MaintenanceButtonView / CharacterInfoButtonView: Plays character interaction animation on tap, then publishes OnButtonClicked event after animation completes. Note: event fires after animation completion.
- MerchantButtonView: SetVisible(bool visible) method. Publishes OnMerchantClicked event.

---

### Task 15 — Create MainView.cs

- Holds references to all child Views as [SerializeField] private.
- Implements public methods called by Presenter (delegates to each child View method).
- Exposes public events that forward each child View's events to the Presenter.

---

### Task 16 — Create MainPresenter.cs

- Receives MainUseCase, MainView, ISceneNavigator via constructor injection.
- Initialize() method:
  - Queries data via GetMainViewModel() -> calls each View method
  - Merchant NPC active state -> calls MerchantButtonView.SetVisible()
  - Subscribes to each button event
- HUD toggle state: managed as bool _isHudVisible -> calls HudView.ShowHud() / HideHud().
- Receives navigation button events -> requests scene transition via ISceneNavigator.

---

### Task 17 — Create MainSceneBootstrapper.cs

- MonoBehaviour. Executes in Awake().
- Retrieves ICharacterRunRepository from GameContext.
- Creates MainUseCase instance.
- MainView connected via Inspector.
- Creates MainPresenter instance and injects dependencies.
- Calls MainPresenter.Initialize().

---

### Task 18 — Create decisions.md

Create empty file at .claude/specs/features/main-scene/decisions.md.

---

## 5. Validation

| # | Item | How to Check |
|---|---|---|
| V-01 | No compile errors in Unity console | Check console |
| V-02 | actionPoints, maxActionPoints, maxHp fields added to CharacterRunData | Check file |
| V-03 | No console errors after placing MainSceneBootstrapper in scene and pressing Play | Editor check |
| V-04 | HUD elements slide out/in when HUD on/off button is tapped | Editor Play check |
| V-05 | HUD on/off button remains fixed on screen when HUD is off | Editor Play check |
| V-06 | Scene transitions to target scene after character animation plays on navigation button tap | Editor Play check |
| V-07 | MerchantButtonView is inactive by default | Editor check |
| V-08 | Background image is displayed at the lowest layer in Canvas during Play | Editor Play check |

---

## 6. Claude Code Delivery Guide

- Run claude from project root
- CLAUDE.md will be loaded automatically
- Deliver this Tasks file and request implementation in order
- Record any judgment calls in .claude/specs/features/main-scene/decisions.md
- DO NOT create files outside Assets/_Game/