D-01 [DECISION] CharacterRunData field is `EvolutionNodeId` (not `currentEvolutionNodeId` as tasks.md referenced) — used actual field name from CharacterRunData.cs
D-02 [DECISION] EvolutionNodeView uses NodeState enum from Domain layer (EvolutionTreeUseCase.cs) — NodeState is a domain concept shared across Domain and Presentation
D-03 [DECISION] Presenter passes null for skillIcons Sprite — actual sprite loading requires Addressables integration which is Manual Work scope (M-11). nodeIcon은 D-10에서 직접 참조로 전환됨.
D-04 [DECISION] Used `Awake()` + `InitializeAsync().Forget()` pattern matching CharacterInfoSceneBootstrapper, not `Start()` as tasks.md mentioned — consistency with existing codebase pattern
D-05 [DECISION] EvolutionNodeView의 상태 시각화를 _highlightBorder/_dimOverlay/_evolvableIndicator 3개 개별 오브젝트 방식에서 단일 _frameImage + 5개 상태별 Sprite 교체 방식으로 변경. 각 NodeState(Current/Evolvable/Reachable/Locked/Hidden)에 대응하는 node_frame_*.png 스프라이트를 SerializeField로 보유.
D-06 [DECISION] _nodeIcon.enabled를 icon null 여부에 따라 제어. icon이 null일 때 Image가 흰색 단색으로 렌더링되어 _frameImage를 덮는 문제 수정. Hidden 상태에서 questionMarkSprite 적용 시 enabled = true로 복원.
D-07 [DECISION] 프리팹에서 _nodeIcon 참조를 루트 EvolutionNode의 Image에서 NodeIcon 자식의 Image로 변경. 루트 Image는 Button의 TargetGraphic으로만 사용.
D-08 [DECISION] ScrollToNode를 노드가 뷰포트 정중앙에 오도록 수정. Content가 stretch anchor(0,0)-(1,1)이므로 실제 콘텐츠 크기 = viewportSize + sizeDelta로 계산. 노드의 콘텐츠 내 절대 위치에서 viewport/2를 빼서 normalizedPosition 산출.
D-09 [DECISION] BuildTree에서 노드 Y 위치에 viewportHeight/2 오프셋 적용하여 최상단 노드도 뷰포트 중앙까지 스크롤 가능하게 함. Content pivot이 top-center(0.5, 1)이라 sizeDelta 증가 시 하방으로만 확장되므로, 노드 위치 오프셋으로 상단 패딩을 확보. sizeDelta는 원래 트리 크기(ContentWidth, ContentHeight)만 사용 — 추가 패딩 없이도 stretch anchor 특성상 viewport/2의 자연 여유가 존재하여 가장자리 노드 센터링과 화면 이탈 방지 모두 충족.
D-10 [DECISION] EvolutionNodeSO에 _nodeIconSprite(Sprite) 직접 참조 필드 추가. Addressables 미연동 상태에서 노드 아이콘을 표시하기 위함. 기존 _nodeIconSpriteKey(string)는 향후 Addressables 전환용으로 유지. Presenter에서 node.NodeIconSprite를 Setup()에 전달.
D-11 [DECISION] EvolutionNodeView의 _questionMarkSprite를 private → [SerializeField]로 변경하고 SetQuestionMarkSprite() 메서드 제거. 프리팹 Inspector에서 직접 할당 방식으로 단순화.

---

# --- Patch-001 ---

**Date:** 2026-04-29 | **Source:** ReplayScene Plan v2.0.0 §9 / §11 OQ-X2 / OQ-P1 / OQ-P2

NodeState→EvolutionNodeState rename + Selectable 제거 + NodeView wholesale 변경 (enum-agnostic) + EvolutionTreePresenter mapping 책임 이관 + EvolutionTreeUseCase 반환 타입 + NodeDescriptionPopupView Dim 추가.

D-P1-01 [DECISION] OQ-P2 옵션 B 채택 — _originalIcon 필드 제거 + 외부 통제 완전 이관
  - NodeView가 보유하던 _originalIcon (default icon caching) 제거.
  - Setup(string nodeId, Sprite icon) 시 즉시 _nodeIcon.sprite에 반영 (cache 없음).
  - Hidden 상태 진입 시 question mark swap, Hidden 종료 시 원본 복귀는 Caller (Presenter) 책임.
  - EvolutionTreePresenter는 _nodeIconCache (Dictionary<string, Sprite>)로 default icon을 별도 보관 — 향후 Hidden 종료 케이스 발생 시 활용.
  - 1차 EvolutionTreeScene 흐름에서는 Hidden→Other 상태 전환이 발생 안 하므로 functional 영향 없음 (AccountData 수정이 EvolutionTreeScene 내부에서 발생 안 함).

D-P1-02 [DECISION] D-11 무효화 — _questionMarkSprite SerializeField화 반대 방향
  - D-11이 _questionMarkSprite를 NodeView SerializeField로 노출하는 방향이었으나, Patch-001 §9 option 4 채택으로 NodeView가 enum뿐 아니라 default icon 보유 책임도 외부에 위임.
  - _questionMarkSprite 필드 자체 제거. Hidden 상태 시 Presenter가 SetIconSprite(questionMark) 호출.
  - D-11 무효 처리.

D-P1-03 [DECISION] NodeDescriptionPopupView Dim outside-tap-to-close — Button 컴포넌트 패턴
  - Specify §2-4 "팝업 외부 탭 닫힘" 충족 위해 _dimBackground (Image, raycastTarget=true) + _dimBackgroundButton (Button on Dim) 추가.
  - IPointerClickHandler 미사용. Button.onClick → HandleCloseClicked 패턴.
  - ReplayScene 측 ReplayNodeDescriptionPopupView도 동일 패턴 적용 — 두 씬 일관성 회복.

D-P1-04 [DECISION] EvolutionTreePresenter mapping 책임 이관 — Plan §9-4 C1 self-mapping
  - 6 Sprite를 NodeView에 일괄 주입(SetUISprites)하던 구조 제거.
  - Presenter가 6 Sprite instance field로 보유 + MapStateToFrameSprite(EvolutionNodeState) private 메서드로 mapping.
  - foreach 루프 내부: Setup(node.NodeId, iconSprite) → SetFrameSprite(MapStateToFrameSprite(state)) → Hidden 시 SetIconSprite(_questionMarkSprite).
  - C2 공통 helper (StateSpriteResolver<T>) 안 채택 — 두 enum 값 수 다름 (3 vs 5) + mapping 자체가 도메인 지식.

D-P1-05 [DECISION] EvolutionNodeState enum 5값 — Selectable 제거
  - 기존 NodeState 6값 (Current / Evolvable / Reachable / Locked / Hidden / Selectable) 중 Selectable는 ReplayScene 전용이었으나 EvolutionTreeScene 측 ClassifyNodeState는 사용한 적 없음.
  - EvolutionNodeState (5값): Current / Evolvable / Reachable / Locked / Hidden — EvolutionTreeScene 전용.
  - ReplayScene 전용 enum (Selectable / Locked / Hidden) 은 별도 ReplayNodeState로 정의 (ReplayScene Tasks T1).
  - .meta 파일은 git mv로 GUID 보존 정상 처리됨.

BL-P1-01 [BACKLOG] EvolutionTreePresenter._uiSpriteKeys 배열 → named struct 그룹화
  - Plan §9-6 권장 (mandatory 아님) — `EvolutionStateVisualConfig` 같은 named struct로 그룹화하면 가독성 ↑.
  - 본 Patch는 string[] _uiSpriteKeys 구조 유지.
  - Phase 7 또는 별도 Patch에서 검토.
