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

D-15 [DECISION] 행동 순서 큐를 매 행동마다 재계산하는 방식에서 확정 큐(committedQueue) 방식으로 전환. 틱 시작 시 GetPredictedActionOrder(20)로 1회 계산·확정, 이후 행동 완료마다 ConsumeActorFromCommittedQueue로 소비, UI는 PeekCommittedQueue로 표시. 재계산은 사망/이탈 시에만 InvalidateCommittedQueueIfStale로 트리거. 이유: 기존 방식은 매 UpdateActionOrderUI 호출마다 랜덤 타이브레이크가 새로 발생하여 표시 순서가 계속 바뀌는 문제. _predictedResultBuffer 크기 10→20으로 확장(20명분 지원).

D-16 [DECISION] committedQueue 크기를 QueueBuildCount(MaxParticipants×MaxBattleTurns = 6×100 = 600)로 정의. 매직 넘버 대신 의미 있는 상수 조합 사용. 정상 플레이 중 큐 소진 방지 및 순서 불변 보장. _predictedResultBuffer도 동일 크기로 확장.

D-17 [DECISION] ProcessAllyTurn과 ProcessEnemyTurn 시작 시 InfoTooltip.Hide() 호출 추가. 이유: InfoTooltipView에 자동 숨김 로직이 없어서, 롱프레스로 스킬 툴팁이 표시된 후 다음 입력이 없으면 영구적으로 화면에 남아 스킬 버튼 레이캐스트를 차단하는 버그. 각 턴 시작 시 Hide()로 정리.

D-18 [DECISION] 사망/이탈 처리를 InvalidateCommittedQueueIfStale(전체 재계산)에서 PruneDeadFromCommittedQueue(부분 제거 + 끝 보충)으로 교체. 규칙: (1) RemoveAll(IsDead)로 해당 항목만 제거 — 나머지 상대적 순서 유지 (2) 제거 후 MinQueueDisplay(5) 미만이면 GetPredictedActionOrder(QueueBuildCount)의 fresh[existing..] 만 끝에 덧붙임 — 기존 항목 변경 없음. 이유: 기존 Invalidate 방식은 사망 시 전체 재계산으로 살아있는 캐릭터들의 큐 순서까지 바뀌는 문제.

D-19 [DECISION] ProcessTick의 타이브레이크를 무작위(Random.Range)에서 결정론적(a.Id.CompareTo(b.Id), 낮은 Id 우선)으로 변경. 이유: committedQueue는 GetPredictedActionOrder의 insertion sort(아군 먼저, Id 오름차순)로 순서를 결정하는데, ProcessTick이 무작위 타이브레이크를 사용하면 같은 틱에 동점인 참가자들의 실제 실행 순서와 큐 순서가 어긋남. 어긋난 상태에서 ConsumeActorFromCommittedQueue가 ID 검색으로 중간 항목을 제거하면 큐에 중복 항목이 생겨 표시/실행 불일치 발생.

D-20 [DECISION] ShowHitDamage의 success 파라미터 의미 재정의 및 "Miss" 텍스트 제거. 기존: success=true→데미지 표시, false→"Miss". 변경: success=true→보너스/감소 적용(큰 황색 폰트+펀치 스케일), false→일반 데미지(작은 회색 폰트). 항상 실제 데미지 수치 표시. 방어 QTE의 !qteResults[j] 반전도 제거(qteResults[j] 그대로 사용): 방어 성공=감소 데미지→success 시각, 방어 실패=일반 데미지→fail 시각.

D-21 [DECISION] InfoTooltipView의 표시/숨김 방식을 `_root.SetActive` → `CanvasGroup.alpha` 방식으로 전환. 이유: `_root = gameObject`(Reset 기본값) 상태에서 `Hide()`가 MonoBehaviour 자신의 gameObject를 SetActive(false)하면, 부모(_skillAreaRect 등)가 비활성화된 턴에서 `Show()`가 `SetActive(true)`를 호출해도 부모 체인이 비활성이므로 tooltip이 표시되지 않는 버그. CanvasGroup alpha/blocksRaycasts 방식으로 변경하면 gameObject는 항상 active 상태 유지 — 계층 배치에 관계없이 `Show()`/`Hide()` 동작 보장. `_root` 필드 제거.

D-22 [DECISION] InfoTooltipView에 전체화면 블로커 패턴으로 닫기 기능 추가. v2.0.0 범위 내 구현 누락이었음. 구현: InfoTooltipView를 full-stretch CanvasGroup 루트로 변경, 자식으로 Blocker(투명 Image+Button, _closeBlocker)와 TooltipPanel(중앙 고정 패널) 배치. Show() 시 CanvasGroup.blocksRaycasts=true로 전체 입력 차단 + 블로커 터치 → Hide(). 별도 OnDismissed 이벤트 불필요 — BattlePresenter가 각 턴 시작 시 Hide()로 이미 정리. _tooltipRect 필드 제거, 위치는 TooltipPanel의 Inspector anchor(center)로 결정.

D-23 [DECISION] ProcessAllyTurn/ProcessEnemyTurn 시작 시 InfoTooltip.Hide() 호출 제거(D-17 롤백). D-22에서 블로커 패턴으로 플레이어가 직접 닫는 방식이 구현되었으므로 턴 시작 시 강제 Hide는 불필요. 툴팁 닫기는 블로커 터치 단일 경로로만 처리.

D-24 [DECISION] 적 턴 표시 방식을 화면 중앙 DOTween 레이블에서 하단 스킬 영역 공유 패널로 전환. 신규: EnemyTurnInfoView(하단 패널, TMP_Text 1개). BattleView.ShowEnemyTurnPanel(text): HideSkillUI() + EnemyTurnInfoView.Show(). HideEnemyTurnPanel(): EnemyTurnInfoView.Hide(). ProcessEnemyTurn 재구성: (1) 스킬/타겟 먼저 결정 (2) 패널에 "적 [이름]이(가) [스킬명] 사용" 즉시 표시 (3) QTE 전환 직전 패널 숨김 (4) 턴 종료 시 HideEnemyTurnPanel(). ShowEnemyTurnLabel/ShowSkillNameLabel/ShowLabel 제거. _turnLabel/_turnLabelGroup 필드 제거. KoreanSubjectParticle() helper 추가(받침 유무로 이/가 선택). 이유: 기존 center-screen 레이블이 1.8초 blocking으로 동작 흐름 단절.