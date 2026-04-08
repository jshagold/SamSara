# BattleScene v2.0.0 Decisions

(v1.0.0 decisions archived — below records v2.0.0 implementation judgments)

---

D-01 [DECISION] GetPredictedActionOrder 반환 타입을 List<int> → BattleParticipant[]로 교체. Task 6 스펙 요구. BattlePresenter 호출부도 동시에 업데이트하여 컴파일 유지.

D-02 [SPEC-GAP] EnemySO에 portraitSpriteKey 필드 없음. BattleParticipant.PortraitSpriteKey 초기화 시 BattleSpriteKeyHp100을 임시 사용. 추후 EnemySO에 별도 portrait key 추가 필요.

D-03 [DECISION] QTERingView.RunRing에 CancellationToken을 optional 파라미터로 추가. 스펙에는 없으나 async 메서드 취소 지원이 없으면 씬 전환 시 문제 발생 가능.

D-04 [DECISION] ActionOrderView를 ObjectPool<Image> 방식에서 5개 고정 ActionOrderSlotView 방식으로 전환. ActionOrderEntry struct 제거. 기존 BattlePresenter 호출부도 Task 21에서 BattleParticipant[] 기반으로 동시 업데이트.

D-05 [DECISION] SkillSelectionView 3-state 시각 표현: 별도 overlay Image 없이 Button.Image.color tint 방식으로 구현. Usable=white, OnCooldown=dark gray, HpInsufficient=red. 프리팹 구조 의존성 없음.

D-06 [DECISION] BattlePresenter의 아군 턴 입력을 단일 UniTaskCompletionSource<AllyInput>(_allyInputTcs)로 통합. 스킬/타겟/Wait/Confirm 4가지 입력을 하나의 대기 루프로 처리. 타겟 재선택·스킬 재선택 가능.

D-07 [DECISION] Wait 선택 시 ExecuteWait(UseCase) 미호출. 메인 루프가 ConsumeGauge+ReduceCooldowns를 동일하게 처리. ExecuteWait는 BattleUseCase에 제공되어 있으나 현재 Presenter에서는 직접 호출하지 않음. 단순성 우선.

D-08 [BACKLOG] BattleResultPopupView.ShowEndPresentation의 _endAnnouncementText/_endAnnouncementGroup은 Inspector 연결 필요 (M-18 작업). 없으면 연출 스킵 후 결과 팝업만 표시 (null-safe 처리됨).

D-09 [DECISION] 방어 QTE 후 SlideOut은 BattlePresenter.ProcessEnemyTurn에서 직접 호출. TransitionToSkillUI를 경유하지 않음 (적 턴에는 스킬 UI 복귀 불필요).

D-10 [SPEC-GAP] SkillSO에 hitCount 필드 없음. per-hit QTE에서 hitCount = qteDataList.Length로 대체. 스킬 1개 = QTE 입력 1회이면 hit 1개. 추후 SkillSO에 hitCount 추가 시 교체 필요.

D-11 [DECISION] AllyFieldView에 OnLongPress 이벤트 추가. 스펙에는 명시 없으나 아군 롱프레스 → InfoTooltip 표시를 위해 필요. EnemyFieldView와 동일한 패턴 적용.

D-12 [DECISION] InfoTooltipView.Show의 화면 위치는 Screen.width*0.5f, Screen.height*0.5f 고정값 사용. 롱프레스 발생 위치를 정확히 넘기려면 PointerEventData가 필요하나 현재 핸들러 시그니처에 없음. 추후 refine 가능.

D-13 [DECISION] EnemyFieldView/AllyFieldView에 DistributeUnits() 메서드 추가. HorizontalLayoutGroup 대신 코드에서 컨테이너 폭 기반 anchoredPosition을 직접 계산하여 유닛 균등 배치. CharacterUnitView.Setup()에서 _originalPosition 즉시 저장 제거, SetOriginalPosition() 퍼블릭 메서드로 분리하여 배치 완료 후 호출. 이유: Instantiate 직후에는 레이아웃 미반영으로 좌표가 (0,0)이 되며, HorizontalLayoutGroup도 동적 Instantiate 자식에 대해 안정적으로 동작하지 않음.

D-14 [DECISION] ProcessEnemyTurn의 ShowHitDamage success 파라미터 버그 수정. QTE 없는 적 공격(기본공격/스킬)에서 success=false로 하드코딩되어 데미지가 적용되면서도 "Miss" 표시. true로 변경하여 실제 데미지 수치 표시. 방어 QTE per-hit에서는 qteResults[j] → !qteResults[j]로 반전 (플레이어 방어 성공=공격 빗나감, 방어 실패=공격 적중).
