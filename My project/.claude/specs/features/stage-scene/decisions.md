# StageScene — Decisions

D-01 [DECISION] SceneKey.Stage가 이미 존재함 — SceneKey.cs 수정 생략
D-02 [DECISION] StageSceneUseCase는 GameContext에 등록하지 않고 StageSceneBootstrapper에서 직접 생성 — MainScene/MaintenanceScene 패턴과 동일
D-03 [DECISION] GameContext에 StageRepo (IStageRepository) public accessor 추가 — StageSceneBootstrapper에서 UseCase 생성 시 필요
D-04 [BACKLOG] 배경 Sprite 로딩 미구현 — Initialize에서 SetBackground(null) 호출. BiomeSpriteKey를 통한 Addressables 로딩 필요
D-05 [BACKLOG] 랜덤 스테이지 노드 생성 알고리즘 미구현 — SelectNextStage에서 빈 List 설정. 스펙에 생성 로직 미정의
D-06 [DECISION] NodeMapView._nodeTypeIcons Sprite[] 배열 추가 — NodeType enum int 값을 인덱스로 사용 (0=Start, 1=Battle, 2=Event, 3=Boss)
D-07 [DECISION] NodeMapView.FocusOnNode는 _nodeContainer 자체를 DOTween으로 이동 — 별도 _focusRoot 없이 스크롤 처리
D-08 [DECISION] OptionButtonView 탭 핸들러 미구현 (TODO 주석) — 스펙에 옵션 버튼 동작 미정의
D-09 [SPEC-GAP] CharacterMarkerView의 좌표계 미정의 — world position 사용하나, 올바른 시각적 동작을 위해 캐릭터 마커를 _nodeContainer의 자식으로 Inspector 배치 권장
D-10 [DECISION] NodeView.Reset()에서 Image 3개를 GetComponentsInChildren<Image>()로 할당 — 인덱스 [0]=NodeIcon, [1]=HighlightEffect, [2]=CompletedMark. 프리팹 계층 순서 유지 필요
D-11 [DECISION] NodeMapView.Reset()은 빈 구현 — _nodeContainer(Transform 예외), _nodeViewPrefab(프리팹 참조)은 자동 할당 불가
D-12 [DECISION] StageCompletePopupView.Reset()에서 _buttonPrefab은 프리팹 참조이므로 자동 할당 제외 — Inspector에서 수동 연결 필요
