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

---

# --- v2.0.0 ---

**Version:** v2.0.0 | **Date:** 2026-04-29 | **Base:** Specify v2.1.1 / Plan v2.0.0 / Tasks v2.0.0

v2.0.0 전면 재작성 결과 잔존 5개 파일 (ReplaySceneUseCase / ReplayScenePresenter / ReplaySceneView / ReplaySceneBootstrapper / Popup/NodeDescriptionPopupView) Rename + rewrite + 신규 12개 파일 + GameContext.PendingReplayContext 추가. 이전 v1.0.0 결정 (D-01 / BL-03 / BL-04) 보존.

D-V20-01 [DECISION] Tasks-MD §1-4 namespace 가정 vs 실제 코드 5건 불일치 — 실제 namespace 적용
  - 출처: Hak 직접 지시 (2026-04-29) — "Tasks는 Claude 웹에서 작성한 것이라 naming이 잘못 설정되어 있을 가능성 높음. 실제 코드로 적용하고 decision 표기."
  - 불일치 항목:
    - `ICharacterAccountRepository` — Tasks 가정 `Samsara.Features.Character.Data`, 실제 `Samsara.Features.Character.Domain`
    - `ISkillMasterDataRepository` — Tasks 가정 `Samsara.Features.Skill.Data`, 실제 `Samsara.Features.Skill.Domain`
    - `SkillSO` — Tasks 미명시, 실제 `Samsara.Core.MasterData`
    - `ISpriteLoader` — Tasks 가정 `Samsara.Core.Sprites`, 실제 `Samsara.Core.AssetLoading`
    - `GameContext` / `GlobalBootstrapper` — Tasks 가정 `Samsara.App` (using 필요), 실제 글로벌 namespace (using 불필요)
  - 모든 신규 / rewrite 파일에 실제 namespace 적용.

D-V20-02 [DECISION] TreeScrollView OnNodeTapped 이벤트 부재 — Tasks-MD §6-7 fallback 적용
  - TreeScrollView 실제 시그니처: `BuildTree(TreeLayoutResult)` / `GetNodeView(string)` / `GetAllNodeViews()` / `ScrollToNode(string)`. `event Action<EvolutionNodeSO> OnNodeTapped` 없음.
  - 노드 클릭 이벤트는 NodeView 본인이 보유 (`event Action<string> OnNodeClicked`, Patch-001 후에도 보존).
  - T13 ReplayView 설계: `TreeScrollView` 프로퍼티만 노출. OnNodeTapped 이벤트는 정의하지 않음.
  - T14 ReplayPresenter 책임: BuildTreeAsync 직후 `_view.TreeScrollView.GetAllNodeViews()` 순회 → 각 NodeView.OnNodeClicked 직접 구독. nodeId 수신 시 `_allNodes` 배열 순회로 EvolutionNodeSO 변환.
  - 동일 패턴이 EvolutionTreePresenter (기존 + Patch-001 후) 에도 사용됨 — 일관성.
  - Tasks-MD §6-7 명시: "If signature is `event Action<string nodeId>` form, ReplayView matches nodeId in the `_evolutionNodes` array, converts to EvolutionNodeSO ... it is more appropriate to place this conversion responsibility in ReplayPresenter."

D-V20-03 [DECISION] ReplaySceneBootstrapper 파일명 retain — majority naming convention
  - T15 만 "Scene" suffix 유지. UseCase / Presenter / View 는 "Scene" suffix 제거 (Plan §4-1).
  - 4개 Feature (EvolutionTreeScene / Ending / Event / CharacterInfoScene) 와 동일 패턴.

D-V20-04 [DECISION] RestartFlow / PostRestartSceneRouter 인터페이스 + Default 구현체 분리 — Plan §5-4 / §5-5
  - `IRestartFlow` + `DefaultRestartFlow` (GameContext.ResetRunForReplayAsync 단순 위임).
  - `IPostRestartSceneRouter` + `DefaultPostRestartSceneRouter` (SceneKey.Main 고정 반환).
  - 1차 개발은 모두 thin wrapper지만, RQ-04 / RQ-07 / RQ-08 (PreviousRunResult 기반 분기 확장) 대비.
  - ReplayUseCase는 두 인터페이스만 의존, 구현체는 ReplaySceneBootstrapper에서 주입.

D-V20-05 [DECISION] EvolutionTreeSceneBootstrapper L15 주석 갱신 — Patch-001 부수 정리
  - Patch-001로 NodeView.SetUISprites 메서드 삭제. EvolutionTreeSceneBootstrapper의 `_uiSpriteKeys` SerializeField 주석 ("Order matches SetUISprites: ...") 이 stale.
  - "Index order consumed by EvolutionTreePresenter._uiSpriteKeys: [0]=current, [1]=evolvable, [2]=reachable, [3]=locked, [4]=hidden, [5]=questionMark" 로 갱신.
  - 코드 동작 영향 없음 — 정합성 유지.

D-V20-06 [DECISION] ReplayUseCase.ClassifyNode은 List<string>.Contains 사용 — LINQ 아님
  - Tasks-MD V-11 명시: "List.Contains is a List method, not LINQ — allowed."
  - v1.x.x 잔존 ReplaySceneUseCase는 foreach 루프 사용했으나 T8 example 따라 `.Contains()` 적용 (clarity).

D-V20-07 [DECISION] ReplayPresenter는 GetAllNodes IReadOnlyList → EvolutionNodeSO[] 변환 후 TreeLayoutCalculator에 전달
  - IReplayUseCase.GetAllNodes() 시그니처는 `IReadOnlyList<EvolutionNodeSO>` (Plan §5-3) — 캡슐화 + 외부 수정 차단.
  - TreeLayoutCalculator.CalculateLayout(EvolutionNodeSO[]) 는 배열 인자만 받음 (기존 시그니처 유지).
  - ReplayPresenter.BuildTreeAsync 내부에서 IReadOnlyList 순회하며 배열로 복사. 1차 노드 수 ~수십 개 수준이라 GC 부담 미미.

D-V20-08 [DECISION] ReplayUseCase는 GameContext 직접 의존 X — DefaultRestartFlow가 GameContext 의존 단일 책임
  - v1.x.x ReplaySceneUseCase는 GameContext 직접 보유 (생성자 주입).
  - v2.0.0 ReplayUseCase: ICharacterAccountRepository / EvolutionNodeSO[] / IRestartFlow / IPostRestartSceneRouter / ISceneNavigator / PendingReplayContext 만 보유.
  - GameContext.ResetRunForReplayAsync 호출은 DefaultRestartFlow 단일 책임으로 이관.
  - Constitution §3 (Logic 클래스 — 생성자 주입) + §4 (Domain Layer pure C#) 더 깊이 준수.

BL-V20-01 [BACKLOG] ReplayScene 신규 UI 텍스트 하드코딩 GBL-001 누적 대상
  - 추적 가능한 // UI text hardcoded: 주석 부착 위치:
    - ReplayPresenter.Initialize — guidance text 1건 ("재시작할 진화체를 선택하세요.")
    - ReplayPresenter.ComposeStatsText — stat 포맷 1건 ("체력 {} 힘 {} 강인함 {} 민첩 {}")
    - ReplayPresenter.ComposeConditionsText — 빈 조건 1건 + 조건 항목 1건 ("해금 조건 없음" / "- {} >= {}")
    - ReplayPresenter.HandleRestartClickedAsync — PopupRequest 1건 ("재시작 확정" / "{}(으)로 새 런을 시작합니다." / "재시작" / "취소")
    - ReplayNodeDescriptionPopupView.Show — Hidden 표시 2건 ("???" CharacterName, StatsText)
    - ReplaySkillDescriptionPopupView.Show — damage 포맷 1건 ("데미지: {:F1}")
  - 총 ReplayScene 신규 7~8건. Phase 7 GBL-001 글로벌 정리 시점에 누적 합산 처리.
