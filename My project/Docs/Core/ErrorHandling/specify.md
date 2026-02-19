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
* **FR-02: Error Classification**: Categorize errors into `WARNING`, `NETWORK`, and `CRITICAL`.
* **FR-03: Instant Flow Termination**: Upon a `CRITICAL` error, trigger an immediate "Emergency Stop" that halts game logic to prevent further undefined behavior.
* **FR-04: Whole State Snapshot**: Capture a JSON-serialized snapshot of all active **Data** classes + Execution Context (Scene name, frame count, device status).
* **FR-05: One-Shot Execution**: Ensure the handling logic for `CRITICAL` errors fires only once per session to prevent recursive failure or log spamming.
* **FR-06: Recovery UI Controller**: 
    - For `NETWORK` errors: Display a modal with a "Retry" option. 
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