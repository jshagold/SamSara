# BattleScene — Decisions

D-01 [SPEC-GAP] IStageMasterDataRepository에 GetEnemyById(int), GetEvolutionNodeById(string) 추가. Task 6의 InitializeBattle에서 EnemySO와 EvolutionNodeSO를 조회해야 하지만, 기존 Repository에 해당 메서드가 없었음. StageMasterDataRepository가 이미 Resources/MasterData를 로드하는 패턴이므로 여기에 추가.

D-02 [SPEC-GAP] BattleUseCase 생성자에 IStageMasterDataRepository 추가 (spec에는 ISkillMasterDataRepository, ICharacterRunRepository만 명시). Task 20 BattleSceneBootstrapper가 IStageMasterDataRepository를 이미 acquire하므로 주입 가능.

D-03 [DECISION] SkillSO에 coolDown 필드가 없으므로, SkillCost 배열에서 CostType.CoolDown 항목의 Value를 쿨다운 턴 수로 사용. Task spec의 "SkillSO.coolDown" 참조는 이 방식으로 해석.

D-04 [DECISION] SelectEnemySkill에서 사용 가능한 스킬이 없을 때 DefaultAttackId(-1)를 반환. ExecuteAction에서 DefaultAttackId인 경우 스킬 없이 순수 Strength vs Toughness로 데미지 계산 (damageMultiplier = 1.0, 쿨다운/HP코스트 없음).

D-05 [DECISION] GetUsableSkills의 HP 코스트 판정: actor.CurrentHp <= cost.Value이면 사용 불가 (사용 시 사망 방지). 즉, HP 코스트를 지불한 후에도 HP > 0이어야 사용 가능.

D-06 [DECISION] ReduceCooldowns에서 Dictionary 키 순회를 위해 new List<int>(keys) 사용. Dictionary 순회 중 값 변경이 필요하므로 키 복사본 생성. 이 메서드는 턴당 1회 호출이므로 GC 영향 미미.

D-07 [SPEC-GAP] GameContext에 ISkillMasterDataRepository public accessor(SkillMasterDataRepo) 추가. BattleSceneBootstrapper가 BattleUseCase와 BattlePresenter에 주입하기 위해 필요하나 기존 GameContext에는 SkillUseCase만 노출되어 있었음.

D-08 [DECISION] CharacterUnitView의 3-stage sprite switch를 Addressables 대신 Image.color tint로 구현 (Phase 1). 실제 HP 단계별 스프라이트 전환은 Addressables 로드 구현 후 Phase 2에서 적용 예정.

D-09 [BACKLOG] BattleQTEView의 터치 판정은 Input.GetMouseButtonDown/Input.GetTouch 기반 직접 구현. 새 Input System 전환 시 리팩토링 필요.

D-10 [DECISION] ActionOrderView, SkillSelectionView, DamagePopupView에 UnityEngine.Pool.ObjectPool<T> 적용하여 빈번한 Instantiate/Destroy 방지.

D-11 [DECISION] BattlePresenter의 비동기 이벤트 대기(스킬 선택, 타겟 선택, 결과 확인)를 UniTaskCompletionSource 패턴으로 구현. 이벤트 핸들러가 TrySetResult 호출하여 await 해제.

D-12 [DECISION] GameContext에서 구 BattleUseCase 필드/생성 코드 제거 (Task 0에서 스텁 삭제됨). 신규 BattleUseCase는 BattleSceneBootstrapper에서 씬 단위로 생성.
