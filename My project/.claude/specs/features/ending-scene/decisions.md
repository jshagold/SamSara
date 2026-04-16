# EndingScene — Decisions

**Feature:** EndingScene (Phase 6) | **Date:** 2026-04-16

---

D-01 [SPEC-GAP] ICharacterAccountRepository에 MarkDirty() 미정의
  - EndingUseCase.CompleteEnding()이 AccountData 필드를 직접 수정한 후 저장하려면
    CharacterAccountRepository._isDirty 를 true 로 설정해야 한다.
    기존 인터페이스에 MarkDirty()가 없었으므로 ICharacterAccountRepository 및
    CharacterAccountRepository에 void MarkDirty() 를 추가함.
    ICharacterRunRepository 의 기존 MarkDirty() 패턴과 동일.

D-02 [DECISION] 탭 감지를 Update() 방식으로 구현 — EventView 패턴 통일
  - 초기 구현은 투명 TapArea Button을 사용했으나,
    기존 EventView.cs가 Input.GetMouseButtonDown(0) + Input.touchCount를
    Update()에서 감지하는 패턴을 사용하고 있음.
    EndingView도 동일 패턴으로 수정 → UniTaskCompletionSource를 _tapTcs 필드로 보관하고
    Update()에서 입력 감지 시 TrySetResult() 호출.
    투명 Button 오브젝트 불필요. Editor 작업 단순화.

D-03 [DECISION] EndingMasterDataRepository에서 LINQ 미사용
  - GetEndingByType()은 foreach + List<T>로 구현. CLAUDE.md §Update() 내 LINQ 금지
    규칙을 준수하기 위해 생성 시점에도 LINQ 사용 자제.

D-04 [BACKLOG] HandleRestart()에서 현재 SceneKey.Main으로 이동
  - ReplayScene/SplashScene 미구현으로 1st dev에서는 Main 씬으로 복귀.
    향후 RunData 리셋 로직 + ReplayScene/SplashScene 구현 시 연결 필요.

D-05 [DECISION] EndingDialogueView.Reset()에서 _dialoguePanel을 gameObject로 자동 할당
  - EndingDialogueView 자체가 패널 루트이므로 gameObject를 기본값으로 설정.
    다른 컴포넌트(_portraitImage, _speakerNameText, _dialogueText)는
    복잡한 레이아웃으로 인해 GetComponentInChildren으로 자동 구분 불가,
    Inspector에서 수동 연결 필요(M-04 항목).

--- Patch-001 ---

D-06 [SPEC-GAP] EndingCondition에 StringValue 추가 — Patch-001 IntValue 불일치
  - Patch-001 스펙은 EvolutionId 조건에 IntValue(int)를 사용하도록 명시했으나,
    CharacterRunData.EvolutionNodeId의 실제 타입이 string이므로
    IntValue만으로는 비교 불가.
    EndingCondition에 StringValue(string) 필드를 추가하고,
    EndingConditionType.EvolutionId 조건 평가 시 StringValue와 비교하도록 구현.
    IntValue는 스펙 명세 유지 + 미래 정수 조건(StatRange 등)에 재사용.

D-07 [DECISION] EndingResolver.Resolve()에서 List 없이 직접 최댓값 추적
  - Patch 스펙은 "matched 목록을 Priority 내림차순 정렬 후 top 선택"을 명시했으나,
    LINQ 금지 + 정렬용 List 할당을 피하기 위해
    foreach 단일 순회 중 최고 Priority를 직접 추적하는 방식으로 구현.
    결과는 동일하며 할당이 없음.

--- Patch-003 ---

D-08 [SPEC-GAP] StageRunData에 ClearedStageCount 필드 추가
  - RunSummaryData.StagesCleared 값을 런타임에 추적하려면 영속 필드가 필요했으나
    StageRunData에 정의되어 있지 않았음.
  - int ClearedStageCount (기본값 0) 추가.
    Newtonsoft.Json 역직렬화 시 기존 세이브파일에 필드가 없어도 0으로 초기화됨
    → 기존 세이브 호환성 유지.
  - IStageRepository에 IncrementClearedStageCount() 추가,
    StageRepository에서 _isDirty 설정과 함께 구현.

D-09 [DECISION] BossVictory 경로에서 MoveToNode 미호출
  - 보스 승리 후 EndingScene 전환 시 MoveToNode(Day++, ActionPoints 리셋, CurrentNodeIndex++)를
    호출하지 않음.
  - 런이 종료되므로 Day/CurrentNodeIndex 갱신은 의미 없음.
  - ClearedStageCount 증가 + 저장만 수행 후 EndingScene 전환.

D-10 [DECISION] EvolutionName 조회를 GameContext.EvolutionNodes foreach로 수행
  - StagePresenter는 CharacterRepository 직접 접근 없이
    GameContext.EvolutionNodes(EvolutionNodeSO[])를 foreach로 순회해 CharacterName을 조회.
  - 매칭 없으면 "Unknown" 반환 (Fail Fast 미적용 — 진화 미선택 상태 런 허용).

D-11 [DECISION] StagePresenter Bootstrapper 수정 불필요
  - StagePresenter는 이미 GameContext 전체를 보유하고 있으므로
    EndingResolver, CharacterRunRepo, StageRepo, EvolutionNodes 모두
    _gameContext를 통해 접근 가능.
  - StageSceneBootstrapper DI 변경 없음.

D-12 [DECISION] IncrementClearedStageCount를 StageUseCase가 아닌 StageSceneUseCase에 추가
  - Patch-003 스펙은 "StageUseCase.cs 수정 검토"를 언급했으나,
    Assets/_Game/Features/Stage/Domain/StageUseCase.cs는 생성자만 있는 stub 상태이며
    실제 스테이지 씬 로직은 StageSceneUseCase.cs에 집중되어 있음.
  - StagePresenter가 직접 사용하는 StageSceneUseCase에 IncrementClearedStageCount()를 추가.
  - StageUseCase stub는 미변경 (현재 기능 없음).

--- Patch-002 ---

D-13 [DECISION] 기존 EventSO .asset 중 Death result 사용 없음 — MasterData 마이그레이션 불필요
  - Assets/Resources/MasterData/Event/ 내 5개 EventSO .asset 전수 확인 결과:
    Event_00.asset: _resultType 2 (StatChange)
    Event_Chain_01.asset: _resultType 0 (None), choices: _resultType 2 (StatChange)
    Event_Chain_02.asset: _resultType 0 (None)
    Event_Shop.asset: _resultType 3 (ShopEncounter)
    Event_Test_OneShot.asset: _resultType 1 (HpChange)
  - 모두 _resultType 5 (Death) 없음. Death → Ending 교체 후 기존 에셋 재직렬화 불필요.
  - Patch-002 Manual Work: Ending result 테스트 에셋 신규 생성만 필요.

D-14 [SPEC-GAP] ISceneNavigator를 GameContext 생성자에 추가
  - EndingEntryService가 ISceneNavigator를 필요로 하나, GameContext 생성자에
    ISceneNavigator 파라미터가 없었음.
  - GameContext 생성자에 ISceneNavigator sceneNavigator 파라미터 추가 +
    SceneNavigator public accessor 노출.
  - GlobalBootstrapper에서 _sceneNavigator를 GameContext 생성 시 함께 전달하도록 수정.

D-15 [DECISION] EventPresenter ISceneNavigator도 GameContext.SceneNavigator로 통합
  - Patch-002 스펙은 "ISceneNavigator, PendingEventContext accessed via GameContext"를 명시.
  - D-14에서 GameContext.SceneNavigator accessor가 추가됨에 따라
    EventPresenter 생성자에서 ISceneNavigator 파라미터도 제거하고
    _gameContext.SceneNavigator로 모든 내비게이션 접근.
  - EventSceneBootstrapper에서 sceneNavigator 변수 및 전달 불필요.
  - 다른 Presenter들(Stage, Battle, Maintenance, Ending)은 ISceneNavigator를
    여전히 개별 파라미터로 받고 있어 완전 통일은 아님 —
    향후 DI 라이브러리 도입 시 일괄 정리 예정 (D-04 BACKLOG 연장선).

D-16 [DECISION] EventResultType.Death 제거 시 enum ordinal 연속성 유지
  - Death(5) 제거 후 Ending(5)이 동일 ordinal을 점유.
  - 기존 .asset들이 _resultType 5를 사용하지 않으므로 역직렬화 오염 없음 (D-13 근거).
  - 새 Ending 에셋은 _resultType 5 + _hasEndingType + _endingType 조합으로 설정.

--- Patch-004 ---

D-17 [DECISION] StageSceneUseCase.IsStageComplete() — IsBoss 기반 → 마지막 인덱스 기반으로 교체
  - 기존: node.BattleData.IsBoss 확인 (BattleNodeDataSO._isBoss 사용).
  - Patch-004에서 _isBoss 필드 제거 → 판단 기준 필요.
  - 새 구현: nodeIndex >= nodes.Length - 1 (노드 배열 마지막 = 끝 노드).
  - 스테이지는 선형 배열 기준 1st dev에서 동작. 다중 끝 노드가 필요하면
    StageNodeSO에 IsEndNode flag 추가 필요 (미래 확장 대상).

D-18 [DECISION] StageProgressService를 GameContext에서 Repo 직접 주입으로 생성
  - 스펙 "IStageSceneUseCase or related UseCase" 언급 있었으나,
    StageSceneUseCase는 씬별 생성 (StageSceneBootstrapper) — GameContext에서 보유 불가.
  - StageProgressService는 IStageRepository + ICharacterRunRepository만으로 충분.
  - GameContext에서 직접 `new StageProgressService(_stageRepo, _characterRunRepo)` 생성.
  - 씬 생명주기 커플링 없음.

D-19 [DECISION] BattleNodeDataSO._isBoss 제거 cascade — BattleUseCase / Dev 파일 수정
  - 제거 대상 확인 결과 IsBoss 참조 위치:
    * BattleUseCase.cs:106 — Debug.Log에서만 참조 (기능 로직 아님) → 로그 라인 수정.
    * StageSceneUseCase.cs — IsStageComplete()에서 참조 → D-17 방식으로 교체.
    * Dev/BattleDataLoadTest.cs — IsBoss 프로퍼티 직접 접근 → 로그 라인 수정.
    * Dev/StageSceneTestDataGenerator.cs — Reflection string "_isBoss" 사용 → 런타임 무시, 수정 불필요.
    * Dev/BattleTestDataCreator.cs — JSON string literal → 역직렬화 미사용 필드, 수정 불필요.
  - Patch-004 "DO NOT Modify BattleScene Feature" 원칙 하에 Debug.Log 제거는
    기능 수정이 아닌 컴파일 오류 해소이므로 허용.

D-20 [DECISION] StageSceneUseCase.IncrementClearedStageCount() 유지 (호출자 변경만)
  - Patch-003 D-12: StageSceneUseCase에 IncrementClearedStageCount() 추가.
  - Patch-004: StageProgressService가 IStageRepository.IncrementClearedStageCount()를 직접 호출.
  - StageSceneUseCase.IncrementClearedStageCount()는 미호출 상태로 유지 (제거 시 불필요한 위험).
  - StagePresenter에서 직접 호출 제거됨 (StageProgressService 위임).

D-21 [BACKLOG] MaintenancePresenter 탐색 이벤트 — PendingEventContext Origin 설정
  - MaintenancePresenter.cs 미구현 (파일 없음).
  - 탐색 이벤트 구현 시: Origin=MaintenanceExploration, IsStageEndNode=false 설정 필요.
  - StageProgressService 호출 금지 (Feature 경계 위반 — EventPresenter도 동일 원칙 적용).

D-22 [DECISION] EndingMasterDataRepository에서 LINQ 완전 제거
  - 기존 GetEndingByType() 구현이 LINQ를 import하고 있었음.
  - Patch-004에서 GetEndingsByTriggerKind()로 교체 + using System.Linq 제거.
  - foreach + List<T> 패턴으로 구현 (CLAUDE.md §8 준수).

D-23 [DECISION] Patch-002 테스트 EventSO 에셋 — Patch-004 이후 상태
  - Event_Test_Ending_Death.asset (_resultType: 0 = None으로 생성됨),
    Event_Test_Ending_EventEnding.asset (미생성 확인) —
    Patch-002에서 _resultType 5 (Ending)으로 설정하기 위해 생성 예정이었으나
    Patch-004에서 EventResultType.Ending이 제거됨.
  - 기존 EventSO .asset 전수 확인 결과 _resultType 5 사용 에셋 없음 → 마이그레이션 불필요.
  - Event_Test_Ending_Death.asset은 현재 _resultType 0이므로 그대로 유지 가능.

D-24 [SPEC-GAP] 기존 EndingSO .asset 6개 — Patch-004 이후 전면 재설정 필요 (Manual Work)
  - 기존 .asset들은 _endingType 필드 기반 (구 EndingType: BattleDefeat/EventDeath/BossVictory/EventEnding).
  - Patch-004에서 EndingType이 [Flags] Good/Bad로 재정의, _endingType → _categories로 rename,
    _triggerKind 신규 추가됨.
  - Inspector에서 TriggerKind + Categories + Conditions 전면 재설정 필요.
  - 대상 에셋 목록:
    * Ending_BattleDefeat_Fallback.asset     → TriggerKind=BattleDefeat, Conditions=empty, Priority=0, Categories=Bad
    * Ending_BattleDefeat_SpecialEvolution.asset → TriggerKind=BattleDefeat, Conditions=[EvolutionId=node_warrior], Priority=10, Categories=Bad
    * Ending_BossVictory_Fallback.asset      → TriggerKind=BattleVictory, Conditions=[StageCompleteFlag], Priority=0, Categories=Good
    * Ending_BossVictory_SpecialEvolution.asset → TriggerKind=BattleVictory, Conditions=[StageCompleteFlag + EvolutionId=node_warrior], Priority=10, Categories=Good
    * Ending_EventDeath_Fallback.asset       → TriggerKind=EventResult, Conditions=empty, Priority=0, Categories=Bad
    * Ending_EventEnding_Fallback.asset      → TriggerKind=EventResult, Conditions=empty, Priority=0, Categories=Good
  - ※ Ending_BossVictory_Fallback.asset _id=0 는 D-24 이후에도 유지 (기능 이상 없으면 허용).

D-25 [DECISION] EndingEntryService 시그니처 변경 — IEndingEntryService accessor 중복 수정
  - GameContext의 기존 `public IEndingEntryService EndingEntryService` accessor 줄이
    Patch-002 당시 한 줄에 선언됐으나 Patch-004에서 StageProgressService accessor 추가 시
    인접 줄로 정렬. 기능 변경 없음.
