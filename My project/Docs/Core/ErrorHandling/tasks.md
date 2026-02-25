# Tasks: Mobile Error Handling System

**Generated from**: [plan.md](./plan.md) | [specify.md](./specify.md)  
**Date**: 2026-02-11 | **Branch**: `feat/error-handling`

---

## How to read this file

| Symbol | Meaning |
|--------|---------|
| `[P]` | Parallelizable — can be worked on simultaneously with other `[P]` tasks in the same phase |
| `[Data]` | Data Layer — DTOs, enums, interfaces. **No MonoBehaviour. No logic.** |
| `[Domain]` | Domain Layer — Pure C# business logic. **Strictly NO UnityEngine UI references.** |
| `[Presentation]` | Presentation Layer — MonoBehaviour Views, Presenters. **Must implement IDisposable; inject via constructor.** |
| `[Bootstrapper]` | Wiring — **The ONLY place allowed to use `new` for Domain/Data classes.** |

---

## Phase 1: Data Layer

> Pure DTOs, enums, interfaces. No MonoBehaviour, no behavior. Placed in `Assets/_Game/Core/ErrorHandling/`.

- [x] T001 [P] [Data] Create `ErrorSeverity` enum (WARNING, NETWORK, CRITICAL) in `Assets/_Game/Core/ErrorHandling/ErrorSeverity.cs` *(spec FR-02)*
- [x] T002 [P] [Data] Create `ErrorReport` DTO (Severity, Message, StackTrace, SnapshotId, TimestampUtc, ContextScene, ContextFrame — raw data only; JSON-serializable via Newtonsoft.Json) in `Assets/_Game/Core/ErrorHandling/ErrorReport.cs` *(spec FR-08)*
- [x] T003 [P] [Data] Create `LogRingBufferEntry` struct (LogType, Message, FrameIndex — non-allocating; use struct not class. *Note:* Using `UnityEngine.LogType` here couples Core to Unity; acceptable for this project; if future portability is required, consider a local `LogCategory` enum in Core (L3).) in `Assets/_Game/Core/ErrorHandling/LogRingBufferEntry.cs` *(spec FR-01)*
- [x] T004 [P] [Data] Create `StateSnapshot` DTO (SnapshotId, SceneName, FrameCount, DeviceModel, OperatingSystem, MemoryUsage, DataSummaryJson, RingBufferPayload as `LogRingBufferEntry[]` — raw fields only) in `Assets/_Game/Core/ErrorHandling/StateSnapshot.cs` *(spec FR-04)*
- [x] T005 [P] [Data] Create `NetworkRetryCommand` wrapper (holds `Func<UniTask>` retryAction; captures exact failed operation for 1:1 retry; no side effects) in `Assets/_Game/Core/ErrorHandling/NetworkRetryCommand.cs` *(spec FR-06)*
- [x] T006 [P] [Data] Create `IStabilityFlag` interface (`bool IsSaveAllowed { get; }`) in `Assets/_Game/Core/ErrorHandling/IStabilityFlag.cs` *(spec FR-07)*
- [x] T007 [P] [Data] Create `IErrorReportSink` interface (`void Report(ErrorReport report)`) in `Assets/_Game/Core/ErrorHandling/IErrorReportSink.cs` *(spec FR-08)*
- [x] T008 [P] [Data] Create `IPopupService` interface (`UniTask<bool> ShowCommonPopup(string title, string desc, string firstText, string secondText)`) in `Assets/_Game/Core/ErrorHandling/IPopupService.cs` *(spec UR-06, Constitution: Domain must not depend on concrete PopupManager)*
- [x] T009 [P] [Data] Create `ITitleNavigationService` interface (`void NavigateToTitle()`) in `Assets/_Game/Core/ErrorHandling/ITitleNavigationService.cs` *(spec FR-06, Constitution: Domain must not call SceneManager directly)*

---

## Phase 2: Domain Layer

> Pure C# business logic. **Strictly NO UnityEngine references** in Domain (scene load and popup are behind ITitleNavigationService and IPopupService). Placed in `Assets/_Game/App/Systems/ErrorHandling/`.

- [x] T010 [P] [Domain] Implement `StabilityFlag` (Pure C#; implements `IStabilityFlag`; flag defaults to `true`; `Block()` method sets to `false` permanently per process — never reset to `true` in same session) in `Assets/_Game/App/Systems/ErrorHandling/StabilityFlag.cs` *(spec FR-07)*
- [x] T011 [P] [Domain] Implement `LogRingBuffer` (Pure C#; fixed-size array of 100 `LogRingBufferEntry`; modulo-index wrapping; zero allocation on append; expose `LogRingBufferEntry[] GetSnapshot()` — copy current entries into a new array so `StateSnapshot.RingBufferPayload` can store it; avoid ReadOnlySpan for DTO compatibility) in `Assets/_Game/App/Systems/ErrorHandling/LogRingBuffer.cs` *(spec FR-01, M2)*
- [x] T012 [Domain] Implement `ErrorClassifier` (Pure C#; **Strictly NO UnityEngine references**; input: `Exception exception, ErrorSeverity? explicitTag`; if tag provided use it directly; else classify by Exception type: NetworkException/TimeoutException/SocketException → NETWORK, NullReferenceException/IndexOutOfRangeException/InvalidOperationException → CRITICAL, ArgumentException/ArgumentNullException → WARNING, unknown → CRITICAL fail-safe; return `ErrorSeverity`) in `Assets/_Game/App/Systems/ErrorHandling/ErrorClassifier.cs` *(spec FR-02)*
- [x] T013 [Domain] Implement `ErrorSnapshotCapture` (Pure C#; **Strictly NO UnityEngine UI references**; constructor receives `GameContext`; `CaptureAsync()` → reads Scene.name, Time.frameCount, SystemInfo.deviceModel/OS, calls `GameContext.GetDataSummaryJson()`, copies Ring Buffer snapshot via `LogRingBuffer.GetSnapshot()` into `StateSnapshot.RingBufferPayload`; offload JSON serialization to `UniTask.RunOnThreadPool` to avoid frame drops; return `StateSnapshot`. *Permitted:* UnityEngine.SceneManagement/Time/SystemInfo as read-only diagnostic APIs only; no state mutation, no UI (M4).) in `Assets/_Game/App/Systems/ErrorHandling/ErrorSnapshotCapture.cs` *(spec FR-04)*

  > **Prerequisite**: `GameContext` must expose `GetDataSummaryJson()`. Add this method to `Assets/_Game/App/Context/GameContext.cs` before implementing.

- [x] T014 [Domain] Add `GetDataSummaryJson()` to existing `GameContext` (serialize summaries of all held Repositories into JSON string; use Newtonsoft.Json; no UnityEngine UI) in `Assets/_Game/App/Context/GameContext.cs` *(spec FR-04 clarification)*
- [x] T015 [Domain] Implement `ErrorReportTransmission` (Pure C#; implements `IErrorReportSink`; constructor receives no external deps for now; `Report(ErrorReport)`: (1) attempt remote send — stub/no-op until endpoint defined; (2) on failure: encrypt and write to `Application.persistentDataPath + "/error_reports/"` via `UniTask.RunOnThreadPool`; (3) on app launch: scan buffer dir, retry send, delete on success) in `Assets/_Game/App/Systems/ErrorHandling/ErrorReportTransmission.cs` *(spec FR-08)*
- [x] T016 [Domain] Implement `ErrorRecoveryFlow` (Pure C#; **Must implement IDisposable**; **constructor**: `IStabilityFlag stabilityFlag, IErrorReportSink reportSink, IPopupService popupService, ITitleNavigationService titleNavigation` — **no GameContext, no PopupManager, no UnityEngine**; one-shot CRITICAL guard `_criticalHandled`; **retry counter**: start at 0 on first NETWORK failure, increment after each failed retry; escalate to CRITICAL when counter reaches 3 (i.e. 3 retries attempted = 4th total failure) (L2); `NetworkRetryCommand` cache; `HandleCriticalAsync(ErrorReport, StateSnapshot)`: if already handled skip, else flag.Block(), report to sink, await `popupService.ShowCommonPopup` with "To Title" only → call `titleNavigation.NavigateToTitle()`; `HandleNetworkAsync(ErrorReport, NetworkRetryCommand)`: show "Reconnect"+"To Title" via `popupService.ShowCommonPopup`, on Reconnect await command.Invoke(), increment counter, if count ≥ 4 escalate to CRITICAL) in `Assets/_Game/App/Systems/ErrorHandling/ErrorRecoveryFlow.cs` *(spec FR-05, FR-06, FR-09)*

---

## Phase 3: Presentation Layer (and App/Systems lifecycle)

> T017 is [Domain]: App-level lifecycle hook (no UI). T018–T020 are [Presentation]: Views and Presenters. **Must implement IDisposable. Inject dependencies via constructor.**

- [ ] T017 [Domain] Implement `GlobalErrorInterceptor` (MonoBehaviour; **App/Systems lifecycle hook only — no UI logic** (M1); init via `Initialize()` called by Bootstrapper; `OnEnable`: register `Application.logMessageReceived` + `AppDomain.UnhandledException`; `OnDisable`: unregister; on log callback: if `LogType.Log/Warning` → append to Ring Buffer only (no state change); if `LogType.Error/Exception` → classify → snapshotAsync → if CRITICAL call RecoveryFlow.HandleCriticalAsync, else if NETWORK call RecoveryFlow.HandleNetworkAsync; hold `CancellationToken` from `GetCancellationTokenOnDestroy`) in `Assets/_Game/App/Systems/ErrorHandling/GlobalErrorInterceptor.cs` *(spec FR-01, FR-03)*
- [ ] T018 [Presentation] Create `ErrorPopupView` MonoBehaviour (full-screen modal **content** shown by PopupManager (IPopupService); Dimmer CanvasGroup; TMP_Text for title, message, debug info (ErrorCode + SnapshotId); Buttons for Reconnect (hide when escalated) and ToTitle; `Reset()` auto-assigns components via `GetComponentInChildren`; expose `ShowAsync(...)` and **`SetButtonsInteractable(bool)` for loading state (UR-05)**; PopupManager hosts this view with standard entry/exit animations (UR-06, M5)) in `Assets/_Game/Features/ErrorHandling/Presentation/ErrorPopupView.cs` *(spec UR-01 to UR-05)*
- [ ] T019 [Presentation] Create `ErrorPopupPresenter` (Pure C#; **Must implement IDisposable**; **inject via constructor**: `ErrorPopupView view, ErrorRecoveryFlow recoveryFlow`; `Initialize()`: subscribe to recovery flow state changes, render initial state; **while retry is in-flight** (e.g. NetworkRetryCommand.Invoke() running), call `_view.SetButtonsInteractable(false)` and show loading state; restore interactability on completion (UR-05, M3); `Dispose()`: unsubscribe all events; never directly access UI components — call methods on view only) in `Assets/_Game/Features/ErrorHandling/Presentation/ErrorPopupPresenter.cs` *(spec UR-01 to UR-06)*
- [ ] T020 [Presentation] Create `ErrorPopupView.prefab` (attach `ErrorPopupView` MonoBehaviour; include Dimmer, title TMP_Text, message TMP_Text, debug info TMP_Text, Reconnect Button, ToTitle Button; set Canvas SortOrder above gameplay; verify `Reset()` assigns all components correctly) in `Assets/_Game/Features/ErrorHandling/Presentation/Prefabs/ErrorPopupView.prefab` *(spec UR-01)*

---

## Phase 4: Bootstrapper (DI & Wiring)

> **The ONLY place allowed to use `new` for Domain/Data classes.** Modify existing `GlobalBootstrapper` and `AutoSaveManager`.

- [ ] T021 [Bootstrapper] Add **method injection** for `IStabilityFlag` in existing `AutoSaveManager`: add `void Initialize(IStabilityFlag flag)`; store as `_stabilityFlag`; **MonoBehaviour cannot use constructor injection.** In `SaveAllAsync()`, `SaveAllSync()`, and `OnApplicationPause/Quit`: check `_stabilityFlag.IsSaveAllowed` — if `false` skip save entirely, do not set dirty, do not call repository methods. In `Assets/_Game/App/Systems/AutoSaveManager.cs` *(spec FR-07)*
- [ ] T022 [Bootstrapper] Create `TitleNavigationService` (implements `ITitleNavigationService`; `NavigateToTitle()` calls `SceneManager.LoadScene("IntroScene")`) in `Assets/_Game/App/Systems/ErrorHandling/TitleNavigationService.cs`. Ensure existing `PopupManager` implements `IPopupService` (add interface to `Assets/_Game/App/Systems/PopupManager.cs` with `ShowCommonPopup` signature). *(spec FR-06, Constitution C1/H1)*
- [ ] T023 [Bootstrapper] Wire Error Handling System in existing `GlobalBootstrapper` (in `Awake`, **only place to use `new`**: (1) `new StabilityFlag()`, (2) `new TitleNavigationService()`, (3) `new LogRingBuffer()`, (4) `new ErrorClassifier()`, (5) `new ErrorSnapshotCapture(GameContext)`, (6) `new ErrorReportTransmission()`, (7) `new ErrorRecoveryFlow(stabilityFlag, transmission, popupService, titleNavigation)` — pass **IPopupService** (PopupManager) and **ITitleNavigationService** (TitleNavigationService); **do not pass GameContext**, (8) `new GlobalErrorInterceptor(...)` → call `.Initialize()`, (9) call `_autoSaveManager.Initialize(stabilityFlag)`, (10) **wrap** `transmission.RetryBufferedReportsAsync()` in try/catch or `.ContinueWith(e => Debug.LogWarning("RetryBufferedReports failed", e))` then `.Forget()` so startup retry failures are logged (L1)) in `Assets/_Game/App/Bootstrapper/GlobalBootstrapper.cs` *(spec FR-07)*

---

## Phase 5: Integration — Business Logic Guard

> Add explicit `IStabilityFlag` checks to all features that mutate Data. This is the "Business Logic Block" for FR-03 (Option 1: explicit checks). Apply to each feature that modifies Repository state.

- [ ] T024 [Domain] Add `IStabilityFlag` guard to combat logic (any UseCase/system that modifies character stats or health: check `IStabilityFlag.IsSaveAllowed` before mutation; if `false` return/skip immediately). **경로 입력 필요** — 대상 파일: `Assets/_Game/Features/` 내 해당 UseCase/시스템 `.cs` *(spec FR-03)*
- [ ] T025 [Domain] Add `IStabilityFlag` guard to inventory mutation logic (any UseCase/system that calls `InventoryRepo.AddItem`, `ConsumeItem` etc.: check flag before mutation). **경로 입력 필요** — 대상 파일: `Assets/_Game/Features/` 내 해당 UseCase/시스템 `.cs` *(spec FR-03)*
- [ ] T026 [Domain] Add `IStabilityFlag` guard to character progression logic (any UseCase/system that modifies CharacterRepository or evolution state: check flag before mutation). **경로 입력 필요** — 대상 파일: `Assets/_Game/Features/` 내 해당 UseCase/시스템 `.cs` *(spec FR-03)*

---

## Dependency Order

```text
T001–T009  (Data: DTOs & Interfaces including IPopupService, ITitleNavigationService)
    ↓ [all parallelizable]
T010–T011  (StabilityFlag, LogRingBuffer — no deps)
    ↓
T012       (ErrorClassifier — depends on T001 ErrorSeverity)
T014       (GameContext.GetDataSummaryJson — depends on GameContext)
    ↓
T013       (ErrorSnapshotCapture — depends on T004 StateSnapshot, T014 GameContext)
T015       (ErrorReportTransmission — depends on T002 ErrorReport, T007 IErrorReportSink)
    ↓
T016       (ErrorRecoveryFlow — depends on T010, T015, T008, T009, T005; no GameContext)
    ↓
T017       (GlobalErrorInterceptor — depends on T011 Ring Buffer, T012 Classifier, T013 Snapshot, T016 RecoveryFlow)
T018       (ErrorPopupView — depends on T004, T002)
    ↓
T019       (ErrorPopupPresenter — depends on T018, T016)
T020       (Prefab — depends on T018)
    ↓
T021       (AutoSaveManager.Initialize(IStabilityFlag) — depends on T006 IStabilityFlag)
T022       (TitleNavigationService + PopupManager implements IPopupService — depends on T008, T009)
T023       (GlobalBootstrapper wiring — depends on ALL above; call AutoSaveManager.Initialize(stabilityFlag))
    ↓
T024–T026  (Feature guards — 경로 입력 필요; depends on T006 IStabilityFlag, T023)
```

---

## Progress Tracker

| Phase | Tasks | Done | Remaining |
|-------|-------|------|-----------|
| 1: Data | T001–T009 | 9 | 0 |
| 2: Domain | T010–T016 | 7 | 0 |
| 3: Presentation (+ T017 Domain) | T017–T020 | 0 | 4 |
| 4: Bootstrapper | T021–T023 | 0 | 3 |
| 5: Integration | T024–T026 | 0 | 3 *(경로 입력 필요)* |
| **Total** | **26** | **16** | **10** |
