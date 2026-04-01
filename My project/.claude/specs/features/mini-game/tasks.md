# MiniGame — Tasks

**Version:** 1.0.0 | **Date:** 2026-03-31 | **Status:** ✅ Confirmed

## 1. Overview

Implementation instructions for Claude Code to build the MiniGame Feature. Based on the class structure defined in Plan, this document covers the order and method for creating actual files.

---

## 2. Prerequisites

- Read `CLAUDE.md` first
- Read the following existing files before starting:
    - `Assets/_Game/Features/Character/Data/CharacterRunData.cs`
    - `Assets/_Game/Features/Character/Data/CharacterRunRepository.cs`
    - `Assets/_Game/Features/Character/Domain/ICharacterRunRepository.cs`
    - `Assets/_Game/Features/Character/MasterData/StatType.cs`
    - `Assets/_Game/App/GameContext.cs`
    - `Assets/_Game/App/GlobalBootstrapper.cs`
    - `Assets/_Game/Core/Navigation/ISceneNavigator.cs`
    - `Assets/_Game/Core/Navigation/SceneKey.cs`
    - `Assets/_Game/Features/MaintenanceScene/Presentation/MaintenanceSceneBootstrapper.cs` (existing Bootstrapper pattern reference)
    - `Assets/_Game/Features/MaintenanceScene/Presentation/MaintenancePresenter.cs` (existing Presenter pattern reference)
    - `Assets/_Game/Features/MiniGame/Domain/MiniGameUseCase.cs` (existing stub — replacement target)
- Create folder: `Assets/_Game/Features/MiniGame/Presentation/`
- Create folder: `.claude/specs/features/mini-game/`

---

## 3. Files to Create/Modify

| Order | File | Path | Action |
|---|---|---|---|
| 1 | `SceneKey.cs` | `Assets/_Game/Core/Navigation/` | Modify |
| 2 | `GameContext.cs` | `Assets/_Game/App/` | Modify |
| 3 | `MiniGameUseCase.cs` | `Assets/_Game/Features/MiniGame/Domain/` | Replace (stub → actual) |
| 4 | `GuideTooltipView.cs` | `Assets/_Game/Features/MiniGame/Presentation/Play/` | Create |
| 5 | `TimingBarView.cs` | `Assets/_Game/Features/MiniGame/Presentation/Play/` | Create |
| 6 | `MarkerView.cs` | `Assets/_Game/Features/MiniGame/Presentation/Play/` | Create |
| 7 | `TouchButtonView.cs` | `Assets/_Game/Features/MiniGame/Presentation/Play/` | Create |
| 8 | `RoundIndicatorView.cs` | `Assets/_Game/Features/MiniGame/Presentation/Play/` | Create |
| 9 | `OptionButtonView.cs` | `Assets/_Game/Features/MiniGame/Presentation/TopBar/` | Create |
| 10 | `MiniGameResultPopupView.cs` | `Assets/_Game/Features/MiniGame/Presentation/Result/` | Create |
| 11 | `MiniGameView.cs` | `Assets/_Game/Features/MiniGame/Presentation/` | Create |
| 12 | `MiniGamePresenter.cs` | `Assets/_Game/Features/MiniGame/Presentation/` | Create |
| 13 | `MiniGameSceneBootstrapper.cs` | `Assets/_Game/Features/MiniGame/Presentation/` | Create |
| 14 | `decisions.md` | `.claude/specs/features/mini-game/` | Create (empty) |

---

## 4. Implementation Order and Instructions

### Task 1 — Modify SceneKey.cs

Add `MiniGame` entry to the `SceneKey` enum.

**Constitution check:** §6 No raw string scene names. Manage via enum.

---

### Task 2 — Modify GameContext.cs

- Add `PendingTrainingStat` (StatType?, nullable) public property. For scene-to-scene data transfer.
- Add `MiniGameUseCase` public accessor (follow existing UseCase pattern).
- Create `MiniGameUseCase` instance in GameContext constructor and wire dependencies.

**Constitution check:** §6 GameContext is pure C# class. Manual DI wiring.

---

### Task 3 — Replace MiniGameUseCase.cs

Delete existing stub file and replace with actual implementation.

- Constructor injection of `ICharacterRunRepository`.
- Include `_logClass` tag. (§8 Log Tag)

**Public methods:**
- `GetCurrentStatValue(StatType statType)`: Return current value of the specified stat from CharacterRunData.
- `CalculateStatDelta(MiniGameVerdict verdict)`: Return random integer within range based on verdict. Use `UnityEngine.Random.Range`.
- `async UniTask ApplyResultAndSave(StatType statType, int delta)`: Apply action point -1 and stat delta to CharacterRunData → call `SaveDataAsync()`.

**MiniGameVerdict enum:** Define in separate file in Domain folder.

    public enum MiniGameVerdict { Fail, Maintain, Success }

**Timing parameters (private const):**
- `RoundCount = 3`
- `SuccessDeltaMin = 8`, `SuccessDeltaMax = 10`
- `MaintainDeltaMin = -2`, `MaintainDeltaMax = 2`
- `FailDeltaMin = -5`, `FailDeltaMax = -3`

> Values above are temporary. Will change when stat design is finalized.
> MarkerSpeed, SuccessZoneStart/End belong in Presentation layer (Presenter), not UseCase.

**Constitution check:** §2 Data/Logic separation — no UI references in UseCase. §3 Only Bootstrapper creates instances — UseCase created in GameContext. §7 Fail Fast — throw on invalid StatType. §8 `_logClass` tag required. §9 Save-on-Action — immediate save in `ApplyResultAndSave`. §9 Dual-Mode — use async save.

---

### Task 4 — Create GuideTooltipView.cs

- Inherits `MonoBehaviour`. `TMP_Text` component reference.
- `SetText(string text)`: Set guide text.
- `Reset()` auto-assignment: `GetComponentInChildren<TMP_Text>()`. (§7 Reset() Auto-Assignment)
- Include `_logClass` tag. (§8)
- Use `[SerializeField] private`. (§8 Inspector Variables)

---

### Task 5 — Create TimingBarView.cs

- Inherits `MonoBehaviour`. `Image` component (bar background) and `RectTransform` (success zone area) references.
- `SetSuccessZone(float normalizedStart, float normalizedEnd)`: Set success zone position/size visually.
- `GetBarWidth()`: Return bar RectTransform width.
- `Reset()` auto-assignment. (§7) Exception: `RectTransform` is NOT auto-assigned. (§7 Exception)
- Include `_logClass` tag. (§8)
- Use `[SerializeField] private`. (§8)

---

### Task 6 — Create MarkerView.cs

- Inherits `MonoBehaviour`. `RectTransform` reference.
- `StartOscillation(float barWidth, float speed)`: Start left-right oscillation using DOTween. (§11)
- `StopOscillation()`: Stop at current position. DOTween Kill.
- `GetNormalizedPosition(float barWidth)`: Return current marker x position normalized to 0~1.
- `ResetPosition()`: Return marker to start position (0).
- Include `_logClass` tag. (§8)
- `OnDestroy()`: DOTween Kill. Use `?.` null-conditional. (§8 Safe Cleanup)
- GC optimization: No LINQ/new inside Update(). (§8)

---

### Task 7 — Create TouchButtonView.cs

- Inherits `MonoBehaviour`. `Button` component reference.
- `OnTouchClicked` event (Action).
- `SetInteractable(bool interactable)`: Set Button.interactable.
- `Reset()` auto-assignment. (§7)
- Include `_logClass` tag. (§8)
- Use `[SerializeField] private`. (§8)

---

### Task 8 — Create RoundIndicatorView.cs

- Inherits `MonoBehaviour`. `TMP_Text` (round text) + `Image[]` (round result icons) references.
- `SetRound(int current, int total)`: Display "2/3" format text.
- `SetRoundResult(int index, bool success)`: Display O/X icon. Color or sprite switch.
- `ResetAll()`: Reset all icons.
- `Reset()` auto-assignment. (§7) `Image[]` uses `GetComponentsInChildren<Image>()`.
- Include `_logClass` tag. (§8)
- Use `[SerializeField] private`. (§8)

---

### Task 9 — Create OptionButtonView.cs

- Inherits `MonoBehaviour`. `Button` component reference.
- `OnOptionClicked` event (Action).
- `Reset()` auto-assignment. (§7)
- Include `_logClass` tag. (§8)
- Use `[SerializeField] private`. (§8)

---

### Task 10 — Create MiniGameResultPopupView.cs

- Inherits `MonoBehaviour`. MiniGame-dedicated result popup.
- References: `TMP_Text` (verdict), `TMP_Text` (stat name), `TMP_Text` (before value), `TMP_Text` (after value), `Button` (return button).
- `async UniTask Show(MiniGameVerdict verdict, string statName, int beforeValue, int afterValue)`: Activate popup → await button tap. (§5 UniTask async)
- `Hide()`: Deactivate popup.
- Initial state: deactivated (`gameObject.SetActive(false)`).
- `Reset()` auto-assignment. (§7)
- Include `_logClass` tag. (§8)
- Use `[SerializeField] private`. (§8)
- No Coroutines. UniTask only. (§11)

---

### Task 11 — Create MiniGameView.cs

- All child Views as `[SerializeField] private`: `GuideTooltipView`, `TimingBarView`, `MarkerView`, `TouchButtonView`, `RoundIndicatorView`, `OptionButtonView`, `MiniGameResultPopupView`.
- Expose getter properties or direct references for Presenter access.
- `Reset()` auto-assignment. (§7) `RectTransform` exception.
- Include `_logClass` tag. (§8)
- Use `[SerializeField] private`. (§8)
- No null-guard on SerializeField — let NullReferenceException fire. (§7 Fail Fast)

---

### Task 12 — Create MiniGamePresenter.cs

- Constructor injection of `MiniGameUseCase`, `MiniGameView`, `ISceneNavigator`.
- Include `_logClass` tag. (§8)

**`Initialize(StatType statType)` method:**
1. Store training target stat.
2. `GetCurrentStatValue(statType)` → store before-value.
3. `GuideTooltipView.SetText()` set guide text.
4. `TimingBarView.SetSuccessZone(start, end)` set success zone.
5. `RoundIndicatorView.SetRound(1, RoundCount)` initialize.
6. Subscribe events: `TouchButtonView.OnTouchClicked`, `OptionButtonView.OnOptionClicked`.
7. Start first round: `MarkerView.StartOscillation()` + `TouchButtonView.SetInteractable(true)`.

**Round progression logic (OnTouchClicked handler):**
1. `TouchButtonView.SetInteractable(false)`.
2. `MarkerView.StopOscillation()`.
3. `MarkerView.GetNormalizedPosition()` → determine success zone hit.
4. `RoundIndicatorView.SetRoundResult(roundIndex, success)`.
5. Short delay (`await UniTask.Delay()`). (§11 UniTask, no Coroutines)
6. Next round: `MarkerView.ResetPosition()` → `StartOscillation()` → `SetInteractable(true)`.
7. Or final round → `FinishGame()`.

**`FinishGame()` method:**
1. Determine `MiniGameVerdict` by success count (0~1: Fail, 2: Maintain, 3: Success).
2. `MiniGameUseCase.CalculateStatDelta(verdict)` → delta.
3. beforeValue (stored during initialization).
4. afterValue = beforeValue + delta.
5. `await MiniGameUseCase.ApplyResultAndSave(statType, delta)`. (§9 Save before popup)
6. `await MiniGameResultPopupView.Show(verdict, statName, beforeValue, afterValue)`.
7. `ISceneNavigator.LoadScene(SceneKey.Maintenance)`.

**Timing parameters (private const):**
- `MarkerSpeed` (TBD)
- `SuccessZoneStart`, `SuccessZoneEnd` (TBD)
- `RoundResultDelay` (milliseconds, e.g., 500ms)

**`Dispose()` method:** Unsubscribe events. Use `?.` null-conditional. (§8 Safe Cleanup)

**Constitution check:** §3 No direct UI manipulation in Presenter. §5 UniTask only. §8 GC — no LINQ/new in Update().

---

### Task 13 — Create MiniGameSceneBootstrapper.cs

- `MonoBehaviour`. Async initialization in `Start()`. (§3 SceneBootstrapper from Start())
- `await GlobalBootstrapper.Instance.InitializationTask`.
- Read `PendingTrainingStat` from `GameContext`. If null, throw `InvalidOperationException`. (§7 Fail Fast) Reset to null after reading.
- Obtain `MiniGameUseCase` from `GameContext`.
- Obtain `ISceneNavigator` from `GlobalBootstrapper`.
- `MiniGameView` via `[SerializeField] private` Inspector connection.
- Create `MiniGamePresenter` instance and inject. (§3 Only Bootstrapper may new)
- Call `MiniGamePresenter.Initialize(statType)`.
- `OnDestroy()`: `MiniGamePresenter?.Dispose()`. (§8 Safe Cleanup)
- Include `_logClass` tag. (§8)
- No null-guard on SerializeField. (§7 Fail Fast)

---

### Task 14 — Create decisions.md

Create empty file at `.claude/specs/features/mini-game/decisions.md`.

---

## 5. Validation

| # | Item | How to Verify |
|---|---|---|
| V-01 | No compile errors in Unity console | Console check |
| V-02 | `SceneKey` enum contains `MiniGame` entry | File check |
| V-03 | `GameContext` has `PendingTrainingStat` and `MiniGameUseCase` accessor | File check |
| V-04 | `MiniGameSceneBootstrapper` runs without errors on Play | Editor check |
| V-05 | Marker oscillates left-right on timing bar | Editor Play check |
| V-06 | Touch button tap stops marker and displays O/X icon | Editor Play check |
| V-07 | Result popup shows after 3 rounds (verdict + stat before→after) | Editor Play check |
| V-08 | Result popup return button transitions to MaintenanceScene | Editor Play check |
| V-09 | Before popup display, `run_save.json` has AP -1 and stat delta | JSON file check |
| V-10 | All View classes include `_logClass` tag | Code check |
| V-11 | All MonoBehaviour Views implement `Reset()` auto-assignment | Code check |
| V-12 | `OnDestroy()` / `Dispose()` use `?.` null-conditional | Code check |

---

## 6. Manual Tasks (Hak performs after Claude Code implementation)

| Order | Task |
|---|---|
| MG-01 | Create `Assets/_Game/Scenes/MiniGame.unity` scene file |
| MG-02 | Set up Main Camera + Canvas (Screen Space - Camera) |
| MG-03 | Place MiniGameSceneBootstrapper at scene root, connect MiniGameView in Inspector |
| MG-04 | Connect all View [SerializeField] fields in Inspector |
| MG-05 | Add MiniGame scene to Build Settings |
| MG-06 | Set TimingBar background image + success zone image/color |
| MG-07 | Set marker image |
| MG-08 | Set touch button image/size |
| MG-09 | Set round result icons (success/fail) images |

---

## 7. Claude Code Handoff Guide

- Run `claude` from project root
- `CLAUDE.md` auto-loads
- Pass `.claude/specs/features/mini-game/tasks.md` for sequential implementation
- Record judgment calls in `.claude/specs/features/mini-game/decisions.md`
- Do NOT create files outside `Assets/_Game/` (except decisions.md)