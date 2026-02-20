---
description: Generate an actionable, dependency-ordered tasks.md for the feature based on available design artifacts.
handoffs: 
  - label: Analyze For Consistency
    agent: speckit.analyze
    prompt: Run a project analysis for consistency
    send: true
  - label: Implement Project
    agent: speckit.implement
    prompt: Start the implementation in phases
    send: true
---

## User Input

```text
$ARGUMENTS
```

You **MUST** consider the user input before proceeding (if not empty).

## Outline

1. **Context Loading**:
   - Locate and read `plan.md` and `specify.md` in the target documentation directory (`Docs/[Category]/[FeatureName]/`).
   - Read the **Samsara Project Constitution** if available.
   - **Zero-Guessing**: Do NOT invent file paths. Use EXACTLY what is written in `plan.md`.

2. **Execute Samsara Layered Task Generation Workflow**:
   Organize tasks strictly following the **Samsara Architecture & Layer Rules**:
   - **Phase 1: `[Data]` Layer**: MasterData(SO), Cached Repositories, DTOs. (Target: `Assets/_Game/Features/[FeatureName]/Data/` or `Core/`).
   - **Phase 2: `[Domain]` Layer**: Pure C# UseCases, Business Logic, Event Proxies. MUST NOT have Unity dependencies. (Target: `.../Domain/`).
   - **Phase 3: `[Presentation]` Layer**: MonoBehaviours, Presenters, Views. UI logic only. (Target: `.../Presentation/`).
   - **Phase 4: `[Bootstrapper]` (DI & Wiring)**: Instantiate Logic classes (`new`) and perform constructor injection. (Target: `GlobalBootstrapper` or `[Feature]Bootstrapper`).

3. **Generate `tasks.md`**:
   - **File Location**: Create `tasks.md` in the SAME `Docs/...` directory as `plan.md`.
   - **Task Paths**: The implementation file paths inside the checklist MUST point to the actual Unity structure (`Assets/_Game/...`).

4. **Report**:
   - Output the path to the generated `tasks.md`.
   - Validate that ALL tasks follow the checklist format and layer rules.

## Task Generation Rules

**CRITICAL**: Tasks MUST enforce the Data/Logic Separation and Pure DI rules defined in the Samsara Constitution.

### Checklist Format (REQUIRED)

Every task MUST strictly follow this format:

`- [ ] [TaskID] [P?] [LayerCategory] Description with exact file path`

**Format Components**:
1. **Checkbox**: ALWAYS `- [ ]`
2. **Task ID**: Sequential (T001, T002...)
3. **[P] marker**: Include ONLY if parallelizable.
4. **[LayerCategory] label**: REQUIRED. MUST be `[Data]`, `[Domain]`, `[Presentation]`, or `[Bootstrapper]`.
5. **Description & Path**: Action with exact `.cs` or `.prefab` path.

**Examples**:
- ✅ CORRECT: `- [ ] T001 [P] [Data] Create StateSnapshot DTO in Assets/_Game/Core/ErrorHandling/StateSnapshot.cs`
- ✅ CORRECT: `- [ ] T003 [Domain] Implement ErrorRecoveryFlow UseCase (Pure C#) in Assets/_Game/App/Systems/ErrorHandling/ErrorRecoveryFlow.cs`
- ✅ CORRECT: `- [ ] T008 [Presentation] Create ErrorPopupView MonoBehaviour in Assets/_Game/Features/ErrorHandling/Presentation/ErrorPopupView.cs`
- ✅ CORRECT: `- [ ] T010 [Bootstrapper] Wire dependencies in Assets/_Game/App/Bootstrapper/GlobalBootstrapper.cs`
- ❌ WRONG: `- [ ] T001 Create System` (Missing tags and paths)

### Implementation Constraints to Add to Tasks:
- For `[Data]` tasks involving save: Add note to implement `SaveDataAsync` and `SaveDataSync`.
- For `[Domain]` tasks: Add note "Strictly NO UnityEngine references".
- For `[Presentation]` Presenter tasks: Add note "Must implement IDisposable and inject dependencies via constructor".
- For `[Bootstrapper]` tasks: Add note "The ONLY place allowed to use 'new' for Domain/Data classes".