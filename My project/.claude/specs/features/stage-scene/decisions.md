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


--- StageScene Patch-004 (v2.0.0) ---

D-13 [DECISION] StageProgressService — IStageSceneUseCase 대신 IStageMasterDataRepository 직접 주입
  - 스펙 "IStageSceneUseCase 주입"을 명시했으나, StageSceneUseCase는 StageSceneBootstrapper에서
    씬 단위로 생성되며 GameContext(전역)에서 보유할 경우 이중 인스턴스 문제 발생.
  - Stage.Domain에서 StageScene.Domain을 참조하면 순환 의존성 위험.
  - 대안: IStageMasterDataRepository를 StageProgressService에 직접 주입,
    IsStageEndNode(int) private 헬퍼로 동일 판단 로직 내재화.
  - GameContext에서 _stageMasterDataRepo를 함께 전달. 씬 생명주기 커플링 없음.

D-14 [BACKLOG] T21 MaintenancePresenter IsStageEndNode 제거 — 탐색 이벤트 미구현으로 no-op
  - MaintenancePresenter에 PendingEventContext.IsStageEndNode = false 코드가 원래 없었음.
  - 탐색 이벤트(HandleExplorationAsync)가 TODO stub 상태이므로 적용 불필요.
  - 탐색 이벤트 구현 시: Origin=MaintenanceExploration 설정, IsStageEndNode 미설정(default false).

D-15 [DECISION] EndingEntryService — EnterEndingAsync 공통 헬퍼 추출
  - TryEnterEndingAsync(EndingTriggerKind) 와 TryEnterEndingAsync(EndingCandidateSlot) 양쪽에서
    RunSummaryData 조립 + PendingEndingContext 설정 + NavigateToAsync 로직이 동일.
  - private UniTask EnterEndingAsync(int endingId) 헬퍼로 추출해 중복 제거.

D-16 [DECISION] StagePresenter 패배 시 슬롯 null 처리
  - BattleNodeDataSO._defeatEndings가 Inspector에서 미설정인 경우 slot == null.
  - slot이 null이면 매칭 시도 자체를 스킵하고 경고 로그만 출력.
  - 엔딩 슬롯 미설정 노드에서 패배 시 현재 씬 유지 (씬 이탈 없음).
