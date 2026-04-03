D-01 [DECISION] SkillRuntimeData는 SkillUseCase 생성자 내부에서 new로 생성
  - SkillRuntimeData는 전투 세션별 내부 상태 객체로, 외부 의존성이 아님
  - Constitution §3 예외: Bootstrapper/Composition Root 외 new 허용 사유 — 내부 상태 관리 객체

D-02 [DECISION] SkillMasterDataRepository.LoadAsync()는 내부적으로 동기 처리 후 UniTask.CompletedTask 반환
  - Resources.LoadAll은 메인 스레드 전용 동기 API
  - 인터페이스는 UniTask LoadAsync()로 정의하여 향후 Addressables 전환 시 변경 없이 확장 가능
  - 기존 StageMasterDataRepository의 Initialize() 패턴과 달리 비동기 인터페이스 채택

D-03 [DECISION] SkillRuntimeData에 GetAllSkillIds() 헬퍼 메서드 추가
  - Spec에 명시되지 않았으나, SkillUseCase.GetAvailableSkills()에서 모든 등록된 skillId를 순회해야 함
  - Dictionary Keys를 외부에 직접 노출하지 않고 배열 복사본 반환

D-04 [DECISION] CanUseSkill에서 HP 비용 체크 시 currentHp <= cost.Value (같을 때도 사용 불가)
  - Spec 원문: "check currentHp > cost.Value" — 등호 시 사용 불가로 해석
  - [SUPERSEDED by Patch-001] CanUseSkill 자체가 제거됨

D-05 [DECISION] Patch-001 적용 — SkillUseCase 전투 런타임 기능 일체 제거
  - InitializeBattle, CleanupBattle, CanUseSkill, ConsumeSkill, TickCooldowns, GetAvailableSkills 제거
  - SkillRuntimeData.cs 삭제 (전투 세션 쿨다운 상태 관리 전담 클래스였으므로 불필요)
  - 이에 따라 D-01, D-03, D-04는 실효됨
  - 근거: 쿨다운은 전투 흐름(틱 기반 행동 순서) 의존 로직 → BattleUseCase 책임

D-06 [DECISION] Specify v1.1.0 명세에 맞춰 메서드 이름 변경
  - GetSkill() → GetSkillById()
  - GetQTEPattern() → GetQTEPatternById()
  - GetSkillsForNode() → GetSkillsForEvolutionNode()
  - 기존 코드(tasks.md 기반)의 이름이 Specify v1.1.0 명칭과 달랐으므로 Patch-001 적용 시점에 일치시킴
