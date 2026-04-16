# EndingScene — Tasks

**Version:** 1.0.0 | **Date:** 2026-04-16 | **Feature:** EndingScene (Phase 6)
**Based on:** Specify v1.0.0 / Plan v1.0.0
**Constitution Reference:** §1~§11 full verification complete

---

## Constitution Checklist

| Section | Applied | Details |
|---|---|---|
| §1 Folder Structure | YES | Features/Ending/ under Data/Domain/Presentation/MasterData. .asset in Resources/MasterData/Ending/ |
| §2 Zero Guessing | YES | All paths/class names explicit. No Singleton. Data/Logic separation. |
| §3 Bootstrapper | YES | EndingSceneBootstrapper -> Start() initialization. |
| §4 Feature Module | YES | Clean Architecture layers. Domain is pure C#. |
| §5 Async/Popup | YES | UniTask. EndingResultPopupView is scene-specific popup. |
| §6 GameContext | YES | EndingMasterDataRepo, EndingUseCase, PendingEndingContext added. |
| §7 UI Standards | YES | Fail Fast (no null-guard). Reset() Auto-Assignment. Component Caching. |
| §8 Coding Standards | YES | _camelCase, suffixes (*View, *Presenter, *UseCase, *Repository, *SO), _logClass, SafeCleanup. |
| §9 Data Persistence | YES | Save-on-Action. CompleteEnding() saves immediately. Dirty Flag. |
| §10 Wrapper Pattern | YES | IEndingMasterDataRepository(Domain) -> EndingMasterDataRepository(Data). |
| §11 Libraries | YES | UniTask, DOTween (background fade), TMP, Newtonsoft.Json. |

---

## Pre-Implementation Checklist

Claude Code MUST verify the following before starting implementation.

- Read CLAUDE.md first
- Read the following existing files to understand patterns:
    - Assets/_Game/Features/Event/MasterData/EventSO.cs
    - Assets/_Game/Features/Event/MasterData/EventResultType.cs
    - Assets/_Game/Features/Event/Domain/EventUseCase.cs
    - Assets/_Game/Features/Event/Presentation/EventPresenter.cs
    - Assets/_Game/Features/BattleScene/Domain/PendingBattleContext.cs (pattern reference)
    - Assets/_Game/Features/BattleScene/Presentation/BattlePresenter.cs
    - Assets/_Game/Features/Character/Data/CharacterAccountData.cs
    - Assets/_Game/Features/Character/Data/CharacterAccountRepository.cs
    - Assets/_Game/Features/Character/Domain/ICharacterAccountRepository.cs
    - Assets/_Game/Features/Character/Data/CharacterRunData.cs
    - Assets/_Game/Features/Stage/Data/StageRunData.cs
    - Assets/_Game/App/GameContext.cs
    - Assets/_Game/Core/Navigation/SceneKey.cs
    - Assets/_Game/App/SceneNavigator.cs
    - Assets/_Game/Core/SpriteLoading/ISpriteLoader.cs
    - Assets/_Game/Features/Event/Presentation/Background/BackgroundView.cs (pattern reference)
    - Assets/_Game/Features/Event/Presentation/EventSceneBootstrapper.cs (pattern reference)
- Create folder: Assets/_Game/Features/Ending/Data/
- Create folder: Assets/_Game/Features/Ending/Domain/
- Create folder: Assets/_Game/Features/Ending/MasterData/
- Create folder: Assets/_Game/Features/Ending/Presentation/ and subfolders (Background/, Dialogue/, Title/, Result/, TopBar/)
- Create folder: .claude/specs/features/ending-scene/

---

## Files to Create/Modify

| # | File | Path | Action |
|---|---|---|---|
| 1 | EndingType.cs | Assets/_Game/Features/Ending/MasterData/ | Create |
| 2 | EndingDialogue.cs | Assets/_Game/Features/Ending/MasterData/ | Create |
| 3 | EndingSO.cs | Assets/_Game/Features/Ending/MasterData/ | Create |
| 4 | IEndingMasterDataRepository.cs | Assets/_Game/Features/Ending/Domain/ | Create |
| 5 | EndingMasterDataRepository.cs | Assets/_Game/Features/Ending/Data/ | Create |
| 6 | RunSummaryData.cs | Assets/_Game/Features/Ending/Domain/ | Create |
| 7 | PendingEndingContext.cs | Assets/_Game/Features/Ending/Domain/ | Create |
| 8 | EndingUseCase.cs | Assets/_Game/Features/Ending/Domain/ | Create |
| 9 | CharacterAccountData.cs | Assets/_Game/Features/Character/Data/ | Modify |
| 10 | SceneKey.cs | Assets/_Game/Core/Navigation/ | Modify |
| 11 | SceneNavigator.cs | Assets/_Game/App/ | Modify |
| 12 | GameContext.cs | Assets/_Game/App/ | Modify |
| 13 | EndingBackgroundView.cs | Assets/_Game/Features/Ending/Presentation/Background/ | Create |
| 14 | EndingTitleView.cs | Assets/_Game/Features/Ending/Presentation/Title/ | Create |
| 15 | EndingDialogueView.cs | Assets/_Game/Features/Ending/Presentation/Dialogue/ | Create |
| 16 | EndingResultPopupView.cs | Assets/_Game/Features/Ending/Presentation/Result/ | Create |
| 17 | OptionButtonView.cs | Assets/_Game/Features/Ending/Presentation/TopBar/ | Create |
| 18 | SkipButtonView.cs | Assets/_Game/Features/Ending/Presentation/TopBar/ | Create |
| 19 | EndingView.cs | Assets/_Game/Features/Ending/Presentation/ | Create |
| 20 | EndingPresenter.cs | Assets/_Game/Features/Ending/Presentation/ | Create |
| 21 | EndingSceneBootstrapper.cs | Assets/_Game/Features/Ending/Presentation/ | Create |
| 22 | decisions.md | .claude/specs/features/ending-scene/ | Create (empty) |

---

## Implementation Tasks

### Task 1: EndingType.cs (§1, §8)
- Create EndingType enum: BattleDefeat, EventDeath, BossVictory, EventEnding
- namespace: Samsara.Features.Ending.MasterData

### Task 2: EndingDialogue.cs (§1, §8)
- [System.Serializable] class
- Fields: _speakerName (string), _speakerPortraitKey (string), _text (string), _backgroundIndex (int, default -1)
- Expose via public read-only properties
- namespace: Samsara.Features.Ending.MasterData

### Task 3: EndingSO.cs (§1, §2, §8)
- ScriptableObject. [CreateAssetMenu(menuName = "Samsara/MasterData/EndingSO")]
- Fields: _id (int), _endingType (EndingType), _isGameOver (bool), _title (string), _dialogues (EndingDialogue[]), _backgroundSpriteKeys (string[]), _resultText (string), _unlocksMainBgKey (string), _unlocksMainBgmKey (string)
- Expose via public read-only properties
- namespace: Samsara.Features.Ending.MasterData

### Task 4: IEndingMasterDataRepository.cs (§4, §10)
- Interface definition
- GetEnding(int endingId) -> EndingSO
- GetEndingByType(EndingType type) -> EndingSO[]
- GetAllEndings() -> EndingSO[]
- namespace: Samsara.Features.Ending.Domain

### Task 5: EndingMasterDataRepository.cs (§4, §10)
- Implements IEndingMasterDataRepository
- Constructor: Resources.LoadAll<EndingSO>("MasterData/Ending")
- Cache as Dictionary<int, EndingSO>
- Include _logClass (§8)
- namespace: Samsara.Features.Ending.Data

### Task 6: RunSummaryData.cs (§2, §8)
- Pure C# class
- Fields: TotalDays (int), FinalEvolutionName (string), StagesCleared (int), FinalGold (int)
- namespace: Samsara.Features.Ending.Domain

### Task 7: PendingEndingContext.cs (§2, §6)
- Pure C# class
- Fields: EndingId (int), RunSummary (RunSummaryData)
- Reference PendingBattleContext, PendingEventContext pattern
- namespace: Samsara.Features.Ending.Domain

### Task 8: EndingUseCase.cs (§2, §4, §8, §9)
- Pure C# class. Constructor DI: IEndingMasterDataRepository, ICharacterAccountRepository
- State: _currentEnding (EndingSO), _dialogueIndex (int)
- Methods:
    - LoadEnding(int endingId): set _currentEnding, _dialogueIndex = 0
    - GetCurrentDialogue() -> EndingDialogue?: bounds check on _dialogueIndex, null if dialogue complete
    - AdvanceDialogue() -> bool: _dialogueIndex++, return whether next dialogue exists
    - IsDialogueComplete() -> bool
    - GetBackgroundKeyForCurrentDialogue() -> string?: check BackgroundIndex of current dialogue against EndingSO.BackgroundSpriteKeys, return key if transition needed, else null
    - CompleteEnding(): if not IsGameOver, add ID to AccountData.UnlockedEndingIds. If UnlocksMainBgKey not empty, update AccountData.MainSceneBgSpriteKey. If UnlocksMainBgmKey not empty, update AccountData.MainSceneBgmKey. Save immediately (§9 Save-on-Action)
    - IsEndingAlreadyUnlocked(int endingId) -> bool
- Include _logClass (§8)
- namespace: Samsara.Features.Ending.Domain

### Task 9: CharacterAccountData.cs Modify (§6, §9)
- READ existing file first to understand structure
- Add fields: UnlockedEndingIds (List<int>, default new List<int>()), MainSceneBgSpriteKey (string, default ""), MainSceneBgmKey (string, default "")
- Maintain Newtonsoft.Json serialization compatibility

### Task 10: SceneKey.cs Modify (§6)
- READ existing file first
- Add Ending entry to enum

### Task 11: SceneNavigator.cs Modify (§6, §10)
- READ existing file first
- Add SceneKey.Ending -> "Ending" scene name mapping

### Task 12: GameContext.cs Modify (§6)
- READ existing file first
- Add EndingMasterDataRepo (IEndingMasterDataRepository) public property
- Add EndingUseCase (EndingUseCase) public property
- Add PendingEndingContext (PendingEndingContext?) public get/set property
- In constructor: create EndingMasterDataRepository, create EndingUseCase (DI injection)

### Task 13: EndingBackgroundView.cs (§7, §8, §11)
- MonoBehaviour
- SerializeField: _backgroundImage (Image)
- Reset() Auto-Assignment (§7)
- SetBackground(Sprite sprite): immediate change
- FadeToBackground(Sprite sprite, float duration): DOTween CanvasGroup or Image.color fade transition, return UniTask
- Include _logClass, SafeCleanup (§8)

### Task 14: EndingTitleView.cs (§7, §8, §11)
- MonoBehaviour
- SerializeField: _titleText (TMP_Text), _canvasGroup (CanvasGroup)
- Reset() Auto-Assignment (§7)
- ShowTitle(string title, float fadeInDuration, float holdDuration, float fadeOutDuration) -> UniTask: fade in -> hold -> fade out using DOTween
- Include _logClass, SafeCleanup (§8)

### Task 15: EndingDialogueView.cs (§7, §8)
- MonoBehaviour
- SerializeField: _dialoguePanel (GameObject), _portraitImage (Image), _speakerNameText (TMP_Text), _dialogueText (TMP_Text)
- Reset() Auto-Assignment (§7)
- ShowDialogue(string speakerName, Sprite portrait, string text): dialogue mode — show portrait/name
- ShowNarration(string text): narration mode — hide portrait/name, text only
- HideDialogue(): hide panel
- Include _logClass, SafeCleanup (§8)

### Task 16: EndingResultPopupView.cs (§5, §7, §8)
- MonoBehaviour
- SerializeField: _titleText (TMP_Text), _summaryText (TMP_Text), _resultText (TMP_Text), _restartButton (Button)
- Reset() Auto-Assignment (§7)
- Show(string title, RunSummaryData summary, string resultText) -> UniTask: display popup + await restart button click (UniTask-based)
- Include _logClass, SafeCleanup (§8)

### Task 17: OptionButtonView.cs (§7, §8)
- Reference existing OptionButtonView from other scenes for identical pattern
- Include _logClass, SafeCleanup (§8)

### Task 18: SkipButtonView.cs (§7, §8)
- MonoBehaviour
- SerializeField: _skipButton (Button)
- Reset() Auto-Assignment (§7)
- 1st dev: set _skipButton.interactable = false in Start()
- Include _logClass, SafeCleanup (§8)

### Task 19: EndingView.cs (§4, §7, §8)
- MonoBehaviour. Root View.
- SerializeField: _backgroundView (EndingBackgroundView), _titleView (EndingTitleView), _dialogueView (EndingDialogueView), _resultPopupView (EndingResultPopupView), _optionButtonView (OptionButtonView), _skipButtonView (SkipButtonView)
- Expose via public read-only properties
- Reset() Auto-Assignment (§7)
- Include _logClass (§8)

### Task 20: EndingPresenter.cs (§2, §3, §4, §5, §8, §11)
- Pure C# class (NOT MonoBehaviour). Constructor DI: EndingUseCase, EndingView, PendingEndingContext, ISpriteLoader, ISceneNavigator, IPopupManager
- InitializeAsync() -> UniTask:
    1. EndingUseCase.LoadEnding(context.EndingId)
    2. Load initial background (BackgroundSpriteKeys[0]) via ISpriteLoader -> View.BackgroundView.SetBackground()
    3. ShowTitlePhaseAsync()
- ShowTitlePhaseAsync() -> UniTask:
    1. View.TitleView.ShowTitle() await (fade in -> hold -> fade out)
    2. ShowDialoguePhaseAsync()
- ShowDialoguePhaseAsync() -> UniTask:
    1. Get current dialogue (GetCurrentDialogue)
    2. If background transition needed, load via ISpriteLoader -> View.BackgroundView.FadeToBackground()
    3. Determine narration/dialogue mode -> View.DialogueView.ShowNarration() or ShowDialogue() (load portrait via ISpriteLoader)
    4. Await screen tap (UniTask)
    5. AdvanceDialogue() -> if true, repeat; if false, ShowResultPhaseAsync()
- ShowResultPhaseAsync() -> UniTask:
    1. EndingUseCase.CompleteEnding()
    2. View.DialogueView.HideDialogue()
    3. View.ResultPopupView.Show() await (title, RunSummary, ResultText)
    4. HandleRestart()
- HandleRestart():
    1. Set PendingEndingContext to null
    2. Navigate to ReplayScene via ISceneNavigator (1st dev: SceneKey.Main. TODO: RunData reset logic to be formally connected when ReplayScene/SplashScene implemented)
- Dispose(): SafeCleanup (§8)
- Include _logClass (§8)

### Task 21: EndingSceneBootstrapper.cs (§3, §6, §8)
- MonoBehaviour. SceneBootstrapper.
- SerializeField: _endingView (EndingView)
- In Start():
    1. await GlobalBootstrapper.Instance.InitializationTask
    2. Get dependencies from GameContext: EndingUseCase, PendingEndingContext, ISpriteLoader, ISceneNavigator, IPopupManager
    3. Create EndingPresenter (new — Bootstrapper only §3)
    4. presenter.InitializeAsync().Forget()
- Reset() Auto-Assignment: _endingView (§7)
- Include _logClass (§8)

### Task 22: decisions.md
- Create empty file: .claude/specs/features/ending-scene/decisions.md
- Record any judgment calls not covered by Spec with [DECISION], [BACKLOG], or [SPEC-GAP] tags

---

## Validation

| # | Item | Method |
|---|---|---|
| V-01 | No compile errors in Unity console | Console check |
| V-02 | SceneKey enum has Ending entry | File check |
| V-03 | GameContext has EndingMasterDataRepo, EndingUseCase, PendingEndingContext | File check |
| V-04 | EndingSceneBootstrapper plays without errors after scene setup | Editor Play |
| V-05 | Ending title fade in/out works correctly | Editor Play |
| V-06 | Narration mode displays (no portrait, text only) | Editor Play |
| V-07 | Dialogue mode displays (portrait + name + text) | Editor Play |
| V-08 | Screen tap advances to next dialogue | Editor Play |
| V-09 | Background image fade transition | Editor Play |
| V-10 | Result summary popup displays (title, Day, evolution, stages, gold) | Editor Play |
| V-11 | Restart button transitions to MainScene | Editor Play |
| V-12 | Ending (IsGameOver=false) completion adds to AccountData.UnlockedEndingIds + saves | Save file check |
| V-13 | Game over (IsGameOver=true) completion does not change UnlockedEndingIds | Save file check |
| V-14 | Skip button is disabled | Editor check |

---

## Manual Work (after Claude Code implementation)

| # | Task |
|---|---|
| M-01 | Create Assets/_Game/Scenes/Ending.unity scene file |
| M-02 | Place Main Camera + Canvas (Screen Space - Camera) setup |
| M-03 | Place EndingSceneBootstrapper + connect EndingView in Inspector |
| M-04 | Place EndingView child Views + Inspector connections (EndingBackgroundView, EndingTitleView, EndingDialogueView, EndingResultPopupView, OptionButtonView, SkipButtonView) |
| M-05 | Add Ending scene to Build Settings |
| M-06 | Place placeholder sprites (ending backgrounds, portraits) in Art/Sprites/, mark as Addressable |
| M-07 | Create test EndingSO .asset files (1 game over, 1 ending) in Assets/Resources/MasterData/Ending/ |

---

## Claude Code Delivery Guide

- Run `claude` from project root
- CLAUDE.md is loaded automatically
- Pass `.claude/specs/features/ending-scene/tasks.md` for sequential implementation
- Record additional judgment calls in `.claude/specs/features/ending-scene/decisions.md` with appropriate tags
- Do NOT create files outside `Assets/_Game/` (except decisions.md)