> 완성된 MD 파일 내용만 보관. Ctrl+A → Ctrl+C 후 파일에 바로 붙여넣기 가능.
---
**상태:** ✅ v2.0.0 노션 업로드 완료
**대응 KR 버전:** Plan-KR v2.0.0
---
## 버전 히스토리
<table header-row="true">
<tr>
<td>버전</td>
<td>날짜</td>
<td>수정 내용</td>
</tr>
<tr>
<td>v1.0.0</td>
<td>2026-04-20</td>
<td>최초 작성 (Specify v1.1.0 기반)</td>
</tr>
<tr>
<td>v2.0.0</td>
<td>2026-04-27</td>
<td>전면 재작성. Specify v2.1.1 기준으로 재정합. v1.0.0의 Cross-feature 침범 / 구현 디테일 직접 기재 / Spec에 없던 구조 결정의 Plan 단계 원창 도입 등 결함을 제거. RQ-14/15/16 충족 위치를 §1-3에 명시 후 각 섹션에서 구체화. §1\~§11 본문 작성·반영 완료.</td>
</tr>
</table>
> **v1.0.0 본문 보관:** Archive 폴더에 별도 보관 예정 (GBL-002 추적 — 메이저 버전업 시 이전 본문 보관 정책 적용). v2.0.0 본문 검토 완료 후 일괄 보관 처리.
---
## 현재 버전: v2.0.0
```markdown
# ReplayScene — Plan

**Status:** v2.0.0
**Feature:** ReplayScene (Phase 6)
**Specify reference:** v2.1.1
**Constitution references:** §3, §6, §7, §8, §9

---

## Version History

| Version | Date       | Changes |
|---------|------------|---------|
| v1.0.0  | 2026-04-20 | Initial draft (based on Specify v1.1.0) |
| v2.0.0  | 2026-04-27 | Full rewrite. Realigned to Specify v2.1.1. Removed v1.0.0 defects (cross-feature trespass, direct restatement of implementation details, Plan-stage origination of structural decisions absent from the Spec). Required-coverage locations for RQ-14/15/16 are pinned in §1-3 and concretized in each section. §1–§11 authored and applied. |

> **v1.0.0 body archival:** To be preserved separately under an Archive subpage (tracked as GBL-002 — major-bump previous-body retention policy). Bulk archival happens after v2.0.0 review completes.

---

## Current Version: v2.0.0

## 1. Overview

### 1-1. Purpose of this Plan

This Plan unwinds the requirements (WHAT) defined by Specify v2.1.1 for ReplayScene into **how to implement them (HOW)**.

- WHAT belongs to Specify, HOW to Plan, and "in what order shall which files be created" to Tasks.
- Plan is a code-coherence judgment area, but **Plan v2.0.0 does not treat ReplayScene's existing implementation code as an investigation target.** Coherence verification is performed in a separate stage after Plan v2.0.0 is finalized.
- Information about external Feature code (GameContext, EvolutionTreeScene, etc.) uses only facts secured through external investigation results.

### 1-2. Specify v2.1.1 → Plan v2.0.0 Mapping

| Specify section | Plan section | Treatment |
| --- | --- | --- |
| §1 Overview | §1 Overview | Scope of responsibility |
| §2 Entry and Exit | §7-1 entry flow / §8-1 entry-info extension | Data-passing structure + extension interfaces |
| §3-1 Top guidance UI | §4 folder structure / §5 ReplayGuidanceView | Separate component |
| §3-2 Tree area + RQ-14 | §2-1 / §4 / §9 Core/Tree usage strategy | Reuse 4 Core/Tree components + place ReplayScene-specific enum under Core/Tree |
| §3-3 Node-state classification (3 kinds + dedicated enum + separate mapping values) | §5 ReplayNodeState definition / §5 ReplayUseCase classification logic / §9 visual mapping | Dedicated enum + classification logic + mapping |
| §3-4 Option button | §5 ReplayView | OptionButtonView composition (G-05) |
| §4-1 ~ §4-3 Node tap / popup / 2-step confirm | §7-2 node tap / §7-3 restart confirmation | Presenter flow |
| §4-4 RQ-15 Popup separate operation | §4 folder structure / §5 ReplayNodeDescriptionPopupView | ReplayScene-specific popup definition |
| §5-1 ~ §5-3 Reset responsibility / selected-node application / extension structure | §7-3 / §8-2 / §5 IRestartFlow | UseCase responsibility split + extension-point interface |
| §5-4 RQ-16 Reset consistency | §5 IRestartFlow responsibility / §10 consistency-guarantee design | Consistency-guarantee mechanism |
| §5-5 Failure handling | §10 Fail Fast / network-error separation | Failure-handling flow |
| §6 Scene-transition external configuration | §8-3 / §5 IPostRestartSceneRouter | Externally injected interface |
| §7 Other-feature preceding work | §2-3 preceding-Patch application assumption | Assumed already applied |
| §8 First-pass scope and future extension | §8 extension-structure design overall | Extension-point shape decisions |
| §9 RQ-01 ~ RQ-16 | Whole Plan | Decisions distributed across sections |
| §10 OQ-01 ~ OQ-03 | §11 Open Questions | OQ-01 deferred (Phase 7) + OQ-02/OQ-03 finalized + new OQs |

### 1-3. Where v2.1.0 / v2.1.1 New / Revised Requirements Are Satisfied

This table pre-pins where this Plan satisfies requirements that were newly introduced in Specify v2.1.0 and clarified in v2.1.1. **Cross-check this table when authoring each section.**

| Specify new/revised requirement | Satisfaction location in this Plan | Mechanism summary |
| --- | --- | --- |
| RQ-14 (v2.1.1) — share 4 tree components + dedicate enums separately + co-locate them in the same area | §2-1 / §4-1 / §4-2 / §9 | Reuse the 4 Core/Tree components (NodeView, NodeConnectionView, TreeScrollView, TreeLayoutCalculator). Split the existing single NodeState enum into EvolutionNodeState + ReplayNodeState and place both under Core/Tree. No tree-related new code is introduced inside the ReplayScene Feature folder. |
| §3-3 — ReplayScene-specific state enum + separate mapping values | §4-2 ReplayNodeState definition / §5 ReplayPresenter mapping | Define a ReplayScene-only enum with Selectable/Locked/Hidden under Core/Tree. Mapping values (state → visual effect) are defined inside ReplayPresenter as SerializeField. Fully separated from EvolutionTreeScene's enum and mapping. |
| RQ-15 / §4-4 — popup separate operation | §4-1 / §5 ReplayNodeDescriptionPopupView | Define a ReplayScene-specific node-info popup (`ReplayNodeDescriptionPopupView`) in its own folder. Class name, file, and namespace are separated from EvolutionTreeScene's `NodeDescriptionPopupView`. Only the simple-message restart-confirmation popup reuses the common popup (CommonPopupView). |
| RQ-16 / §5-4 — reset consistency | §5-3 IRestartFlow responsibility / §10 consistency-guarantee design | RestartFlow groups all RunData repository resets into a single call unit. The first-pass implementation reuses GameContext.ResetRunForReplayAsync's already-guaranteed bulk-processing structure as-is. Partial-application blocking is defined in §10. |

---

## 2. Dependency Analysis

This section defines how ReplayScene depends externally. **No concrete change to other Features is decided in this Plan;** their already-applied results are taken as preconditions.

**RQs satisfied by this section:** RQ-05 (AccountData read-only), RQ-14 (Core/Tree reuse), partially RQ-16 (reset consistency — leverages GameContext's existing guarantee).

### 2-1. External Features / Systems Depended On

| Dependency | Usage | Notes |
| --- | --- | --- |
| Core/Tree/ — NodeView | Reuse (instantiated as a prefab in the scene) | RQ-14 (one of the 4 shared components) |
| Core/Tree/ — NodeConnectionView | Reuse | RQ-14 (4 shared) |
| Core/Tree/ — TreeScrollView | Reuse (4-direction scroll container) | RQ-14 (4 shared) |
| Core/Tree/ — TreeLayoutCalculator | Reuse (node layout computation) | RQ-14 (4 shared) |
| Core/Tree/ — ReplayNodeState (new) | ReplayScene-only enum, **placed under Core/Tree** | §3-3 (dedicated enum) + RQ-14 (both enums under the same shared tree area). Distinct from EvolutionTreeScene's enum. |
| EvolutionTreeScene Feature | **No reference** | The two scenes do not depend on each other; they share Core/Tree only. |
| GameContext | DI container + orchestration-method calls | Exposes EvolutionNodes / CharacterAccountRepo / SceneNavigator / PopupManager / SpriteLoader / SkillMasterDataRepo / ResetRunForReplayAsync. This Plan adds a new auto-property `PendingReplayContext` on GameContext (decided in §6-1). No other method logic is modified. |
| GameContext.ResetRunForReplayAsync | See §2-2 treatment | Reuse the existing method as-is. Called from ReplayScene through the IRestartFlow abstraction layer. |
| ICharacterRunRepository | Indirect call (RestartFlow → GameContext → Repository) | InitializeNewRun(RunConfigSO, int? overrideEvolutionNodeId). Preceding Patch-003 assumed applied. ReplayScene does not call directly. |
| IInventoryRepository | Indirect call | InitializeNewRun(RunConfigSO). Preceding Patch-001 assumed applied. |
| IShopRepository | Indirect call | InitializeNewRun(RunConfigSO). Signature-unification state assumed. |
| IStageRepository | Indirect call | InitializeNewRun(RunConfigSO). Signature-unification state assumed. |
| ICharacterAccountRepository | Read-only direct call | Query unlocked-node ID collection. Writes forbidden (Specify §5-1, RQ-05). |
| EvolutionNodeSO collection | Read-only reference | Accessed via GameContext.EvolutionNodes property. |
| RunConfigSO | Passed as a parameter to InitializeNewRun | Accessed via GameContext.RunConfig property. ReplayScene does not own it directly. |
| ISceneNavigator | Scene-transition call | SceneKey.Main (first-pass) or externally configurable target. |
| IPopupManager | Popup display call | Node-info popup (dedicated) + restart-confirmation popup (common). |
| ISpriteLoader | Async sprite loading for node icons / frames | Complies with global decision G-21. |
| OptionButtonView | Composition (ReplayScene's own file) | Per global decision G-05. ReplayScene owns its own OptionButtonView under Presentation/. |
| CommonPopupView (common popup) | Reused for the restart-confirmation popup | Simple-message form. Not subject to RQ-15 separate operation. |

### 2-2. Treatment of GameContext.ResetRunForReplayAsync

**Current code state (external investigation):**
- GameContext.ResetRunForReplayAsync(int selectedEvolutionNodeId) is defined (lines 221-253).
- Body: bulk InitializeNewRun calls on CharacterRunRepo / StageRepo / ShopRepo / InventoryRepo + ApplyStatsFromEvolutionNode + SaveAllDataSync.
- Sole caller: ReplaySceneUseCase.ExecuteReplayAsync (line 77).

**Gap analysis vs. v2.1.1 requirements:**

| Specify requirement | Does the current GameContext implementation satisfy it? |
| --- | --- |
| RQ-16 (reset consistency — memory + persistence as a unit) | Satisfied — bulk handling of 4-repo InitializeNewRun + SaveAllDataSync |
| §5-3 (structure to allow injection of additional initialization logic) | Not satisfied — inserting additional steps requires modifying GameContext.cs |
| §6 (scene-transition external configuration) | Irrelevant — scene transition is the Presenter's responsibility |

**Plan v2.0.0 decision:**

GameContext.ResetRunForReplayAsync logic is **kept as-is**, but **a new auto-property `PendingReplayContext` is added on GameContext for inter-scene data passing** (decided in §6-1). On the ReplayScene side, an IRestartFlow abstraction layer is introduced; the structure calls GameContext.ResetRunForReplayAsync through this abstraction.

- ReplayUseCase does **not** call GameContext.ResetRunForReplayAsync directly.
- ReplayUseCase delegates restart execution through the IRestartFlow interface.
- The first-pass implementation `DefaultRestartFlow` is a thin wrapper around GameContext.ResetRunForReplayAsync.
- If additional initialization is needed later, inject another IRestartFlow implementation or extend DefaultRestartFlow.
- For inter-scene data passing, `PendingReplayContext` is exposed on GameContext as a `{ get; set; }` auto-property, mirroring other PendingContext patterns (PendingEndingContext, PendingEventContext). This is included in the formal change scope of the ReplayScene Feature (the same pattern by which other PendingContexts were added to GameContext — see G-09, G-17). Already registered as G-22 in Project Status §2 Global Decisions.

Effects of this structure:
- **RQ-16 satisfied:** the bulk-processing structure already guaranteed by GameContext is reused as-is. DefaultRestartFlow simply delegates without any extra consistency mechanism (detail in §10).
- **§5-3 satisfied:** IRestartFlow abstraction provides an extension structure.
- **GameContext change scope:** adding the PendingReplayContext property is part of the ReplayScene Feature's change scope. The ResetRunForReplayAsync logic itself is not modified — no cross-feature trespass.
- **Existing single-call-site impact:** the existing direct call from ReplaySceneUseCase.ExecuteReplayAsync (line 77) becomes ReplayUseCase's IRestartFlow call. This is a body change inside the ReplayScene Feature, not a GameContext change. The ReplayScene Feature change is within the scope this Plan v2.0.0 defines.

### 2-3. Assumption That 3 Preceding Patches Are Already Applied

This Plan assumes the following are already-applied facts (see Status > Preceding work info):

- CharacterRunRepository.InitializeNewRun(RunConfigSO, int? overrideEvolutionNodeId = null) signature is available
- CharacterRunData.LastRunResult field exists (None / GameOver / Ending)
- IInventoryRepository.InitializeNewRun(RunConfigSO) signature unification is done
- EndingScene → SceneKey.Replay scene transition is formally restored (EndingPresenter.HandleRestart line 148, confirmed by external investigation)
- LastRunResult is saved within EndingScene.CompleteEnding()

### 2-4. When External-Feature Code-State Confirmation Is Needed

If an external Feature's current code state must be confirmed during Plan authoring, **request confirmation from Hak together with the command.** This Plan v2.0.0 author does not directly query external code.

Additional investigation result reflected at §4 authoring time: the EvolutionTreeScene-side NodeState enum currently exists in `Assets/_Game/Core/Tree/NodeState.cs` as a single enum that fuses EvolutionTreeScene-only values and ReplayScene-only values. Splitting is handled in §4-2.

---

## 3. Layer-wise Responsibility Allocation (Clean Architecture)

This section allocates ReplayScene code by layer per Constitution §3.

**RQs satisfied by this section:** RQ-05 (AccountData read-only), RQ-09 (Selectable/Locked/Hidden, 3 kinds), RQ-12 (no back), RQ-14 (4 shared components + dedicated enums + co-located area), RQ-15 (popup separate operation), §3-3 (dedicated enum + separate mapping values).

### 3-1. Data Layer

ReplayScene defines no new data structures.

- Data read: CharacterRunData.LastRunResult (defined by preceding Patch-003), AccountData.UnlockedEvolutionNodeIds (existing), EvolutionNodeSO collection (MasterData)
- Data written: none. On restart execution, IRestartFlow → GameContext.ResetRunForReplayAsync → underlying Repository.InitializeNewRun resets RunData (indirectly).
- New SaveData: none
- New SOs: none

### 3-2. Domain Layer

| Component | Location | Responsibility | RQ / Specify items satisfied |
| --- | --- | --- | --- |
| `IReplayUseCase` | Features/ReplayScene/Domain | Domain-action interface | Constitution §3 (interface separation) |
| `ReplayUseCase` | Features/ReplayScene/Domain | (1) Query unlocked-node list (2) Classify node states (3) Orchestrate restart execution (delegates to IRestartFlow) (4) Request scene transition (delegates to IPostRestartSceneRouter) | RQ-09, RQ-13 |
| `PendingReplayContext` (mutable) | Features/ReplayScene/Domain | Inter-scene data-passing container. EndingScene sets, ReplayScene consumes (first-pass: LastRunResult; future-extensible). Exposed on GameContext as `{ get; set; }` (§6). | RQ-03, RQ-04 (Specify §2-2 extension structure) |
| `Core/Tree/ReplayNodeState` (enum) | **Core/Tree area** | ReplayScene-specific node-state classification (Selectable / Locked / Hidden) | §3-3 (dedicated enum) + RQ-14 (Core/Tree placement). Distinct from EvolutionTreeScene's enum. |
| `IRestartFlow` (extension-point interface) | Features/ReplayScene/Domain | Extension point for injecting additional initialization logic during restart | RQ-07, Specify §5-3 |
| `DefaultRestartFlow` | Features/ReplayScene/Domain | First-pass implementation. Thin wrapper around GameContext.ResetRunForReplayAsync. | RQ-16 consistency is already guaranteed by GameContext |
| `IPostRestartSceneRouter` (extension-point interface) | Features/ReplayScene/Domain | Decides scene-transition target after restart | RQ-08, Specify §6 |
| `DefaultPostRestartSceneRouter` | Features/ReplayScene/Domain | First-pass implementation. Returns SceneKey.Main as a constant. | — |

**Responsibility boundaries:**
- ReplayUseCase **does not directly call scene transitions.** Delegates to IPostRestartSceneRouter (§8-3).
- ReplayUseCase **does not directly call Repository.InitializeNewRun.** Delegates to IRestartFlow (§8-2). RQ-16 consistency guarantee is the IRestartFlow implementation's internal responsibility.
- For first-pass, only the basic implementations DefaultRestartFlow and DefaultPostRestartSceneRouter are registered.

### 3-3. Presentation Layer

| Component | Location | Responsibility | RQ / Specify items satisfied |
| --- | --- | --- | --- |
| `ReplaySceneBootstrapper` | Features/ReplayScene/Presentation | Instantiate and inject all Domain/Presentation dependencies on scene entry | Constitution §3, §8 |
| `ReplayPresenter` | Features/ReplayScene/Presentation | Mediates View ↔ UseCase. Handles node tap / option button / restart confirmation. No direct UI manipulation. | Constitution §3 |
| `ReplayView` | Features/ReplayScene/Presentation | Composition of tree area / top guidance area / top-right option button + event emission | RQ-12, Specify §3-4 |
| **Core/Tree/NodeView** (reused) | Core/Tree | Per-node visual representation | RQ-14 (4 shared). No new addition on the ReplayScene side. |
| **Core/Tree/NodeConnectionView** (reused) | Core/Tree | Connection-line visuals | RQ-14 (4 shared) |
| **Core/Tree/TreeScrollView** (reused) | Core/Tree | 4-direction scroll container | RQ-14 (4 shared) |
| **Core/Tree/TreeLayoutCalculator** (reused) | Core/Tree | Layout computation | RQ-14 (4 shared) |
| `ReplayNodeDescriptionPopupView` | Features/ReplayScene/Presentation/Popup | ReplayScene **dedicated** node-info popup. Different content per state (Selectable/Locked/Hidden). Class name / file / namespace are separated from EvolutionTreeScene's `NodeDescriptionPopupView`. | RQ-15, Specify §4-4 (separate operation) |
| `ReplayGuidanceView` | Features/ReplayScene/Presentation | Non-closable, top-fixed guidance UI | Specify §3-1 |
| Restart-confirmation popup | App/Popup (reused) | Reuses common popup (CommonPopupView). Simple-message form (title/text/confirm/cancel). | Not subject to RQ-15 separate operation (simple message) |

**State → visual-effect mapping (RQ-14 + Specify §3-3):**

Specify §3-3 requires "use the same mapping structure as EvolutionTreeScene + define ReplayScene-specific enum and mapping values separately." RQ-14 requires "share 4 tree components + define enums per scene + co-locate both enums under the shared tree area." A way to satisfy both:

- **Enum definition location:** Core/Tree/ReplayNodeState (RQ-14 — same area)
- **Enum-value usage scope:** ReplayScene processes only its own enum's values (Selectable/Locked/Hidden). EvolutionTreeScene processes only its own (Current/Evolvable/Reachable/Locked/Hidden). The two enums are entirely separate.
- **Visual-effect mapping values:** ReplayScene-side mapping values are defined inside ReplayPresenter (as SerializeField or code constants). EvolutionTreeScene-side mapping is inside EvolutionTreePresenter (per §9-4 C1 self-mapping decision — external investigation showed it currently lives in the NodeView's internal switch but moves into EvolutionTreePresenter as part of the §9-3 NodeView signature change).

This satisfies both §3-3's "ReplayScene-specific mapping values defined separately" and RQ-14's "co-locate both enums under the shared tree area."

---

## 4. Folder Structure

This section specifies where the files needed for ReplayScene implementation are placed. Per-file responsibilities were defined in §3, so this section addresses **placement only**. Implementation handling such as how a NodeView accommodates the split enums is dealt with separately in §9 (Core/Tree usage strategy).

**RQs satisfied by this section:** RQ-14 (Core/Tree placement), RQ-15 (popup separate operation reflected in folder structure).

### 4-1. ReplayScene Feature Folder Structure

    Assets/_Game/Features/ReplayScene/
    ├── Domain/
    │   ├── IReplayUseCase.cs
    │   ├── ReplayUseCase.cs
    │   ├── PendingReplayContext.cs
    │   ├── IRestartFlow.cs
    │   ├── DefaultRestartFlow.cs
    │   ├── IPostRestartSceneRouter.cs
    │   └── DefaultPostRestartSceneRouter.cs
    └── Presentation/
        ├── ReplaySceneBootstrapper.cs
        ├── ReplayPresenter.cs
        ├── ReplayView.cs
        ├── ReplayGuidanceView.cs
        ├── OptionButtonView.cs
        └── Popup/
            └── ReplayNodeDescriptionPopupView.cs

**Convention application result (external investigation — synthesis of 4 Features' conventions):**
- Flat Domain/ structure (majority convention — EvolutionTreeScene/Ending/Event/CharacterInfoScene all flat)
- Flat Bootstrapper/Presenter/View at the top of Presentation/ + a Popup/ subfolder
- No Data/ — ReplayScene defines no persistent data of its own (same pattern as EvolutionTreeScene/CharacterInfoScene)
- No MasterData/ — ReplayScene defines no SOs of its own

**Class-name convention (external investigation — majority):**

All four Features (EvolutionTreeScene/Ending/Event/CharacterInfoScene) use **the "Scene" suffix only on Bootstrapper / no "Scene" suffix on Presenter/UseCase/View** pattern. This Plan adopts that majority convention as the formal naming.
- `ReplaySceneBootstrapper` (Bootstrapper retains "Scene")
- `ReplayUseCase` / `ReplayPresenter` / `ReplayView` (others without "Scene")

The renaming of existing partial ReplayScene implementation code (`ReplaySceneUseCase` / `ReplayScenePresenter` / `ReplaySceneView`) is handled as an implementation directive in the Tasks stage.

**OptionButtonView placement:**

By global decision G-05, ReplayScene also owns its own OptionButtonView. EvolutionTreeScene puts BackButtonView + OptionButtonView under TopBar/, but per RQ-12 ReplayScene has no back button, so only the OptionButtonView component exists. Creating a TopBar/ folder for a single file is over-segmentation, so it is placed directly under Presentation/.

**ReplayGuidanceView placement:**

A single component for "non-closable fixed guidance UI" per Specify §3-1; in the first-pass stage no extra component will join the guidance area. Placed directly under Presentation/. When tutorials etc. add components later, splitting into a Guidance/ folder is appropriate.

**Rationale for the absence of Data/:**

ReplayScene defines no new data structures (Repository / SaveData / SO) (§3-1). The Constitution §4 folder guideline is read as "place Data/ when there's a Repository implementation and SaveData"; since there are zero such assets, this Feature does not even create a Data/ folder.

**Rationale for the absence of MasterData/:**

ReplayScene defines no new SOs (§3-1).

### 4-2. ReplayScene-related Items in the Core/Tree Area

    Assets/_Game/Core/Tree/
    ├── NodeView.cs                   # existing — RQ-14 (4 shared), reused by ReplayScene
    ├── NodeConnectionView.cs         # existing — RQ-14 (4 shared)
    ├── TreeScrollView.cs             # existing — RQ-14 (4 shared)
    ├── TreeLayoutCalculator.cs       # existing — RQ-14 (4 shared)
    ├── EvolutionNodeState.cs         # changed — separates the existing NodeState.cs into the EvolutionTreeScene-only enum
    ├── ReplayNodeState.cs            # new — ReplayScene-only enum
    └── Prefabs/
        ├── TreeNode.prefab           # existing
        └── TreeNodeConnectionView.prefab  # existing

**Current state vs. this Plan's result:**

Per investigation, currently `Assets/_Game/Core/Tree/NodeState.cs` is a single file with one enum that fuses 6 values (Current/Evolvable/Reachable/Locked/Hidden/Selectable). Specify v2.1.1 §3-3 / RQ-14 require separate enums per scene. **Splitting is required per this Plan v2.0.0.**

Result of the split:
- `EvolutionNodeState` (EvolutionTreeScene-only): Current / Evolvable / Reachable / Locked / Hidden
- `ReplayNodeState` (ReplayScene-only): Selectable / Locked / Hidden

`Locked` and `Hidden` carry the same meaning across scenes but are **defined as distinct values of distinct enums** (Specify v2.1.1 RQ-14 — two enums defined separately). The fact that the meanings match is addressed in §9 when designing how the shared component (NodeView) receives both enums.

**File-name decision:** The existing `NodeState.cs` file is renamed to `EvolutionNodeState.cs`, not left as-is. Reason: with the enum split, having one enum per file is clearer for both sides. The name `NodeState.cs` does not express "for which scene," obscuring the split intent.

**Files affected by the enum split (per investigation):**

- `Assets/_Game/Core/Tree/NodeView.cs` — affects the `SetNodeState(NodeState)` signature. The design of how NodeView accommodates the split enums is defined in §9.
- `Assets/_Game/Features/EvolutionTreeScene/Domain/EvolutionTreeUseCase.cs` — `NodeState` references → change to `EvolutionNodeState`
- `Assets/_Game/Features/EvolutionTreeScene/Presentation/EvolutionTreePresenter.cs` — same
- ReplayScene-side UseCase / Presenter are fully rewritten per this Plan v2.0.0, so under the new naming (`ReplayUseCase` / `ReplayPresenter`) they reference `ReplayNodeState`.

This section states the impact scope as factual information only. Concrete task decomposition is defined in the Tasks stage.

### 4-3. New Item under Scenes/

    Assets/_Game/Scenes/Replay.unity

**Current state:** investigation shows `Assets/_Game/Scenes/Replay.unity` does not exist.

**Basis:** Defined as "scene" in Specify §1. SceneKey.Replay is invoked from EndingPresenter.HandleRestart as a result of preceding Patch-005 (assumed in §2-3). Build Settings registration + scene-file creation are handled as Manual Work in the Tasks stage.

### 4-4. MasterData / Resources / SO Areas

**No new items.** ReplayScene defines no SOs of its own and accesses the EvolutionNodeSO collection read-only via GameContext.EvolutionNodes (§2-1).

### 4-5. New / Changed File Counts by Area

This table compares the Plan v2.0.0 "final state" against the current state (where partial ReplayScene implementation already exists). The Tasks stage defines concrete decomposition into new creation / rename / move / content change.

| Area | New | Changed | Notes |
| --- | --- | --- | --- |
| Features/ReplayScene/Domain/ | 6 | 1 | New: IReplayUseCase + PendingReplayContext + IRestartFlow / DefaultRestartFlow + IPostRestartSceneRouter / DefaultPostRestartSceneRouter / Changed: existing ReplaySceneUseCase.cs → renamed to ReplayUseCase.cs + content rewritten |
| Features/ReplayScene/Presentation/ | 2 | 4 | New: ReplayGuidanceView + OptionButtonView / Changed: ReplaySceneBootstrapper.cs (rewritten, name preserved) + ReplayScenePresenter.cs → ReplayPresenter.cs (rename + rewrite) + ReplaySceneView.cs → ReplayView.cs (rename + rewrite) + Popup/NodeDescriptionPopupView.cs → Popup/ReplayNodeDescriptionPopupView.cs (rename + rewrite) |
| Core/Tree/ | 1 | 1 | New: ReplayNodeState.cs / Changed: NodeState.cs → renamed to EvolutionNodeState.cs + enum reduced to 5 values |
| Core/Tree/NodeView.cs | — | 1 | Signature / internal-handling change driven by the enum split (concrete change defined in §9) |
| Scenes/ | 1 | — | Replay.unity (Manual Work) |
| MasterData / Resources / SO | 0 | 0 | n/a |
| **Totals** | **10** | **7** | — |

---

## 5. Core Class Design

This section defines, for each class identified in §3's responsibility table, **signatures + dependencies + internal logic specifications**. If §3 is the What of responsibility, this section is the per-class How.

This section covers **signatures and internal logic specs**. Scenario call flows (the order in which methods are invoked) are in §7; Bootstrapper dependency-injection order is in §6.

**RQs satisfied by this section:** RQ-03/04 (entry info + extension structure), RQ-05 (AccountData read-only), RQ-07 (extension-point interfaces), RQ-08 (scene-transition external configuration), RQ-09 (3-kind classification), RQ-13 (no visual distinction for default evolution form), RQ-15 (popup separate operation).

### 5-0. Pre-decisions

- **Node ID type (option A decided):** Inside ReplayScene, unify to string. Since EvolutionNodeSO.NodeId is string and CharacterAccountData.UnlockedEvolutionNodeIds is List\<string\>, this is the natural domain expression. GameContext.ResetRunForReplayAsync has an int signature, so **the responsibility for string → int conversion is placed in DefaultRestartFlow** (the conversion point closest to GameContext).
- **External dependency-type facts (investigation):**
    - `EvolutionNodeSO`: `NodeId` (string), `IsHidden` (bool), `CharacterName` (string), `BaseStats` (CharacterStatsSO), `UnlockConditions` (StatCondition[]), `SkillIds` (int[]), `NodeIconSpriteKey` (string), and other sprite-key fields. **There is no Description field.**
    - `CharacterAccountData.UnlockedEvolutionNodeIds` (List\<string\>)
    - `GameContext`: `EvolutionNodes` (EvolutionNodeSO[]), `CharacterAccountRepo` (ICharacterAccountRepository), `SceneNavigator` (ISceneNavigator), `PopupManager` (IPopupManager), `SpriteLoader` (ISpriteLoader), `RunConfig` (RunConfigSO), `ResetRunForReplayAsync(int) : UniTask`
    - `ISceneNavigator.NavigateToAsync(SceneKey, CancellationToken?) : UniTask`
    - `IPopupManager.ShowYesNoAsync(PopupRequest, CancellationToken?) : UniTask<bool>`
    - `PopupRequest(string title, string message, string confirmText = "Confirm", string cancelText = "Cancel")` constructor used
    - `SceneKey.Replay` / `SceneKey.Main` enum values exist
    - EvolutionTreeScene's `NodeDescriptionPopupView` receives data via a `NodeDescriptionData` struct (stats/condition text composed by the Presenter; Sprites loaded via ISpriteLoader and passed in)

### 5-1. PendingReplayContext (inter-scene data passing)

**Location:** `Assets/_Game/Features/ReplayScene/Domain/PendingReplayContext.cs`

A data container that an external scene (e.g., EndingScene) fills on scene entry and ReplayScene consumes. Consistent with other scenes' PendingContext patterns (PendingEndingContext, PendingEventContext) — exposed on GameContext as a `{ get; set; }` auto-property (mutable object, per investigation).

**Fields (auto-properties):**

| Type | Name | Description |
| --- | --- | --- |
| `LastRunResult` | `PreviousRunResult` | Previous run's termination result. References the enum defined by preceding Patch-003 (None / GameOver / Ending) directly. |

**Design rationale:**
- Adopts the PendingContext pattern — consistent with other scenes (PendingEndingContext, PendingEventContext same shape). Exposed on GameContext as a `PendingReplayContext` auto-property (§6-1 / §6-2).
- **Mutable object.** Other scenes' PendingContext patterns (PendingEndingContext, PendingEventContext) all use mutable auto-properties, so for consistency this class is also mutable. By convention this class is set once on scene entry and not mutated until ReplayScene exits (single-direction consumption).
- First-pass field is a single `PreviousRunResult` (Specify §2-2). Future extensions (reached stage, accumulated karma, etc.) are accommodated by adding new fields (Specify §2-2 / RQ-04).
- Located in the Domain layer (Constitution §4 — pure C# without external dependencies).
- Inside Domain (e.g., ReplayUseCase) the context is used as-is (no separate translation class).

**RQs satisfied:** RQ-03, RQ-04.

### 5-2. ReplayNodeState (Core/Tree-area enum)

**Location:** `Assets/_Game/Core/Tree/ReplayNodeState.cs`

ReplayScene-specific node-state classification. A separate enum from EvolutionTreeScene's `EvolutionNodeState` (Specify RQ-14, §3-3).

**Values:**

| Value | Classification condition |
| --- | --- |
| `Selectable` | Node included in AccountData.UnlockedEvolutionNodeIds |
| `Locked` | Not unlocked + IsHidden == false |
| `Hidden` | Not unlocked + IsHidden == true |

**Design rationale:**
- Specify §3-3 / RQ-14 — ReplayScene-dedicated enum defined separately, placed under Core/Tree.
- Locked / Hidden carry the same meaning as EvolutionTreeScene's EvolutionNodeState same-named values, but are defined as distinct enum values (RQ-14 — two enums defined separately).
- Classification logic itself is the sole responsibility of §5-3 ReplayUseCase.
- The default evolution form is treated as the same Selectable as "any other unlocked node" without special classification (RQ-13).

**RQs satisfied:** RQ-09, RQ-13, RQ-14.

### 5-3. IReplayUseCase / ReplayUseCase

**Location:** `Assets/_Game/Features/ReplayScene/Domain/IReplayUseCase.cs`, `ReplayUseCase.cs`

Domain-action interface + implementation. Core of ReplayScene's Domain layer.

**Interface signatures:**

    IReadOnlyList<EvolutionNodeSO> GetAllNodes()
    ReplayNodeState ClassifyNode(EvolutionNodeSO node)
    UniTask ExecuteRestartAsync(string selectedNodeId, CancellationToken ct = default)

**Ctor dependencies:**
- `ICharacterAccountRepository` — read-only AccountData access
- `EvolutionNodeSO[]` — full node collection (injected from GameContext.EvolutionNodes)
- `IRestartFlow` — restart execution delegate
- `IPostRestartSceneRouter` — delegate that decides scene transition target after restart
- `ISceneNavigator` — performs the scene transition
- `PendingReplayContext` — entry-info container (forwarded to IRestartFlow / IPostRestartSceneRouter on extension)

**Per-method internal logic:**

**GetAllNodes() — return the full node list (for tree rendering):**
1. Expose the constructor-injected `EvolutionNodeSO[]` as `IReadOnlyList<EvolutionNodeSO>`.

**ClassifyNode(node) — classify a single node:**
1. `node == null` → throw `InvalidOperationException` (Constitution §7 Fail Fast).
2. `_accountRepo.AccountData.UnlockedEvolutionNodeIds.Contains(node.NodeId)` → `true` → return `ReplayNodeState.Selectable`.
3. `node.IsHidden == true` → return `ReplayNodeState.Hidden`.
4. otherwise → return `ReplayNodeState.Locked`.

Call efficiency: List.Contains is sufficient at the first-pass node-count scale (~tens). If node count grows, consider HashSet caching (§11 OQ).

**ExecuteRestartAsync(selectedNodeId, ct) — restart orchestration:**
1. `selectedNodeId == null` or empty → throw `InvalidOperationException`.
2. `await _restartFlow.ExecuteAsync(selectedNodeId, _pendingContext, ct)`
3. `var nextSceneKey = _sceneRouter.ResolveNextSceneKey(selectedNodeId, _pendingContext)`
4. `await _sceneNavigator.NavigateToAsync(nextSceneKey, ct)`

**Design rationale:**
- All four §3-2 responsibilities (1)-(4) are cohesively in this class.
- Avoid AccountData exposure — only EvolutionNodeSO[] is exposed via `GetAllNodes()`; AccountData is not exposed directly (RQ-05 — read-only and exposure minimized).
- Restart-execute → scene-transition order: IRestartFlow completes RunData reset, then IPostRestartSceneRouter decides the next scene. Reversing the order risks entering the next scene before the new RunData is persisted.
- Reason `_pendingContext` is held: future extensions may add `PreviousRunResult`-based branches inside IRestartFlow or IPostRestartSceneRouter, so the context is retained. In first-pass it is merely passed through to ExecuteAsync / ResolveNextSceneKey.

**RQs satisfied:** RQ-05, RQ-09, RQ-13.

### 5-4. IRestartFlow / DefaultRestartFlow

**Location:** `Assets/_Game/Features/ReplayScene/Domain/IRestartFlow.cs`, `DefaultRestartFlow.cs`

Restart-execution extension point + first-pass implementation. Realizes Specify §5-3's "structure to inject additional initialization logic."

**Interface signature:**

    UniTask ExecuteAsync(string selectedNodeId, PendingReplayContext pendingContext, CancellationToken ct = default)

**DefaultRestartFlow Ctor dependencies:**
- `GameContext` — target to call `ResetRunForReplayAsync(int)`

**DefaultRestartFlow internal logic:**
1. Convert `selectedNodeId` from string to int (§5-0 decision — conversion responsibility lives in this class).
2. Conversion-failure exceptions naturally throw and propagate (Constitution §7 Fail Fast).
3. `await _gameContext.ResetRunForReplayAsync(intNodeId)`
4. `pendingContext` is unused in the first-pass implementation. Included in the signature for future IRestartFlow implementations to leverage.

**Design rationale:**
- Reason IRestartFlow takes `PendingReplayContext`: future implementations can branch on `pendingContext.PreviousRunResult` (Specify §5-3).
- string → int conversion responsibility lives in this class (§5-0).
- Direct GameContext dependency: calling GameContext.ResetRunForReplayAsync is this class's core responsibility. Constitution §3 forbids "new inside Logic classes + Singleton.Instance access." This class does not access a Singleton — it receives GameContext via constructor injection. Not a violation.
- RQ-16 consistency is already guaranteed by GameContext.ResetRunForReplayAsync (§2-2). This class adds no extra mechanism — it merely delegates. Detail of consistency mechanism is in §10.

**RQs satisfied:** RQ-07, RQ-16 (indirect — leveraging GameContext's guarantee).

### 5-5. IPostRestartSceneRouter / DefaultPostRestartSceneRouter

**Location:** `Assets/_Game/Features/ReplayScene/Domain/IPostRestartSceneRouter.cs`, `DefaultPostRestartSceneRouter.cs`

Extension point that decides the post-restart scene-transition target + first-pass implementation. Realizes Specify §6's "structure for external configuration."

**Interface signature:**

    SceneKey ResolveNextSceneKey(string selectedNodeId, PendingReplayContext pendingContext)

**DefaultPostRestartSceneRouter Ctor dependencies:**
- None

**DefaultPostRestartSceneRouter internal logic:**
1. Ignore `selectedNodeId` and `pendingContext`; return `SceneKey.Main` as a constant.

**Design rationale:**
- Synchronous method (only decides the scene-transition target — actual transition is performed by ReplayUseCase via ISceneNavigator).
- DefaultPostRestartSceneRouter receives selectedNodeId / pendingContext but does not use them in first-pass. Future branches (e.g., if PreviousRunResult is Ending, route via EventScene) can use the same signature (Specify §6 extension structure).
- SceneKey.Main is confirmed to exist in the SceneKey enum (per investigation).

**RQs satisfied:** RQ-08.

### 5-6. ReplaySceneBootstrapper

**Location:** `Assets/_Game/Features/ReplayScene/Presentation/ReplaySceneBootstrapper.cs`

Instantiates and injects all Domain/Presentation dependencies on scene entry. This section covers signatures only; instantiation order / flow are defined in §6 Dependency Injection.

**MonoBehaviour. Lifecycle:**

    private void Start()
    private async UniTaskVoid InitializeAsync()

**SerializeFields:**

| Type | Name | Description |
| --- | --- | --- |
| `ReplayView` | `_replayView` | View component. Assigned via Inspector in the scene. |
| `ReplayPresenter.StateVisualConfig` | `_stateVisualConfig` | Visual-effect mapping values used by ReplayPresenter (Sprite Key based). Edited directly in the Inspector. (Concrete shape in §5-7.) |

**Responsibilities:**
- In `Start()`, call `InitializeAsync().Forget()`.
- The `InitializeAsync()` body is defined in §6 as instantiation-order + DI steps.

**Design rationale:**
- Constitution §3: SceneBootstrapper is a MonoBehaviour using Start().
- Awake() vs Start() — adopts the **majority Start()** convention (per investigation: Ending/Event/CharacterInfo use Start; only EvolutionTree uses Awake. EvolutionTree's Awake is tracked as I-02, deferred). This Plan aligns with the majority convention and the I-02 direction.
- Other than ReplayView, all other components (ReplayGuidanceView, OptionButtonView, ReplayNodeDescriptionPopupView) are owned via SerializeField by ReplayView (§5-8). The Bootstrapper SerializeField only takes ReplayView. The node-info popup does not go through PopupManager — ReplayPresenter calls Show/Hide directly via ReplayView (see §5-9 display-method paragraph).
- PendingReplayContext reception mechanism — consistent with other scenes' PendingContext patterns (per investigation — PendingEndingContext, PendingEventContext same shape). EndingScene sets `gameContext.PendingReplayContext = new PendingReplayContext { PreviousRunResult = ... }` immediately before transitioning to SceneKey.Replay; ReplaySceneBootstrapper reads `gameContext.PendingReplayContext` immediately upon InitializeAsync() entry. Exact flow in §6.

**RQs satisfied:** Constitution §3, §8.

### 5-7. ReplayPresenter

**Location:** `Assets/_Game/Features/ReplayScene/Presentation/ReplayPresenter.cs`

Mediates View ↔ UseCase. No direct UI manipulation (Constitution §3).

**Ctor dependencies:**
- `IReplayUseCase` — Domain calls
- `ReplayView` — View reference
- `IPopupManager` — only for the restart-confirmation popup (CommonPopupView). The node-info popup does not go through PopupManager (see §5-9 display-method paragraph).
- `ISpriteLoader` — async sprite load for node icons (used at popup display time)
- `ISkillMasterDataRepository` — for loading owned-skill icons (used at popup display time. Per investigation, EvolutionTreeScene's NodeDescriptionPopupView uses the SO.SkillIds → SkillMasterData → Sprite-load pattern. Same pattern.)
- `StateVisualConfig` — node-state → visual-effect mapping values (injected from the Bootstrapper SerializeField)

**Nested type — StateVisualConfig (Serializable data container):**

| Type | Name | Description |
| --- | --- | --- |
| `string` | `SelectableFrameSpriteKey` | Frame Sprite Key for Selectable nodes (ISpriteLoader key) |
| `string` | `LockedFrameSpriteKey` | Frame Sprite Key for Locked nodes |
| `string` | `HiddenFrameSpriteKey` | Frame Sprite Key for Hidden nodes |
| `string` | `QuestionMarkSpriteKey` | Sprite Key for replacing the icon on Hidden nodes |

May be defined inside ReplayPresenter or in a separate file. Received via SerializeField on the Bootstrapper and injected into the Presenter's constructor (§5-6). Sprite Keys are async-loaded via ISpriteLoader and converted to Sprites — see §9-5 flow.

**Public method signatures:**

    void Initialize()

**Internal method signatures (event handlers):**

    private void OnNodeTapped(EvolutionNodeSO node)
    private async UniTaskVoid OnRestartRequestedAsync(string selectedNodeId)
    private void OnOptionButtonTapped()

**Per-method internal logic:**

**Initialize() — initialization:**
1. Subscribe to ReplayView's node-tap event: `_view.OnNodeTapped += OnNodeTapped`.
2. Subscribe to ReplayView's option-button-tap event: `_view.OnOptionButtonTapped += OnOptionButtonTapped`.
3. Get the full node list from the UseCase: `var nodes = _useCase.GetAllNodes()`.
4. **Async-load the 4 SpriteKeys in StateVisualConfig via ISpriteLoader** → cache 4 Sprites; for each node, map ReplayNodeState → Frame Sprite + (when Hidden) Icon Sprite → call NodeView's `SetFrameSprite(frame)` + (when Hidden) `SetIconSprite(questionMark)` (§9-5 flow).
5. Actual tree building is delegated by ReplayView to TreeScrollView (§5-8).

**OnNodeTapped(node) — display the popup on node tap:**
1. `var state = _useCase.ClassifyNode(node)` — re-confirm state.
2. Build `ReplayNodeDescriptionData` (see §5-9 data structure). Fields differ by state:
    - `Selectable` → fill all fields (NodeIcon, CharacterName, StatsText, ConditionsText, SkillIcons) + `CanRestart = true`
    - `Locked` → fill only CharacterName + ConditionsText + `CanRestart = false` + `IsHiddenLocked = false`
    - `Hidden` → fill only ConditionsText + `CanRestart = false` + `IsHiddenLocked = true` (CharacterName/StatsText shown as "???" by the popup)
3. Async-load NodeIcon via `await _spriteLoader.LoadSpriteAsync(node.NodeIconSpriteKey)`. For SkillIcons, iterate `node.SkillIds`, query SkillMasterData, async-load each SkillSO's IconSpriteKey via ISpriteLoader. (Concrete loading flow is in §7.)
4. Compose StatsText / ConditionsText with Presenter-internal helpers:
    - StatsText: format `node.BaseStats`'s 4 stats + MaxActionPoints into a displayable string (Specify §3-3 — display-info decision deferred. Exact format finalized at OQ-01 cleanup.)
    - ConditionsText: iterate `node.UnlockConditions` to a displayable string.
5. Call `_view.NodeDescriptionPopupView.Show(data)` to display the popup. Calls the ReplayNodeDescriptionPopupView held as SerializeField by ReplayView directly (§5-9). Does not go through PopupManager.
6. The popup's `OnRestartClicked` event callback invokes `OnRestartRequestedAsync(node.NodeId)`.

**OnRestartRequestedAsync(selectedNodeId) — restart-confirmation popup → execution:**
1. Show the restart-confirmation popup: `var confirmed = await _popupManager.ShowYesNoAsync(new PopupRequest(...))` — PopupRequest message is finalized after OQ-01 cleanup.
2. `confirmed == false` → terminate (user canceled).
3. `confirmed == true` → `await _useCase.ExecuteRestartAsync(selectedNodeId)` — restart execution + scene transition complete inside the UseCase.

**OnOptionButtonTapped() — option-menu invocation:**

The option menu itself is outside the Specify scope (Specify §1 — option-menu content/UI is a separate, unfinished plan). For first-pass, leave this handler as an empty method; fill the body once the option menu plan is finalized.

**Design rationale:**
- View direct manipulation forbidden — all UI changes are delegated via ReplayView method calls (Constitution §3).
- Location of StateVisualConfig — per the §1-3 RQ-14 satisfaction table, "defined inside ReplayPresenter as SerializeField." Received as SerializeField on the Bootstrapper and injected into the Presenter constructor.
- Does not own ISceneNavigator — scene transitions are ReplayUseCase's responsibility (§3-2 ReplayUseCase responsibility (4)).
- The restart-confirmation popup uses IPopupManager.ShowYesNoAsync directly (simple message → reuse CommonPopupView). The node-info popup is a ReplayNodeDescriptionPopupView held as SerializeField by ReplayView, which ReplayPresenter calls Show/Hide on directly — does not go through PopupManager (RQ-15 separate operation; same pattern in EvolutionTreeScene — see §5-9 display-method paragraph).
- StatsText / ConditionsText composition is the Presenter's responsibility — same pattern as EvolutionTreeScene's NodeDescriptionPopupView (per investigation: "stat-text composition logic resides inside the Presenter"). Data (SO) ↔ display (View) separation (Constitution §2 Data/Logic Separation).
- ISkillMasterDataRepository dependency — for loading owned-skill icons. Same pattern is applied in EvolutionTreeScene (per investigation hint).

**RQs satisfied:** Constitution §3, RQ-15 (popup separation), RQ-13 (visual mapping).

### 5-8. ReplayView (MonoBehaviour)

**Location:** `Assets/_Game/Features/ReplayScene/Presentation/ReplayView.cs`

Composition of tree area / top guidance area / top-right option button + event emission.

**SerializeFields:**

| Type | Name | Description |
| --- | --- | --- |
| `TreeScrollView` | `_treeScrollView` | Reused Core/Tree tree container |
| `ReplayGuidanceView` | `_guidanceView` | Top-fixed guidance UI |
| `OptionButtonView` | `_optionButton` | Top-right option button |
| `ReplayNodeDescriptionPopupView` | `_nodeDescriptionPopupView` | Node-info popup placed directly in the scene. Presenter calls Show/Hide directly. Does not go through PopupManager. |

**Public properties:**

| Type | Name | Description |
| --- | --- | --- |
| `TreeScrollView` | `TreeScrollView` | Accessed by Presenter for tree-build commands |
| `ReplayGuidanceView` | `GuidanceView` | Accessed by Presenter for guidance text setting |
| `ReplayNodeDescriptionPopupView` | `NodeDescriptionPopupView` | Accessed by Presenter for direct Show/Hide |

**Public events:**

    event Action<EvolutionNodeSO> OnNodeTapped
    event Action OnOptionButtonTapped

**Reset() Auto-Assignment (Constitution §7):**
- Auto-assign via `GetComponentInChildren<TreeScrollView>()` etc.
- Excluding Transform/RectTransform.

**Methods:**
- Self methods are minimal. Composition-container role.

**Responsibilities:**
- Relay TreeScrollView's emitted node-tap event to this class's `OnNodeTapped`.
- Relay OptionButtonView's emitted tap event to this class's `OnOptionButtonTapped`.

**Design rationale:**
- The View is a composition container + event-relay role. Holds child components (TreeScrollView, ReplayGuidanceView, OptionButtonView) as SerializeFields and exposes them externally.
- No back button (RQ-12). No component like EvolutionTreeScene's BackButtonView.
- ReplayNodeDescriptionPopupView is held as SerializeField by ReplayView. Placed directly in the scene + Presenter calls Show/Hide directly (does not go through PopupManager). Same pattern as EvolutionTreeScene's NodeDescriptionPopupView (per investigation). Only simple-message popups (e.g., restart confirmation) go through PopupManager.

**RQs satisfied:** RQ-12, Constitution §3, §7.

### 5-9. ReplayNodeDescriptionPopupView (MonoBehaviour, Popup)

**Location:** `Assets/_Game/Features/ReplayScene/Presentation/Popup/ReplayNodeDescriptionPopupView.cs`

ReplayScene **dedicated** node-info popup. Class name / file / namespace are separated from EvolutionTreeScene's `NodeDescriptionPopupView` (RQ-15).

**Display method:**
- Held as SerializeField by ReplayView (placed directly in the scene). Does not go through PopupManager.
- ReplayPresenter calls `_view.NodeDescriptionPopupView.Show(data)` / `.Hide()` directly.
- Same pattern as EvolutionTreeScene's NodeDescriptionPopupView (per investigation). Only simple-message-form popups (restart confirmation, etc.) go through PopupManager.

**Modal input blocking (satisfies Specify §4-2 auto-transition blocking):**

When the popup is active, background (tree area, option button, guidance UI, etc.) input must be blocked. This is a general UX standard and serves as the natural mechanism for satisfying Specify §4-2's "tapping another node while the popup is open does not auto-transition."

Implementation approach: apply the same pattern PopupManager uses for CommonPopupView (Dim Image + raycastTarget=true) to this popup. Holds its own Dim background as a child of `_popupRoot`.

EvolutionTreeScene's NodeDescriptionPopupView currently lacks a Dim background and has the same defect (per investigation). This defect is outside ReplayScene Plan v2.0.0's authoring responsibility and will be corrected via a separate Patch on the EvolutionTreeScene side (registered as §11 OQ).

**Input data structure — `ReplayNodeDescriptionData` (defined in the Domain layer or in this file):**

| Type | Name | Description |
| --- | --- | --- |
| `Sprite` | `NodeIcon` | Node icon. Loaded via ISpriteLoader by Presenter and forwarded. |
| `string` | `CharacterName` | EvolutionNodeSO.CharacterName directly. When Hidden, the popup shows "???". |
| `string` | `StatsText` | Composed by Presenter from `EvolutionNodeSO.BaseStats` • `MaxActionPoints`. When Hidden, the popup shows "???". |
| `string` | `ConditionsText` | Composed by Presenter from `EvolutionNodeSO.UnlockConditions`. Always shown (even on Hidden nodes). |
| `Sprite[]` | `SkillIcons` | Composed by Presenter — iterate `EvolutionNodeSO.SkillIds`, query SkillMasterData, load Sprites and forward. Hidden when state is Hidden/Locked. |
| `bool` | `CanRestart` | True only when state is Selectable. Controls visibility of the popup's restart button. |
| `bool` | `IsHiddenLocked` | Same meaning as the same-named field on EvolutionTreeScene's side. Triggers the CharacterName/StatsText/SkillIcons hide branch. |

**EvolutionTreeScene's same-named field (`CanEvolve`) is renamed to `CanRestart` in this ReplayScene side** (RQ-15 — reflect the difference of primary action between the two popups even at the data-structure level).

**SerializeFields (required — display/modal mechanism):**

| Type | Name | Description |
| --- | --- | --- |
| `GameObject` | `_popupRoot` | Popup body GameObject. SetActive(true) on Show, SetActive(false) on Hide. |
| `Image` | `_dimBackground` | Dim background Image. Child of `_popupRoot`. Full-screen RectTransform (anchorMin=(0,0), anchorMax=(1,1), sizeDelta=(0,0)). raycastTarget=true blocks background input. The Dim color RGBA(0, 0, 0, 0.5) used by PopupManager is recommended. |

**Dim-background placement principles:**
- `_dimBackground` is a direct child of `_popupRoot`.
- Its raycastTarget=true intercepts all input over tree area / option button / guidance UI.
- Popup content (icon/name/conditions etc.) is placed above `_dimBackground` in sibling order or as its children, so the user can interact with content.

**SerializeFields (content):**

The component composition is similar to EvolutionTreeScene's NodeDescriptionPopupView (icon Image, character name/stats/conditions Texts, skill-icon container, restart button). Exact spec is finalized in Tasks Manual Work. This Plan does not treat the visual composition as a decision area (analogous to EndingScene Plan-MD §6).

**Public method signatures:**

    void Show(ReplayNodeDescriptionData data)
    void Hide()

**Public events:**

    event Action OnRestartClicked
    event Action OnCloseClicked

**Per-method internal logic:**

**Show(data) — compose the popup content per state:**
1. `_popupRoot.SetActive(true)` — Dim background + content active simultaneously.
2. Set `data.NodeIcon` on the icon Image.
3. Branch on `data.IsHiddenLocked == true` (same pattern as EvolutionTreeScene):
    - CharacterName Text → "???"
    - StatsText Text → "???"
    - ConditionsText Text → display `data.ConditionsText` (conditions are shown even for Hidden — see Specify §4-1 table).
    - SkillIcon container disabled.
    - Restart button disabled / hidden.
4. `data.IsHiddenLocked == false`:
    - Set CharacterName / StatsText / ConditionsText / SkillIcons all from `data` directly.
    - The restart button is enabled/disabled per `data.CanRestart`.

**Hide() — close the popup:**
1. `_popupRoot.SetActive(false)` — Dim + content inactive simultaneously.

**Popup-close triggers:**
- X-button tap → emit `OnCloseClicked` (Specify §4-2).
- Tap outside the `_dimBackground` area → emit `OnCloseClicked` (Specify §4-2 "closes when tapping outside the popup").
- No auto-transition when tapping another node — `_dimBackground.raycastTarget=true` intercepts tree input naturally satisfying Specify §4-2.

**Design rationale:**
- RQ-15 / Specify §4-4 — class / file / namespace are all separated from EvolutionTreeScene's NodeDescriptionPopupView. Same folder pattern (`Presentation/Popup/`), but the class name is prefixed with `Replay` for stronger identification (option B adopted).
- Data structures are also separated — `NodeDescriptionData` (EvolutionTreeScene) and `ReplayNodeDescriptionData` (ReplayScene) defined separately. The primary-action difference (Evolve vs Restart) is reflected in fields (`CanEvolve` vs `CanRestart`) and events (`OnEvolveClicked` vs `OnRestartClicked`).
- The View only displays data it receives. Sprite loading and text composition are Presenter responsibilities (per investigation, same pattern in EvolutionTreeScene).
- The node-info popup emits `OnRestartClicked` → ReplayPresenter subscribes → invokes the restart-confirmation popup (§5-7 OnRestartRequestedAsync).
- **Modal-pattern unification:** apply the same Dim + raycastTarget pattern PopupManager applies to CommonPopupView to this scene-dedicated popup. General UX standard + naturally satisfies Specify §4-2 auto-transition blocking. The Dim absence on EvolutionTreeScene is corrected via a separate Patch (§11 OQ).

**RQs satisfied:** RQ-15, Specify §4-2 (auto-transition blocking naturally satisfied).

### 5-10. ReplayGuidanceView (MonoBehaviour)

**Location:** `Assets/_Game/Features/ReplayScene/Presentation/ReplayGuidanceView.cs`

Non-closable, top-fixed guidance UI. Specify §3-1.

**SerializeField:**

| Type | Name | Description |
| --- | --- | --- |
| `TMP_Text` | `_guidanceText` | Display the guidance message |

**Public method signature:**

    void SetGuidanceText(string text)

**Per-method internal logic:**

**SetGuidanceText(text):**
1. `_guidanceText.text = text`

**Design rationale:**
- Specify §3-1 — same visual form as a popup but with no close action. Simple text container.
- Guidance text itself is OQ-01 in Specify §10 (unfinished). For first-pass the Presenter passes a temporary string via SetGuidanceText. Subject to Phase 7 GBL-001 cleanup.

**RQs satisfied:** Specify §3-1.

### 5-11. OptionButtonView (MonoBehaviour)

**Location:** `Assets/_Game/Features/ReplayScene/Presentation/OptionButtonView.cs`

Top-right option button. Per global decision G-05 (each scene owns its own).

**SerializeField:**

| Type | Name | Description |
| --- | --- | --- |
| `Button` | `_button` | Unity Button component |

**Public event:**

    event Action OnTapped

**Per-method internal logic:**

**Awake / OnEnable:**
1. Add a listener to `_button.onClick` → emit `OnTapped`.

**OnDestroy / OnDisable:**
1. Remove the listener from `_button.onClick` (Constitution §8 Safe Cleanup).

**Design rationale:**
- G-05 — option button is included individually per scene's View. ReplayScene is no exception.
- Simple button + event-emission component. Option-menu display itself is handled by ReplayPresenter (§5-7 OnOptionButtonTapped).

**RQs satisfied:** G-05.

---

## 6. Dependency Injection

This section defines the concrete flow of **how the classes defined in §5 are instantiated and how they are injected into one another.** The body flow of ReplaySceneBootstrapper.InitializeAsync() is the main subject.

If §5 was per-class What-and-How, this section is **scene-entry lifecycle-level How**.

**RQs satisfied by this section:** RQ-03 (entry-info reception), Constitution §3 (Bootstrapper pattern), §6 (GameContext usage), §8.

### 6-1. PendingReplayContext Reception Mechanism

An item deferred at §5-6: how does ReplaySceneBootstrapper obtain the data populated into PendingReplayContext?

**Decision: adopt the PendingContext pattern (consistent with other scenes).**
- Add a new auto-property `PendingReplayContext PendingReplayContext { get; set; }` on GameContext (§6-2 step 0 cross-feature change).
- The EndingScene side, immediately before transitioning to SceneKey.Replay, sets `gameContext.PendingReplayContext = new PendingReplayContext { PreviousRunResult = ... }`.
- Very early in ReplaySceneBootstrapper.InitializeAsync(), read `gameContext.PendingReplayContext` and apply a null guard.
- After consumption, GameContext is **not** reset to null (same pattern as other PendingContexts — Presenter reading is consumption).

**Consistency with other scenes:** With this decision, ReplayScene also follows the PendingContext pattern, consistent with EndingScene/EventScene. The "direct RunData read (option A)" considered earlier was discarded due to a consistency defect against other scene patterns. The PendingContext pattern makes "record immediately before scene entry + consume immediately upon entry" semantics explicit in code; using a different pattern only for ReplayScene would cause confusion in future code reviews.

**Cross-feature change acknowledged:**

This decision corrects §2-2's "GameContext.cs is not modified" decision. Adding the PendingReplayContext auto-property is included in the formal change scope of the ReplayScene Feature (same pattern by which other PendingContexts were added to GameContext — see G-09, G-17). §2-2 body has been corrected this session. Already registered as G-22 in Project Status §2 Global Decisions.

**EndingScene-side change required:**

EndingScene.CompleteEnding() or EndingPresenter.HandleRestart() must, immediately before transitioning to SceneKey.Replay, set `gameContext.PendingReplayContext`. This is issued as an EndingScene-side Patch (Patch-006 expected). This Plan v2.0.0 is outside the authoring responsibility for other Features' Patches; the Patch-006 spec is handled in a separate workflow (registered as §11 OQ).

### 6-2. Body Flow of ReplaySceneBootstrapper.InitializeAsync()

Defined as an ordered list. Each step calls the signatures defined in §5 directly.

**Step 0 (precondition — GameContext-side change):**
- Add the auto-property `public PendingReplayContext PendingReplayContext { get; set; }` to GameContext.cs (§6-1).

**Bootstrapper flow:**

1. **Wait for GlobalBootstrapper initialization:**
    - `await GlobalBootstrapper.Instance.InitializationTask` (majority SceneBootstrapper pattern — per investigation, the same in 4 Features).
2. **Obtain a GameContext reference:**
    - `var gameContext = GlobalBootstrapper.Instance.GameContext`
3. **PendingReplayContext reception + null guard:**
    - `var pendingContext = gameContext.PendingReplayContext`
    - `pendingContext == null` → error handling (Constitution §7 Fail Fast — throw `InvalidOperationException` or log + return). Conforms to other scenes' PendingContext null-guard pattern (per investigation — Ending/Event Bootstrappers both have PendingContext null guards).
4. **Instantiate first-pass extension-point implementations:**
    - `var restartFlow = new DefaultRestartFlow(gameContext)` (§5-4)
    - `var sceneRouter = new DefaultPostRestartSceneRouter()` (§5-5)
5. **Instantiate ReplayUseCase:**
    - `var useCase = new ReplayUseCase(gameContext.CharacterAccountRepo, gameContext.EvolutionNodes, restartFlow, sceneRouter, GlobalBootstrapper.Instance.SceneNavigator, pendingContext)` (§5-3)
    - Note: `SceneNavigator` is referenced directly from GlobalBootstrapper.Instance (per investigation, majority pattern — Ending/Event/CharacterInfo same). GameContext.SceneNavigator is the same object, but the majority pattern is followed for consistency.
6. **Instantiate ReplayPresenter:**
    - `var presenter = new ReplayPresenter(useCase, _replayView, gameContext.PopupManager, gameContext.SpriteLoader, gameContext.SkillMasterDataRepo, _stateVisualConfig)` (§5-7)
    - `gameContext.SkillMasterDataRepo` is of type `ISkillMasterDataRepository` (per investigation — `Assets/_Game/App/GameContext.cs` L81).
    - `_stateVisualConfig` is this Bootstrapper's SerializeField (§5-6).
7. **Start ReplayPresenter initialization:**
    - Call `presenter.Initialize()` (§5-7 synchronous signature).
    - The need for async initialization is reviewed consistently with other scenes at the Phase 7 full review (per Hak agreement).
8. **Retain the reference:**
    - Store `_presenter = presenter` in this Bootstrapper's instance field (kept for the Bootstrapper's lifetime).

### 6-3. SerializeField → Instantiation Mapping

A summary of how the SerializeField dependencies defined in §5 are used in the §6-2 flow.

| Bootstrapper SerializeField | §6-2 step | Use site |
| --- | --- | --- |
| `_replayView` | 6 | ReplayPresenter Ctor parameter |
| `_stateVisualConfig` | 6 | ReplayPresenter Ctor parameter |

The internal SerializeFields of ReplayView (TreeScrollView, ReplayGuidanceView, OptionButtonView) and the internal SerializeFields of ReplayNodeDescriptionPopupView are unrelated to this Bootstrapper — resolved by each component's own Reset() Auto-Assignment or by direct placement in the scene.

### 6-4. PendingReplayContext set Responsibility (External Feature)

This section states the external precondition for ReplayScene's dependency injection. The Patch issued by this decision is handled in another Feature's workflow and is outside this Plan's authoring responsibility.

| Responsible Feature | Change |
| --- | --- |
| EndingScene | EndingScene.CompleteEnding() or EndingPresenter.HandleRestart() sets `gameContext.PendingReplayContext = new PendingReplayContext { PreviousRunResult = ... }` immediately before transitioning to SceneKey.Replay |

The Patch for this change is to be issued as Patch-006 (EndingScene). The exact set location + LastRunResult value (whether to copy `gameContext.CharacterRunRepo.RunData.LastRunResult` directly, or use a value EndingScene knows directly) is finalized at Patch-006 authoring time.

This Plan v2.0.0 defines the §6-2 flow assuming the above external change is applied. Before application, ReplayScene's standalone behavior would have PendingReplayContext null, so step 3 in §6-2 errors out.

### 6-5. Design Rationale Synthesis

- **Adheres to the majority SceneBootstrapper pattern:** per investigation, EndingScene/EventSystem/CharacterInfoScene all follow `await GlobalBootstrapper.Instance.InitializationTask → obtain GameContext reference → instantiate dependencies → create Presenter → Initialize/InitializeAsync`. ReplayScene follows the same pattern.
- **PendingContext pattern consistency:** ReplayScene receives data via PendingReplayContext following the other scenes' pattern. Same as EndingScene/EventScene.
- **GameContext formal change:** adding the PendingReplayContext auto-property is part of ReplayScene Feature's formal change scope (G-22 registered). §2-2 has been corrected accordingly.
- **Where extension-point instances are created:** DefaultRestartFlow / DefaultPostRestartSceneRouter are instantiated directly here in this Bootstrapper as the first-pass implementations. Future extensions replace step 4 of this Bootstrapper with another implementation.
- **External-Feature Patch dependency:** EndingScene must add the PendingReplayContext set logic for ReplayScene to work normally. This external change is stated in §6-4 + issued as a separate Patch (Patch-006).

---

## 7. Scenario Flows

This section organizes the order in which the classes/methods defined in §5 are invoked by user-action units. If §5 is per-class design and §6 is the scene-entry lifecycle flow, this section is **the per-user-interaction flow after entering the scene**.

This section refers to §5 for signatures and internal-logic definitions; only call orders are addressed here. No new methods are introduced — only the signatures defined in §5 are used.

**RQs satisfied by this section:** RQ-09 (3-kind classification), RQ-10 (popup for all states), RQ-11 (2-step confirmation), RQ-12 (no back).

### 7-1. Tree-build Flow on Scene Entry

Triggered immediately after scene entry when ReplaySceneBootstrapper invokes ReplayPresenter.Initialize() in §6-2.

1. Call **ReplayPresenter.Initialize()** (§6-2 step 7).
2. ReplayPresenter subscribes to ReplayView's node-tap / option-button-tap events (§5-7 Initialize steps 1-2).
3. ReplayPresenter calls `_useCase.GetAllNodes()` and receives the full EvolutionNodeSO collection (§5-3 GetAllNodes).
4. ReplayPresenter iterates the collection, invoking `_useCase.ClassifyNode(node)` per node and receiving a ReplayNodeState (§5-3 ClassifyNode).
5. ReplayPresenter maps each node's state to Frame Sprite + (when Hidden) Icon Sprite based on StateVisualConfig (§5-7 Initialize step 4 / §9-5).
6. ReplayPresenter passes the node collection to ReplayView's TreeScrollView; TreeScrollView builds NodeView/NodeConnectionView instances. ReplayPresenter then injects visual representation into each NodeView via `SetFrameSprite` + (when Hidden) `SetIconSprite` (Core/Tree component responsibility + §9-3 signatures).
7. ReplayPresenter sets the guidance text on ReplayView's GuidanceView (calls SetGuidanceText in §5-7 Initialize — first-pass uses an inline literal, tracked by GBL-001).

When this flow completes, the user can scroll the tree freely and view nodes. Node-state classification is performed once on entry; since AccountData is not modified inside ReplayScene, there is no re-classification timing.

### 7-2. Info-popup Display Flow on Node Tap

Triggered when the user taps a single node in the tree (Specify §4-1).

1. NodeView detects tap input → TreeScrollView emits the node-tap event.
2. ReplayView relays TreeScrollView's event as its own `OnNodeTapped` event (§5-8).
3. ReplayPresenter's **OnNodeTapped(node)** handler is invoked (§5-7).
4. ReplayPresenter calls `_useCase.ClassifyNode(node)` to re-confirm the state (§5-7 OnNodeTapped step 1).
5. ReplayPresenter constructs a ReplayNodeDescriptionData per state (§5-7 OnNodeTapped step 2):
    - Selectable → fill all fields + `CanRestart = true`
    - Locked → fill only CharacterName + ConditionsText + `CanRestart = false` + `IsHiddenLocked = false`
    - Hidden → fill only ConditionsText + `CanRestart = false` + `IsHiddenLocked = true`
6. ReplayPresenter async-loads NodeIcon via ISpriteLoader (when Selectable/Locked. When Hidden, may be unused due to the IsHiddenLocked branch).
7. ReplayPresenter async-loads SkillIcon Sprites by combining ISkillMasterDataRepository → ISpriteLoader (only when Selectable).
8. ReplayPresenter composes StatsText / ConditionsText from EvolutionNodeSO data.
9. ReplayPresenter calls `_view.NodeDescriptionPopupView.Show(data)` to display the popup (§5-9). Does not go through PopupManager.
10. ReplayNodeDescriptionPopupView's `_popupRoot.SetActive(true)` activates the Dim background and content simultaneously. Branches on `data.IsHiddenLocked` to display content (§5-9 Show steps 1-4).
11. `_dimBackground.raycastTarget=true` intercepts input over the tree area / option button / guidance UI → the user can only interact with popup content.

**While the popup is active, tapping another node is handled (Specify §4-2 naturally satisfied):**

While the popup is active, tapping a tree node has its input intercepted by `_dimBackground.raycastTarget=true`, so it never reaches NodeView. As a result, the OnNodeTapped handler is not called, and the tree is inactive until the user closes the popup. No extra Presenter guard logic is needed — the modal mechanism naturally satisfies §4-2's "does not auto-transition."

### 7-3. Popup-close Flow

Triggered when the user taps the popup's X button or outside the Dim background (Specify §4-2).

1. ReplayNodeDescriptionPopupView detects the close input (X-button tap or `_dimBackground` area tap) → emits `OnCloseClicked` (§5-9).
2. ReplayPresenter receives the event → calls `_view.NodeDescriptionPopupView.Hide()` → `_popupRoot.SetActive(false)` deactivates Dim + content simultaneously.

Tree-area input becomes active again, and the user can tap another node to restart the §7-2 flow.

### 7-4. Restart-finalize Flow (2-step Confirmation)

Triggered when the user taps the restart button in a Selectable-node popup (Specify §4-3, RQ-11).

**Step 1 — invoke the restart-confirmation popup:**

1. ReplayNodeDescriptionPopupView's restart button is tapped → emits `OnRestartClicked` (§5-9).
2. ReplayPresenter receives the event → invokes the **OnRestartRequestedAsync(selectedNodeId)** handler (§5-7).
3. ReplayPresenter calls `_popupManager.ShowYesNoAsync(new PopupRequest(...))` to display the restart-confirmation popup (§5-7 OnRestartRequestedAsync step 1, reuses CommonPopupView).
4. While the restart-confirmation popup is active, if the user cancels → handler terminates, ReplayNodeDescriptionPopupView state is preserved (still active).

**Step 2 — restart execution + scene transition:**

1. On confirm → ReplayPresenter calls `_useCase.ExecuteRestartAsync(selectedNodeId)` (§5-7 OnRestartRequestedAsync step 3).
2. ReplayUseCase calls `_restartFlow.ExecuteAsync(selectedNodeId, _pendingContext, ct)` (§5-3 ExecuteRestartAsync step 2).
3. DefaultRestartFlow converts selectedNodeId from string to int and calls `_gameContext.ResetRunForReplayAsync(intNodeId)` (§5-4 steps 1-3).
4. GameContext bulk-invokes InitializeNewRun on all RunData repositories (CharacterRunRepo / StageRepo / ShopRepo / InventoryRepo) + ApplyStatsFromEvolutionNode + SaveAllDataSync (§2-2 external investigation).
5. GameContext.ResetRunForReplayAsync completes → DefaultRestartFlow.ExecuteAsync completes → ReplayUseCase proceeds to the next step.
6. ReplayUseCase calls `_sceneRouter.ResolveNextSceneKey(selectedNodeId, _pendingContext)` and obtains the next SceneKey (§5-3 ExecuteRestartAsync step 3).
7. DefaultPostRestartSceneRouter returns SceneKey.Main as a constant (§5-5).
8. ReplayUseCase calls `_sceneNavigator.NavigateToAsync(SceneKey.Main, ct)` to transition to MainScene (§5-3 ExecuteRestartAsync step 4).

**Failure handling:** If an exception occurs in any step above, it propagates per Constitution §7 Fail Fast. Project-level designated errors such as network errors follow that error's own flow (Specify §5-5). Detail in §10.

### 7-5. Option-menu Invocation Flow

Triggered when the user taps the top-right option button (Specify §3-4).

1. OptionButtonView detects tap input → emits `OnTapped` (§5-11).
2. ReplayView relays the event as its own `OnOptionButtonTapped` event (§5-8).
3. ReplayPresenter's **OnOptionButtonTapped()** handler is invoked (§5-7).
4. **In first-pass this is an empty method** — option-menu plan unfinished (§5-7 OnOptionButtonTapped, registered as §11 OQ).

When the option-menu plan is finalized, this handler body is filled in. The "exit game" item inside the option menu becomes one of ReplayScene's exit paths (Specify §2-3).

### 7-6. Hardware Back-key Input Handling

When the user presses Android's hardware back key, the input is ignored (Specify RQ-12).

ReplayScene does not register a back-key handler. Unless Unity defaults or GlobalBootstrapper handles the back key, ReplayScene takes no action. Whether other scenes (e.g., EvolutionTreeScene) handle the back key is outside ReplayScene's responsibility.

### 7-7. Inter-scenario State-flow Summary

Summary of entry/exit states for the 5 scenarios defined in this section.

| Scenario | Entry state | Exit state |
| --- | --- | --- |
| §7-1 Tree build | Immediately after scene entry | Tree displayed + waiting for user input |
| §7-2 Node tap | Waiting for user input | ReplayNodeDescriptionPopupView active (Dim + content) |
| §7-3 Popup close | ReplayNodeDescriptionPopupView active | Waiting for user input (tree displayed) |
| §7-4 Restart finalize | ReplayNodeDescriptionPopupView active (Selectable) | MainScene transition (scene exit) |
| §7-5 Option menu | Arbitrary (tree or popup displayed) | First-pass: no action |

Only §7-4 restart-finalize ends ReplayScene. §7-2/§7-3 are modal flows that return to the same waiting-for-user-input state.

---

## 8. Extension Structure

This section consolidates how the 3 extension structures required by Specify v2.1.1 — RQ-04 (entry info extensible), RQ-07 (additional initialization-logic injection at restart), RQ-08 (scene-transition external configuration) — are satisfied by §5 class design.

If §5 is per-class definitions and §6/§7 are first-pass implementation flows, **this section pins the mechanisms by which parts can change at future-extension time**.

**RQs satisfied by this section:** RQ-03 / RQ-04 (entry-info extension), RQ-07 (restart extension), RQ-08 (scene-transition external configuration), Specify §2-2 / §5-3 / §6 (extension-structure requirements).

### 8-1. Entry-info Extension (RQ-04)

**Current first-pass state:**
- PendingReplayContext has a single field `PreviousRunResult` (LastRunResult) (§5-1).
- ReplayUseCase holds PendingReplayContext as-is and forwards it to IRestartFlow / IPostRestartSceneRouter (§5-3).
- In first-pass, no code actually branches on `PreviousRunResult`. Only the data-passing path is active.

**What changes on extension:**

| Extension scenario | Change location | Form of change |
| --- | --- | --- |
| Add 1 entry-info field (e.g., reached stage) | `PendingReplayContext.cs` | Add a new auto-property |
| Add EndingScene-side set logic | EndingScene Patch issued separately | `gameContext.PendingReplayContext = new PendingReplayContext { newField = ... }` |
| ReplayUseCase-side use | Add branches in IRestartFlow / IPostRestartSceneRouter implementations as needed | See §8-2 / §8-3 |

**What does not change on extension:**
- ReplayUseCase body (forwards PendingReplayContext as-is — branching is on the IRestartFlow / IPostRestartSceneRouter side).
- ReplayPresenter / ReplayView (they don't know about PendingReplayContext — UseCase holds it).
- ReplaySceneBootstrapper (a simple forwarder).

**Design rationale:**
- Adding a new field to PendingReplayContext does not affect existing code (mutable auto-property pattern).
- The signatures (§5-4 / §5-5) where IRestartFlow / IPostRestartSceneRouter take PendingReplayContext are unused in first-pass but pre-arranged for use at extension time.

### 8-2. Restart-execution Additional Initialization-logic Injection (RQ-07)

**Specify §5-3 requirement:** future plans may inject the following kinds of additional initialization at restart time:
- Additional stat/skill adjustments based on previous-run data
- Initial item grants from event outcomes
- Other plan extensions

**First-pass implementation:**
- IRestartFlow interface defined (§5-4): `ExecuteAsync(selectedNodeId, pendingContext, ct)`
- DefaultRestartFlow first-pass implementation: thin wrapper around GameContext.ResetRunForReplayAsync (no extra logic)
- ReplayUseCase delegates to IRestartFlow (`_restartFlow.ExecuteAsync(...)`) — does not call GameContext directly

**What changes on extension:**

| Extension scenario | Change location | Form of change |
| --- | --- | --- |
| Single extra-logic case (e.g., -10% gold if PreviousRunResult is GameOver) | Extend DefaultRestartFlow body | Branch on `pendingContext.PreviousRunResult` + extra processing |
| Multiple-scenario branching (e.g., per entry-path branches) | Author a new IRestartFlow implementation | Separate implementations like `GameOverRestartFlow`, `EndingRestartFlow` |
| Replace first-pass logic entirely | Change ReplaySceneBootstrapper §6-2 step 4 | `var restartFlow = new {NewRestartFlow}(gameContext)` |

**What does not change on extension:**
- IRestartFlow interface signature (already takes PendingReplayContext — no extra parameters needed).
- ReplayUseCase body (only invokes the interface — unaffected by implementation swaps).
- GameContext.ResetRunForReplayAsync (first-pass implementation kept as-is).

**Extension patterns — branch vs composition:**

Two approaches at future-extension time:

| Pattern | Application timing | Example |
| --- | --- | --- |
| **Branch pattern** | Conditional branches inside a single IRestartFlow body | DefaultRestartFlow branches on PreviousRunResult |
| **Composition pattern** | Add a new IRestartFlow implementation | EnhancedRestartFlow calls DefaultRestartFlow + extra processing (Decorator), or a ChainedRestartFlow that runs an IRestartFlow collection sequentially |

The branch pattern fits 1-2 branches. The composition pattern fits many branches or cross-cutting concerns (logging, validation, etc.). Plan v2.0.0 does not enforce a pattern — choose at extension time.

**Design rationale:**
- IRestartFlow's interface accepts a single instance — Bootstrapper's step 4 explicitly instantiates the first-pass implementation (§6-2 step 4). Even when applying the composition pattern, only this step changes.
- IRestartFlow takes PendingReplayContext together — both branch and composition patterns can leverage PreviousRunResult.
- RQ-16 consistency-guarantee responsibility is in GameContext.ResetRunForReplayAsync in first-pass (§10 detail). As long as future IRestartFlow implementations don't bypass GameContext, consistency is automatically maintained.

### 8-3. Scene-transition External Configuration (RQ-08)

**Specify §6 requirement:** future plans may introduce branches that, immediately after entry, route through scenes like EventScene before MainScene. The branching point must not enter ReplayScene's interior — the post-restart scene-transition target must be externally configurable.

**First-pass implementation:**
- IPostRestartSceneRouter interface defined (§5-5): `ResolveNextSceneKey(selectedNodeId, pendingContext) → SceneKey`
- DefaultPostRestartSceneRouter first-pass implementation: returns SceneKey.Main as a constant
- ReplayUseCase delegates to IPostRestartSceneRouter to obtain the next SceneKey, and performs the transition itself via ISceneNavigator

**What changes on extension:**

| Extension scenario | Change location | Form of change |
| --- | --- | --- |
| Single branch (e.g., if PreviousRunResult is Ending, route via EventScene) | Extend DefaultPostRestartSceneRouter body | Branch on `pendingContext.PreviousRunResult` |
| Multiple branches (e.g., per evolution-form scenes) | Extend DefaultPostRestartSceneRouter body | Branch on `selectedNodeId` |
| Per-scenario separate Routers | New IPostRestartSceneRouter implementation + change Bootstrapper §6-2 step 4 | `var sceneRouter = new {NewSceneRouter}()` |

**What does not change on extension:**
- IPostRestartSceneRouter signature (takes both selectedNodeId + pendingContext — no extra parameters needed).
- ReplayUseCase body (just transitions via ISceneNavigator using the Router result).
- ReplaySceneBootstrapper step 5 (ReplayUseCase instantiation — receives sceneRouter as a parameter).

**What "ReplayScene external configuration" means:**
- This interface does not mean "ReplayScene-external code decides the post-restart scene transition"; it means **inside ReplayScene's Domain layer, the decision responsibility is separated into IPostRestartSceneRouter**.
- In future scenarios where an external Feature actually decides the SceneKey (e.g., the event system decides the next scene), a separate IPostRestartSceneRouter implementation that depends on the external system is authored.

**Design rationale:**
- IPostRestartSceneRouter is a synchronous method (only decides scene transition — actual transition is via ISceneNavigator).
- By separating decision logic from transition execution, the Router implementation owns "where to" only; "how" (scene-transition Navigator) is performed by ReplayUseCase.
- DefaultPostRestartSceneRouter receives selectedNodeId and pendingContext but does not use them in first-pass. This signature can be reused for future branches (Specify §6 extension structure).

### 8-4. Summary Mapping of Extension Structures

Mapping of the 3 extension structures from first-pass ↔ future extension.

| RQ / Specify | Extension mechanism | First-pass state | Future-extension change unit |
| --- | --- | --- | --- |
| RQ-04 (entry-info extension) | PendingReplayContext mutable auto-properties | A single PreviousRunResult field | Add new fields + external set logic (EndingScene Patch) |
| RQ-07 (restart additional initialization) | IRestartFlow abstraction + DefaultRestartFlow | Thin GameContext wrapper (no extra logic) | Extend DefaultRestartFlow body or add a new implementation + change Bootstrapper instantiation |
| RQ-08 (scene-transition external configuration) | IPostRestartSceneRouter abstraction + DefaultPostRestartSceneRouter | Returns SceneKey.Main as a constant | Extend DefaultPostRestartSceneRouter body or add a new implementation + change Bootstrapper instantiation |

All 3 extension areas are designed to extend **without changing the skeletons of ReplayUseCase / ReplayPresenter / ReplayView / Bootstrapper**. Change units are limited to interface implementations or 1-2 lines in Bootstrapper instantiations.

### 8-5. Design Rationale Synthesis

- **All 3 extension points are abstracted as Domain-layer interfaces** (Constitution §3 — Domain is pure C# without external dependencies). Domain-action changes are possible without Presentation-layer changes.
- **Default* implementation pattern:** for first-pass, place a basic implementation that only guarantees simple behavior; replace with another at extension time. Since the interface always exists, even first-pass code visibly chooses the "default" implementation explicitly — extension intent is conveyed in code.
- **PendingReplayContext is shared by all extension points:** since both IRestartFlow and IPostRestartSceneRouter receive PendingReplayContext, future entry-info-based branches can be added on either interface. Two separated responsibility areas, but the context is shared.
- **Explicit separation of changing vs unchanging areas:** the intent of explicitly tabulating "what changes" / "what does not change" is to agree, at Plan time, on "what to touch / what not to touch" at future-extension time.

---

## 9. Core/Tree Usage Strategy

This section defines the Core/Tree component-usage strategy so that RQ-14 (4 tree components shared + dedicated enums + co-located area) aligns with NodeView's actual behavior. §4-2 provides the fact of enum splitting and §5 provides per-class signatures; this section addresses **the design of how the shared component (NodeView) accommodates two domain enums simultaneously**.

This section takes as factual the externally-confirmed current NodeView implementation (sprite-swap approach, switch branching, two-step `SetUISprites + SetNodeState` call pattern) and decides how this implementation aligns with RQ-14's "share" concept.

**RQs satisfied by this section:** RQ-14 (4 shared + dedicated enums + co-located area).

### 9-1. Facts About NodeView's Current Implementation

The starting facts for this section's decisions.

| Item | Fact |
| --- | --- |
| Visual representation | **Sprite swap** (not color change) |
| Frame Sprite kinds | 6 (Current / Evolvable / Reachable / Locked / Hidden / QuestionMark) |
| Sprite supply path | Presenter async-loads 6 via ISpriteLoader → bulk-injects into NodeView via `SetUISprites(...)` |
| State → Sprite mapping location | NodeView internal switch (L51-79) |
| Mapping trigger | Presenter calls `SetNodeState(NodeState state)` |
| `NodeState` definition location | `Core/Tree/NodeState.cs` (single enum, 6 fused values) |
| `NodeState.Selectable` handling | No case in the switch → only icon resets, no frame change (defect) |
| Separate mapping object | None (no StateVisualConfig or similar) |

These facts essentially conflict with §4-2's enum-splitting decision: the two split enums (`EvolutionNodeState` 5 / `ReplayNodeState` 3) cannot both use the same `SetNodeState(NodeState)` signature.

### 9-2. NodeView Responsibility Redefinition

Among 4 options for NodeView to accommodate two split enums simultaneously, **option 4 (enum externalization)** is adopted.

**Adopted option — NodeView does not know the enums and only receives Sprites:**

| Change item | Before | After |
| --- | --- | --- |
| NodeView responsibility | State (enum) → Sprite branching + Sprite swap | Sprite swap only |
| Mapping responsibility location | NodeView internal switch | Inside each Presenter |
| Domain NodeView is aware of | NodeState (defined fused under Core/Tree) | None (does not know domain enums) |

**Rejected options and reasons:**

| Option | Reason |
| --- | --- |
| Option 1 (Generic `SetNodeState<T>`) | Per-enum branches remain inside NodeView. Generic constraints cannot abstract away the branching. |
| Option 2 (overloads) | NodeView learns about both domain enums. Adding a new scene requires NodeView changes (violates RQ-14's share essence). |
| Option 3 (common interface) | C# enums cannot implement interfaces — wrapper classes required (over-abstraction). |

**Reason for adopting option 4:**
- Satisfies Core/Tree's "share" essence — NodeView knows neither domain enum. Adding a new scene does not require NodeView changes.
- Single Responsibility — NodeView's responsibility simplifies to "Sprite swap." Mapping is domain knowledge and naturally lives in the domain side (Presenter).
- Naturally aligns with §4-2's enum-splitting decision.

### 9-3. NodeView Signature Change (B1 — Wholesale Change)

Per option 4, NodeView's public signature is changed wholesale.

**Before (current — `Core/Tree/NodeView.cs`):**

    public void SetUISprites(Sprite current, Sprite evolvable, Sprite reachable,
        Sprite locked, Sprite hidden, Sprite questionMark)
    public void Setup(string nodeId, Sprite icon)
    public void SetNodeState(NodeState state)

**After:**

    public void Setup(string nodeId, Sprite icon)
    public void SetFrameSprite(Sprite frameSprite)
    public void SetIconSprite(Sprite iconSprite)

**Summary of changes:**

| Change item | Treatment |
| --- | --- |
| `SetUISprites(6 sprites)` | **Removed** — NodeView no longer takes 6-sprite pre-injection. The Presenter forwards a single mapped result. |
| `SetNodeState(NodeState)` | **Removed** — enum branching disappears from NodeView. |
| New `SetFrameSprite(Sprite)` | Replaces the Frame Image's sprite with the argument. Enum-agnostic. |
| New `SetIconSprite(Sprite)` | Used to swap to "?" icon on Hidden nodes. Restoring the default icon also uses this. |
| `Setup(nodeId, icon)` | Kept — node ID and one default icon are injected. |

**NodeView internal changes:**
- Remove the 6 `private Sprite _xxxFrameSprite` fields.
- Remove the L51-79 switch body.
- Replace with simple assignments like `_frameImage.sprite = frameSprite`.
- The `_originalIcon` field can be retained for storage at Setup time (or moved to external control via SetIconSprite — Tasks-stage decision).

**EvolutionTreePresenter accompanying change (cross-feature impact):**
- Existing: `nodeView.SetUISprites(6 sprites) → nodeView.SetNodeState(state)` pattern.
- After: the Presenter decides one frame Sprite via its own mapping (§9-4) → `nodeView.SetFrameSprite(frame)` + when Hidden `nodeView.SetIconSprite(questionMark)`.
- This change is not an additional cross-feature trespass: §4-2's enum-splitting decision already explicitly entailed an EvolutionTreePresenter change, so this is within the RQ-14 change scope.

**Existing call-site impact:**

| Call site | Impact |
| --- | --- |
| EvolutionTreePresenter L82-86 (external investigation) | SetUISprites + SetNodeState calls → wholesale replacement with new methods |
| ReplayScenePresenter L81-83 (external investigation) | The `Selectable → Evolvable` workaround is naturally removed by ReplayPresenter rewrite per Plan v2.0.0 |

### 9-4. State → Sprite Mapping Responsibility Location (C1 — each Presenter Maps Itself)

With option 4 adopted, the "enum → Sprite" mapping is performed by each Presenter. C1 self-mapping is adopted.

**ReplayPresenter mapping (first-pass):**

ReplayNodeState (3 values) → 1 Frame Sprite:

| ReplayNodeState | Frame Sprite |
| --- | --- |
| Selectable | Selectable-only Frame Sprite |
| Locked | Locked Frame Sprite |
| Hidden | Hidden Frame Sprite |

Additionally, when Hidden, swap the Icon Sprite to QuestionMark. When Selectable / Locked, keep the node's original icon.

For this mapping, ReplayPresenter holds **3 Frame Sprites + 1 QuestionMark Sprite = 4 Sprite Keys** as SerializeField/Config. Concrete shape is defined in §9-5.

**EvolutionTreePresenter mapping (redefined — cross-feature impact):**

EvolutionNodeState (5 values) → 1 Frame Sprite:

| EvolutionNodeState | Frame Sprite |
| --- | --- |
| Current | Current Frame Sprite |
| Evolvable | Evolvable Frame Sprite |
| Reachable | Reachable Frame Sprite |
| Locked | Locked Frame Sprite |
| Hidden | Hidden Frame Sprite |

When Hidden, swap the Icon Sprite to QuestionMark.

EvolutionTreePresenter holds 5 Frame Sprites + 1 QuestionMark Sprite = 6 Sprite Keys (the same assets currently injected into NodeView via SetUISprites — only the holding location moves from NodeView to Presenter).

**Rejected option — C2 common helper (`StateSpriteResolver<T>`):**

| Reason |
| --- |
| Generics ill-suited to enum branching (same reason as option 1's rejection) |
| The two enums have different value counts (3 vs 5) — a common mapping interface abstraction is awkward |
| Mapping is itself domain knowledge — abstraction cost outweighs benefit |

**Reason for adopting C1 self-mapping:**
- Two enums are different domains — generalization produces awkward abstractions.
- Each Presenter explicitly holds its own domain mapping — better code readability.
- Mapping changes only impact the corresponding Presenter.

### 9-5. ReplayScene Visual-effect Mapping Structure (§5-7 Correction)

Per the decisions in §9-2 / §9-3 / §9-4, the `StateVisualConfig` definition in §5-7 is corrected.

**Before correction (target of correction):**

| Type | Name | Description |
| --- | --- | --- |
| `Color` | `SelectableTint` | Selectable node tint |
| `Color` | `LockedTint` | Locked node tint |
| `Color` | `HiddenTint` | Hidden node tint |

**After correction:**

Define the visual-effect mapping data structure ReplayPresenter holds as `StateVisualConfig` (Serializable data container). Sprite-based, not color-based.

| Type | Name | Description |
| --- | --- | --- |
| `string` | `SelectableFrameSpriteKey` | Selectable node Frame Sprite Key (ISpriteLoader key) |
| `string` | `LockedFrameSpriteKey` | Locked node Frame Sprite Key |
| `string` | `HiddenFrameSpriteKey` | Hidden node Frame Sprite Key |
| `string` | `QuestionMarkSpriteKey` | Sprite Key for replacing the Hidden node icon |

**Sprite Key vs direct Sprite — reason for adopting Sprite Key:**
- EvolutionTreePresenter currently uses SpriteKeys and async-loads via ISpriteLoader (external investigation — iterates `_uiSpriteKeys[0~5]`). ReplayScene adopts the same pattern for consistency.
- Direct-Sprite holding would allow Inspector assignment but bypasses the ISpriteLoader pattern (Addressables, etc.) — reduces consistency with global decision G-21.

**ReplayPresenter Initialize flow reinforcement (reflecting §9-3 signatures):**
1. Subscribe to ReplayView events (no change).
2. Call `_useCase.GetAllNodes()` (no change).
3. **Async-load the 4 SpriteKeys in StateVisualConfig via ISpriteLoader** → cache 4 Sprites.
4. For each node, call `_useCase.ClassifyNode(node)` → receive `ReplayNodeState`.
5. Map ReplayNodeState → Frame Sprite + (when Hidden) Icon Sprite via the Presenter's internal mapping (§9-4).
6. Call NodeView's `SetFrameSprite(frame)` + (when Hidden) `SetIconSprite(questionMark)` (post-change signatures).

**§5-7 OnNodeTapped flow is unaffected by this section:**
- OnNodeTapped is the popup-display flow on node tap — unrelated to Sprite mapping.
- The §5-7 OnNodeTapped body is unchanged.

### 9-6. Stating EvolutionTreeScene-side Impact

**This Plan v2.0.0's responsibility area and EvolutionTreeScene-side changes stated:**

The §9-3 wholesale change to NodeView signatures + §9-4 EvolutionTreePresenter mapping ownership change entail code changes in the EvolutionTreeScene Feature. This Plan v2.0.0 does not author EvolutionTreeScene Feature's Spec/Plan/Tasks, but since RQ-14 essentially entails changes to a "shared component," these changes are formally part of this Plan v2.0.0's results.

**EvolutionTreeScene accompanying change items:**

| Change item | Change location |
| --- | --- |
| Reorganize the `_uiSpriteKeys` array into a named structure such as EvolutionStateVisualConfig | EvolutionTreePresenter (recommendation, not mandatory) |
| Replace SetUISprites + SetNodeState calls with SetFrameSprite + SetIconSprite calls | EvolutionTreePresenter L82-86 |
| Change L121-122 / L157 NodeState comparisons to EvolutionNodeState comparisons | EvolutionTreePresenter |
| Change ClassifyNodeState() return type from NodeState to EvolutionNodeState | EvolutionTreeUseCase L102 |

The concrete decomposition of these changes is handled in ReplayScene Tasks Manual Work or in a separate EvolutionTreeScene Patch (registered as §11 OQ).

### 9-7. Final Shape of the Core/Tree Area

Aggregate result of §4-2 / §9-3 decisions, the Core/Tree area's final state.

    Assets/_Game/Core/Tree/
    ├── NodeView.cs                   # changed — enum-agnostic, SetFrameSprite/SetIconSprite/Setup signatures
    ├── NodeConnectionView.cs         # unchanged
    ├── TreeScrollView.cs             # unchanged (instantiation responsibility only)
    ├── TreeLayoutCalculator.cs       # unchanged
    ├── EvolutionNodeState.cs         # new — result of splitting the existing NodeState.cs (5 values)
    ├── ReplayNodeState.cs            # new — ReplayScene-only enum (3 values)
    └── Prefabs/
        ├── TreeNode.prefab           # unchanged
        └── TreeNodeConnectionView.prefab  # unchanged

**Resulting responsibilities of the Core/Tree area:**
- **NodeView:** "Sprite swap + node ID storage + click-event emission" — knows neither domain enum.
- **NodeConnectionView:** connection-line visuals (unchanged).
- **TreeScrollView:** 4-direction scroll + NodeView instantiation + lookup (unchanged).
- **TreeLayoutCalculator:** node layout computation (unchanged).
- **EvolutionNodeState / ReplayNodeState:** per-domain enum definitions — placed under Core/Tree, but the 4 Core/Tree components do not use them.

**Meaning of "co-located area" (RQ-14 naturally satisfied):**

RQ-14 requires both enums to be "placed under the same shared tree area." By this decision, both enums live under `Core/Tree/`. Components like NodeView do not use the enums themselves, but co-locating the enums preserves the area-meaning of "the collection of tree-domain enums."

### 9-8. Design Rationale Synthesis

- **Core meaning of adopting option 4:** redefining NodeView as a "domain-agnostic component" guarantees Core/Tree's share essence at the code level. When a new scene uses the tree, NodeView changes are not required.
- **B1 wholesale change:** the §4-2 enum split alone causes a compile error against the SetNodeState(NodeState) signature, making incremental change (B2) impractical. A single wholesale change is superior in both consistency and work efficiency.
- **C1 self-mapping:** the two domain enums have different value sets + mapping is domain knowledge. A common helper would be over-abstraction.
- **§5-7 correction is naturally entailed:** once external investigation confirmed NodeView is sprite-based, a color-based StateVisualConfig cannot align with the actual code. It is natural that §9's decisions entail the §5-7 correction.
- **Formal acknowledgment of EvolutionTreeScene accompanying changes:** since RQ-14 itself targets the "shared component" for changes, EvolutionTreeScene-side changes are not cross-feature trespass by ReplayScene Feature but a formal result of satisfying RQ-14.

---

## 10. Consistency / Failure Handling

This section organizes how RQ-16 (reset consistency) and Specify §5-5 (failure handling) are guaranteed across §5 class design and §7 scenario flows. §2-2 / §5-3 / §5-4 / §7-4 covered the normal flow; **this section covers behavior and consistency mechanisms when the normal flow breaks**.

This section introduces no new classes or methods — it organizes the failure modes of signatures defined in §5.

**RQs satisfied by this section:** RQ-16 (reset consistency), Specify §5-4 / §5-5 (consistency + failure handling), Constitution §7 (Fail Fast).

### 10-1. Reset Consistency Mechanism (RQ-16)

**Specify §5-4 requirement:** at restart execution, the 4 in-memory RunData repositories (ICharacterRunRepository, IInventoryRepository, IShopRepository, IStageRepository) and the persistence layer must be reset as a unit. No partial-application state — with only some repositories reset or persistence partially failed — may occur.

**Guarantee location — GameContext.ResetRunForReplayAsync (first-pass):**

Per §2-2 external investigation, GameContext.ResetRunForReplayAsync handles the following as a unit inside a single method body:
1. CharacterRunRepo.InitializeNewRun(RunConfig, overrideEvolutionNodeId)
2. StageRepo.InitializeNewRun(RunConfig)
3. ShopRepo.InitializeNewRun(RunConfig)
4. InventoryRepo.InitializeNewRun(RunConfig)
5. ApplyStatsFromEvolutionNode(selectedEvolutionNodeId)
6. SaveAllDataSync()

The 6 steps run synchronously inside one method (when InitializeNewRun is synchronous) or sequentially via await, with SaveAllDataSync called last. If any step throws, the whole method exits via throw to the caller (DefaultRestartFlow → ReplayUseCase).

**ReplayScene-side responsibility:**

ReplayScene does not add its own consistency mechanism. DefaultRestartFlow only thinly wraps GameContext.ResetRunForReplayAsync, with no extra steps or validation logic (§5-4). Consistency-guarantee responsibility is GameContext's single responsibility, and ReplayScene only relies on the assumption "calling GameContext guarantees consistency."

**Partial-application blocking mechanism:**

| Mechanism | Provided by |
| --- | --- |
| Bulk-process the 4 repositories via a single method call | GameContext.ResetRunForReplayAsync |
| Bulk persistence (SaveAllDataSync) | Last step of GameContext.ResetRunForReplayAsync |
| Whole-method throw on exception | C# default behavior (no try-catch) |
| No additional caller-side processing | DefaultRestartFlow's simple delegation |

**Memory-state vs persistence-state consistency:**

GameContext processes "memory RunData update → persistence" in order, so on normal completion memory == persistence. By the location of the exception:

| Exception location | Memory state | Persistence state |
| --- | --- | --- |
| During Repository.InitializeNewRun (steps 1-4) | Partial memory state with only some repositories reset | Unchanged (previous run data preserved) |
| During ApplyStatsFromEvolutionNode (step 5) | 4 repositories have new run data; only stats missing | Unchanged (previous run data preserved) |
| During SaveAllDataSync (step 6) | New run data complete | Some persistence succeeded or failed |

In any case, ReplayScene propagates the thrown exception as-is (Constitution §7 Fail Fast). From the user's perspective, the next scene transition does not occur and the user remains in ReplayScene (the §7-4 flow is interrupted by throw before step 8, so NavigateToAsync is never invoked).

**Partial-application recovery is outside Plan v2.0.0:**

Recovery mechanisms (e.g., transactional rollback, backup-data restore) for the "partial memory state" / "partial persistence" rows above are GameContext's responsibility and are not added on the ReplayScene side. In first-pass, the probability of partial-application is judged "very low" (Repository.InitializeNewRun-fail scenarios are limited to extremes like out-of-memory), so no extra mechanism is introduced. When persistence reliability becomes important in the future, GameContext-side transactional mechanisms are reviewed (registered as §11 OQ).

### 10-2. Failure Classification

**Specify §5-5 requirement:** failures in ReplayScene are classified into two kinds.
- General failures (Constitution §7 Fail Fast applies)
- Project-level designated errors (network errors, etc. — follow that error's own flow)

**ReplayScene-side failure classification:**

| Failure type | Trigger | Handling |
| --- | --- | --- |
| PendingReplayContext null | §6-2 step 3 (immediately after Bootstrapper InitializeAsync entry) | Throw InvalidOperationException — Constitution §7 Fail Fast |
| EvolutionNodeSO collection empty | §7-1 step 3 (tree build) | Show empty tree + wait for user input (no throw — missing data is a build-stage responsibility) |
| Async sprite load failure | §7-1 step 5 / §7-2 steps 6-7 | Propagate ISpriteLoader's throw — Constitution §7 Fail Fast |
| ClassifyNode called with null node | §7-1 step 4 / §7-2 step 4 | Throw InvalidOperationException (§5-3 ClassifyNode step 1) |
| ExecuteRestartAsync called with empty selectedNodeId | §7-4 step 1 of step 2 | Throw InvalidOperationException (§5-3 ExecuteRestartAsync step 1) |
| string→int conversion failure for selectedNodeId | §7-4 step 3 of step 2 (inside DefaultRestartFlow) | FormatException or OverflowException naturally throws — Constitution §7 Fail Fast |
| Exception inside GameContext.ResetRunForReplayAsync | §7-4 step 4 of step 2 | Propagate the exception GameContext throws |
| Scene-transition failure (exception inside NavigateToAsync) | §7-4 step 8 of step 2 | Propagate the exception ISceneNavigator throws |
| Network error (project-level designated error) | Possible at any step | Follow that error's own flow — ReplayScene adds nothing |

**Fail Fast application principles:**
- Nowhere in ReplayScene's Domain layer do we absorb exceptions with try-catch.
- Same in the Presenter layer — exceptions thrown from a UniTaskVoid method (`OnRestartRequestedAsync`) are delegated to the .Forget() pattern or UniTask's own handling mechanism.
- If an exception must be transformed into a user-visible form (error popup), Plan v2.0.0 classifies that as a global error-handler responsibility — no transformation on ReplayScene side.

**Project-level designated errors — network-error separate handling:**

Specify §5-5 specifies that project-level designated errors such as network errors follow that error's own flow. Plan v2.0.0 uses these assumptions:
- The first-pass ReplayScene has no network dependencies (all data is local SaveData / SO).
- If ISpriteLoader uses network-based Addressables, it relies on ISpriteLoader's own error handling (retry, fallback).
- If GameContext.ResetRunForReplayAsync's SaveAllDataSync includes cloud sync, it relies on that sync logic's own error handling.

ReplayScene does not add code to recognize and branch on network errors. When network-dependent features (e.g., server validation on restart) are added in the future, separate handling logic is reviewed (§11 OQ).

### 10-3. User-side Impact and UX

**User perspective on Fail Fast outcomes:**

| Failure location | User screen change | Next action |
| --- | --- | --- |
| Bootstrapper entry (PendingReplayContext null) | ReplayScene screen does not appear or tree is empty | Option menu or forced exit (option menu unimplemented in first-pass) |
| Tree-build failure | Empty tree or error screen | Wait for user input |
| Node-tap → popup-display failure | Popup does not appear | User can tap another node |
| Restart-finalize → ResetRunForReplayAsync failure | After the restart-confirmation popup closes, no scene transition | Stay in ReplayScene; user can select another node and retry |
| Restart-finalize → scene-transition failure (RunData reset succeeded) | RunData is in new-run state but the scene remains ReplayScene | If the user retries, another reset occurs from the already-new-run state. No data corruption |

**UX limits in first-pass:**

In the "restart-finalize failure" cases above, no explicit error message is shown to the user. In first-pass, only console logs remain unless a global error handler intervenes. This is recognized as a first-pass ReplayScene scope limit, improved when Phase 7 or global error handling is introduced (§11 OQ).

### 10-4. UseCase / Presenter Responsibility Boundaries (Failure-handling Side)

**ReplayUseCase has no try-catch:**

§5-3 ExecuteRestartAsync runs awaits sequentially without try-catch. If any step throws, the whole method exits via throw, propagating to the caller (ReplayPresenter.OnRestartRequestedAsync). This is Constitution §7 Fail Fast applied formally.

**ReplayPresenter has no try-catch:**

§5-7 OnRestartRequestedAsync also awaits without try-catch. UniTaskVoid method exception handling is delegated to UniTask's own mechanism (.Forget() pattern, etc.).

**Maintaining the "no direct UI manipulation" principle:**

Even on failure, ReplayPresenter / ReplayUseCase do not call methods like "show error popup" on ReplayView. If user-side error-message display is added in the future, choose between:
- A global error handler displays via a common method like IPopupManager.ShowErrorAsync.
- Add an ErrorView component on the ReplayScene side (separate operation as in RQ-15).

In first-pass, neither is applied; relies on global handling or console logs.

### 10-5. Design Rationale Synthesis

- **Consistency responsibility cohered in GameContext:** per §2-2 decision, GameContext.ResetRunForReplayAsync has the single responsibility for RQ-16 consistency. ReplayScene does not introduce additional mechanisms (transaction, rollback, partial-application blocking). Future consistency strengthening goes via GameContext changes.
- **Formal adoption of Fail Fast:** nowhere in ReplayScene's Domain / Presentation layers do we absorb exceptions with try-catch. Exceptions propagate up the call stack and are finally handled by the global error handler or UniTask's own mechanism. Constitution §7 applied as-is.
- **Network-error separation deferred to first-pass:** since ReplayScene has no direct network dependency in first-pass, no network-error handling logic is added. ISpriteLoader's or SaveAllDataSync's internal network handling is each system's responsibility.
- **Explicit acknowledgment of UX limits:** "no explicit user-side error display on restart-finalize failure" is an intended first-pass limit. Improved when global error handling is introduced (registered as §11 OQ).
- **Partial-application blocking deferred to GameContext:** if ReplayScene implements its own "all 4 repositories or nothing" guarantee, responsibility duplicates with GameContext. Plan v2.0.0 acknowledges GameContext's single responsibility.

---

## 11. Open Questions

This section registers, for tracking, items identified during Plan v2.0.0 authoring that are not decided in this Plan or whose decisions are deferred. Combines Specify §10's OQ-01 ~ OQ-03 results + items newly identified during this Plan's authoring.

Each OQ specifies the handling timing, scope, related RQ, and related Plan section. **OQs are not Plan v2.0.0 decision areas** — they have appropriate handling paths such as Tasks / Patch / Phase 7 / Project Backlog.

### 11-1. Specify v2.1.1 OQ Handling Results

Results of handling Specify §10 OQ-01 ~ OQ-03 at this Plan's time of authoring.

| Specify OQ ID | Topic | Handling at this Plan's time |
| --- | --- | --- |
| OQ-01 | UI text (guidance message, popup messages) | **Deferred — tracked as Phase 7 GBL-001 accumulation.** In first-pass, ReplayPresenter / ReplayNodeDescriptionPopupView / restart-confirmation popup all use Korean inline literals. ReplayScene's new occurrences are registered under Project Backlog GBL-001 (system prompt §2 "no UI-visible text hardcoding" grace policy applies). |
| OQ-02 | External-configuration timing (scene-transition external configuration) | **Finalized — introduced in first-pass as the IPostRestartSceneRouter abstraction.** Decisions in §5-5 / §8-3. DefaultPostRestartSceneRouter returns SceneKey.Main as a constant, but the interface signature takes PendingReplayContext + selectedNodeId, prepared for future extension. |
| OQ-03 | Additional-initialization-logic injection timing | **Finalized — introduced in first-pass as the IRestartFlow abstraction.** Decisions in §5-4 / §8-2. DefaultRestartFlow is a thin wrapper around GameContext.ResetRunForReplayAsync, but the interface signature takes PendingReplayContext, prepared for future extension. |

Only OQ-01 remains deferred to Phase 7. OQ-02 / OQ-03 are finalized in this Plan v2.0.0 by introducing extension-point interfaces.

### 11-2. Plan v2.0.0 New OQs — Organized by Handling Path

Items newly identified during this Plan's authoring.

#### 11-2-1. Tasks-stage Handling Items

Handled with concrete task decomposition at Tasks authoring time.

| OQ ID | Topic | Related Plan section | Tasks-stage handling direction |
| --- | --- | --- | --- |
| OQ-P1 | EvolutionTreeScene-side cross-feature change task decomposition | §9-3 / §9-6 | NodeView signature change (remove SetUISprites + SetNodeState → add SetFrameSprite + SetIconSprite) + EvolutionTreePresenter mapping ownership change + EvolutionTreeUseCase.ClassifyNodeState return-type change. Either include in ReplayScene Tasks Manual Work or issue as a separate EvolutionTreeScene Patch — decided at Tasks authoring time. |
| OQ-P2 | NodeView `_originalIcon` field handling | §9-3 | Whether to retain it for storage at Setup time, or fully transition to external control via SetIconSprite — decided in Tasks. |
| OQ-P3 | ReplayNodeDescriptionPopupView visual composition spec | §5-9 | Concrete composition of SerializeField content (icon Image, character name/stats/conditions Texts, skill-icon container, restart button) — finalized in Tasks Manual Work (analogous to EndingScene Plan-MD §6). |
| OQ-P4 | Replay.unity scene-file creation + Build Settings registration | §4-3 | Handled as Manual Work in the Tasks stage. |

#### 11-2-2. Separate Patch Issuance Targets

Items requiring changes in other Features. Outside ReplayScene Plan v2.0.0's responsibility area.

| OQ ID | Topic | Related Plan section | Patch issuance direction |
| --- | --- | --- | --- |
| OQ-X1 | EndingScene PendingReplayContext set logic addition | §6-1 / §6-4 | **EndingScene Patch-006** expected. EndingScene.CompleteEnding() or EndingPresenter.HandleRestart() sets `gameContext.PendingReplayContext = new PendingReplayContext { PreviousRunResult = ... }` immediately before transitioning to SceneKey.Replay. Exact set location + LastRunResult value are finalized at Patch-006 authoring time. |
| OQ-X2 | EvolutionTreeScene NodeDescriptionPopupView Dim missing correction | §5-9 / §9-6 | **EvolutionTreeScene Patch** expected. Currently EvolutionTreeScene's NodeDescriptionPopupView operates without a Dim background, defecting against Specify §4-2's "does not auto-transition while popup is active." Adding Dim background + raycastTarget=true is required. ReplayNodeDescriptionPopupView applies the same pattern from first-pass, restoring consistency between the two scenes. |

#### 11-2-3. Phase 7 / Project Backlog Handling Items

Global work spanning multiple Features or Phase 7 cleanup targets.

| OQ ID | Topic | Related Plan section | Phase 7 / Backlog handling direction |
| --- | --- | --- | --- |
| OQ-G1 | UI-visible text inline-literal accumulation | §5-7 / §5-9 / §5-10 / overall | **Tracked as Project Backlog GBL-001 accumulation.** Register ReplayScene's new occurrences (guidance text, restart-confirmation popup messages, node-info popup "???" markers, option-menu labels). Cleaned up at Phase 7 global UI-text-management system construction. |
| OQ-G2 | Plan v1.0.0 body archival | Plan-KR top notice | **Tracked as Project Backlog GBL-002.** At §11-authoring completion, both Plan-KR / Plan-MD v1.0.0 bodies are archived under an Archive folder. Major-bump (v1.x.x → v2.0.0) previous-body retention policy applies. Temporarily applied until system prompt §14 revision (per userMemories). |
| OQ-G3 | Bootstrapper Awake() vs Start() inconsistency | §5-6 | **Existing tracking item I-02.** Only EvolutionTreeScene uses Awake() (other scenes use Start). Phase 7 batch fix planned. ReplayScene adopts the majority Start() convention, aligned with this tracking item's direction. |
| OQ-G4 | List.Contains efficiency (HashSet caching review) | §5-3 | **Phase 7 review deferred.** ReplayUseCase.ClassifyNode calls `_accountRepo.AccountData.UnlockedEvolutionNodeIds.Contains(node.NodeId)` per call. Sufficient at first-pass node-count scale (~tens), but as node count grows, HashSet caching is reviewed. |
| OQ-G5 | Partial-application recovery mechanism (transactional rollback) | §10-1 | **Deferred to GameContext-side review.** First-pass judges partial-application probability very low. When persistence reliability becomes important, transactional mechanisms in GameContext.ResetRunForReplayAsync are reviewed. Outside ReplayScene Feature responsibility. |
| OQ-G6 | User-side error-message display (global error handler) | §10-3 / §10-4 | **Phase 7 review deferred.** For scenarios needing explicit user-side error display (restart-finalize failure, etc.), introduction of a global error handler is reviewed. Decide between common methods like IPopupManager.ShowErrorAsync or per-scene ErrorView components (separate operation). |
| OQ-G7 | Error-handling logic for added network-dependent features | §10-2 | **Reviewed at future plan time.** ReplayScene has no network dependency in first-pass. When network-dependent features such as server validation on restart are added, separate handling logic is reviewed. |
| OQ-G8 | Option menu plan unfinished | §5-7 / §7-5 | **Phase 7 or separate-plan-finalization handling.** ReplayPresenter.OnOptionButtonTapped is an empty method in first-pass. Body filled when the option menu plan is finalized. Since the "exit game" item inside the option menu becomes one of ReplayScene's exit paths, it also impacts Specify §2-3's exit-path spec. |
| OQ-G9 | Asynchronous Presenter initialization need | §6-2 step 7 | **Phase 7 batch review.** ReplayPresenter.Initialize() is synchronous. When the patterns used by other scenes (EvolutionTree / Ending / Event / CharacterInfo) are reviewed for consistency — Initialize vs InitializeAsync — this is organized together. |

### 11-3. OQ Handling Flow Summary

Map of when and how the OQs registered in this section are handled.

| Handling timing | Target OQs | Form of handling |
| --- | --- | --- |
| **ReplayScene Tasks authoring** | OQ-P1, OQ-P2, OQ-P3, OQ-P4 | Concrete task decomposition + Manual Work specification |
| **Separate Patch issuance** | OQ-X1 (EndingScene Patch-006), OQ-X2 (EvolutionTreeScene Patch) | Handled in other Features' workflows |
| **Project Backlog accumulation** | OQ-G1 (GBL-001), OQ-G2 (GBL-002) | Global-work tracking |
| **Phase 7 batch review** | OQ-G3 (I-02), OQ-G4, OQ-G6, OQ-G8, OQ-G9 | Multi-Feature consistency review |
| **Future plan-finalization timing** | OQ-G5, OQ-G7 | Reviewed when the corresponding feature is introduced |

### 11-4. Design Rationale Synthesis

- **OQs are not deferred decisions but explicit handling-path indications:** every OQ specifies "at which stage and how it is handled." Plan v2.0.0 does not avoid by saying "out of decision area" — rather, it **explicitly draws the boundary between Plan responsibility and other-stage responsibility**.
- **Explicit handling-path branching across Tasks / Patch / Phase 7 / Backlog:** each OQ is clearly tagged as Tasks-stage decomposition, separate Patch issuance, Phase 7 batch review, or Project Backlog accumulation. Prevents future confusion of "where is this item handled."
- **Result of avoiding cross-feature trespass:** OQ-X1 / OQ-X2 being tagged as separate-Patch-issuance targets is the result of Plan v2.0.0 covering only ReplayScene's change scope. EvolutionTreeScene / EndingScene changes belong to those Features' workflows.
- **Project Backlog leverage:** global work like OQ-G1 (UI text) and OQ-G2 (archival) accumulates in Project Backlog. Cleaned up together at Phase 7.
- **Significance of Phase 7 batch review:** items needing "consistency review with other scenes" — OQ-G3 (Bootstrapper Awake/Start), OQ-G9 (Presenter Initialize asynchrony) — are deferred to Phase 7. Independent decision in ReplayScene risks divergence from other scenes.

---
```