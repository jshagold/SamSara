# Implementation Plan: Mobile Error Handling System

**Branch**: `feat/error-handling` | **Date**: 2026-02-11 | **Spec**: [specify.md](./specify.md)  
**Input**: `Docs/Core/ErrorHandling/specify.md` (with clarifications)

---

## Summary

**Goal (spec §1)**  
Centralized runtime error management: prevent data corruption, avoid app crashes, enable precise root-cause analysis.

**Approach**  
Interceptor (FR-01: LogType.Error/Exception only + Ring Buffer) → Classify WARNING/NETWORK/CRITICAL (FR-02: Hybrid explicit tagging + Exception Type) → On CRITICAL: emergency stop with Save-Blocker priority (FR-03), one-shot handler (FR-05), Stability Flag block (FR-07), snapshot via GameContext (FR-04), transmission/buffer (FR-08), recovery UI with Command-pattern retry (FR-06). Network escalation after 4th failure to CRITICAL (FR-09). System does **not** implement saving; it exposes a **Stability Flag** that saving logic must respect.

---

## Technical Context

| Item | Value |
|------|--------|
| **Language/Version** | C# (Unity 6.2) |
| **Dependencies** | UniTask, Newtonsoft.Json; **existing**: `PopupManager`, `AutoSaveManager`, `GameContext` |
| **Storage** | Remote (TBD); device secure cache for offline report buffer (FR-08) |
| **Target** | Mobile (Android / iOS) |
| **Project** | Unity, feature-based under `Assets/_Game/` |
| **Constraints** | No frame drops from interception/snapshot; zero-guessing logs; Data/Logic/UI separation (spec §7); Ring Buffer must be non-allocating |

**Verified paths**: `Assets/_Game/App/Bootstrapper/GlobalBootstrapper.cs`, `Assets/_Game/App/Systems/PopupManager.cs` (has `ShowCommonPopup`), `Assets/_Game/App/Systems/AutoSaveManager.cs`, `Assets/_Game/App/Context/GameContext.cs`.

---

## Constitution Check

| Principle | Status |
|-----------|--------|
| Zero Guessing | ✅ Spec §7: raw data only; all clarifications resolved. |
| Data/Logic Separation | ✅ ErrorData, ErrorUI, ErrorLogic (spec §7). |
| Pure DI | ✅ Logic constructor-injected; Bootstrapper uses `new`. |
| Bootstrapper Hierarchy | ✅ Error system wired from GlobalBootstrapper. |
| Layer Rules | ✅ Core for DTOs/interfaces; App for systems; Features for UI. |

**Gate**: PASS.

---

## A. Pure Data Structures

**Rule**: Data/Logic separation. No MonoBehaviour. Raw, verified data only (no speculative fields).

| Type | Purpose | Fields |
|------|---------|--------|
| **ErrorSeverity** | `enum`: WARNING, NETWORK, CRITICAL (FR-02). | — |
| **ErrorReport** | DTO: Report payload for transmission/buffer (FR-08). JSON-serializable. | `Severity`, `Message` (raw), `StackTrace` (raw), `SnapshotId`, `TimestampUtc`, `ContextScene`, `ContextFrame` |
| **StateSnapshot** | DTO: Whole-state snapshot for diagnostics (FR-04). | `SnapshotId`, `SceneName`, `FrameCount`, `DeviceModel`, `OperatingSystem`, optional `MemoryUsage`, `DataSummaryJson` (from `GameContext.GetDataSummaryJson()`), `RingBufferPayload` (last N Log/Warning entries) |
| **LogRingBufferEntry** | Entry in Ring Buffer (context only, non-allocating). | `LogType`, `Message`, `Timestamp` (or frame index) |
| **Stability flag state** | Single boolean (or read-only property): save allowed vs blocked. Set to blocked on CRITICAL (or NETWORK escalation); never reset until process exit (FR-07). | — |
| **NetworkRetryCommand** | Cached command/action for retry (FR-06). | `Func<UniTask>` or command object; captures exact failed operation |

**Placement**: `Assets/_Game/Core/ErrorHandling/` for types (ErrorSeverity, ErrorReport, StateSnapshot, LogRingBufferEntry, NetworkRetryCommand); StabilityFlag **implementation** in `Assets/_Game/App/Systems/ErrorHandling/`.

---

## B. Interfaces & Events

| Contract | Purpose | Signature |
|----------|---------|-----------|
| **IStabilityFlag** | Save-block contract (FR-07). Consumed by **AutoSaveManager** before any save. | `bool IsSaveAllowed { get; }` |
| **IErrorReportSink** | Error report transmission/buffer (FR-08). Interceptor calls; Transmission implements. | `void Report(ErrorReport report)` |
| **PopupManager** | Existing popup system for error modal (UR-06). Verified: `ShowCommonPopup(string title, string desc, string firstText, string secondText)` returns `UniTask<bool>`. | Use existing method; `firstText` = "Reconnect" or empty; `secondText` = "To Title"; return value indicates user choice. |

**Events**:  
- Optional: `Action<ErrorReport> OnErrorReported` if single handler pattern preferred over `IErrorReportSink`.  
- Recovery flow events: Internal to ErrorRecoveryFlow; no global events needed if PopupManager await pattern is used.

All dependencies injected via constructor; Bootstrapper wires.

---

## C. System Logic & Integration

**Handlers (Unity/C#)**  

- **GlobalErrorInterceptor** (MonoBehaviour or static hook):
  - Register `Application.logMessageReceived` (filter: `LogType.Error` and `LogType.Exception` only for error handling flows).
  - Register `AppDomain.UnhandledException`.
  - Maintain lightweight, non-allocating Ring Buffer for `LogType.Log` and `LogType.Warning` (context only; never triggers state changes).
  - On Error/Exception capture → classify → snapshot (with Ring Buffer payload) → if CRITICAL set Stability Flag + one-shot recovery → Report to sink → show recovery UI.
  - Performance: Ring Buffer must use fixed-size array with index wrapping; no allocations during capture.

- **ErrorClassifier** (Pure C#):
  - Input: exception/log message + optional explicit severity tag.
  - **Hybrid Strategy** (FR-02):
    - If explicit tag provided (developer-controlled): use tag directly.
    - Else (unhandled system exception): classify by Exception Type:
      - `NetworkException`, `TimeoutException`, `SocketException` → `NETWORK`
      - `NullReferenceException`, `IndexOutOfRangeException`, `InvalidOperationException` → `CRITICAL`
      - `ArgumentException`, `ArgumentNullException` → `WARNING` (unless context indicates CRITICAL)
      - Unknown types → `CRITICAL` (fail-safe).
  - Output: `ErrorSeverity`.

- **ErrorSnapshotCapture** (Pure C#):
  - Builds `StateSnapshot`:
    - Execution context: `Scene.name`, `Time.frameCount`, `SystemInfo.deviceModel`, `SystemInfo.operatingSystem`, optional memory.
    - Data summary: Call `GameContext.GetDataSummaryJson()` (FR-04 clarification).
    - Ring Buffer payload: Copy current Ring Buffer entries (read-only snapshot).
  - Must not cause frame drops; use async/ThreadPool if JSON serialization is heavy.

- **StabilityFlag** (Pure C#):
  - Implements `IStabilityFlag`.
  - Set to `false` (block) on CRITICAL (or NETWORK escalation to CRITICAL).
  - Never set back to `true` in the same process (FR-07).

- **ErrorRecoveryFlow** (Pure C#):
  - **One-shot guard** for CRITICAL (FR-05): static/session flag `_criticalHandled`; skip handler if already true.
  - **Network retry counter**: Track consecutive retries; at 4th failure, escalate to CRITICAL (FR-09).
  - **Command caching** (FR-06): When NETWORK error occurs, capture failed operation as `NetworkRetryCommand` (e.g., `Func<UniTask>`). On Retry click, re-invoke only this cached command (1:1 retry, isolated).
  - Decides which UI to show: Reconnect+Title (network, count < 4) vs Title only (escalated/critical).
  - Integrates with PopupManager: Call `ShowCommonPopup` with appropriate button labels; await result; update counter or trigger escalation.
  - **Return to Title**: On "To Title" action, call `SceneManager.LoadScene("IntroScene")` to return to title screen.

- **ErrorReportTransmission** (Pure C#):
  - Implements `IErrorReportSink`.
  - On `Report(ErrorReport)`:
    1. Try send to remote endpoint (TBD; stub/no-op until endpoint defined).
    2. On failure: Encrypt report and write to `Application.persistentDataPath/error_reports/` (or secure cache).
    3. On next app launch: Retry send buffered reports; on success, delete local file (FR-08).

- **LogRingBuffer** (Pure C#):
  - Fixed-size array (e.g., 100 entries) with index wrapping.
  - On `LogType.Log` or `LogType.Warning`: Append entry (overwrite oldest if full).
  - Non-allocating: Reuse entry objects or use structs.
  - Expose `LogRingBufferEntry[] GetSnapshot()` for StateSnapshot (read-only copy).

- **ErrorPopupPresenter** + **ErrorPopupView**:
  - Presenter subscribes to recovery flow state changes.
  - View: Full-screen modal with dimmer (UR-01), title/message (UR-02), debug info (UR-03), context-aware buttons (UR-04), loading state (UR-05).
  - Use existing **PopupManager.ShowCommonPopup** for show/hide and animations (UR-06).

**Unity lifecycle**  

- **GlobalBootstrapper** (`Assets/_Game/App/Bootstrapper/GlobalBootstrapper.cs`):
  - In `Awake` (early, before other systems):
    1. Create `StabilityFlag` (implements `IStabilityFlag`).
    2. Create `GlobalErrorInterceptor`; register log callback and unhandled exception handler.
    3. Inject `IStabilityFlag` into **AutoSaveManager** (constructor or setter).
    4. Wire interceptor → classifier → snapshot → recovery flow → transmission → UI.

- **AutoSaveManager** (`Assets/_Game/App/Systems/AutoSaveManager.cs`):
  - Before any save (periodic `SaveAllAsync()` or `OnApplicationPause/Quit` → `SaveAllSync()`):
    - **MUST** check `IStabilityFlag.IsSaveAllowed`.
    - If `false`: Skip save entirely; do not set dirty flag; do not call repository save methods.
    - If `true`: Proceed with normal save flow.

- **Business Logic Blocking** (FR-03):
  - On CRITICAL: Stability Flag blocks saves (highest priority).
  - **Data mutation blocking**: Features that mutate Data (combat, inventory, character stats) must check `IStabilityFlag.IsSaveAllowed` before executing mutations (Option 1: explicit checks). If flag is `false`, skip the mutation operation entirely. This ensures no corrupted data is created in memory.
  - Unity lifecycle (Update, FixedUpdate) continues; only business logic mutations are blocked.

- **Error popup**: Shown via PopupManager; same entry/exit animations as other popups (UR-06).

**Source layout**  

```text
Assets/_Game/
├── App/Bootstrapper/GlobalBootstrapper.cs      # Wire interceptor, inject IStabilityFlag into AutoSaveManager
├── App/Systems/
│   ├── AutoSaveManager.cs                     # Check IStabilityFlag before any save
│   ├── PopupManager.cs                        # Existing; error UI uses ShowCommonPopup
│   └── ErrorHandling/
│       ├── StabilityFlag.cs                    # IStabilityFlag implementation
│       ├── ErrorClassifier.cs                  # Hybrid classification (FR-02)
│       ├── ErrorSnapshotCapture.cs            # Snapshot builder (FR-04)
│       ├── ErrorRecoveryFlow.cs               # One-shot, retry count, escalation, command cache (FR-05, FR-06, FR-09)
│       ├── ErrorReportTransmission.cs         # Send + buffer + retry (FR-08)
│       ├── GlobalErrorInterceptor.cs          # Log callback + unhandled exception + Ring Buffer (FR-01)
│       └── LogRingBuffer.cs                   # Non-allocating Ring Buffer for context
├── Core/ErrorHandling/
│   ├── ErrorSeverity.cs                       # enum WARNING, NETWORK, CRITICAL
│   ├── ErrorReport.cs                         # DTO: report payload
│   ├── StateSnapshot.cs                       # DTO: execution context + data summary + Ring Buffer
│   ├── LogRingBufferEntry.cs                  # Ring Buffer entry (struct or class)
│   ├── NetworkRetryCommand.cs                 # Command pattern for retry (Func<UniTask> wrapper)
│   └── IStabilityFlag.cs                      # Contract: bool IsSaveAllowed { get; }
└── Features/ErrorHandling/Presentation/
    ├── ErrorPopupView.cs                      # Full-screen modal (UR-01–UR-05)
    ├── ErrorPopupPresenter.cs                 # Binds recovery flow to view
    └── Prefabs/ErrorPopupView.prefab
```

---

## Traceability: Spec → Plan

| FR | Component / behavior |
|----|-----------------------|
| FR-01 | GlobalErrorInterceptor: LogType.Error/Exception only; Ring Buffer for Log/Warning (context) |
| FR-02 | ErrorClassifier: Hybrid (explicit tag mandatory for dev-controlled; Exception Type for unhandled) |
| FR-03 | Emergency stop: (1) Stability Flag block, (2) Business logic mutation block, (3) Unity lifecycle continues |
| FR-04 | ErrorSnapshotCapture: GameContext.GetDataSummaryJson() + Ring Buffer payload |
| FR-05 | ErrorRecoveryFlow: One-shot guard (_criticalHandled flag) |
| FR-06 | ErrorRecoveryFlow + PopupManager: Command-pattern retry (NetworkRetryCommand); Reconnect+Title vs Title only |
| FR-07 | IStabilityFlag + AutoSaveManager: Check before save; never reset until process exit |
| FR-08 | ErrorReportTransmission: Send → on fail encrypt+buffer → retry on next launch → delete cache |
| FR-09 | ErrorRecoveryFlow: Retry counter; at 4th failure → escalate to CRITICAL |

---

## Complexity Tracking

| Violation | Why Needed | Simpler Alternative Rejected |
|-----------|------------|-----------------------------|
| (none) | — | — |

---

## Next Steps

1. ✅ **Clarifications resolved**: All NEEDS CLARIFICATION items from previous plan are now answered in spec §8.
2. **Review** this architecture before coding.
3. **Implementation decisions**:
   - ✅ **Business logic blocking**: Option 1 (explicit checks) — Features check `IStabilityFlag.IsSaveAllowed` before data mutations.
   - ✅ **Return to Title**: Scene name is `"IntroScene"` — Use `SceneManager.LoadScene("IntroScene")` for title return.
   - **Ring Buffer**: Use fixed-size array with modulo indexing; avoid allocations.
   - **Remote endpoint**: Implement transmission as stub/no-op until endpoint is defined.
4. Run **speckit.tasks** to break into tasks, or proceed to implementation.

**Generated artifact**: This file (`plan.md`) with all clarifications integrated. Architecture is ready for implementation.
