# Specify: Player Progress Persistence System (SaveLoadManager)

## 1. Goal
* **What**: An automatic data persistence system that saves and restores all player progress across game sessions without requiring manual player input.
* **Why**: To ensure players never lose their progress due to unexpected interruptions (force quit, battery loss, OS kill, phone calls) while keeping gameplay smooth and uninterrupted by save operations.

## 2. User Journeys & Experience
* **Player Experience**:
    * **Invisible Saving**: The game saves progress automatically in the background. The player never sees a "Saving…" indicator or experiences a gameplay freeze.
    * **Guaranteed Restoration**: Every time the player reopens the app, their exact progress — character state, inventory, and daily actions — is restored to precisely where they left off.
    * **Emergency Preservation**: If the player receives a call, switches apps, or the OS suspends the game, all unsaved progress is immediately written to storage before control is handed back to the OS.
    * **Corruption Prevention**: If a system error is detected, the save system immediately locks down to prevent writing a corrupted state over the last known-good save data.
* **Developer Experience**:
    * **Predictable Lifecycle**: Data loading happens once at startup; saving is driven by a clear, inspectable dirty flag. No hidden or unexpected I/O.
    * **Isolation of Save Concerns**: Each data domain (character, inventory, daily state) owns its own persistence. A failure in one area does not silently affect another.

## 3. Functional Requirements

* **FR-01: Startup Data Restoration**
    * When the game session starts, the system must restore all player data (character, inventory, daily state) from storage before any gameplay logic is executed.
    * Loading must complete in parallel across all data domains to minimize startup time.
    * If any data file does not exist (first launch), the system must report "no save data" so the calling system can trigger new-game initialization.
    * A load failure on any domain must be treated as a **Critical Exception** — the game must not proceed to gameplay with partially-loaded state.

* **FR-02: Automatic Periodic Saving**
    * The system must automatically persist player progress at a fixed interval (every 3 minutes) during active gameplay.
    * A save cycle must only perform actual storage writes when data has changed since the last write (see FR-03). If no data has changed, the cycle is silently skipped.
    * The periodic save loop must be cancellable when the game session ends (scene unload, application quit) to prevent orphaned background operations.

* **FR-03: Dirty Flag — Change Tracking**
    * Each data domain must signal the save orchestrator when its in-memory state changes.
    * The save orchestrator tracks a single "data has changed" flag (`isDirty`). This flag is set to `true` on any change notification and reset to `false` after a successful save.
    * This mechanism ensures the storage layer is written to only when necessary, eliminating redundant I/O.

* **FR-04: Emergency Save on App Lifecycle Events**
    * When the application is suspended (player switches to another app, mobile OS sends the game to the background) or exits (player closes the game, OS kills the process), the system must perform an **immediate, synchronous** save of all data on the main thread.
    * This save must not use background threads or asynchronous operations, as the OS may terminate the process before async work completes.
    * The emergency save must execute regardless of the dirty flag state to guarantee no progress is lost.

* **FR-05: Non-Blocking Async Saves**
    * All periodic saves (FR-02) must be executed off the main thread to prevent gameplay frame drops or stuttering.
    * Async saves for all data domains must run in parallel, not sequentially.

* **FR-06: Data Integrity — Fail Fast**
    * Every save and load operation must verify that the in-memory data cache exists before proceeding.
    * If the cache is absent (null or uninitialized), the operation must immediately throw a descriptive exception rather than silently returning a default value or partial result.
    * This ensures data corruption is caught at the source rather than propagated as a silent failure.

* **FR-07: Stability-Aware Save Gate**
    * The save system must honor a "save-blocked" signal from the Error Handling system (see ErrorHandling spec).
    * When this signal is active, all save operations — both periodic and emergency — must be suppressed.
    * This gate prevents a corrupted in-memory state from being written over the last intact save file during an error recovery scenario.
    * The gate, once activated, is irreversible for the duration of the current application process.

## 4. Data Scope

The following player data categories must be persisted:

| Data Category | Contents |
|---------------|----------|
| **Character Progress** | Active character identity, current story node position, character stats |
| **Inventory State** | Full list of all held items with their quantities |
| **Daily Action State** | Current in-game day number, per-character action slot availability |

## 5. User Scenarios

* **Scenario A — Normal Gameplay Session**:
    A player opens the game → character data, inventory, and daily state are restored instantly → the player plays for 20 minutes → the auto-save timer fires 6 times in the background → the player exits through the title menu → the final state is saved synchronously on exit → all progress is intact on the next launch.

* **Scenario B — Interrupted Session (Mobile)**:
    A player is mid-combat and receives a phone call → the OS suspends the app → the save system immediately performs an emergency synchronous save before yielding control → the player returns 10 minutes later → the game restores state from the emergency save → no progress is lost.

* **Scenario C — Critical System Error**:
    A runtime error occurs during gameplay → the Error Handling system activates the save-blocked signal → the in-game auto-save timer fires but the system detects the gate is active → the write is suppressed → when the player returns to the title screen and relaunches, the last clean save is intact.

* **Scenario D — First Launch (No Save Data)**:
    A new player opens the game → the system attempts to load all data domains → all three domains report "no save data found" → the system signals "first launch" to the initialization flow → the game creates fresh starting data and saves it immediately.

## 6. Success Criteria

* **Progress Recoverability**: In any normal exit scenario (menu quit, home button, multitasking), 100% of game state changes made more than 3 seconds before exit are recoverable on the next launch.
* **Invisible Performance**: Players report zero instances of gameplay freeze or stutter attributable to save operations.
* **Corruption-Free Emergency Saves**: Emergency saves complete successfully on all supported devices before the OS terminates the process (verified by restoring state after a forced interrupt test).
* **First-Launch Reliability**: New-game initialization succeeds on 100% of first launches, with no partial or missing data state.
* **Save Gate Compliance**: Zero instances of save data being written after a save-blocked signal has been issued, confirmed across all error-recovery test scenarios.

## 7. Constraints

* **Strict Data/Logic Separation**: The orchestration logic (when to save, dirty tracking, lifecycle events) must be strictly separated from the data definitions (what is saved) and the I/O implementation (how data is serialized).
* **Single In-Memory Cache**: Each data domain loads its data exactly once into memory at startup. All in-session mutations are applied to this in-memory cache; the cache is serialized to disk only during save operations.
* **Synchronous Emergency Only**: Async (non-blocking) saving is reserved exclusively for periodic saves. Emergency saves triggered by lifecycle events (app pause, quit) must always be synchronous on the main thread.
* **No Partial Writes**: If a save operation fails mid-write, the previous save file must remain fully intact. The system must not leave the storage in a partially-overwritten state.
* **Inter-system Contract**: This system does not implement error detection. It only participates in the error recovery contract by reading the stability signal provided by the Error Handling system (FR-07).
* **Single Save Slot**: The system persists a single save state per player. Multiple save slots or save history/versioning are explicitly out of scope.

## 8. Assumptions

* The 3-minute auto-save interval is a fixed game design decision, not a player-configurable setting.
* The OS provides at least 1–2 seconds of execution time after `OnApplicationPause` before killing the process on all target mobile platforms.
* Save files are stored in the platform-standard application data directory. No custom path routing or cloud sync is in scope for this system.
* Manual save (player-initiated "Save Now" action) is out of scope. All save triggers are system-driven.
