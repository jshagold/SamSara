# EventSystem — Decisions

D-01 [DECISION] SceneNavigator 수정 불필요. 기존 구현이 key.ToString()을 사용하므로 SceneKey.Event 추가만으로 "Event" 씬 이름이 자동 매핑됨.

D-02 [SPEC-GAP] IStageRepository에 SetPendingChainedEventId(int) 미정의 — EventUseCase가 체인 이벤트 ID를 저장하려면 IStageRepository에 해당 메서드가 필요함. IStageRepository.cs, StageRepository.cs에 추가하여 해결.

D-03 [DECISION] ICharacterRunRepository에 UpdateHp/UpdateStat 메서드 없음 — RunData 직접 변경 + MarkDirty() 호출 방식으로 구현. MarkDirty()는 인터페이스에 이미 존재하므로 추가 수정 불필요.

D-04 [DECISION] ApplyChoice / ApplyDirectResult의 반환 타입을 스펙의 EventResult 그대로 유지. 저장은 SaveDataAsync().Forget()으로 fire-and-forget (Save-on-Action). Presenter가 await할 필요 없이 결과를 즉시 받을 수 있음.

D-05 [SPEC-GAP] EventResult.ResultType == Battle일 때 PendingBattleContext 조립에 필요한 BattleNodeData 출처가 스펙에 미정의. 현재 HandlePostResultAsync에서 ReturnScene으로 복귀하는 Stub으로 처리. 추후 Patch 필요.

D-06 [DECISION] EventMasterDataRepository 로딩을 Initialize() 메서드 없이 생성자에서 처리. GameContext 생성자는 메인 스레드에서 실행되므로 Resources.LoadAll 호출 안전.

D-07 [DECISION] EventPresenter.InitializeAsync() 내 배경 설정을 스펙 명시 항목 외로 추가. PendingEventContext.BackgroundSpriteKey가 있을 경우 Resources.Load<Sprite>로 로드하여 BackgroundView에 반영. 필수 UX 동작으로 판단.

D-08 [DECISION] EventUseCase.GetChainInfo()에서 EventType 충돌 해소를 위해 MasterData.EventType.Chained로 참조. UnityEngine.EventType과 Samsara.Features.Event.MasterData.EventType이 충돌(CS0104)하므로 using 제거 대신 네임스페이스 별칭 방식 사용. UnityEngine은 Mathf/Debug 때문에 제거 불가.
**Why:** using UnityEngine 제거 불가, using MasterData 유지 필요.
**How to apply:** Event 네임스페이스 내에서 EventType을 참조할 때는 항상 MasterData.EventType으로 명시.

D-09 [DECISION] StagePresenter에서 SceneKey.ActionEvent(씬 파일 없음)를 SceneKey.Event로 교체하고 EventNodeDataSO.EventId를 사용해 PendingEventContext를 조립. ReturnScene은 SceneKey.Stage로 고정. EventData가 null이면 에러 로그 후 early return(Fail Fast).
**Why:** ActionEvent 씬은 존재하지 않는 플레이스홀더였고 EventSystem 구현 후 대체 대상.
**How to apply:** Stage 외 씬에서 EventScene 진입 시에도 동일하게 ReturnScene만 다르게 설정하여 PendingEventContext를 조립.

D-10 [DECISION] 이벤트 완료 후 Stage 노드 진행 처리를 위해 PendingEventContext에 IsCompleted 플래그 추가. GameContext에 별도 LastEventResult 프로퍼티를 추가하지 않은 이유: EventPresenter는 GameContext 의존성이 없고(PendingEventContext만 주입) 추가하면 아키텍처 위반. PendingEventContext는 이미 주입된 객체이므로 자연스러운 신호 전달 경로.
**Why:** Battle의 LastBattleResult 패턴(GameContext 직접 참조)을 그대로 따르면 EventPresenter에 GameContext를 추가해야 함. 이는 불필요한 의존성 확대.
**How to apply:** IsCompleted는 EventPresenter가 ReturnScene으로 이동하기 직전에만 true로 설정. Death 결과는 Stage로 돌아오지 않으므로 설정하지 않음. StagePresenter가 소비 후 PendingEventContext를 null로 초기화하여 중복 처리 방지.
