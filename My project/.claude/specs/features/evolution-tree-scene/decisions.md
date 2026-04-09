D-01 [DECISION] CharacterRunData field is `EvolutionNodeId` (not `currentEvolutionNodeId` as tasks.md referenced) — used actual field name from CharacterRunData.cs
D-02 [DECISION] EvolutionNodeView uses NodeState enum from Domain layer (EvolutionTreeUseCase.cs) — NodeState is a domain concept shared across Domain and Presentation
D-03 [DECISION] Presenter passes null for Sprite parameters (nodeIcon, skillIcons) — actual sprite loading requires Addressables integration which is Manual Work scope (M-11)
D-04 [DECISION] Used `Awake()` + `InitializeAsync().Forget()` pattern matching CharacterInfoSceneBootstrapper, not `Start()` as tasks.md mentioned — consistency with existing codebase pattern
D-05 [DECISION] EvolutionNodeView의 상태 시각화를 _highlightBorder/_dimOverlay/_evolvableIndicator 3개 개별 오브젝트 방식에서 단일 _frameImage + 5개 상태별 Sprite 교체 방식으로 변경. 각 NodeState(Current/Evolvable/Reachable/Locked/Hidden)에 대응하는 node_frame_*.png 스프라이트를 SerializeField로 보유.
D-06 [DECISION] _nodeIcon.enabled를 icon null 여부에 따라 제어. icon이 null일 때 Image가 흰색 단색으로 렌더링되어 _frameImage를 덮는 문제 수정. Hidden 상태에서 questionMarkSprite 적용 시 enabled = true로 복원.
D-07 [DECISION] 프리팹에서 _nodeIcon 참조를 루트 EvolutionNode의 Image에서 NodeIcon 자식의 Image로 변경. 루트 Image는 Button의 TargetGraphic으로만 사용.
D-08 [DECISION] ScrollToNode를 노드가 뷰포트 정중앙에 오도록 수정. Content가 stretch anchor(0,0)-(1,1)이므로 실제 콘텐츠 크기 = viewportSize + sizeDelta로 계산. 노드의 콘텐츠 내 절대 위치에서 viewport/2를 빼서 normalizedPosition 산출.
D-09 [DECISION] BuildTree에서 노드 Y 위치에 viewportHeight/2 오프셋 적용하여 최상단 노드도 뷰포트 중앙까지 스크롤 가능하게 함. Content pivot이 top-center(0.5, 1)이라 sizeDelta 증가 시 하방으로만 확장되므로, 노드 위치 오프셋으로 상단 패딩을 확보. sizeDelta는 원래 트리 크기(ContentWidth, ContentHeight)만 사용 — 추가 패딩 없이도 stretch anchor 특성상 viewport/2의 자연 여유가 존재하여 가장자리 노드 센터링과 화면 이탈 방지 모두 충족.
