**상태:** ⏳ 작성 중
**기능:** EndingScene
**Patch ID:** Patch-005
**Type:** spec-change
**관련 문서:** ReplayScene Specify v1.1.0 / Plan v1.0.0 (선행 작업 요청), EndingScene Specify v2.0.0 §8-2 Scene Transition Flow

---

## 배경

ReplayScene 구현을 위해 EndingScene 측에 2가지 변경이 필요하다.

1. **LastRunResult 저장**: EndingScene이 런 종료 시점에 어떤 결과(GameOver/Ending)로 종료되었는지를 `CharacterRunData.LastRunResult`에 기록해야 한다. ReplayScene/SplashScene이 진입 시점에 이 값을 참조해서 재시작 흐름을 분기하기 위함이다. LastRunResult 필드 및 enum은 CharacterRepository Patch-003에 의해 이미 도입된 상태.

2. **Restart 버튼 씬 전환 대상 정정**: EndingScene Specify v2.0.0 §8-2에 "EndingScene → ReplayScene: Restart button"이 정식 명시되어 있으나, 현재 코드는 ReplayScene 미구현 시점의 임시 동작으로 `SceneKey.Main`으로 이동한다 (EndingPresenter.HandleRestart에 TODO 주석으로 표기). 이번 Patch에서 Specify 정식 상태로 복원한다.

---

## 변경 사항

### 1) EndingUseCase.cs (CompleteEnding 본문 수정)

**경로:** `Assets/_Game/Features/Ending/Domain/EndingUseCase.cs`

**변경 방향:**

`CompleteEnding()` 메서드 기존 동작(해금 기록, 메인씬 변경 키 저장 등)을 유지한 채, **LastRunResult 저장 로직을 추가**한다.

```csharp
public void CompleteEnding()
{
    // 기존: 해금 기록, 메인씬 변경 키 저장 등 (유지)
    // ...

    // 신규: LastRunResult 저장
    var runData = _characterRunRepo.RunData;
    runData.LastRunResult = _currentEnding.IsGameOver
        ? LastRunResult.GameOver
        : LastRunResult.Ending;
    _characterRunRepo.MarkDirty();

    // 실제 디스크 저장은 파이프라인 내의 SaveAllDataSync/SaveDataAsync에서 처리
}
```

**설계 원칙:**
- Domain 레이어(UseCase)에서 도메인 이벤트(런 종료)를 처리하는 자연스러운 배치 (Constitution §3, ReplayScene Plan RQ-P10 결정과 일관)
- MarkDirty() 후 실제 저장 시점은 기존 파이프라인을 따름 (CharacterRepository Patch-001에서 도입된 MarkDirty 메서드 활용, ReplayScene Plan §4-5와 동일)

> 실제 필드명(`_characterRunRepo` vs `_characterRepo` 등), `_currentEnding` 접근 경로, RunData 접근 방식(property vs method)은 Claude Code가 기존 EndingUseCase 코드 확인 후 정확히 매핑할 것.

### 2) EndingPresenter.cs (HandleRestart 씬 전환 대상 변경)

**경로:** `Assets/_Game/Features/Ending/Presentation/EndingPresenter.cs`

**변경 방향:**

```csharp
// 기존
private void HandleRestart()
{
    _gameContext.PendingEndingContext = null;
    _sceneNavigator.NavigateToAsync(SceneKey.Main).Forget();
}

// 신규
private void HandleRestart()
{
    _gameContext.PendingEndingContext = null;
    _sceneNavigator.NavigateToAsync(SceneKey.Replay).Forget();
}
```

**변경 포인트:** `SceneKey.Main` → `SceneKey.Replay`

> 실제 메서드 본문은 Claude Code가 코드 확인 후 정확히 반영. 씬 전환 호출 방식(`.Forget()` 유무 등)은 기존 코드 패턴을 따를 것. `PendingEndingContext` 초기화 등 기존 로직은 유지.

### 3) TODO 주석 제거

**경로:** `Assets/_Game/Features/Ending/Presentation/EndingPresenter.cs`

**조치:**

`HandleRestart` 메서드 근처의 TODO 주석을 제거한다. 예:

```
// TODO: ReplayScene/SplashScene 구현 시 RunData 리셋 로직 연결
```

> 실제 주석 문구는 현재 코드와 다를 수 있다. Claude Code가 HandleRestart 근처 "TODO" 키워드를 grep으로 찾아 ReplayScene/SplashScene 관련 주석을 제거. 다른 용도의 TODO는 건드리지 말 것.

---

## 참조 파일

- ReplayScene Specify-MD v1.1.0
- ReplayScene Plan-MD v1.0.0 §4-5 (EndingScene Patch 의사코드)
- ReplayScene Plan-MD v1.0.0 §11-6 (선행 패치 적용 순서)
- EndingScene Specify-MD v2.0.0 §8-2 (Scene Transition Flow — Restart → ReplayScene 정식 명시)
- CharacterRepository Patch-003 (LastRunResult enum 및 필드 도입)
- CharacterRepository Patch-001 (MarkDirty() 메서드 도입)

---

## 검증 항목

- [ ] `EndingUseCase.CompleteEnding()` 실행 시 `CharacterRunData.LastRunResult`가 올바르게 설정됨
  - IsGameOver == true → `LastRunResult.GameOver`
  - IsGameOver == false → `LastRunResult.Ending`
- [ ] `CompleteEnding()` 실행 후 `CharacterRunRepository._isDirty == true` (MarkDirty 호출 확인)
- [ ] 기존 CompleteEnding 동작(해금 기록, 메인씬 변경 키 저장 등)이 그대로 유지됨
- [ ] `EndingPresenter.HandleRestart()` 실행 시 `SceneKey.Replay`로 씬 전환
- [ ] ReplayScene이 Build Settings에 등록되어 있으면 씬 로드 정상 동작 (미등록 시 검증은 ReplayScene Tasks 구현 단계에서 처리)
- [ ] HandleRestart 근처 ReplayScene/SplashScene 관련 TODO 주석이 제거됨
- [ ] 게임오버 엔딩 진입 → MainScene 재진입 시 `CharacterRunData.LastRunResult == GameOver` (저장/로드 사이클 확인, 검증 환경에 따라 Hak 수동 확인)
- [ ] 정상 엔딩 진입 → MainScene 재진입 시 `CharacterRunData.LastRunResult == Ending`

---

## Claude Code Implementation Guide

- Read CLAUDE.md first
- Files to modify:
    - `Assets/_Game/Features/Ending/Domain/EndingUseCase.cs`
    - `Assets/_Game/Features/Ending/Presentation/EndingPresenter.cs`
- Files to reference:
    - `Assets/_Game/Features/Character/MasterData/LastRunResult.cs` (CharacterRepository Patch-003로 도입된 enum)
    - `Assets/_Game/Features/Character/Data/CharacterRunData.cs` (LastRunResult 필드 존재 확인)
    - `Assets/_Game/Features/Character/Domain/ICharacterRunRepository.cs` (MarkDirty 메서드 존재 확인)
    - `Assets/_Game/Core/Navigation/SceneKey.cs` (SceneKey.Replay 존재 확인 — 이미 v2.0.0 작업 과정에서 추가되었을 가능성, 없으면 확인 필요)
- Before implementing, verify current EndingUseCase.CompleteEnding() body structure and preserve all existing behavior
- LastRunResult save logic placement: directly inside CompleteEnding() per ReplayScene Plan RQ-P10 (Domain layer responsibility)
- Use `MarkDirty()` instead of calling `SaveDataAsync()` — actual disk save happens later in the pipeline
- DO NOT create files outside `Assets/_Game/`
- If `SceneKey.Replay` does not exist yet, record as [SPEC-GAP] in `.claude/specs/features/ending-scene/decisions.md` — but ReplayScene Tasks will add it, so this is expected to be pre-existing or added soon
- If you make any judgment calls not covered by this Patch, record them in `.claude/specs/features/ending-scene/decisions.md` with appropriate tags ([DECISION], [BACKLOG], [SPEC-GAP])