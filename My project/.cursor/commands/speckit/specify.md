---
description: Create or update the feature specification from a natural language feature description directly into the Docs hierarchy. Customized for the Samsara Unity mobile game project.
handoffs: 
  - label: Build Technical Plan
    agent: speckit.plan
    prompt: Create a plan for the spec. I am building with Unity 6.2, C#, UniTask, Newtonsoft.Json, DOTween, TMP. Architecture is Feature-based Modular (DDD + Clean Architecture). See constitution.md for full rules.
  - label: Clarify Spec Requirements
    agent: speckit.clarify
    prompt: Clarify specification requirements
    send: true
---

## User Input

```text
$ARGUMENTS
```

You **MUST** consider the user input before proceeding (if not empty).

## Project Context (Always Apply)

This specification is for **Samsara** — a Unity 6.2 mobile roguelike turn-based RPG.
Before writing any spec, internalize these constraints:

- **Engine:** Unity 6.2 / C#
- **Platform:** Mobile (Android / iOS)
- **Architecture:** Feature-based Modular (DDD + Clean Architecture)
- **Async:** UniTask only (no Coroutines, no standard Task)
- **Serialization:** Newtonsoft.Json
- **UI:** TMP (TextMeshPro), Unity UI
- **Animation/Tween:** DOTween
- **Root path:** `Assets/_Game/` — never assume `Assets/Scripts`
- **Full rules:** See `constitution.md` (always treat it as the source of truth)

---

## Outline

The text the user typed after `/speckit.specify` in the triggering message **is** the feature description. Assume you always have it available in this conversation even if `$ARGUMENTS` appears literally below. Do not ask the user to repeat it unless they provided an empty command.

Given that feature description, do this:

### 1. Categorize the Feature

Analyze the feature description and assign one of these categories:

| Category | When to use | Example features |
|----------|-------------|-----------------|
| `Core` | Pure interfaces, base classes, and shared utilities. No Unity or external library dependencies. No game-content dependency. | `ISceneNavigator`, `IPopupManager`, `SceneKey`, shared domain base classes |
| `App` | Concrete implementations of `Core` interfaces. Top-level wiring: bootstrappers, GameContext, external library wrappers. Depends on Unity and third-party libs. | `GlobalBootstrapper`, `GameContext`, `SceneNavigator` (impl), `AutoSaveManager`, external SDK wrappers |
| `Features` | Self-contained game mechanic or content module. Depends on `Core` interfaces but never on other `Features` directly. | `Battle`, `CharacterEvo`, `Maintenance`, `Stage`, `ActionEvent` |

### 2. Determine Folder Name (PascalCase)

- Extract the core feature name from the description.
- Convert strictly to PascalCase (e.g., `SaveSystem`, `BattleSystem`, `CharacterEvo`).
- **Do not use numbers, prefixes, or dashes.**

### 3. Set Target Paths

- `FEATURE_DIR` = `Docs/[Category]/[FeatureName]/`
- `SPEC_FILE` = `FEATURE_DIR/specify.md`

### 4. Execution Flow

1. Parse user description from Input.
   If empty → ERROR "No feature description provided"

2. Extract key concepts:
   - **Actors**: Who interacts with this feature? (Player, System, NPC, etc.)
   - **Actions**: What can they do?
   - **Data**: What data is created, read, updated, or deleted?
     - Is this **RunData** (resets on reincarnation / 윤회) or **AccountData** (permanent across runs)?
   - **Constraints**: What rules apply? (Constitution rules, GDD decisions)

3. For unclear aspects:
   - Make informed guesses based on `constitution.md`, GDD documents, and Unity best practices.
   - Only mark `[NEEDS CLARIFICATION: specific question]` if:
     - The choice significantly impacts feature scope or player experience
     - Multiple reasonable interpretations exist with meaningfully different implications
     - No reasonable default exists in `constitution.md` or the GDD
   - **LIMIT: Maximum 3 [NEEDS CLARIFICATION] markers total**
   - Priority order: game scope > data lifetime (RunData vs AccountData) > player experience > technical details

4. Fill Player / System Scenarios section.
   If no clear flow can be determined → ERROR "Cannot determine user scenarios"

5. Generate Functional Requirements.
   - Each requirement must be testable.
   - Every requirement must be traceable to a `constitution.md` rule or a GDD decision.

6. Define Success Criteria.
   - Measurable, implementation-agnostic outcomes.
   - Use game-relevant metrics (frame rate, response feel, player action count, data integrity).
   - Each criterion must be verifiable without knowing implementation details.

7. Identify Key Entities (if data is involved).
   - Explicitly label each entity as `RunData` or `AccountData`.
   - Flag any entity that spans both lifetimes as `[NEEDS CLARIFICATION]`.

8. Return: SUCCESS (spec ready for writing)

### 5. Write the Specification

Write to `SPEC_FILE` using the template structure below. Replace all placeholders with concrete details derived from the feature description. Preserve section order and headings. **Do not run any external scripts.**

---

## Spec Template

```markdown
# Feature Specification: [Feature Name]

**Category**: Core | Features
**Folder**: `Docs/[Category]/[FeatureName]/`
**Status**: Draft
**Last Updated**: [DATE]
**GDD Reference**: [GDD document name or section — e.g., GDD_BattleSystem_v1.0]

---

## 1. Overview

### Purpose
[1-2 sentences. Why does this feature exist? What value does it provide to the player or system?]

### Scope

**In Scope:**
- [What is included]

**Out of Scope (excluded from 1st development phase):**
- [What is explicitly excluded — include reason]

---

## 2. Actors & Interactions

| Actor | Role | Interaction |
|-------|------|-------------|
| [Player / System / NPC] | [Role description] | [What they do] |

---

## 3. Data Model

### Data Lifetime
> Every entity MUST be explicitly labeled as RunData or AccountData.
> RunData resets on every reincarnation (윤회). AccountData persists permanently across runs.

| Entity | Lifetime | Description |
|--------|----------|-------------|
| [EntityName] | `RunData` \| `AccountData` | [Description] |

### Entity Definitions

#### [EntityName]
```
[FieldName]: [Type] — [Description] (RunData | AccountData)
```

---

## 4. Functional Requirements

> Each requirement maps to a constitution.md rule or GDD decision.

### FR-[N]: [Requirement Title]
- **Description**: [What must happen]
- **Acceptance Criteria**:
  - [ ] [Testable condition 1]
  - [ ] [Testable condition 2]
- **Constitution Rule**: [Relevant section — e.g., §4 Cached Repository, §6 Data Lifetime]
- **GDD Reference**: [Relevant GDD document name]

---

## 5. Player / System Scenarios

### Happy Path
1. [Step-by-step description of the normal flow]

### Edge Cases
- **[Case Name]**: [Description and expected behavior]

### Error Cases
- **[Error Name]**: [Trigger condition and how it is handled]

---

## 6. Success Criteria

> Technology-agnostic. Measurable. Game-context focused.

| # | Criterion | Measurement |
|---|-----------|-------------|
| SC-1 | [Outcome to achieve] | [How to verify] |

---

## 7. Constitution Compliance

> Verifies the feature design does not violate constitution.md.
> Remove rows that do not apply to this feature.

| Rule | Compliant? | Notes |
|------|-----------|-------|
| §2 Zero Guessing — paths use `Assets/_Game/` | ✅ / ⚠️ / ❌ | |
| §2 Data/Logic Separation — no hardcoded game data | ✅ / ⚠️ / ❌ | |
| §2 Pure DI — Logic classes use Constructor Injection | ✅ / ⚠️ / ❌ | |
| §3 Layer Structure — Data / Domain / Presentation separated | ✅ / ⚠️ / ❌ | |
| §4 Cached Repository — data loaded once, cache modified | ✅ / ⚠️ / ❌ | |
| §4 Dual-Mode Saving — both Async and Sync implemented | ✅ / ⚠️ / ❌ | |
| §5 PopupManager — not called from Domain layer | ✅ / ⚠️ / ❌ | |
| §6 Singleton restriction — only GlobalBootstrapper allowed | ✅ / ⚠️ / ❌ | |
| §6 SceneNavigator — scene transitions via ISceneNavigator only | ✅ / ⚠️ / ❌ | |
| §6 AccountData/RunData — not mixed in the same class | ✅ / ⚠️ / ❌ | |
| §8 Fail Fast — no silent null returns | ✅ / ⚠️ / ❌ | |
| §8 Safe Cleanup — no exceptions thrown in Dispose/OnDestroy | ✅ / ⚠️ / ❌ | |

---

## 8. Dependencies

| Dependency | Type | Reason |
|------------|------|--------|
| [Feature or system name] | `Core` \| `Feature` \| `External Lib` | [Why it is needed] |

---

## 9. Assumptions

> Decisions made without explicit confirmation. If wrong, the spec must be revised.

- [Assumption 1]
- [Assumption 2]

---

## 10. Open Questions

> Must be resolved before implementation begins.

- [ ] [Question or unresolved item]
```

---

### 6. Specification Quality Validation

After writing the initial spec, validate it. Create a checklist file at `FEATURE_DIR/checklists/requirements.md`:

```markdown
# Specification Quality Checklist: [FEATURE NAME]

**Purpose**: Validate specification completeness before proceeding to planning
**Created**: [DATE]
**Feature**: [Link to specify.md]

## Content Quality

- [ ] No implementation details (no class names, Unity APIs, or specific library calls)
- [ ] Focused on WHAT and WHY, not HOW
- [ ] All mandatory sections completed (Sections 1-7 required; 8-10 when applicable)
- [ ] GDD Reference points to an existing GDD document

## Requirement Completeness

- [ ] No [NEEDS CLARIFICATION] markers remain
- [ ] Every requirement is testable and unambiguous
- [ ] Every entity has an explicit RunData / AccountData label
- [ ] Success criteria are measurable and game-context focused
- [ ] Happy path, edge cases, and error cases are all defined
- [ ] Scope is clearly bounded (both In Scope and Out of Scope stated)
- [ ] Dependencies identified

## Constitution Compliance

- [ ] §2 Path rule: `Assets/_Game/` used, `Assets/Scripts` never assumed
- [ ] §2 Data/Logic separation: no hardcoded game data in logic classes
- [ ] §2 Pure DI: no `new` instantiation of Logic classes outside Bootstrapper
- [ ] §3 Layer separation: Data / Domain / Presentation roles are clear
- [ ] §4 Cached Repository: single load at startup principle reflected
- [ ] §4 Dual-Mode Saving: both Async and Sync saving addressed (if feature saves data)
- [ ] §5 PopupManager: no popup calls from Domain layer
- [ ] §6 Singleton restriction: no Singleton other than GlobalBootstrapper
- [ ] §6 SceneNavigator: all scene transitions go through ISceneNavigator
- [ ] §6 AccountData/RunData: two lifetimes are clearly separated
- [ ] §8 Fail Fast: no silent null returns, CheckDataIntegrity referenced where applicable
- [ ] §8 Safe Cleanup: Dispose/OnDestroy safety handling addressed

## Feature Readiness

- [ ] Every Functional Requirement has Acceptance Criteria
- [ ] Player/System scenarios cover the primary flow
- [ ] Success criteria describe verifiable game outcomes
- [ ] No blocking Open Questions remain (or all are explicitly listed)
```

**Validation handling:**

- **All items pass** → Mark checklist complete and proceed to step 7.
- **Items fail** → Update the spec to fix each issue. Re-validate (max 3 iterations). If still failing after 3 iterations, document remaining issues in checklist notes and warn the user.
- **[NEEDS CLARIFICATION] markers remain** → Present questions to the user (max 3) using this format:

```markdown
## Question [N]: [Topic]

**Context**: [Quote the relevant spec section]

**What we need to know**: [The specific question from the NEEDS CLARIFICATION marker]

**Suggested Answers**:

| Option | Answer | Implications |
|--------|--------|--------------|
| A      | [First option] | [Impact on the feature] |
| B      | [Second option] | [Impact on the feature] |
| C      | [Third option] | [Impact on the feature] |
| Custom | Provide your own answer | Write your answer freely |

**Your choice**: _[Waiting for response]_
```

Number questions sequentially (Q1, Q2, Q3 — max 3 total). Present all questions together before waiting for responses. After the user responds, update the spec and re-run validation.

---

### 7. Completion Report

Report completion with:
- `FEATURE_DIR` path
- Checklist pass/fail summary
- Any remaining Open Questions
- Readiness for next phase: `/speckit.plan`

---

## General Guidelines

- Focus on **WHAT** the feature does and **WHY** it exists in Samsara.
- **Never specify HOW** to implement (no class names, Unity APIs, or library-specific calls).
- Written to be unambiguous enough for Cursor and Gemini to implement correctly from the spec alone.
- Do NOT embed checklists inside the spec itself — that is always a separate file.

### Reasonable Defaults for Samsara (Do NOT ask about these)

| Topic | Default assumption |
|-------|--------------------|
| Async pattern | UniTask (not Coroutine, not standard Task) |
| Save format | JSON via Newtonsoft.Json |
| Save location | `Application.persistentDataPath` |
| UI framework | Unity UI + TMP |
| Animation | DOTween |
| Error handling | Fail Fast (throw InvalidOperationException) + Fail Safe cleanup (null-conditional in Dispose) |
| Scene transition | ISceneNavigator — except Main to Maintenance which is a camera move |
| Popup interaction | Caller awaits UniTask result from PopupManager |
| Data reset boundary | On reincarnation (윤회) |
| RunData scope | Character stats, Gold, Karma (업보), current Day |
| AccountData scope | Unlocked evolution nodes, Codex (도감), Gems (보석) |

### Success Criteria Guidelines for Samsara

**Good examples:**
- "AccountData (unlocked evolution nodes) is fully preserved after reincarnation (윤회)"
- "No frame drop is perceptible during scene transitions (60 fps maintained)"
- "After a forced app close, the game restores exactly to the last saved state on relaunch"
- "Battle outcome stat changes are reflected in RunData immediately and accurately"

**Bad examples (avoid):**
- "The Repository queries in O(1)" — implementation detail
- "UniTask completes without exception" — technical specification
- "The JSON file serializes correctly" — implementation detail