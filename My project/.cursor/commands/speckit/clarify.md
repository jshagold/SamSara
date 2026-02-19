---
description: Identify underspecified areas in the current feature spec by asking up to 5 highly targeted clarification questions and encoding answers back into the spec.
handoffs: 
  - label: Build Technical Plan
    agent: speckit.plan
    prompt: Create a plan for the spec. I am building with...
---

## User Input

```text
$ARGUMENTS
```

You **MUST** consider the user input before proceeding (if not empty).

## Outline

Goal: Detect ambiguity in the active feature specification (`specify.md`) and record clarifications directly in the spec file, prioritizing the "Data/Logic Separation" and "Zero-Guessing" principles.

Execution steps:

1. **Context Loading**: 
   - Identify the target `specify.md`.
   - If the file is missing, instruct the user to run `/speckit.specify` first.

2. **Samsara-Centric Ambiguity Scan**:
   Analyze the spec using the following Unity-focused categories:

   **A. Data Model & State (Data)**:
   - Are the core entities and their properties clearly defined as pure data?
   - Is it clear which data needs to be persistent (Save/Load)?
   - Are initial states and valid ranges for variables specified?

   **B. System Logic & Interaction (Logic)**:
   - Are the triggers for logic execution (Events, Input, Timers) clear?
   - Is the sequence of operations or state transitions ambiguous?
   - Are the calculation formulas or win/loss conditions defined?

   **C. Unity Integration & Lifecycle**:
   - Are there specific Unity lifecycle requirements (e.g., must run in `FixedUpdate`, happens on `OnDisable`)?
   - How should the system behave during Scene transitions or App Pause/Quit?

   **D. Edge Cases & Error Handling**:
   - What happens if data is corrupted or missing?
   - How does the system handle "Frame-rate dependency" or "Network latency" (if applicable)?

3. **Questioning Loop (Interactive)**:
   - Generate a prioritized queue of **maximum 5 questions**.
   - Present **EXACTLY ONE** question at a time.
   - For each question, provide a **Recommended Option** or a **Suggested Short Answer** based on Samsara best practices (Data/Logic separation).
   - Format multiple-choice questions as a Markdown table.
   - Stop when all critical ambiguities are resolved or the 5-question limit is reached.

4. **Incremental Integration**:
   - After each answer is accepted, immediately update the `specify.md`.
   - Create or update a `## Clarifications` section with a `- Q: <question> -> A: <answer>` bullet.
   - **Crucial**: Also update the relevant functional sections (e.g., Functional Requirements, Data Model) so the clarification is reflected in the actual spec, not just the log.

5. **Final Report**:
   - Summarize the changes and sections updated.
   - Suggest running `/speckit.plan` as the next step.

## Key Rules
- **Zero-Guessing**: If the spec doesn't say it, ask. Never assume a specific Unity plugin or folder structure exists.
- **Data/Logic Focus**: Always push for clarifications that help separate the data shape from the implementation logic.
- **Unity Native**: Use terms like MonoBehaviours, ScriptableObjects, Prefabs, and Events instead of REST, Database, or Endpoints.