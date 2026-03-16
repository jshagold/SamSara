# app-bootstrap — Implementation Decisions

**Date:** 2026-03-12 | **Implementer:** Claude Code (claude-sonnet-4-6)

---

## D-01 — GameContext에서 `using UnityEngine` 허용

**스펙 충돌:** tasks.md TASK-01은 "No `UnityEngine` namespace imports (UniTask excluded)"를 명시하지만,
GlobalBootstrapper의 InitializeAsync()가 `ScriptableObject[]`를 GameContext 생성자에 전달한다
(`_gameContext = new GameContext(masterData)`). `ScriptableObject`는 UnityEngine 타입이므로 import 없이는 컴파일 불가.

**결정:** GameContext에 `using UnityEngine;`을 허용 (단 하나의 예외).
GameContext 내부에서 Unity API(`GameObject`, `Instantiate`, `FindObjectOfType` 등)는 일절 사용하지 않는다.
이 규칙은 "Unity 씬/오브젝트 조작 금지"가 원래 의도이므로, 타입 참조만을 위한 import는 위반으로 보지 않는다.

---

## D-02 — `GlobalBootstrapper.Instance` 및 `GameContext` public 프로퍼티 추가

**스펙 미명시:** tasks.md는 `InitializationTask`만 public 프로퍼티로 명시하고,
`Instance`와 `GameContext` 프로퍼티는 언급하지 않는다.

**결정:** 아키텍처 동작에 필수적이므로 두 프로퍼티를 추가한다.
- `public static GlobalBootstrapper Instance => _instance;`
  SceneBootstrapper가 `await GlobalBootstrapper.Instance.InitializationTask` 패턴으로 사용.
- `public GameContext GameContext => _gameContext;`
  SceneBootstrapper가 DI 조립 시 `GlobalBootstrapper.Instance.GameContext`를 통해 UseCase를 수신.

CLAUDE.md: "SceneBootstrapper는 FeatureBootstrapper를 Start()에서 초기화" — GameContext 없이는 불가능.

---

## D-03 — `InitializeAsync()` 재진입 시 FallbackCanvas.Hide() 호출

**스펙 미명시:** tasks.md는 재시도(retry) 후 성공했을 때 FallbackCanvas를 닫는 코드를 명시하지 않는다.

**결정:** `InitializeAsync()` 진입 시점에 `_fallbackCanvas.Hide()`를 호출한다.
- 최초 실행: canvas가 이미 SetActive(false)이므로 무해.
- 재시도 실행: 사용자가 Retry를 눌러 재진입하면 canvas가 먼저 닫힌다.
이를 통해 V-06 (FallbackCanvas가 IPopupManager 없이 독립 동작)를 유지하면서 UI 상태를 일관성 있게 유지.

---

## D-04 — 스텁 클래스 생성 (missing dependencies)

**스펙 미명시:** tasks.md가 참조하는 아래 타입들이 프로젝트에 존재하지 않았음.

| 타입 | 생성 경로 | 비고 |
|---|---|---|
| `ISceneNavigator` | `Assets/_Game/Core/ISceneNavigator.cs` | 인터페이스 스텁 |
| `IPopupManager` | `Assets/_Game/Core/IPopupManager.cs` | 인터페이스 스텁 |
| `SceneNavigator` | `Assets/_Game/App/Systems/SceneNavigator.cs` | 구체 구현 |
| `PopupManager` | `Assets/_Game/App/Systems/PopupManager.cs` | 구체 구현 (stub, MonoBehaviour 아님) |
| `CharacterRepository` | `Assets/_Game/Features/Character/Data/` | 스텁 |
| `StageRepository` | `Assets/_Game/Features/Stage/Data/` | 스텁 |
| `CharacterUseCase` | `Assets/_Game/Features/Character/Domain/` | 스텁 |
| `EvolutionUseCase` | `Assets/_Game/Features/Character/Domain/` | 스텁 |
| `StageUseCase` | `Assets/_Game/Features/Stage/Domain/` | 스텁 |
| `BattleUseCase` | `Assets/_Game/Features/Battle/Domain/` | 스텁 |
| `EventUseCase` | `Assets/_Game/Features/Event/Domain/` | 스텁 |
| `MiniGameUseCase` | `Assets/_Game/Features/MiniGame/Domain/` | 스텁 |

모든 스텁은 컴파일 통과가 목적이며, 각 Feature 스펙에서 구체 구현으로 교체한다.

---

## D-05 — `PopupManager`를 MonoBehaviour가 아닌 pure C# class로 구현

tasks.md가 `_popupManager = new PopupManager()`으로 생성하므로 MonoBehaviour 불가.
실제 Unity UI 팝업 연결은 Popup Feature 스펙에서 구체화한다.
현재 구현은 `Debug.LogWarning` + `return false`의 no-op stub.

---

## D-06 — `RetryInitialization()` public 메서드 미추가

tasks.md의 GlobalBootstrapper 스펙에 해당 메서드가 없다.
재시도는 FallbackCanvas의 onRetry 콜백(`() => InitializeAsync().Forget()`)을 통해
GlobalBootstrapper 내부에서만 처리된다.
외부(Presenter 등)에서 직접 초기화를 트리거하는 경로는 이 스펙에서 제공하지 않는다.
