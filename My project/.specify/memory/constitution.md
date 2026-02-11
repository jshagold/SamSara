# Samsara Project Constitution

## 1. Project Overview
- **Goal:** Game Development (Samsara Project)
- **Platform:** Mobile (Android / iOS)
- **Engine:** Unity 6.2
- **Structure Pattern:** Feature-based Modular Architecture (DDD + Clean Architecture applied)
- **Root Folders:**
    - **Game Logic:** `Assets/_Game/` (Main Source Code & Assets)
        - `App/`: Global configs, GameContext, Project Bootstrappers.
        - `Core/`: Shared utilities, Base classes, Domain interfaces (No Feature dependencies).
        - `Features/`: Independent Game Modules (e.g., Inventory, Skills, Battle).
        - `Scenes/`: Scene files (.unity) and Scene-specific Bootstrappers.
        - `Dev/`: **Sandbox Area.** (Rules are relaxed here for prototyping).
    - **System Resources:** `Assets/Resources/` (Only for `LoadAll` targets like MasterData)
    - **External Assets:** `Assets/` or `Assets/_External/` (3rd party assets like Cainos)


## 2. Core Principles (Non-Negotiable)

### Zero Guessing (절대 추측 금지)
- **STRICT PATH ADHERENCE:**
    - Do NOT assume `Assets/Scripts`.
    - All script logic MUST be inside `Assets/_Game/`.
- **NO HALLUCINATION:** Never invent file paths, class names, or API versions.
- **Verify Existence:** Before modifying a file, ensure it exists in the file tree.
- **ASK FIRST:** If you are unsure about the existing project structure or where a file is located, you MUST use `/speckit.clarify` to ask the user.
- **NO ASSUMPTIONS:** Do not write code based on 'likely' scenarios. Verify facts first.

### Data / Logic Separation
- **Prohibition:**
    - Never hardcode game data (stats, IDs, text) inside Logic classes.
    - Logic classes must reference Data classes to read values.

### Dependency Injection Strategy (DI-Ready)
To facilitate future migration to DI libraries (VContainer/Zenject):

#### Pure DI (Constructor Injection)
- **Rule:** Logic classes (e.g., `Presenter`, `UseCase`, `Repository`, `Provider`) MUST define dependencies explicitly in their **Constructor**.
- **Prohibition:**
    - Logic classes must NOT instantiate dependencies using `new`.
    - Logic classes must NOT access `Singleton.Instance` internally.
    - Logic classes must NOT use `GameObject.Find`, `GetComponent` (except View).

#### The Bootstrapper Hierarchy (Composition Root)
- **Role:** `*Bootstrapper.cs` is the **ONLY** place allowed to instantiate Logic classes (`new`) and perform injection.
- **Hierarchy & Entry Point:**
    1.  **GlobalBootstrapper:** Initializes core systems (Singleton).
    2.  **SceneBootstrapper:** The **Orchestrator**. The **ONLY** MonoBehaviour allowed to use `Start()` in a scene to initialize children.
    3.  **FeatureBootstrapper:** **Passive**. Must **NOT** use `Start()` or `Awake()` for logic initialization. It waits for `.Initialize()` call from the `SceneBootstrapper`.
- **Scope:** Do NOT pass `GameContext` into Presenters. Inject only specific `UseCases` or `Repositories`.
- **Lifecycle:** Parent Bootstrapper is responsible for calling `.Initialize()` on setup and `.Dispose()` on destroy for all children.


## 3. Architecture & Layer Rules
All features inside `Assets/_Game/Features/[FeatureName]` MUST follow this structure:

### Data Layer (`.../Data`)
- **Role:** Repositories, Mappers, Data Definitions.
- **Rules:**
    - **MasterData(SO):** Class definitions go here. Actual `.asset` files go to `Assets/Resources/MasterData`.
    - **Persistence Standards:**
        1.  **Cached Repository:** Load data into memory (`_cachedData`) once. Modify cache, then save.
        2.  **Dual-Mode Saving:**
            - `UniTask SaveDataAsync()`: For periodic auto-save (ThreadPool I/O).
            - `void SaveDataSync()`: For `OnApplicationPause`/`Quit` (Main Thread Blocking).
        3.  **Dirty Flag:** Only save when `_isDirty` is true via `AutoSaveManager`.
        4.  **Fail Fast:** Throw `InvalidOperationException` immediately if data integrity check fails.
        5.  **Asset Files:** Actual `.asset` files go to `Assets/Resources/MasterData/[FeatureName]` (for `Resources.LoadAll`).
    - **Resource Management Warning:**
        - **Strict Limit:** The `Resources/` folder is **ONLY** for lightweight `ScriptableObject` data (MasterData) or tiny Prefabs.
        - **Prohibition:** NEVER place heavy assets like **Textures, AudioClips, or large Prefabs** in `Resources/`.
        - **Reason:** It increases app startup time and memory footprint significantly. Use Direct References (`[SerializeField]`) or Addressables instead.

### Domain Layer (`.../Domain`)
- **Role:** Pure C# Business Logic, UseCases, Models.
- **Rules:**
    - **NO UNITY DEPENDENCIES:** Avoid `UnityEngine.UI` or `GameObject`.
    - **UseCase Patterns:**
        1.  **Event Proxy:**
            - Expose Repository events via `add/remove` accessors.
            - Do not define new events if they just mirror Repository events.
            - *Example:* `public event Action OnUpdate { add => _repo.OnUpdate += value; remove => _repo.OnUpdate -= value; }`
        2.  **Data Aggregation (DTO):**
            - UseCases must gather data from multiple Repositories (`ItemRepo`, `UserRepo`) and return a combined `*Info` or `*DTO` object (e.g., `CharacterSummaryInfo`).
            - Do NOT return raw `SaveData` or `Entity` classes to the Presenter.
        3.  **Strict Validation:**
            - If required data is null, throw `InvalidOperationException` with `_logClass` prefix immediately. Do not return null silently.

### Presentation Layer (`.../Presentation`)
- **Role:** Handle UI Logic, View components, and Feature-Specific Assets.
- **Rule:**
    - `MonoBehaviour` views go here.
    - `Presenters` connect Domain UseCase to Views.
- **Structure (Cohesion):**
    - **C# Scripts (Root):**
        - Place all logic scripts (`Bootstrapper.cs`, `Presenter.cs`, `View.cs`) directly in `Presentation/`.
        - **Reason:** Easy access to core logic.
    - **Assets (Subfolders):**
        - `Prefabs/`: Feature-related prefabs (e.g. `CommonPopupView.prefab`)
        - `Art/`: Feature-specific Sprites, Animations, Materials.
        - `Sounds/`: Feature-specific AudioClips.
- **Presenter Patterns:**
    1.  **Lifecycle Management:**
        - MUST implement `IDisposable`.
        - `Initialize()`: Render initial data (`Refresh()`) and subscribe to UseCase events.
        - `Dispose()`: Unsubscribe from all events to prevent memory leaks.
    2.  **View Interaction:**
        - Presenters push data to the View (`_view.UpdateList(data)`).
        - Presenters never manipulate UI components (`Text`, `Image`) directly; they call methods on the View.
- **Naming:**
    - `*Bootstrapper`: Entry point for the feature.
    - `*Presenter`: Connects Domain to View.
    - `*View`: Inherits `MonoBehaviour`, handles UI elements.


## 4. Data Persistence Standards (Repository Implementation)

### Cached Repository Pattern
- **Rule:** Repositories MUST load data into memory (`_cachedData`) exactly once at startup.
- **Operation:**
    - All `Get`, `Add`, `Remove` operations MUST modify the **In-Memory Cache**, not the file directly.
    - `SaveDataAsync()` simply serializes the current cache to disk.
- **Fail Fast:**
    - Every public method MUST call a `CheckDataIntegrity()` method first.
    - If `_cachedData` is null, throw `InvalidOperationException` immediately. Do not return default values silently.

### I/O Thread Offloading
- **Rule:** File I/O (Read/Write) MUST be offloaded to the ThreadPool to prevent main thread freezing (Frame drop).
- **Implementation:**
    - Use `await UniTask.RunOnThreadPool(() => File.WriteAllText(...))` for async saving.
    - Use `Newtonsoft.Json` (Json.NET) for serialization.

### Dual-Mode Saving (Async & Sync)
- **Requirement:** Repositories MUST implement both:
    1.  `UniTask SaveDataAsync()`: For periodic auto-saves (Background thread).
    2.  `void SaveDataSync()`: For emergency saves (Main thread blocking).
- **Usage:**
    - **Async:** Normal gameplay loop (`AutoSaveManager`).
    - **Sync:** `OnApplicationPause` (Mobile) or `OnApplicationQuit` (PC). **NEVER** use async here, as the OS may kill the app process immediately.

### Dirty Flag Pattern (Auto-Save Strategy)
- **Rule:** Do NOT save to disk on every data change.
- **Mechanism:**
    - Repositories expose an event: `event Action OnDataChanged;`
    - `AutoSaveManager` listens to this event and sets `_isDirty = true;`.
    - Saves occur only when `_isDirty` is true during the periodic loop.

### Async Safety (Cancellation)
- **Rule:** Long-running async loops (like AutoSave) MUST support cancellation.
- **Implementation:**
    - Use `this.GetCancellationTokenOnDestroy()` in `MonoBehaviour`.
    - Pass the token to `UniTask.Delay`.


## 5. UI & Async Interaction Standards

### Async UI Flow (Popup & Transition)
- **Pattern:** Use `UniTask<T>` to wait for user interaction instead of callbacks.
- **Rule:**
    - Popups must return a `UniTask<bool>` (or Enum) indicating the user's choice.
    - Presenters must `await` the popup result before proceeding.
- **Example:** `bool isRetry = await PopupManager.Show(...);`

### UI Object Pooling
- **Library:** Use `UnityEngine.Pool.IObjectPool<T>`.
- **Lifecycle:**
    - **Get:** `SetActive(true)` -> `transform.SetAsLastSibling()`.
    - **Release:** `SetActive(false)`. Do NOT Destroy unless pool is full.

### Error Handling & Retry Logic
- **Critical Loading:** Wrap in `while(true)` loop with `try-catch`.
- **User Choice:** If error occurs, show Popup (Retry/Quit).
- **Initialization:** Wait for `GlobalBootstrapper.InitializationTask` using `UniTask.WhenAll`.


## 6. Global Architecture & State Management

### Async & Threading (UniTask)
- **Library:** Must use `Cysharp.Threading.Tasks` (UniTask) instead of standard C# `Task` or Coroutines for async logic.
- **Fire-and-Forget:**
    - When calling an async method from a synchronous context (e.g., `Start`, `Button.onClick`), use `.Forget()` or `.Preserve()` (if storing the task).
- **Parallel Loading:**
    - Use `UniTask.WhenAll` for loading independent data chunks (e.g., `Inventory`, `Character`, `DailyState`) to minimize loading time.
    - Example: `await UniTask.WhenAll(taskA, taskB, taskC);`
- **Safety:** Long-running loops must support Cancellation Tokens.

### Global State

#### Singleton Rule (Restricted):
- **Rule:** `GlobalBootstrapper` is the **ONLY** allowed Singleton (`DontDestroyOnLoad`).
- **Prohibition:**
    - Do NOT create Singletons for Managers (e.g., `InventoryManager.Instance` is FORBIDDEN).
    - Access all Managers/Repositories via `GameContext` or Dependency Injection.

#### GameContext (The Container)
- **Role:** Acts as the **Service Container** and **Lifetime Scope** for the game session. Performs Manual DI wiring.
- **Responsibility:**
    - Holds instances of all Repositories and UseCases.
    - Performs the "Wiring" (Manual DI) of UseCases in its constructor.
- **Usage:**
    - Passed down from `GlobalBootstrapper` to Scene Bootstrappers.
    - **Note:** Do not pass the entire `GameContext` to small logic classes. Extract and pass only needed UseCases/Repositories.

#### Initialization Sequence
- **MasterData:** Must be initialized **Synchronously** (or blocked await) before GameContext setup to ensure static data integrity.
- **RuntimeData:** Must be initialized **Asynchronously** via `GameContext.LoadAllDataAsync()`.


## 7. Unity UI Coding Standards (Strict)

### Fail Fast (Defensive Coding Forbidden)
- **Rule:** DO NOT use `if(component != null)` for `[SerializeField]` UI elements.
- **Reason:** Missing references must cause a `NullReferenceException` immediately during runtime to be fixed. Hiding errors is a bug.
- **Target:** All View scripts referencing Text, Image, Button, etc.

### Automate with Reset()
- **Rule:** Implement `Reset()` in View scripts to auto-assign clear UI components (Image, TMP_Text, Button).
- **Action:** Automatically find and assign child components using `GetComponentInChildren<T>()` or `transform.Find()` within `Reset()`.
- **Goal:** Minimize manual drag-and-drop-errors in the Inspector.
- **EXCEPTION:**
    - **DO NOT** auto-assign `Transform` or `RectTransform` in `Reset()`.
    - **Reason:** Transforms are generic and ambiguous; auto-assignment often picks the wrong object. These must be assigned manually in the Inspector.

### Component Caching
- **Rule:** Cache the specific UI component (e.g., `Image`) instead of `RectTransform`.
- **Reason:** You can access `image.rectTransform` freely, but you cannot access `image` from `rectTransform` without `GetComponent` (overhead)
- **Example:** `[SerializeField] private Image _hpBar;` (Good) vs `private RectTransform _hpBarRect` (Bad).

### Resolution Independence
- **Rule:** Trust the `Canvas Scaler`
- **Action:** Use fixed `float` values for padding/size in code. Do not manually calculate screen ratios or pixel density.


## 8. General Coding Standards
- **Log Tagging:**
    - Every class should define a private log tag: `private readonly string _logClass = $"[{nameof(ClassName)}]";`
    - Use this tag in all `Debug.Log` calls for filtering.
- **Critical Failures:**
    - Data loading failures (MasterData/RuntimeData) must treat as **Critical Exceptions**.
    - Use `try-catch` blocks in Initialization phases to log specific errors (`Debug.LogError`) before throwing.
- **Safe Cleanup (OnDestory/Dispose):**
    - **Rule:** Do NOT throw exceptions (e.g., `InvalidOperationException`) inside `OnDestroy()` or `Dispose()` if a dependency is null.
    - **Action:** Use the Null-conditional operator (`?.`) for cleanup calls. (e.g., `_presenter?.Dispose();`).
- **Naming Conventions:**
    - Public/Methods: `PascalCase`
    - Private fields: `_camelCase` (starts with underscore)
    - Suffixes: `*View`, `*Presenter`, `*UseCase`, `*Repository`, `*SO`
- **Serialization:** Use `[SerializeField] private` for inspector variables. Do not use `public` fields for internal state.
- **Performance:** Avoid `GetComponent`, `FindObjectOfType` in `Update()` loops. Cache references in `Awake()` or `Start()`.

### Performance & Optimization (Mobile Critical)
- **Hot Path Rule (`Update`, `FixedUpdate`):**
    - **NO Memory Allocation:** Do NOT use `new`, LINQ, or String Concatenation (`+`) inside `Update()` loops.
    - **Reason:** To prevent Garbage Collection (GC) spikes which cause frame drops (Lag).
    - **Alternative:** Use `StringBuilder` for text, pre-allocated Arrays/Lists.
- **String Handling:**
    - Use `StringBuilder` for complex string manipulations.
    - Use `ZString` (if available) or `StringBuilder` instead of default string `+` operator in frequent calls.


## 9. Workflow & Governance
- **Asset Placement:**
    - If an asset (Sprite/Prefab) is used ONLY by this feature, place it in `Presentation/Art` or `Presentation/Prefabs`.
    - If shared across features, move to `Assets/_Game/Core/...`.
- **Plan Verification:** Check if the plan respects the `Assets/_Game/...` path and Asset Cohesion rules.

## 10. External Library Standards
- **Wrapper Pattern:**
    - **Rule:** Do NOT depend directly on 3rd party logic libraries (e.g., specific Ad SDK, Analytics).
    - **Action:** Create an `Interface` (e.g., `IAdService`) in `Core/` and implement the wrapper in `App/`.
- **Allowed Direct Use:**
    - widely standard libraries like `UniTask`, `DOTween`, `TMP` are allowed for direct usage to avoid over-engineering.

**Version:** 1.0.0 | **Ratified:** 2026-02-10 | **Last Amended:** 2026-02-10