# EndingScene — Decisions

**Feature:** EndingScene (Phase 6) | **Version:** v2.0.0 | **Date:** 2026-04-20

---

D-01 [DECISION] EndingCandidateSlot.cs 이미 존재 — 수정으로 처리
  - Patch-004 구현 중 EndingCandidateSlot.cs가 이미 생성되었으나,
    _fallback(EndingSO) 필드 및 Fallback 프로퍼티가 누락된 불완전한 상태였음.
  - 신규 생성 대신 기존 파일에 _fallback 필드 추가 + IsEmpty 조건 갱신으로 처리.
  - IsEmpty: (_candidates null 또는 Length==0) AND _fallback==null 으로 수정.

D-02 [DECISION] T4 StageCompleteFlag 위치: EndingResolver.EvaluateOne() — Clean Architecture 준수 확인
  - StageCompleteFlag 평가 코드가 EndingCondition.cs(데이터 클래스)가 아닌
    EndingResolver.cs(로직 클래스)의 EvaluateOne() 메서드에 위치함.
  - §4 Clean Architecture 준수 상태. 리팩토링 불필요.
  - T7 EndingResolver 전체 재작성 시 case 자체 제거.

D-03 [DECISION] Stage 2 외부 Feature 파일들이 이미 v2.0.0 상태로 선구현됨 (verify 완료)
  - T12 BattleNodeDataSO.cs: _victoryEndings/_defeatEndings 이미 추가됨 → skip.
  - T13 EventSO.EventResult: _endingSlot 이미 추가됨 → skip.
  - T14 PendingEventContext.cs: IsStageEndNode 이미 제거됨 → skip.
  - T15 NodeCompletionContext.cs: IsStageEndNode 이미 제거된 순수 DTO → skip.
  - T17 IStageProgressService.cs: 시그니처 변경 불필요 확인 → skip.
  - T18 StagePresenter.cs: 슬롯 기반 HandleBattleResultIfAny, IsStageEndNode setter 없음 → skip.
  - T19 EventPresenter.cs: HandlePostResultAsync 슬롯 기반 구현, StageProgressService 직접 호출 없음 → skip.

D-04 [DECISION] T16: IStageMasterDataRepository 직접 사용 유지 (IStageSceneUseCase 주입 불가)
  - Tasks-MD v2.0.0 T16은 IStageSceneUseCase.IsStageComplete() 주입을 명시하나,
    StageSceneUseCase는 씬 생명주기 (StageSceneBootstrapper에서 생성)로
    GameContext(전역 생명주기)에 주입할 수 없음.
  - 현재 구현: IStageMasterDataRepository를 직접 사용해 StageProgressService 내부에서 끝 노드 판정.
    핵심 설계 원칙 준수 여부 확인:
    ① NodeCompletionContext = 순수 DTO (NodeIndex, NodeType만 보유) ✓
    ② 판정(IsStageEndNode) = StageProgressService 내부 private 메서드 ✓
    ③ 호출자(StagePresenter)가 판정 결과를 Context에 담아 전달하지 않음 ✓
  - §4 Clean Architecture 데이터/판정 책임 분리 원칙 완전 준수.
  - 사용자 Stage 2 지침에서 IStageMasterDataRepository 직접 주입 대안 명시적 수용.

D-05 [DECISION] T21: MaintenanceScene 탐험 이벤트 미구현 — IsStageEndNode setter 없음
  - MaintenancePresenter.HandleExplorationAsync()는 스텁 상태
    (Debug.Log "탐험 씬 미정의, 전환 스킵"). PendingEventContext 자체를 생성하지 않음.
  - IsStageEndNode setter 제거 대상 코드가 존재하지 않음 → skip.
  - 탐험 이벤트 구현 시 Origin=MaintenanceExploration 설정 필요 (IsStageEndNode 불필요).
