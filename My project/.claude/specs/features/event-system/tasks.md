# EventSystem — Tasks

**Version:** 1.0.0 | **Date:** 2026-04-13 | **Status:** Ready for Claude Code
**Based on:** Specify v1.0.0 / Plan v1.0.0

---

## Pre-Implementation Checklist

> Claude Code MUST verify the following before starting any task.

- Read CLAUDE.md first
- Read the following existing files:
    - Assets/_Game/Features/Event/MasterData/EventSO.cs
    - Assets/_Game/Features/Event/MasterData/EventType.cs
    - Assets/_Game/Features/Event/MasterData/EventResultType.cs
    - Assets/_Game/Features/Event/MasterData/SpeakerPosition.cs
    - Assets/_Game/Features/Event/MasterData/EventSource.cs
    - Assets/_Game/Features/Event/Domain/EventUseCase.cs
    - Assets/_Game/Features/Stage/Data/StageRunData.cs
    - Assets/_Game/Features/Stage/Data/StageRepository.cs
    - Assets/_Game/Features/Stage/Domain/IStageRepository.cs
    - Assets/_Game/Features/Character/Data/CharacterRunData.cs
    - Assets/_Game/Features/Character/Data/CharacterRunRepository.cs
    - Assets/_Game/Features/Character/Domain/ICharacterRunRepository.cs
    - Assets/_Game/Features/Character/MasterData/StatType.cs
    - Assets/_Game/Features/BattleScene/Domain/PendingBattleContext.cs (pattern reference)
    - Assets/_Game/App/GameContext.cs
    - Assets/_Game/Core/Navigation/ISceneNavigator.cs
    - Assets/_Game/Core/Navigation/SceneKey.cs
    - Assets/_Game/App/SceneNavigator.cs
    - Assets/_Game/Features/CharacterInfoScene/Presentation/CharacterInfoSceneBootstrapper.cs (pattern reference)
- Create folder: Assets/_Game/Features/Event/Data/
- Create folder: Assets/_Game/Features/Event/Presentation/ and sub-folders
- Create folder: .claude/specs/features/eventsystem/

---

## Files to Create/Modify

| # | File | Path | Action |
|---|---|---|---|
| 1 | SceneKey.cs | Assets/_Game/Core/Navigation/ | Modify |
| 2 | SceneNavigator.cs | Assets/_Game/App/ | Modify |
| 3 | IEventMasterDataRepository.cs | Assets/_Game/Features/Event/Domain/ | Create |
| 4 | EventMasterDataRepository.cs | Assets/_Game/Features/Event/Data/ | Create |
| 5 | PendingEventContext.cs | Assets/_Game/Features/Event/Domain/ | Create |
| 6 | StageRunData.cs | Assets/_Game/Features/Stage/Data/ | Modify |
| 7 | EventUseCase.cs | Assets/_Game/Features/Event/Domain/ | Modify |
| 8 | GameContext.cs | Assets/_Game/App/ | Modify |
| 9 | PortraitView.cs | Assets/_Game/Features/Event/Presentation/Dialogue/ | Create |
| 10 | DialogueView.cs | Assets/_Game/Features/Event/Presentation/Dialogue/ | Create |
| 11 | ChoiceListView.cs | Assets/_Game/Features/Event/Presentation/Dialogue/ | Create |
| 12 | ChainStageIndicatorView.cs | Assets/_Game/Features/Event/Presentation/Indicator/ | Create |
| 13 | EventResultPopupView.cs | Assets/_Game/Features/Event/Presentation/Result/ | Create |
| 14 | BackgroundView.cs | Assets/_Game/Features/Event/Presentation/Background/ | Create |
| 15 | OptionButtonView.cs | Assets/_Game/Features/Event/Presentation/TopBar/ | Create |
| 16 | EventView.cs | Assets/_Game/Features/Event/Presentation/ | Create |
| 17 | EventPresenter.cs | Assets/_Game/Features/Event/Presentation/ | Create |
| 18 | EventSceneBootstrapper.cs | Assets/_Game/Features/Event/Presentation/ | Create |
| 19 | EventDialogueOverlayController.cs | Assets/_Game/Features/Event/Presentation/ | Create |
| 20 | decisions.md | .claude/specs/features/eventsystem/ | Create (empty) |

---

## Implementation Tasks

### Task 1 — SceneKey.cs Modification

Read SceneKey enum and add Event entry if not present.

---

### Task 2 — SceneNavigator.cs Modification

Add SceneKey.Event -> "Event" scene name mapping.

---

### Task 3 — IEventMasterDataRepository.cs

- Namespace: Samsara.Features.Event.Domain
- Interface. Constitution: section 4 Domain Layer, section 10 Wrapper Pattern.

Methods:
- GetEvent(int eventId) -> EventSO
- GetAllEvents() -> EventSO[]
- GetMaintenanceEvents() -> EventSO[] (filter by EventSource == Maintenance)

---

### Task 4 — EventMasterDataRepository.cs

- Namespace: Samsara.Features.Event.Data
- Implements IEventMasterDataRepository
- Resources.LoadAll<EventSO>("MasterData/Event") to load
- Dictionary<int, EventSO> cache (eventId -> EventSO)
- GetMaintenanceEvents(): filter by EventSource == Maintenance
- _logClass required (section 8)
- Constitution: section 1 Resources path, section 8 LogTag, section 10 Wrapper

---

### Task 5 — PendingEventContext.cs

- Namespace: Samsara.Features.Event.Domain
- Pure data class, no logic (section 2 Data/Logic separation)
- Reference PendingBattleContext.cs for pattern

Fields:
- EventId (int)
- ReturnScene (SceneKey)
- BackgroundSpriteKey (string)
- IsReturningFromBattle (bool)

---

### Task 6 — StageRunData.cs Modification

Add field to existing class:
- public int PendingChainedEventId = -1;
- Serialization compatibility maintained (Newtonsoft.Json default value handling) (section 9)

---

### Task 7 — EventUseCase.cs (Extend Existing Stub)

- Namespace: Samsara.Features.Event.Domain
- Pure C# class (not MonoBehaviour) (section 2, section 4)
- Constructor injection: IEventMasterDataRepository, ICharacterRunRepository, IStageRepository (section 3)
- _logClass required (section 8)

State fields:
- _currentEvent (EventSO)
- _dialogueIndex (int)
- _isDialoguePhase (bool)
- _appliedResult (EventResult?)

Methods:
- LoadEvent(int eventId) -> void: Load EventSO and initialize state
- GetCurrentDialogue() -> EventDialogue?: null if dialogues ended
- AdvanceDialogue() -> bool: true=next dialogue exists, false=ended
- HasChoices() -> bool
- GetChoices() -> EventChoice[]
- ApplyChoice(int choiceIndex) -> EventResult: Apply choice result + save chained event ID
- ApplyDirectResult() -> EventResult: Apply result for choice-less events
- GetChainInfo() -> (string eventName, int step, int total)?: null if not chained
- SaveChainedEventId(int nextEventId) -> void: Save to StageRunData
- ClearChainedEventId() -> void: Set PendingChainedEventId = -1

Result application logic (section 9 Save-on-Action):
- HpChange: characterRunRepo.UpdateHp(value) -> save immediately
- StatChange: characterRunRepo.UpdateStat(statType, value) -> save immediately
- ShopEncounter: return result only (Presenter shows stub popup)
- Battle: return result only (Presenter sets PendingBattleContext + scene transition)
- Death: return result only (Presenter handles game over)
- None: no-op

---

### Task 8 — GameContext.cs Modification

Add properties:
- EventMasterDataRepo (IEventMasterDataRepository) — public
- EventUseCase (EventUseCase) — public
- PendingEventContext (PendingEventContext?) — public get/set

In constructor:
- Create EventMasterDataRepository + Resources.LoadAll
- Create EventUseCase (inject: IEventMasterDataRepository, ICharacterRunRepository, IStageRepository)

---

### Task 9 — PortraitView.cs

- MonoBehaviour
- Namespace: Samsara.Features.Event.Presentation
- [SerializeField] private Image _portraitImage
- [SerializeField] private CanvasGroup _dimOverlay
- SetPortrait(Sprite sprite): set portrait image
- SetHighlight(bool isActive): isActive=true -> dimOverlay.alpha=0, false -> 0.5
- Show() / Hide(): gameObject.SetActive
- Reset(): auto-assign _portraitImage, _dimOverlay (section 7)
- _logClass required (section 8)

---

### Task 10 — DialogueView.cs

- MonoBehaviour
- Namespace: Samsara.Features.Event.Presentation
- [SerializeField] private PortraitView _leftPortrait
- [SerializeField] private PortraitView _rightPortrait
- [SerializeField] private TMP_Text _speakerNameText
- [SerializeField] private TMP_Text _dialogueText
- [SerializeField] private GameObject _dialoguePanel
- ShowDialogue(EventDialogue dialogue): Based on SpeakerPosition:
    - Left: _leftPortrait highlight, _rightPortrait dim (if shown)
    - Right: _rightPortrait highlight, _leftPortrait dim (if shown)
    - If no portrait key for a position, Hide that PortraitView
    - Set _speakerNameText, _dialogueText
- HideDialogue(): deactivate _dialoguePanel
- Reset(): auto-assign (section 7). _leftPortrait, _rightPortrait require manual Inspector connection.
- _logClass required (section 8)

---

### Task 11 — ChoiceListView.cs

- MonoBehaviour
- Namespace: Samsara.Features.Event.Presentation
- [SerializeField] private Transform _choiceContainer
- [SerializeField] private GameObject _choiceButtonPrefab
- ShowChoices(EventChoice[] choices, Action<int> onSelect): Instantiate Prefab, set text + callback per button
- HideChoices(): Destroy all children of _choiceContainer
- Reset(): auto-assign _choiceContainer (section 7)
- _logClass required (section 8)

---

### Task 12 — ChainStageIndicatorView.cs

- MonoBehaviour
- Namespace: Samsara.Features.Event.Presentation
- [SerializeField] private TMP_Text _indicatorText
- Show(string eventName, int step, int total): display "(eventName - step/total)"
- Hide(): gameObject.SetActive(false)
- Reset(): auto-assign _indicatorText (section 7)
- _logClass required (section 8)

---

### Task 13 — EventResultPopupView.cs

- MonoBehaviour
- Namespace: Samsara.Features.Event.Presentation
- [SerializeField] private GameObject _popupRoot
- [SerializeField] private TMP_Text _resultText
- [SerializeField] private Button _confirmButton
- ShowAsync(string resultText) -> UniTask: show popup + await confirm button click (section 5, section 11)
- Hide(): deactivate _popupRoot
- Scene-specific popup (Constitution section 5 IPopupManager bypass allowed)
- Reset(): auto-assign (section 7)
- _logClass required (section 8)

---

### Task 14 — BackgroundView.cs

- MonoBehaviour
- Namespace: Samsara.Features.Event.Presentation
- [SerializeField] private Image _backgroundImage
- SetBackground(Sprite sprite): set background image
- Reset(): auto-assign _backgroundImage (section 7)
- _logClass required (section 8)

---

### Task 15 — OptionButtonView.cs

- MonoBehaviour. Same pattern as other scenes.
- Namespace: Samsara.Features.Event.Presentation
- [SerializeField] private Button _optionButton
- OnOptionClicked event (System.Action)
- Reset(): auto-assign _optionButton (section 7)
- _logClass required (section 8)

---

### Task 16 — EventView.cs

- MonoBehaviour. Scene root View.
- Namespace: Samsara.Features.Event.Presentation
- All sub-Views as [SerializeField] private:
    - BackgroundView _backgroundView
    - DialogueView _dialogueView
    - ChoiceListView _choiceListView
    - ChainStageIndicatorView _chainStageIndicatorView
    - EventResultPopupView _eventResultPopupView
    - OptionButtonView _optionButtonView
- Public properties for each sub-View
- OnScreenTapped event (System.Action): screen tap detection for dialogue advance
- Update(): detect touch/mouse input -> fire OnScreenTapped. No LINQ/new in Update (section 8 GC)
- Reset(): auto-assign (section 7)
- _logClass required (section 8)

---

### Task 17 — EventPresenter.cs

- Pure C# class (not MonoBehaviour) (section 2, section 4)
- Namespace: Samsara.Features.Event.Presentation
- Constructor injection: EventUseCase, EventView, ISceneNavigator, PendingEventContext
- _logClass required (section 8)

InitializeAsync() -> UniTask:
- EventUseCase.LoadEvent(pendingContext.EventId)
- If chained: ChainStageIndicatorView.Show()
- If IsReturningFromBattle: ShowResultPopup() (result already applied)
- If dialogues exist: ShowDialoguePhase()
- Else: ShowChoiceOrResultPhase()

ShowDialoguePhase():
- Show current dialogue on View (DialogueView.ShowDialogue)
- Subscribe EventView.OnScreenTapped for tap -> AdvanceDialogue()
- Repeat until dialogues end -> ShowChoiceOrResultPhase()

ShowChoiceOrResultPhase():
- HasChoices() -> show choices on View -> await selection -> ApplyChoice
- No choices -> ApplyDirectResult()
- After result applied -> ShowResultPopup()

ShowResultPopup():
- Compose result text (e.g. "HP +10", "Strength -5")
- EventResultPopupView.ShowAsync() await
- HandlePostResult()

HandlePostResult():
- Battle: set PendingBattleContext (ReturnScene=Event), PendingEventContext.IsReturningFromBattle = true -> BattleScene transition
- ShopEncounter: stub popup ("You met a merchant") -> ReturnScene
- Death: game over popup -> MainScene
- None/HpChange/StatChange: ReturnScene

Dispose(): SafeCleanup (?. operator) (section 8)

---

### Task 18 — EventSceneBootstrapper.cs

- MonoBehaviour (section 3)
- Namespace: Samsara.Features.Event.Presentation
- [SerializeField] private EventView _eventView
- Start():
    - await GlobalBootstrapper.Instance.InitializationTask
    - Get EventUseCase, PendingEventContext, ISceneNavigator from GameContext
    - Create EventPresenter (new — Bootstrapper only, section 3)
    - EventPresenter.InitializeAsync().Forget() (section 11)
- OnDestroy(): _presenter?.Dispose() (section 8 SafeCleanup)
- _logClass required (section 8)

---

### Task 19 — EventDialogueOverlayController.cs

- MonoBehaviour (Prefab root)
- Namespace: Samsara.Features.Event.Presentation
- [SerializeField] private DialogueView _dialogueView
- [SerializeField] private ChoiceListView _choiceListView
- [SerializeField] private GameObject _dimBackground
- Initialize(EventUseCase useCase): inject UseCase
- RunEventAsync(int eventId) -> UniTask<EventResult>: run full event (dialogue -> choices -> return result)
- OnDestroy(): SafeCleanup (section 8)
- _logClass required (section 8)
- This Phase creates structure and code only. External scene integration via future Patches.

---

### Task 20 — decisions.md

Create empty file at .claude/specs/features/eventsystem/decisions.md

---

## Validation

| # | Item | How to Check |
|---|---|---|
| V-01 | No compile errors in Unity console | Console check |
| V-02 | SceneKey enum contains Event entry | File check |
| V-03 | EventSceneBootstrapper placed in scene, no errors on Play | Editor Play |
| V-04 | Dialogue display -> tap -> advance to next dialogue | Editor Play |
| V-05 | Left/right portrait highlight/dim by SpeakerPosition | Editor Play |
| V-06 | Single-speaker dialogue hides other portrait | Editor Play |
| V-07 | Choice buttons display and selection works | Editor Play |
| V-08 | Chained event stage indicator "(EventName - N/M)" accuracy | Editor Play |
| V-09 | HpChange result -> CharacterRunData.Hp changed + saved | Save file check |
| V-10 | StatChange result -> stat changed + saved | Save file check |
| V-11 | ShopEncounter result -> stub popup shown | Editor Play |
| V-12 | Battle result -> BattleScene transition -> return to EventScene | Editor Play |
| V-13 | Death result -> game over popup -> MainScene | Editor Play |
| V-14 | Result popup -> confirm -> return to previous scene | Editor Play |
| V-15 | Chained event PendingChainedEventId save/load | Save file check |
| V-16 | EventDialogueOverlayController compiles without errors | Console check |

---

## Manual Work (After Claude Code Implementation)

| # | Task |
|---|---|
| M-01 | Create Assets/_Game/Scenes/Event.unity scene file |
| M-02 | Place Main Camera + Canvas (Screen Space - Camera) |
| M-03 | Place EventSceneBootstrapper + connect EventView in Inspector |
| M-04 | Place and connect all sub-Views in Inspector (BackgroundView, DialogueView, PortraitView x2, ChoiceListView, ChainStageIndicatorView, EventResultPopupView, OptionButtonView) |
| M-05 | Add Event scene to Build Settings |
| M-06 | Create ChoiceButton Prefab (Button + TMP_Text) |
| M-07 | Create EventDialogueOverlay Prefab (DimBG + DialogueView + ChoiceListView) |
| M-08 | Place placeholder Sprites (portraits, backgrounds) under Art/Sprites/ |
| M-09 | Create test EventSO .asset files (1 one-shot, 1 chained 2-3 step set) |

---

## Claude Code Delivery Guide

- Run claude from project root
- CLAUDE.md auto-loaded
- Pass .claude/specs/features/event-system/tasks.md for sequential implementation
- Record judgment calls in .claude/specs/features/event-system/decisions.md
- DO NOT create files outside Assets/_Game/ (except decisions.md)