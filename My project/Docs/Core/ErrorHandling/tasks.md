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

- [ ] T001 [P] [Data] Create `ErrorSeverity` enum (WARNING, NETWORK, CRITICAL) in `Assets/_Game/Core/ErrorHandling/ErrorSeverity.cs` *(spec FR-02)*
- [ ] T002 [P] [Data] Create `ErrorReport` DTO (Severity, Message, StackTrace, SnapshotId, TimestampUtc, ContextScene, ContextFrame — raw data only; JSON-serializable via Newtonsoft.Json) in `Assets/_Game/Core/ErrorHandling/ErrorReport.cs` *(spec FR-08)*
- [ ] T003 [P] [Data] Create `LogRingBufferEntry` struct (LogType, Message, FrameIndex — non-allocating; use struct not class) in `Assets/_Game/Core/ErrorHandling/LogRingBufferEntry.cs` *(spec FR-01)*
- [ ] T004 [P] [Data] Create `StateSnapshot` DTO (SnapshotId, SceneName, FrameCount, DeviceModel, OperatingSystem, MemoryUsage, DataSummaryJson, RingBufferPayload as `LogRingBufferEntry[]` — raw fields only) in `Assets/_Game/Core/ErrorHandling/StateSnapshot.cs` *(spec FR-04)*
- [ ] T005 [P] [Data] Create `NetworkRetryCommand` wrapper (holds `Func<UniTask>` retryAction; captures exact failed operation for 1:1 retry; no side effects) in `Assets/_Game/Core/ErrorHandling/NetworkRetryCommand.cs` *(spec FR-06)*
- [ ] T006 [P] [Data] Create `IStabilityFlag` interface (`bool IsSaveAllowed { get; }`) in `Assets/_Game/Core/ErrorHandling/IStabilityFlag.cs` *(spec FR-07)*
- [ ] T007 [P] [Data] Create `IErrorReportSink` interface (`void Report(ErrorReport report)`) in `Assets/_Game/Core/ErrorHandling/IErrorReportSink.cs` *(spec FR-08)*

---

## Phase 2: Domain Layer

> Pure C# business logic. **Strictly NO UnityEngine UI references** (UnityEngine.SceneManagement allowed only in ErrorRecoveryFlow for LoadScene). Placed in `Assets/_Game/App/Systems/ErrorHandling/`.

- [ ] T008 [P] [Domain] Implement `StabilityFlag` (Pure C#; implements `IStabilityFlag`; flag defaults to `true`; `Block()` method sets to `false` permanently per process — never reset to `true` in same session) in `Assets/_Game/App/Systems/ErrorHandling/StabilityFlag.cs` *(spec FR-07)*
- [ ] T009 [P] [Domain] Implement `LogRingBuffer` (Pure C#; fixed-size array of 100 `LogRingBufferEntry`; modulo-index wrapping; zero allocation on append; expose `ReadOnlySpan<LogRingBufferEntry> GetSnapshot()`) in `Assets/_Game/App/Systems/ErrorHandling/LogRingBuffer.cs` *(spec FR-01)*
- [ ] T010 [Domain] Implement `ErrorClassifier` (Pure C#; **Strictly NO UnityEngine references**; input: `Exception exception, ErrorSeverity? explicitTag`; if tag provided use it directly; else classify by Exception type: NetworkException/TimeoutException/SocketException → NETWORK, NullReferenceException/IndexOutOfRangeException/InvalidOperationException → CRITICAL, ArgumentException/ArgumentNullException → WARNING, unknown → CRITICAL fail-safe; return `ErrorSeverity`) in `Assets/_Game/App/Systems/ErrorHandling/ErrorClassifier.cs` *(spec FR-02)*
- [ ] T011 [Domain] Implement `ErrorSnapshotCapture` (Pure C#; **Strictly NO UnityEngine UI references**; constructor receives `GameContext`; `CaptureAsync()` → reads Scene.name, Time.frameCount, SystemInfo.deviceModel/OS, calls `GameContext.GetDataSummaryJson()`, copies Ring Buffer snapshot; offload JSON serialization to `UniTask.RunOnThreadPool` to avoid frame drops; return `StateSnapshot`) in `Assets/_Game/App/Systems/ErrorHandling/ErrorSnapshotCapture.cs` *(spec FR-04)*

  > **Prerequisite**: `GameContext` must expose `GetDataSummaryJson()`. Add this method to `Assets/_Game/App/Context/GameContext.cs` before implementing.

- [ ] T012 [Domain] Add `GetDataSummaryJson()` to existing `GameContext` (serialize summaries of all held Repositories into JSON string; use Newtonsoft.Json; no UnityEngine UI) in `Assets/_Game/App/Context/GameContext.cs` *(spec FR-04 clarification)*
- [ ] T013 [Domain] Implement `ErrorReportTransmission` (Pure C#; implements `IErrorReportSink`; constructor receives no external deps for now; `Report(ErrorReport)`: (1) attempt remote send — stub/no-op until endpoint defined; (2) on failure: encrypt and write to `Application.persistentDataPath + "/error_reports/"` via `UniTask.RunOnThreadPool`; (3) on app launch: scan buffer dir, retry send, delete on success) in `Assets/_Game/App/Systems/ErrorHandling/ErrorReportTransmission.cs` *(spec FR-08)*
- [ ] T014 [Domain] Implement `ErrorRecoveryFlow` (Pure C#; **Must implement IDisposable**; inject via constructor: `IStabilityFlag stabilityFlag, IErrorReportSink reportSink, PopupManager popupManager, GameContext gameContext`; responsibilities: one-shot CRITICAL guard `_criticalHandled`; network retry counter; `NetworkRetryCommand` cache; `HandleCriticalAsync(ErrorReport, StateSnapshot)`: block if already handled, else set flag.Block(), report to sink, await popup with "To Title" only → `SceneManager.LoadScene("IntroScene")`; `HandleNetworkAsync(ErrorReport, NetworkRetryCommand)`: show "Reconnect"+"To Title" modal, on Reconnect await command.Invoke(), increment counter, if count ≥ 4 escalate to CRITICAL; all popup calls via `PopupManager.ShowCommonPopup`) in `Assets/_Game/App/Systems/ErrorHandling/ErrorRecoveryFlow.cs` *(spec FR-05, FR-06, FR-09)*

---

## Phase 3: Presentation Layer

> MonoBehaviour Views and Presenters. **Must implement IDisposable. Inject dependencies via constructor.**

- [ ] T015 [Presentation] Implement `GlobalErrorInterceptor` (MonoBehaviour; constructor-friendly init via `Initialize()` called by Bootstrapper; `OnEnable`: register `Application.logMessageReceived` + `AppDomain.UnhandledException`; `OnDisable`: unregister; on log callback: if `LogType.Log/Warning` → append to Ring Buffer only (no state change); if `LogType.Error/Exception` → classify → snapshotAsync → if CRITICAL call RecoveryFlow.HandleCriticalAsync, else if NETWORK call RecoveryFlow.HandleNetworkAsync; hold `CancellationToken` from `GetCancellationTokenOnDestroy`) in `Assets/_Game/App/Systems/ErrorHandling/GlobalErrorInterceptor.cs` *(spec FR-01, FR-03)*
- [ ] T016 [Presentation] Create `ErrorPopupView` MonoBehaviour (full-screen modal with Dimmer CanvasGroup; TMP_Text for title, message, debug info (ErrorCode + SnapshotId); Buttons for Reconnect (hide when escalated) and ToTitle; `Reset()` auto-assigns components via `GetComponentInChildren`; expose `ShowAsync(string title, string message, string errorCode, string snapshotId, bool showReconnect)` — use existing PopupManager animations (UR-06)) in `Assets/_Game/Features/ErrorHandling/Presentation/ErrorPopupView.cs` *(spec UR-01 to UR-05)*
- [ ] T017 [Presentation] Create `ErrorPopupPresenter` (Pure C#; **Must implement IDisposable**; **inject via constructor**: `ErrorPopupView view, ErrorRecoveryFlow recoveryFlow`; `Initialize()`: subscribe to recovery flow state changes, render initial state; `Dispose()`: unsubscribe all events; never directly access UI components — call methods on view only) in `Assets/_Game/Features/ErrorHandling/Presentation/ErrorPopupPresenter.cs` *(spec UR-01 to UR-06)*
- [ ] T018 [Presentation] Create `ErrorPopupView.prefab` (attach `ErrorPopupView` MonoBehaviour; include Dimmer, title TMP_Text, message TMP_Text, debug info TMP_Text, Reconnect Button, ToTitle Button; set Canvas SortOrder above gameplay; verify `Reset()` assigns all components correctly) in `Assets/_Game/Features/ErrorHandling/Presentation/Prefabs/ErrorPopupView.prefab` *(spec UR-01)*

---

## Phase 4: Bootstrapper (DI & Wiring)

> **The ONLY place allowed to use `new` for Domain/Data classes.** Modify existing `GlobalBootstrapper` and `AutoSaveManager`.

- [ ] T019 [Bootstrapper] Add `IStabilityFlag` injection to existing `AutoSaveManager` (add `[SerializeField]`-free constructor parameter or setter; in `SaveAllAsync()` and `SaveAllSync()` and `OnApplicationPause/Quit`: check `IStabilityFlag.IsSaveAllowed` — if `false` skip save entirely, do not set dirty, do not call repository methods) in `Assets/_Game/App/Systems/AutoSaveManager.cs` *(spec FR-07)*
- [ ] T020 [Bootstrapper] Wire Error Handling System in existing `GlobalBootstrapper` (in `Awake`, **only place to use `new`**: (1) `new StabilityFlag()`, (2) `new LogRingBuffer()`, (3) `new ErrorClassifier()`, (4) `new ErrorSnapshotCapture(GameContext)`, (5) `new ErrorReportTransmission()`, (6) `new ErrorRecoveryFlow(stabilityFlag, transmission, _popupManager, GameContext)`, (7) `new GlobalErrorInterceptor(...)` → call `.Initialize()`, (8) inject `stabilityFlag` into `_autoSaveManager`, (9) call `transmission.RetryBufferedReportsAsync().Forget()` for FR-08 auto-retry on launch) in `Assets/_Game/App/Bootstrapper/GlobalBootstrapper.cs` *(spec FR-07)*

---

## Phase 5: Integration — Business Logic Guard

> Add explicit `IStabilityFlag` checks to all features that mutate Data. This is the "Business Logic Block" for FR-03 (Option 1: explicit checks). Apply to each feature that modifies Repository state.

- [ ] T021 [Domain] Add `IStabilityFlag` guard to combat logic (any UseCase/system that modifies character stats or health: check `IStabilityFlag.IsSaveAllowed` before mutation; if `false` return/skip immediately) in affected files under `Assets/_Game/Features/` *(spec FR-03)*
- [ ] T022 [Domain] Add `IStabilityFlag` guard to inventory mutation logic (any UseCase/system that calls `InventoryRepo.AddItem`, `ConsumeItem` etc.: check flag before mutation) in affected files under `Assets/_Game/Features/` *(spec FR-03)*
- [ ] T023 [Domain] Add `IStabilityFlag` guard to character progression logic (any UseCase/system that modifies CharacterRepository or evolution state: check flag before mutation) in affected files under `Assets/_Game/Features/` *(spec FR-03)*

---

## Dependency Order

```text
T001–T007  (Data DTOs & Interfaces)
    ↓ [all parallelizable]
T008–T009  (StabilityFlag, LogRingBuffer — no deps)
    ↓
T010       (ErrorClassifier — depends on ErrorSeverity T001)
T012       (GameContext.GetDataSummaryJson — depends on GameContext)
    ↓
T011       (ErrorSnapshotCapture — depends on T004 StateSnapshot, T012 GameContext)
T013       (ErrorReportTransmission — depends on T002 ErrorReport, T007 IErrorReportSink)
    ↓
T014       (ErrorRecoveryFlow — depends on T008, T013, T005 NetworkRetryCommand)
    ↓
T015       (GlobalErrorInterceptor — depends on T009 Ring Buffer, T010 Classifier, T011 Snapshot, T014 RecoveryFlow)
T016       (ErrorPopupView — depends on T004, T002)
    ↓
T017       (ErrorPopupPresenter — depends on T016, T014)
T018       (Prefab — depends on T016)
    ↓
T019       (AutoSaveManager update — depends on T006 IStabilityFlag)
T020       (GlobalBootstrapper wiring — depends on ALL above)
    ↓
T021–T023  (Feature guards — depends on T006 IStabilityFlag, T020)
```

---

## Progress Tracker

| Phase | Tasks | Done | Remaining |
|-------|-------|------|-----------|
| 1: Data | T001–T007 | 0 | 7 |
| 2: Domain | T008–T014 | 0 | 7 |
| 3: Presentation | T015–T018 | 0 | 4 |
| 4: Bootstrapper | T019–T020 | 0 | 2 |
| 5: Integration | T021–T023 | 0 | 3 |
| **Total** | **23** | **0** | **23** |
