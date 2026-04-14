# RunConfigSO — Decisions

D-01 [DECISION] RunConfigSO는 `Assets/_Game/App/RunConfigSO.cs`에 위치시킴 (Features가 아닌 App 레벨)
     → RunConfigSO는 모든 Feature(Character, Stage, Shop)에 공유되는 앱 수준 설정이므로 App/ 레이어가 적절.

D-02 [DECISION] CharacterStatsSO를 InitializeNewRun 시그니처에서 제거함
     → Hp/MaxHp/Strength/Toughness/Agility는 Phase 6 Reincarnation 시스템에서 설정됨.
     → InitializeNewRun 시점에는 0으로 기본 초기화되며, CharacterStatsSO 의존성을 런타임에 제거함.

D-03 [DECISION] StageRepository.InitializeRun(string stageId) → InitializeNewRun(RunConfigSO config)으로 리네임
     → 메서드명 통일: 모든 Repository가 동일한 InitializeNewRun(RunConfigSO) 시그니처를 사용.
     → CurrentStageId는 config.StartStageId.ToString()으로 설정됨.

D-04 [DECISION] Dev/ 검증 스크립트에서 RunConfigSO를 Resources.Load로 직접 로드함
     → 일부 Dev/ 스크립트는 GlobalBootstrapper 없이 new Repository()를 직접 생성함.
     → GameContext에 접근 불가하므로 Resources.Load<RunConfigSO>("MasterData/DefaultRunConfig")로 대체.

D-05 [DECISION] Stage Dev/ 검증 스크립트에서 테스트 전용 StageId는 InitializeNewRun 후 RunData.CurrentStageId에 직접 대입
     → InitializeNewRun(RunConfigSO)은 config.StartStageId를 사용하므로 테스트 특정 StageId를 별도 지정 필요.
     → RunData는 public 프로퍼티이므로 직접 대입이 가능하며, 테스트 격리를 위해 허용됨.

D-06 [SPEC-GAP] ShopRepository.InitializeNewRun(RunConfigSO config) — config.InitialMerchantAvailable == true 케이스 미정의
     → false 케이스는 ShopRunData 기본값(ActiveMerchantId=-1)과 일치하므로 추가 처리 불필요.
     → true 케이스의 초기 merchantId가 스펙에 미정의 — Phase 6에서 구체화 예정.

---

## 세션 추가 결정 (Tasks 범위 외)

D-07 [DECISION] BattleSceneBootstrapper #if UNITY_EDITOR 블록에서 InitializeNewRun 콜 제거
     → 원래 코드: `characterRunRepo.InitializeNewRun(gameContext.RunConfig)` (에디터 직접 Play 시 RunData 초기화 목적)
     → 제거 이유: BattleScene 진입 시점에 InitializeNewRun을 호출하면 정상 플레이 흐름(StageScene → BattleScene)에서도
        기존 RunData(골드, HP 등 게임 진행 상태)가 초기화되어 날아감.
     → RunData 초기화는 GlobalBootstrapper 초기화 흐름에서만 담당해야 함.
        씬 Bootstrapper는 런타임 데이터를 읽기만 해야 하며 초기화하지 않는다.

D-08 [SPEC-GAP] 신규 플레이어(저장 파일 없음) 진입 시 InitializeNewRun 최초 호출 시점이 Tasks에 미정의
     → Tasks 1~7은 InitializeNewRun 시그니처 변경(Task 4~6)과 기존 콜사이트 업데이트(Task 7)만 정의함.
     → 실제로 '언제, 어디서' InitializeNewRun을 최초 호출하는지(신규 플레이어 진입 흐름)는 정의되지 않음.
     → GlobalBootstrapper 초기화 흐름에서 처리해야 한다는 방향은 합의되었으나 구체 구현 위치는 미정.
     → Phase 6 Reincarnation 흐름 설계 시 함께 정의 필요.

D-09 [DECISION] DebugRunDataEditor 기본값을 RunConfigSO + EvolutionNodeSO.BaseStats 기반으로 교체함 (해결됨)
     → Reset()에서 RunConfigSO를 Resources.Load하여 기본값 세팅.
     → 스탯은 RunConfigSO.DefaultEvolutionNodeId로 EvolutionNodeSO를 찾아 BaseStats에서 가져옴.
     → ApplyToRepo() 로직은 기존 방식(Inspector 값 직접 대입 + MarkDirty) 유지 — 디버그 도구 목적상 InitializeNewRun 호출 불필요.

D-10 [DECISION] DebugStageRunDataEditor 기본값을 RunConfigSO 기반으로 교체함 (해결됨)
     → _applyOnStart 기본값을 true → false로 변경 (Play 시 자동 덮어쓰기 방지).
     → _currentStageId 기본값을 RunConfigSO.StartStageId 기반으로 변경.
     → Reset()에서 RunConfigSO를 Resources.Load하여 기본값 세팅.

D-11 [DECISION] GlobalBootstrapper의 RunConfigSO 로드 방식을 Resources.Load → SerializeField Inspector 연결로 변경
     → 변경 전: InitializeAsync() 내에서 Resources.Load<RunConfigSO>("MasterData/DefaultRunConfig") 런타임 로드.
     → 변경 후: [SerializeField] private RunConfigSO _runConfig — Inspector에서 직접 연결.
     → 이유: RunConfigSO는 런타임 Resources 경로에 의존하지 않아야 함. Inspector 연결 방식이 Unity 직렬화 원칙에 부합하며,
        경로 변경 시 컴파일 오류 없이 버그가 생기는 Resources.Load 방식보다 안전함.
     → null 시 InvalidOperationException throw (Fail Fast 원칙 유지).
     → 수동 작업 필요: Unity Inspector에서 GlobalBootstrapper 컴포넌트의 _runConfig 필드에 DefaultRunConfig.asset 연결.

D-12 [DECISION] GlobalBootstrapper Step 4-A — 신규 런 자동 초기화 구현 (D-08 해결)
     → LoadAllDataAsync() 완료 직후, Day==0 또는 EvolutionNodeId가 비어있으면 미초기화 상태로 판단.
     → CharacterRunRepo / StageRepo / ShopRepo 세 Repository 모두 InitializeNewRun(_runConfig) 호출.
     → CharacterRunData 스탯(Hp/MaxHp/Str/Tgh/Agi)은 RunConfigSO.DefaultEvolutionNodeId로 EvolutionNodeSO를 찾아 BaseStats에서 적용.
     → 초기화 후 세 Repository 병렬 SaveDataAsync() 호출로 즉시 저장.
     → // TODO: Phase 6 — Move to SplashScene/ReplayScene 주석 추가 (임시 구현).
     → 이미 유효한 저장 데이터가 있으면 아무것도 하지 않음.

D-13 [DECISION] GlobalBootstrapper — RunConfigSO ID 참조값 MasterData 존재 여부 검증 추가
     → 위치: Step 4-A 완료 직후, Step 5(TrySetResult) 직전. 앱 시작 시 한 번만 실행.
     → DefaultEvolutionNodeId: System.Array.Exists로 EvolutionNodes 배열에서 직접 검색. 없으면 InvalidOperationException.
     → StartStageId: StageMasterDataRepo.GetStageById() 호출 — 이미 throw하므로 catch 후 RunConfigSO 맥락 메시지로 rethrow.
     → 신규 런/기존 저장 데이터 무관하게 항상 실행. 잘못된 RunConfigSO 설정은 앱 시작 시 즉시 감지됨 (Fail Fast).

D-14 [DECISION] StageSceneUseCase.MoveToNode — Day 진행 시 ActionPoints 초기화 및 MarkDirty 누락 수정
     → 버그: MoveToNode가 Day += 1만 수행하고 ActionPoints를 MaxActionPoints로 리셋하지 않아 AP가 소모된 채로 유지됨.
     → 부가 버그: RunData를 직접 변경 후 MarkDirty()를 호출하지 않아 SaveDataAsync()가 _isDirty==false 조건으로 저장을 건너뜀.
        (Day가 인메모리에서는 증가하지만 실제로 파일에 저장되지 않는 숨겨진 버그)
     → 수정: RunData.ActionPoints = RunData.MaxActionPoints 추가, MarkDirty() 추가 (MiniGameUseCase.ApplyResultAndSave와 동일한 패턴).