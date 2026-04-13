# EventSystem — Decisions

D-01 [DECISION] SceneNavigator 수정 불필요. 기존 구현이 key.ToString()을 사용하므로 SceneKey.Event 추가만으로 "Event" 씬 이름이 자동 매핑됨.

D-02 [SPEC-GAP] IStageRepository에 SetPendingChainedEventId(int) 미정의 — EventUseCase가 체인 이벤트 ID를 저장하려면 IStageRepository에 해당 메서드가 필요함. IStageRepository.cs, StageRepository.cs에 추가하여 해결.

D-03 [DECISION] ICharacterRunRepository에 UpdateHp/UpdateStat 메서드 없음 — RunData 직접 변경 + MarkDirty() 호출 방식으로 구현. MarkDirty()는 인터페이스에 이미 존재하므로 추가 수정 불필요.

D-04 [DECISION] ApplyChoice / ApplyDirectResult의 반환 타입을 스펙의 EventResult 그대로 유지. 저장은 SaveDataAsync().Forget()으로 fire-and-forget (Save-on-Action). Presenter가 await할 필요 없이 결과를 즉시 받을 수 있음.

D-05 [SPEC-GAP] EventResult.ResultType == Battle일 때 PendingBattleContext 조립에 필요한 BattleNodeData 출처가 스펙에 미정의. 현재 HandlePostResultAsync에서 ReturnScene으로 복귀하는 Stub으로 처리. 추후 Patch 필요.

D-06 [DECISION] EventMasterDataRepository 로딩을 Initialize() 메서드 없이 생성자에서 처리. GameContext 생성자는 메인 스레드에서 실행되므로 Resources.LoadAll 호출 안전.

D-07 [DECISION] EventPresenter.InitializeAsync() 내 배경 설정을 스펙 명시 항목 외로 추가. PendingEventContext.BackgroundSpriteKey가 있을 경우 Resources.Load<Sprite>로 로드하여 BackgroundView에 반영. 필수 UX 동작으로 판단.
