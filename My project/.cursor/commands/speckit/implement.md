---
description: Execute the implementation plan by processing tasks in tasks.md, strictly adhering to Samsara Layer constraints.
---

## User Input

```text
$ARGUMENTS
```

You **MUST** consider the user input before proceeding (if not empty).

## Outline

1. **Context Loading**:
   - Locate and read `plan.md` and `tasks.md` in the current documentation directory.
   - Load the Samsara Constitution rules (Data/Logic Separation, Pure DI).

2. **Unity Project Setup Verification**:
   - Verify the existence of the `Assets/_Game/` directory structure.
   - If a `.gitignore` check is required, strictly use the **Unity Standard .gitignore** (ignoring `Library/`, `Temp/`, `Obj/`, `Logs/`, `UserSettings/`). Do NOT generate web/backend ignore files like `.eslintignore` or `.dockerignore`.

3. **Parse Task Structure (Samsara Phases)**:
   Extract tasks grouped by Samsara Layers:
   - Phase 1: `[Data]` (DTOs, Enums, Interfaces)
   - Phase 2: `[Domain]` (Pure C# Business Logic)
   - Phase 3: `[Presentation]` (MonoBehaviours, Views, Presenters)
   - Phase 4: `[Bootstrapper]` (DI & Wiring)
   - Phase 5: Integration/Feature Guards

4. **Samsara Execution Rules (CRITICAL)**:
   When writing `.cs` files, you MUST apply these rules based on the task tag:
   - `[Data]`: MUST NOT contain `using UnityEngine;` (unless required for specific structs). No logic, no methods other than simple constructors.
   - `[Domain]`: MUST NOT inherit `MonoBehaviour`. MUST use Constructor Injection for dependencies. NEVER use `new` to instantiate other domain/data classes.
   - `[Presentation]`: MUST inherit `MonoBehaviour` (for Views/Interceptors) or implement `IDisposable` (for Presenters). UI logic only.
   - `[Bootstrapper]`: The ONLY place you are allowed to use the `new` keyword to instantiate Domain/Data classes and inject them.

5. **Step-by-Step Execution Flow (Anti-Context Collapse)**:
   - **DO NOT execute all tasks at once.**
   - Execute tasks **one Phase at a time** (e.g., complete all `[Data]` tasks first).
   - After completing a Phase, **STOP AND ASK THE USER**: "Phase X is complete. Please check for Unity compile errors. Shall I proceed to Phase Y?"
   - Wait for explicit user confirmation before moving to the next layer.
   - *Exception*: If the user specifies a target in the input (e.g., "/speckit.implement Phase 1 only"), execute only that target and stop.

6. **Progress Tracking**:
   - As each `.cs` file is successfully written, strictly update `tasks.md` by changing `- [ ]` to `- [x]`.
   - Ensure the `using` statements at the top of each file are minimal and correct.
   - Do NOT invent or hallucinate missing scripts. If a dependency is missing, stop and report it.

7. **Completion**:
   - Report the files created/modified in the current run.
   - Remind the developer to check the Unity Console for compile errors.