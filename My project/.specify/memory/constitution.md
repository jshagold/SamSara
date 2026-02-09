# Samsara Project Constitution



## Project Overview
- Goal: Game Development (Samsara Project)
- Engine: Unity 6.2
- **Structure Pattern:** Feature-based Modular Architecture (DDD + Clean Architecture applied)
- **Root Folders:**
    - **Game Logic:** 'Assets/_Game/' (Main Source Code & Assets)
    - **System Resources:** 'Assets/Resources/' (Only for 'LoadAll' targets like MasterData)
    - **External Assets:** 'Assets/' or 'Assets/_External' (3rd party assets like Cainos)



## Core Principles (Non-Negotiable)

### Zero Guessing (절대 추측 금지)
- **STRICT PATH ADHERENCE:**
    - Do NOT assume 'Assets/Scripts'
    - All script logic MUST be inside 'Assets/_Game/'
- **NO HALLUCINATION:** Never invent file paths, class names, or API versions.
- **Verify Existence:** Before modifying a file, ensure it exists in the file tree.
- **ASK FIRST:** If you are unsure about the existing project structure or where a file is located, you MUST use '/speckit.clarify' to ask the user.
- **NO ASSUMPTIONS:** Do not write code based on 'likely' scenarios. Verify facts first.

### Feature Architecture & Layer Separation & Asset Cohesion
- 'Assets/_Game/App': Global configs, Context, Bootstrappers
- 'Assets/_Game/Core': Shared utilities, Base classes, Domain interfaces
- 'Assets/_Game/Features': Main game logic separated by feature (e.g., Inventory, Skills)
- 'Assets/_Game/Scenes': Scene files and Scene Bootstrappers
All features inside 'Assets/_Game/Features/[FeatureName]' follow this structure:

#### Data Layer ('.../Data')
- **Role:** Data Definitions, Repositories, Mappers, API Calls.
- **Rule:**
    - Repository implementations go here.
    - **MasterData(SO):**
        - Definitions (MasterData class) go here.
    - **Asset Files:** Actual '.asset' files go to `Assets/Resources/MasterData/[FeatureName]` (for `Resources.LoadAll`).

#### Domain Layer ('.../Domain')
- **Role:** Business Logic, UseCases, Models (Pure C#).
- **Rule:**
    - **NO UNITY DEPENDENCIES:** Avoid referencing 'UnityEngine.UI' or 'GameObject'.
    - Contains: 'UseCases', 'Models', 'Interfaces'
    - Pure C# Logic.

#### Presentation Layer ('.../Presentation')
- **Role:** Handle UI logic, View components, and **Feature-Specific Assets**.
- **Rule:**
    - 'MonoBehaviour' views go here.
    - 'Presenters' connect Domain UseCase to Views.
- **Folder Structure (Cohesion):**
    - **C# Scripts (Root):**
        - Place all logic scripts ('*Boostrapper.cs', '*Presenter.cs', '*View.cs')
        - **Reason:** Easy access to core logic.
    - **Assets (Subfolders):**
        - 'Prefabs/': Feature-related prefabs (e.g. 'CommonPopupView.prefab')
        - 'Art/': Feature-specific Sprites,Animations, Materials.
        - 'Sounds/': Feature-specific AudioClips.
- **Naming:**
    - '*Bootstrapper': Entry point for the feature.
    - '*Presenter': Connects Domain to View.
    - '*View': Inherits 'MonoBehavior', handles UI elements.

### Data / Logic Separation
- **Prohibition:**
    - Never hardcode game data (stats, IDs, text) inside Logic classes.
    - Logic classes must reference Data classes to read values.



## Unity UI Coding Standards (Strict)

### Fail Fast (Defensive Coding Forbidden)
- **Rule:** DO NOT use 'if(component != null)' for '[SerializeField]' UI elements.
- **Reason:** Missing references must cause a 'NullReferenceException' immediately during runtime to be fixed. Hiding errors is a bug.
- **Target:** All View scripts referencing Text, Image, Button, etc.

### Automate with Reset()
- **Rule:** Implement the 'Reset()' method in View scripts to auto-assign clear UI components (Image, TMP_Text, Button).
- **Action:** Automatically find and assign child components using 'GetComponentInChildren<T>()' or 'transform.Find()' within 'Reset()'.
- **Goal:** Minimize manual drag-and-drop-errors in the Inspector.
- **EXCEPTION (Manual Only):**
    - **DO NOT** auto-assign `Transform` or `RectTransform` in `Reset()`.
    - **Reason:** Transforms are generic and ambiguous; auto-assignment often picks the wrong object. These must be assigned manually in the Inspector.

### Component Caching Strategy
- **Rule:** Cache the specific UI component (e.g., 'Image') instead of 'RectTransform'.
- **Reason:** You can access 'image.rectTransform' freely, but you cannot access 'image' from 'rectTransform' without 'GetComponent' (overhead)
- **Example:** '[SerializeField] private Image _hpBar;' (Good) vs 'private RectTransform _hpBarRect' (Bad).

### Resolution Independence
- **Rule:** Trust the 'Canvas Scaler'
- **Action:** Use fixed 'float' values for padding/size in code. Do not manually calculate screen ratios or pixel density.



## General Coding Standards
- **Naming Conventions:**
    - Public/Methods: 'PascalCase'
    - Private fields: '_camelCase' (starts with underscore)
    - Suffixes: '*View', '*Presenter', '*UseCase', '*Repository', '*SO'
- **Serialization:** Use '[SerializeField] private' for inspector variables. Do not use 'public' fields for internal state.
- **Performance:** Avoid 'GetComponent', 'FindObjectOfType' in 'Update()' loops. Cache references in 'Awake()' or 'Start()'.



## Workflow
- **Asset Placement:**
    - If an asset (Sprite/Prefab) is used ONLY by this feature, place it in 'Presentation/Art' or 'Presentation/Prefabs'.
    - If shared across features, move to 'Assets/_Game/Core/...'.
- **Plan Verification:** Check if the plan respects the 'Assets/_Game/...' path and Asset Cohesion rules.
- Always verify the plan with the user before implementation.



## Governance
- **Version**: 0.1.0 | **Ratified**: 2026-00-00 | **Last Amended**: 2026-02-10
