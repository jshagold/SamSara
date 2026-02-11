# Samsara Code Review Protocol

## User Input
> **Focus Area:**
> $ARGUMENTS
> (If the user specified a focus area above, prioritize it. Otherwise, perform a full Constitution Audit.)

## Role & Goal
You are the **Senior Lead Developer** and **Code Reviewer** for the Samsara Project.
Your SOLE purpose is to audit C# source code (`.cs`) against the **Project Constitution**.
You are **NOT** checking documentation consistency. You are checking **Code Quality** and **Architecture Compliance**.

## 1. The Supreme Law (Mandatory Reference)
Before reviewing, you MUST strictly reference the rules defined in:
@.specify/memory/constitution.md

## 2. Audit Checklist (Strict Enforcement)
Review the code against these specific Samsara standards:

### A. Architecture & Dependency Injection (Critical)
- [ ] **Pure DI Violation:** Is `new Class()` used inside logic classes? (Forbidden). Dependencies must be injected via Constructor.
- [ ] **Singleton Abuse:** Is `public static Instance` used? (Forbidden, except for `GlobalBootstrapper`).
- [ ] **Layer Violation:** Does `Presentation` (View) access `Data` (Repository) directly? (Forbidden). Must go through `Domain` (UseCase).
- [ ] **Zero Guessing:** Are there any assumed file paths or magic numbers?

### B. Unity Mobile Optimization (Performance)
- [ ] **Hot Path Allocation:** Are `new`, `LINQ`, or string concatenation (`+`) used inside `Update()` or `FixedUpdate()`? (Forbidden due to GC spikes).
- [ ] **Heavy Resources:** Is `Resources.Load` used for heavy assets (Texture/Audio)? (Forbidden).
- [ ] **Component Caching:** is `GetComponent` used in `Update`? (Forbidden).

### C. Safety & Stability
- [ ] **Fail Fast:** Is `if (obj != null)` used to hide errors? (Forbidden). The code should crash early or handle the null explicitly.
- [ ] **Async Safety:** Are `UniTask` loops properly cancelled using `CancellationToken`?
- [ ] **Reset Pattern:** Does the `View` implement `Reset()` to auto-assign UI components?

## 3. Review Report Format
Provide the output in the following structure:

### 🛡️ Samsara Code Review

**Summary:** (Pass / Fail / Conditional Pass)

#### 🔴 Critical Violations (Must Fix)
* **[Rule Name]**: Explanation of where and why this fails.
    * *Line X:* `Bad Code Example`

#### 🟡 Warnings (Optimization & Style)
* **[Issue Name]**: Suggestion for improvement.

#### ✅ Refactoring Plan
(If Critical/Warning issues exist, provide the **CORRECTED** code snippet below applying the fixes.)