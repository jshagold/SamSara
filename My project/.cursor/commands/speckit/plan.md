---
description: Execute the implementation planning workflow using the plan template to generate design artifacts.
handoffs: 
  - label: Create Tasks
    agent: speckit.tasks
    prompt: Break the plan into tasks
    send: true
  - label: Create Checklist
    agent: speckit.checklist
    prompt: Create a checklist for the following domain...
---

## User Input

```text
$ARGUMENTS
```

You **MUST** consider the user input before proceeding (if not empty).

## Outline

This command generates technical implementation plans based on an existing feature specification (`specify.md`). The output structure MUST adapt to the complexity of the feature while strictly adhering to Unity/C# paradigms.

1. **Context Loading**: 
   - Identify the target `specify.md`. Target directory is the folder containing it.

2. **Phase 0: Structure Proposal (Consultation)**:
   - Analyze the `specify.md`.
   - **Default Action**: Attempt to fit all design components (Data, Interfaces, Logic) into a single `plan.md`.
   - **Zero-Guessing Rule**: If the feature seems too large for a single file, you **MUST ASK** the developer before creating extra files: *"This feature has significant architectural scope. Should I keep everything in one `plan.md` or split it into contextual files (e.g., `data-model.md`)?"*
   - Do NOT generate multiple files without explicit confirmation if the structure is ambiguous.

3. **Phase 1: Architecture & Planning**:
   Execute the planning workflow and generate the decided files. Ensure the following architectural components are covered across the generated files:

   **A. Pure Data Structures**
   - Define C# data structures (`struct`, `class`, `record`, `ScriptableObject` schemas).
   - **Samsara Rule**: Strict Data/Logic Separation. No MonoBehaviour logic or behaviors here.

   **B. Interfaces & Events**
   - Define C# Interfaces (e.g., `IInteractable`, `ISaveable`, `ICharacterRepository`).
   - Define global/local Events (`Action`, `Func`) for decoupled communication.

   **C. System Logic & Integration**
   - Define the Core Handlers/Managers (`MonoBehaviour`, pure C# controllers).
   - Detail Unity lifecycle integrations (e.g., `Awake`, `OnApplicationQuit` constraints, Coroutines).
   - **Zero-Guessing Enforcement**: If external project systems (e.g., Save System, UI Manager) are unknown, explicitly list them as "NEEDS CLARIFICATION" questions for the developer. Do not invent hypothetical classes.

4. **Stop and Report**: 
   - Report the generated files.
   - Prompt the developer to review the architecture before coding.

## Key Rules
- **Unity/C# Native**: Replace all web terminology (API endpoints, database schemas) with Unity terminology (Events, Interfaces, ScriptableObjects, Prefab structure).
- **Samsara Master Constraints**:
  1. **Zero-Guessing**: Ask, never assume existing codebase structure.
  2. **Data/Logic Separation**: Always split state (Data) from behavior (Logic).
