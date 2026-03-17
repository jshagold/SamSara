# PopupManager — Decisions

**Version:** 1.0.0 | **Date:** 2026-03-17

---

## DEC-01 — IPopupView에 UnityEngine import 허용

**Context:** `IPopupView`는 Core 레이어 인터페이스이므로 원칙적으로 UnityEngine 의존성을 가지면 안 된다.

**Decision:** `GameObject GameObject { get; }` 프로퍼티를 위해 `using UnityEngine;`을 허용한다.

**Reason:** ObjectPool Get/Release 시 `view.gameObject.SetActive(...)` 및 `view.transform.SetAsLastSibling()`을 `PopupManager`에서 직접 호출하려면 `IPopupView`가 `GameObject`를 노출해야 한다. 이를 통해 `PopupManager`가 `CommonPopupView` 구체 타입에 의존하지 않고 인터페이스만 바라볼 수 있다.

**Trade-off:** Core 레이어의 UnityEngine 순수성 위반이지만, 풀링 아키텍처의 캡슐화 이점이 더 크다.

---

## DEC-02 — 기존 스텁 파일 삭제 및 경로 재구성

**Context:** 구현 전 `Assets/_Game/Core/IPopupManager.cs`(글로벌 네임스페이스)와 `Assets/_Game/App/Systems/PopupManager.cs`(스텁) 파일이 존재했다.

**Decision:** 두 스텁 파일을 삭제하고 Spec에서 지정한 경로에 새로 생성했다.
- 삭제: `Core/IPopupManager.cs`, `App/Systems/PopupManager.cs`
- 생성: `Core/Popup/IPopupManager.cs` (namespace: `Samsara.Core.Popup`)
- 생성: `App/PopupManager.cs` (namespace: `Samsara.App`)

**Reason:** 네임스페이스 충돌 방지 및 폴더 구조를 Spec과 일치시키기 위함.

---

## DEC-03 — GameContext에 IPopupManager 등록

**Context:** TASK-07 요구사항: "IPopupManager registered in GameContext"

**Decision:** `GameContext` 생성자에 `IPopupManager popupManager` 파라미터를 추가하고, `public IPopupManager PopupManager { get; }` 프로퍼티로 노출했다.

**Reason:** GameContext가 프로젝트의 DI 컨테이너 역할을 하므로, 팝업 시스템도 GameContext를 통해 Feature 레이어에 주입될 수 있어야 한다. GlobalBootstrapper가 PopupManager를 생성한 뒤 GameContext 생성자에 전달한다.
