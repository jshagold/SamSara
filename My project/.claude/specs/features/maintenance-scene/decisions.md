# MaintenanceScene — Decisions

## D-01: SceneKey.Maintenance 이미 존재 — Task 1 스킵
[DECISION] SceneKey.cs에 `Maintenance` 항목이 이미 등록되어 있어 수정 불필요.

## D-02: GameContext 수정 불필요 — MaintenanceUseCase는 Bootstrapper에서 생성
[DECISION] MaintenanceUseCase는 MainScene의 MainUseCase 패턴과 동일하게 MaintenanceSceneBootstrapper에서 직접 생성. GameContext는 글로벌 공유가 필요한 UseCase만 보관하는 원칙에 따름.

## D-03: MaintenanceSceneBootstrapper는 Start() 사용
[DECISION] Tasks 명세(Start())를 따름. 기존 MainSceneBootstrapper가 Awake()를 사용하는 것은 Constitution §3(SceneBootstrapper는 Start()에서 초기화)과 불일치. MaintenanceScene에서는 Tasks + Constitution 기준에 맞게 Start() 적용.

## D-04: ShowTrainingListMode에 TrainingItemData[] 파라미터 추가
[DECISION] Tasks는 파라미터 없는 ShowTrainingListMode()를 명시했으나, TrainingListView.Show(items)에 실제 스탯 데이터를 전달하려면 파라미터가 필요함. MaintenanceView.ShowTrainingListMode(TrainingItemData[] items)로 설계. Presenter에서 UseCase를 통해 items 빌드 후 전달.

## D-05: 훈련 미니게임 씬 전환 미구현 (SPEC-GAP)
[SPEC-GAP] Tasks에 훈련 스탯 선택 후 전환할 씬(SceneKey)이 정의되어 있지 않음. HandleStatSelectedAsync에서 ConsumeActionPoint() 후 Debug.Log 스텁 처리. Phase 5 구현 시 해당 SceneKey 추가 필요.

## D-06: 탐험 씬 전환 미구현 (SPEC-GAP)
[SPEC-GAP] Tasks에 탐험 버튼 탭 후 전환할 씬(SceneKey)이 정의되어 있지 않음. HandleExplorationAsync에서 ConsumeActionPoint() 후 Debug.Log 스텁 처리. Phase 5 구현 시 해당 SceneKey 추가 필요.

## D-07: Speed → Agility 전면 이름 변경
[DECISION] StatType.Speed, CharacterRunData.Speed, CharacterStatsSO._speed/Speed, CharacterRunRepository 초기화 코드, Dev 검증 파일 전체를 Agility로 변경. 기존 run_save.json의 "Speed" 키는 Newtonsoft.Json 역직렬화 시 매핑되지 않아 0이 됨 — 저장 파일 삭제 후 재생성 필요.

## D-08: CollapseButton → CloseButton 이름 변경
[DECISION] TrainingList 내 닫기 버튼 명칭을 CollapseButton에서 CloseButton으로 변경. Collapse는 패널 축소 의미이고 이 버튼은 TrainingList를 닫는 역할이므로 Close가 정확함. TrainingListView._closeButton, OnCloseClicked, MaintenanceView.OnTrainingListClosed, Presenter.HandleTrainingListClosed 전체 반영.

## D-09: SetInteractable은 button.interactable 대신 color alpha 사용
[DECISION] AP 부족 시 Training/Exploration 버튼을 시각적으로 흐리게 처리하되(alpha 0.4), button.interactable은 변경하지 않음. button.interactable = false이면 onClick 이벤트가 차단되어 AP 부족 팝업을 띄울 수 없기 때문. 클릭 이벤트는 항상 발생 → Presenter에서 CanPerformAction() 체크 후 팝업 표시.

## D-10: CharacterSpriteView.SetSprite 호출 스킵
[DECISION] CharacterInfoPanelView.SetCharacterInfo에서 SetSprite 호출을 스킵. EvolutionNodeId("test_node_id" 등)는 Addressables 스프라이트 키가 아닌 노드 식별자이므로 InvalidKeyException 발생. 정식 구현 시 EvolutionNodeSO MasterData에서 sprite addressable key를 조회하는 로직 필요.

## D-11: DebugRunDataEditor 및 CharacterRunRepository.MarkDirty() 추가
[BACKLOG] Phase 6 정식 게임 시작 흐름(InitializeNewRun 호출) 구현 전까지 테스트용으로 Dev/Features/Character/DebugRunDataEditor.cs 생성. Inspector에서 스탯/골드/행동력/Day/EvolutionNodeId를 직접 수정하고 Apply to Repo로 반영 가능. CharacterRunRepository에 Dev 전용 MarkDirty() 메서드 추가 (인터페이스 미노출).

## D-12: InitializeNewRun() 호출 시점 미정의 (SPEC-GAP)
[SPEC-GAP] 저장 데이터가 없을 때 새 게임을 시작하는 흐름(InitializeNewRun 호출, 초기 ActionPoints 설정 포함)이 스펙에 정의되어 있지 않음. Phase 6에서 구현 예정. 현재는 InitializeNewRun()이 호출되지 않아 모든 런타임 값이 기본값(0)으로 시작됨.
