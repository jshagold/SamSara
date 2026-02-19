# Specify: Mobile Error Handling System

## 1. Goal
* **What**: Define a centralized management system for all runtime errors during gameplay.
* **Why**: To prevent game data corruption, avoid sudden application crashes, and enable developers to identify and fix root causes with precision.

## 2. User Journeys & Experience
* **Player Experience**:
    * **No App Crash**: Handle exceptions gracefully to prevent the app from abruptly closing to the home screen.
    * **Data Safety Guarantee**: If an error occurs, the system must immediately block any save operations to prevent corrupted states from being written to disk.
    * **No Dead-ends**: Ensure the player is never stuck. Provide a recovery path by reverting to the last "Known Good" save point or re-initializing the current game flow.
* **Developer Experience**:
    * **Remote Diagnostics**: Capture precise mobile metadata (Device model, OS version, Memory status, Stack trace) for accurate remote debugging.
    * **Zero-Guessing Analysis**: Logs must strictly consist of raw, verified data to identify the root cause without speculative comments.

## 3. Functional Requirements
* **FR-01: Global Exception Interceptor**: Capture all unhandled exceptions and engine-level logs (e.g., Unity's log callback).
    - **Primary Trigger**: The system MUST ONLY trigger error handling flows (classification, snapshot, recovery UI) on `LogType.Error` and `LogType.Exception` to avoid performance degradation. `LogType.Warning` and `LogType.Log` must NOT trigger state changes or recovery flows.
    - **Context Ring Buffer**: For debugging context, maintain a lightweight, non-allocating Ring Buffer of the last N standard logs (`LogType.Log`, `LogType.Warning`). When a CRITICAL error triggers a StateSnapshot (FR-04), attach this Ring Buffer payload to the snapshot for debugging purposes. The Ring Buffer is read-only context and never triggers state changes.
* **FR-02: Error Classification**: Categorize errors into `WARNING`, `NETWORK`, and `CRITICAL`.
    - **Hybrid Classification Strategy**:
        - **Developer-Controlled Errors**: For errors that developers explicitly handle or throw (e.g., custom exceptions, `Debug.LogError` with severity), **explicit tagging is mandatory**. Developers must specify the severity when reporting (e.g., `throw new NetworkException(...)` with `ErrorSeverity.NETWORK`, or `ErrorReporter.Report(exception, ErrorSeverity.CRITICAL)`).
        - **Unhandled System Exceptions**: For unhandled exceptions from Unity/C# system (e.g., `NullReferenceException`, `IndexOutOfRangeException`, `NetworkException`), classify by **Exception Type**:
            - `NetworkException`, `TimeoutException`, `SocketException` → `NETWORK`
            - `NullReferenceException`, `IndexOutOfRangeException`, `InvalidOperationException` → `CRITICAL`
            - `ArgumentException`, `ArgumentNullException` → `WARNING` (unless context indicates CRITICAL)
            - Default fallback for unknown types → `CRITICAL` (fail-safe)
* **FR-03: Instant Flow Termination**: Upon a `CRITICAL` error, trigger an immediate "Emergency Stop" that halts game logic to prevent further undefined behavior.
    - **Emergency Stop Scope (Priority Order)**:
        1. **Save-Blocker (Hard Lock)**: Immediately apply Stability Flag to completely prevent writing corrupted data to disk. This is the highest priority (see FR-07).
        2. **Business Logic Block**: Block execution of any business logic that mutates Data (e.g., combat calculations, inventory modifications, character stat changes). This prevents further data corruption in memory.
        3. **Unity Core Lifecycle Continues**: Keep Unity's core lifecycle (Update, FixedUpdate, LateUpdate) running so that error handling UI, log transmission systems, and recovery flow can function normally. Only business logic that mutates game state is blocked, not the engine loop itself.
* **FR-04: Whole State Snapshot**: Capture a JSON-serialized snapshot of all active **Data** classes + Execution Context (Scene name, frame count, device status).
    - **Data Source**: `GameContext` exposes a method that returns serializable summaries of all Repositories it holds (e.g., `GetDataSummaryJson()`). The error system calls this method to obtain the current state snapshot without coupling to individual feature repositories.
* **FR-05: One-Shot Execution**: Ensure the handling logic for `CRITICAL` errors fires only once per session to prevent recursive failure or log spamming.
* **FR-06: Recovery UI Controller**: 
    - For `NETWORK` errors: Display a modal with a "Retry" option. 
    - **Retry Mechanism**: When a network error occurs, the system must capture the specific failed network request as an executable 'Command' or cached 'Action' (e.g., `Func<UniTask>` or a command object). Clicking "Retry" must only re-invoke this specific cached operation, ensuring an exact 1:1 retry without causing side effects or affecting other pending network operations. The retry must be isolated to the exact operation that failed.
    - **Escalation Policy**: If "Retry" is clicked more than 3 times and fails, the UI must switch to a "Network Instability" modal that only allows "Return to Title."
* **FR-07: Save-Blocker (Hard Lock)**: Disable all persistence triggers (including mobile lifecycle events like `OnApplicationPause/Quit`) immediately after a `CRITICAL` error. This lock must remain active and irreversible until the application process is completely restarted.
* **FR-08: Secure Log Transmission & Buffering**: 
    - **Primary**: Immediately transmit the generated error report to the designated remote server or analytics tool upon occurrence.
    - **Fallback (Offline Buffering)**: If transmission fails due to network instability, encrypt and temporarily store the report in the device's secure cache area.
    - **Auto-Retry**: Upon subsequent normal application execution and restoration of network connectivity, prioritize the transmission of buffered reports and immediately delete the local cache data after a successful transfer.
* **FR-09: Network Escalation Logic**: Maintain a counter for consecutive network retries. Upon the 4th failure, programmatically escalate the error status from `NETWORK` to `CRITICAL` to trigger the Save-Blocker and force a title-return flow.

## 4. UI/UX Requirements (Functional)
* **UR-01: Full-Screen Modal Interface**: 
    - The UI must consist of a central popup and a darkened, semi-transparent background (Dimmer).
    - It must effectively block and intercept all raycasts and inputs to underlying gameplay elements.
* **UR-02: Header & Message Section**:
    - **Title**: A localized string representing the error category (e.g., "Network Error", "System Failure").
    - **Notice Message**: A user-friendly explanation of the situation to guide the player.
* **UR-03: Technical Metadata (Debug Info)**:
    - Display the **Error Code** and **Snapshot ID** in a small, non-obtrusive font for developer reference.
* **UR-04: Context-Aware Action Buttons**:
    - **Network Error (Initial)**: Provide two options: `[Reconnect]` (Primary) and `[To Title]` (Secondary) (with 3-retries limit).
    - **Network Error (Escalated)**: After 3 failed retries, the `[Reconnect]` button must be removed or disabled, leaving only `[To Title]`.
    - **Critical Error**: Immediately display only the `[To Title]` button to enforce a clean restart flow.
* **UR-05: Interaction Feedback**:
    - While a reconnection or snapshot process is active, buttons should be disabled or replaced by a loading state to prevent redundant inputs.
* **UR-06: Visual & Behavioral Consistency**:
    - **Uniform Animations**: The error popup must use the **same entry/exit animations** (e.g., Scale up, Fade in) as other standard popups in the project.
    - **System Integration**: It must be managed via the existing UI/Popup system to ensure consistent layering, sound effects (SFX), and transition timing.


## 5. User Scenarios
* **Scenario A (Fatal Data Error during Combat)**: 
    * A logic error occurs while calculating damage -> `FR-01` intercepts it -> `FR-03` blocks the next auto-save -> `FR-06` shows a restart popup -> Player returns to title -> Re-loading the game restores the state to the start of the battle (Last Good Save).
* **Scenario B (Network Timeout during Transaction)**:
    * Player attempts to claim a reward -> Network times out -> `FR-02` identifies it as `NETWORK` -> `FR-06` shows a "Retry" popup -> Player clicks Retry -> Re-connection is attempted without reloading the entire game.

## 6. Success Criteria
* **Comprehensive Capture**: Intercept all uncaught exceptions, including mobile-specific errors (e.g., Network timeout, Low memory warnings).
* **Persistence Circuit Breaker**: Upon detection of a 'Fatal' or 'Data-related' error, the auto-save system must be **immediately suspended** until the state is verified as clean.
* **Atomic Recovery**: Successful recovery must result in the player being placed in a "Verified Playable" state, even if it requires discarding unsaved volatile data.
* **Non-Speculative Logging**: Every error entry must provide a clear "State Snapshot" (Memory-only) without any inferred or assumed information.

## 7. Constraints
* **Strict Data/Logic Separation**: Maintain a clear distinction between **ErrorData** (the state), **ErrorUI** (the popup), and **ErrorLogic** (the recovery flow).
* **Zero-Guessing Rule**: The system shall not append speculative causes. Log only what the engine and logic explicitly report.
* **Performance Budget**: Error interception and snapshotting must not cause frame drops (Jank) on mobile devices.
* **All-or-Nothing Persistence**: If an error occurs during a save transaction, the previous save file must remain completely intact (No partial writes).
* **Inter-system Contract**: This system DOES NOT implement saving logic. It only provides the "Stability Flag" that future saving functions MUST respect.

## 8. Clarifications

* **Q: How should the system obtain the list of "all active Data classes" to serialize in the StateSnapshot (FR-04)?**  
  **A: GameContext Export** — `GameContext` exposes a method (e.g., `GetDataSummaryJson()`) that returns serializable summaries of all Repositories it holds. This leverages the existing structure and avoids inventing new registries.

* **Q: What are the specific rules/criteria for classifying an error as WARNING vs NETWORK vs CRITICAL (FR-02)?**  
  **A: Hybrid of A and C** — For developer-controlled errors, explicit tagging is mandatory (e.g., `throw new NetworkException(...)` with severity, or `ErrorReporter.Report(exception, ErrorSeverity.CRITICAL)`). For unhandled exceptions from Unity/C# system, classify by Exception Type: `NetworkException`/`TimeoutException` → NETWORK; `NullReferenceException`/`IndexOutOfRangeException` → CRITICAL; `ArgumentException` → WARNING (unless context indicates CRITICAL); unknown types default to CRITICAL.

* **Q: What exactly should "Emergency Stop" halt when a CRITICAL error occurs (FR-03)?**  
  **A: Option B-based with Save-Blocker Priority** — (1) Apply Hard-Lock to Save System immediately to completely prevent writing corrupted data to disk (highest priority). (2) Block execution of any business logic that mutates Data (e.g., combat, inventory changes). (3) Keep Unity's core lifecycle (Update, FixedUpdate) running so error handling UI and log transmission systems can function normally. Only data-mutating business logic is blocked, not the engine loop itself.

* **Q: What should "Retry" retry when a NETWORK error occurs (FR-06)?**  
  **A: Option A — Failed Network Request Only** — The system must capture the specific failed network request as an executable 'Command' or cached 'Action' (e.g., `Func<UniTask>` or a command object). Clicking "Retry" must only re-invoke this specific cached operation, ensuring an exact 1:1 retry without causing side effects or affecting other pending network operations. The retry is isolated to the exact operation that failed.

* **Q: Which Unity log types should the error system capture from `Application.logMessageReceived` (FR-01)?**  
  **A: Option A-based with Context Buffer** — Primary trigger: ONLY `LogType.Error` and `LogType.Exception` trigger error handling flows (classification, snapshot, recovery) to avoid performance degradation. For debugging context, maintain a lightweight, non-allocating Ring Buffer of the last N standard logs (`LogType.Log`, `LogType.Warning`). When a CRITICAL error triggers a StateSnapshot, attach this Ring Buffer payload for debugging, but never trigger state changes from warnings. `LogType.Warning` and `LogType.Log` are context-only and do not trigger recovery flows.