# MiniGame — Decisions

**Feature:** MiniGame | **Version:** 1.0.0 | **Date:** 2026-04-01

---

D-01 [SPEC-GAP] `ICharacterRunRepository` 인터페이스에 `MarkDirty()` 메서드가 없음 — `ApplyResultAndSave`에서 RunData를 직접 수정 후 `SaveDataAsync()`를 호출하면 `_isDirty`가 false인 채로 저장이 스킵되는 문제 발생. `ICharacterRunRepository`에 `MarkDirty()`를 추가하여 해결함.

D-02 [SPEC-GAP] `MaintenancePresenter.HandleStatSelectedAsync`에서 `ConsumeActionPoint()`를 호출한 뒤 MiniGame 씬으로 전환하는 플로우가 아직 연결되어 있지 않음 (TODO 주석 상태). MiniGame 씬 진입 시 `GameContext.PendingTrainingStat` 설정 및 `ISceneNavigator.NavigateToAsync(SceneKey.MiniGame)` 호출로 연결 필요. MaintenancePresenter에서 AP 소비를 제거하고 MiniGameUseCase.ApplyResultAndSave에서 AP -1 처리하도록 역할 정리 필요. 별도 Patch에서 처리 예정.

D-03 [DECISION] `MiniGameVerdict` 판정 기준을 성공 횟수 기준으로 구현함 (0~1회: Fail, 2회: Maintain, 3회: Success). tasks.md 명세 그대로 적용.

D-04 [DECISION] `MarkerSpeed = 300f`, `SuccessZoneStart = 0.35f`, `SuccessZoneEnd = 0.65f` 임시값 사용. tasks.md "TBD" 명시 항목. 스탯 설계 확정 후 조정 필요.

D-05 [DECISION] `RoundIndicatorView.SetRoundResult`에서 성공/실패 표시를 컬러 변경(green/red)으로 구현함. 추후 스프라이트 교체 가능하도록 Inspector에서 `Image[]` 참조 유지.
