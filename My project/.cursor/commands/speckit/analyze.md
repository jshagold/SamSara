---
description: Perform a non-destructive cross-artifact consistency and quality analysis across spec.md, plan.md, and tasks.md after task generation.
---
# Samsara Code Analysis Protocol


## User Input (Priority Focus)
> **User's specific request:**
> $ARGUMENTS
> (If the user provided specific instructions above, prioritize them while maintaining Constitution compliance.)


## Goal

Identify inconsistencies, duplications, ambiguities, and underspecified items across the three core artifacts (`specify.md`, `plan.md`, `tasks.md`) before implementation. This command MUST run only after `/speckit.tasks` has successfully produced a complete `tasks.md`.

## Operating Constraints

**STRICTLY READ-ONLY**: Do **not** modify any files. Output a structured analysis report. Offer an optional remediation plan (user must explicitly approve before any follow-up editing commands would be invoked manually).

**Constitution Authority**: The project constitution (Samsara Feature-based Modular Architecture) is **non-negotiable** within this analysis scope. Constitution conflicts are automatically CRITICAL and require adjustment of the spec, plan, or tasks—not dilution, reinterpretation, or silent ignoring of the principle. If a principle itself needs to change, that must occur in a separate, explicit constitution update outside `/speckit.analyze`.

## Execution Steps

### 1. Initialize Analysis Context

Locate the target feature documentation directory (e.g., `Docs/[Category]/[FeatureName]/`). Derive absolute paths for:
- SPEC = FEATURE_DIR/specify.md
- PLAN = FEATURE_DIR/plan.md
- TASKS = FEATURE_DIR/tasks.md

Abort with an error message if any required file is missing (instruct the user to run the missing prerequisite command). For single quotes in args, use escape syntax.

### 2. Load Artifacts (Progressive Disclosure)

Load only the minimal necessary context from each artifact:

**From specify.md:**
- Overview/Context
- Functional Requirements
- Non-Functional Requirements
- User Scenarios
- Edge Cases (if present)

**From plan.md:**
- Pure Data Structures & Interfaces
- System Logic (Domain Layer)
- Unity Integration (Presentation/Bootstrapper)
- Source Layout & Technical constraints

**From tasks.md:**
- Task IDs
- Descriptions
- Layer Phase grouping (`[Data]`, `[Domain]`, `[Presentation]`, `[Bootstrapper]`)
- Parallel markers [P]
- Referenced file paths

**From constitution:**
- Load Samsara Constitution principles (Data/Logic Separation, Pure DI, Layer Rules) for validation.

### 3. Build Semantic Models

Create internal representations (do not include raw artifacts in output):

- **Requirements inventory**: Each functional + non-functional requirement with a stable key.
- **Architecture inventory**: Data models, UseCases, and Views mapped from the plan.
- **Task coverage mapping**: Map each task to one or more requirements or architecture components.
- **Constitution rule set**: Extract MUST/SHOULD normative statements from Samsara Constitution.

### 4. Detection Passes (Token-Efficient Analysis)

Focus on high-signal findings. Limit to 50 findings total; aggregate remainder in overflow summary.

#### A. Duplication Detection
- Identify near-duplicate requirements or duplicated logic handlers.
- Mark lower-quality phrasing for consolidation.

#### B. Ambiguity Detection
- Flag vague adjectives (fast, scalable, secure, robust) lacking measurable Unity metrics.
- Flag unresolved placeholders (TODO, TKTK, ???, `<placeholder>`, etc.).

#### C. Underspecification
- Data structures missing clear asset ownership (e.g., ScriptableObject vs raw C# class).
- Tasks referencing files, UI Prefabs, or components not defined in spec/plan.

#### D. Constitution Alignment (Samsara Specific)
- **Layer Violation**: `[Domain]` or `[Data]` tasks referencing `UnityEngine` or MonoBehaviour UI components.
- **DI Violation**: Tasks instructing the use of `new` outside of the `[Bootstrapper]` phase.
- **Path Violation**: File paths not strictly following the `Assets/_Game/...` structure.

#### E. Coverage Gaps
- Requirements with zero associated tasks.
- Tasks with no mapped requirement/architecture component.
- System safeguards (e.g., FR-07 Save-Blocker) not reflected in tasks.

#### F. Inconsistency
- Terminology drift (same concept named differently across files).
- Task ordering contradictions (e.g., `[Presentation]` integration tasks ordered before foundational `[Data]` tasks).
- Conflicting paths between `plan.md` and `tasks.md`.

### 5. Severity Assignment

Use this heuristic to prioritize findings:

- **CRITICAL**: Violates Samsara constitution MUST (e.g., Data/Logic mix, explicit path violation), missing core spec artifact, or requirement with zero coverage that blocks baseline functionality.
- **HIGH**: Duplicate or conflicting requirement, missing task coverage for specific FR, ghost tasks (Zero-guessing violation).
- **MEDIUM**: Terminology drift, missing non-functional task coverage, underspecified Unity edge case.
- **LOW**: Style/wording improvements, minor redundancy not affecting execution order.

### 6. Produce Compact Analysis Report

Output a Markdown report (no file writes) with the following structure:

## Specification Analysis Report

| ID | Category | Severity | Location(s) | Summary | Recommendation |
|----|----------|----------|-------------|---------|----------------|
| C1 | Constitution | CRITICAL | tasks.md:L45 | Domain task uses UnityEngine | Move logic to Presentation |

(Add one row per finding; generate stable IDs prefixed by category initial.)

**Coverage Summary Table:**

| Requirement Key | Has Task? | Task IDs | Notes |
|-----------------|-----------|----------|-------|

**Constitution Alignment Issues:** (if any)

**Unmapped Tasks:** (if any)

**Metrics:**

- Total Requirements
- Total Tasks
- Coverage % (requirements with >=1 task)
- Ambiguity Count
- Duplication Count
- Critical Issues Count

### 7. Provide Next Actions

At end of report, output a concise Next Actions block:

- If CRITICAL issues exist: Recommend resolving before `/speckit.implement`.
- If only LOW/MEDIUM: User may proceed, but provide improvement suggestions.
- Provide explicit command suggestions: e.g., "Manually edit tasks.md to fix path violations", "Run /speckit.plan to adjust architecture".

### 8. Offer Remediation

Ask the user: "Would you like me to suggest concrete remediation edits for the top N issues?" (Do NOT apply them automatically.)

## Operating Principles

### Context Efficiency

- **Minimal high-signal tokens**: Focus on actionable findings, not exhaustive documentation.
- **Progressive disclosure**: Load artifacts incrementally; don't dump all content into analysis.
- **Token-efficient output**: Limit findings table to 50 rows; summarize overflow.
- **Deterministic results**: Rerunning without changes should produce consistent IDs and counts.

### Analysis Guidelines

- **NEVER modify files** (this is read-only analysis).
- **NEVER hallucinate missing sections** (if absent, report them accurately).
- **Prioritize constitution violations** (these are always CRITICAL).
- **Use examples over exhaustive rules** (cite specific instances, not generic patterns).
- **Report zero issues gracefully** (emit success report with coverage statistics).

## Context

$ARGUMENTS