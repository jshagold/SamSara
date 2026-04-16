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
