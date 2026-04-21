# ReplayScene — Decisions

**Feature:** ReplayScene (Phase 6) | **Version:** v1.0.0 | **Date:** 2026-04-21

Record any judgment calls not covered by the Spec here.
Tags: [DECISION], [BACKLOG], [SPEC-GAP]

---

D-01 [BACKLOG] OptionButtonView 크로스-피처 의존성 — ReplaySceneView가 EvolutionTreeScene 네임스페이스 직접 참조
  - ReplaySceneView.cs가 OptionButtonView를 Samsara.Features.EvolutionTreeScene.Presentation.TopBar에서 직접 import.
  - Tasks T13이 "EvolutionTreeView 방식 미러링" 지시만 있을 뿐 OptionButtonView의 네임스페이스 이동 여부는 미정의.
  - ReplayScene ↔ EvolutionTreeScene 간 크로스-피처 의존성 발생.
  - Phase 7에서 OptionButtonView를 Core 또는 공유 레이어로 이동 검토 필요.

BL-03 [BACKLOG] Evolution-node stat application logic duplicated in 3 places
  - GlobalBootstrapper Step 4-A (new-user init path)
  - EvolutionTreeUseCase.ExecuteEvolutionAsync (mid-run evolution)
  - GameContext.ApplyStatsFromEvolutionNode (replay reset, T10)
  - All three apply EvolutionNodeSO.BaseStats → CharacterRunData stats using the same foreach pattern.
  - Phase 7 review for shared utility extraction (Plan RQ-P11, RQ-P15).

BL-04 [BACKLOG] NodeDescriptionPopupView shared-extraction opportunity between EvolutionTreeScene and ReplayScene
  - EvolutionTreeScene: NodeDescriptionPopupView with Evolve button + evolve semantics.
  - ReplayScene: NodeDescriptionPopupView with Restart button + select semantics.
  - Same data displayed (icon/name/stats/skills/conditions), only primary action differs.
  - v1.0.0 keeps them separate per Plan §6-4 (different calling context + button semantics).
  - Post-Phase 7 review for shared Base class or prefab extraction (Plan RQ-P06, RQ-P16).
