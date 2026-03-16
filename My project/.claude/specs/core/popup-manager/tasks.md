# PopupManager — Tasks

**Version:** 1.0.0 | **Date:** 2026-03-16 | **Status:** Ready for Claude Code
**Based on:** Specify v1.0.0 / Plan v1.0.0

---

## Pre-Implementation Checklist

> Claude Code MUST verify the following before starting any task.

- [ ] Read `CLAUDE.md` first
- [ ] Confirm `Assets/_Game/Core/Popup/` folder exists (create if missing)
- [ ] Confirm `Assets/_Game/App/` folder exists
- [ ] Confirm `Assets/_Game/App/Popup/` folder exists (create if missing)
- [ ] Confirm `Assets/_Game/App/Popup/Prefabs/` folder exists (create if missing)
- [ ] DO NOT modify any files not explicitly listed below

---

## TASK-01 — PopupRequest.cs

**Path:** `Assets/_Game/Core/Popup/PopupRequest.cs`
**Type:** Pure C# Class (no MonoBehaviour)
**Priority:** First (no dependencies)

### Implementation Requirements

- namespace: `Samsara.Core.Popup`
- 4 get-only properties: `Title`(nullable), `Message`(required), `ConfirmText`, `CancelText`
- Constructor overloads:

      PopupRequest(string message, string confirmText = "확인", string cancelText = "취소")
        → Title set to null

      PopupRequest(string title, string message, string confirmText = "확인", string cancelText = "취소")
        → All properties assigned

### Rules
- No UnityEngine import.
- No MonoBehaviour.
- Defaults via constructor optional parameters only, no internal hardcoding.

---

## TASK-02 — IPopupView.cs

**Path:** `Assets/_Game/Core/Popup/IPopupView.cs`
**Type:** Pure C# Interface
**Priority:** After TASK-01

### Implementation Requirements

- namespace: `Samsara.Core.Popup`
- Members:
  - `void Setup(PopupRequest request, bool showCancelButton)`
  - `event Action<bool> OnResult`
  - `GameObject GameObject { get; }` — for SetActive/Transform access during pooling
  - `void ResetView()` — cleanup on pool release

### Rules
- Interface declaration only — no implementation logic.
- **Exception:** Located in Core but requires `UnityEngine` import for `GameObject` property. Record this decision in decisions.md.

---

## TASK-03 — IPopupManager.cs

**Path:** `Assets/_Game/Core/Popup/IPopupManager.cs`
**Type:** Pure C# Interface
**Priority:** After TASK-01 (can parallel with TASK-02)

### Implementation Requirements

- namespace: `Samsara.Core.Popup`
- using: `System.Threading`, `Cysharp.Threading.Tasks`
- Methods:
  - `UniTask<bool> ShowConfirmAsync(PopupRequest request, CancellationToken ct = default)`
  - `UniTask<bool> ShowYesNoAsync(PopupRequest request, CancellationToken ct = default)`
  - `void DismissAll()`

### Rules
- NO UnityEngine import — strictly forbidden.
- Interface declaration only — no implementation logic.

---

## TASK-04 — CommonPopupView.cs

**Path:** `Assets/_Game/App/Popup/CommonPopupView.cs`
**Type:** MonoBehaviour, implements IPopupView
**Priority:** After TASK-02

### Implementation Requirements

- namespace: `Samsara.App.Popup`
- Extends `MonoBehaviour`, implements `IPopupView`

**SerializeField UI Elements:**
- `_titleText: TMP_Text`, `_messageText: TMP_Text`
- `_confirmButton: Button`, `_cancelButton: Button`
- `_confirmButtonText: TMP_Text`, `_cancelButtonText: TMP_Text`

**Setup(PopupRequest request, bool showCancelButton):**
- If Title is null, deactivate titleText object
- Bind each text field with request values
- Activate/deactivate cancelButton based on showCancelButton
- confirmButton.onClick → `OnResult?.Invoke(true)`
- cancelButton.onClick → `OnResult?.Invoke(false)`

**ResetView():**
- Both Buttons' `onClick.RemoveAllListeners()`
- `OnResult = null`

**Reset() (Editor auto-assign, Constitution §7):**
- `GetComponentsInChildren<TMP_Text>(true)` / `GetComponentsInChildren<Button>(true)` array assignment
- Transform/RectTransform excluded (Constitution §7 exception)

### Rules
- `[SerializeField] private` usage (no public fields, Constitution §8).
- No `if(component != null)` defensive coding (Constitution §7 Fail Fast).

---

## TASK-05 — PopupManager.cs

**Path:** `Assets/_Game/App/PopupManager.cs`
**Type:** Pure C# Class (NOT MonoBehaviour)
**Priority:** After TASK-03, TASK-04

### Implementation Requirements

- namespace: `Samsara.App`
- Implements `IPopupManager`
- Log tag: `private readonly string _logClass = $"[{nameof(PopupManager)}]";`

**Fields:**
- `_pool: ObjectPool<CommonPopupView>` (readonly)
- `_popupCanvasRoot: Transform` (readonly)
- `_prefab: CommonPopupView` (readonly)
- `_dimBackground: GameObject` (readonly)
- `_activePopup: CommonPopupView` (nullable)
- `_activeUTCS: UniTaskCompletionSource<bool>` (nullable)

**Constructor:** `PopupManager(Transform popupCanvasRoot, CommonPopupView prefab)`

Constructor performs 3 tasks:

(1) Object Pool initialization — `new ObjectPool<CommonPopupView>(...)`

    createFunc:      Instantiate(prefab, canvasRoot) → SetActive(false)
    actionOnGet:     SetActive(true) → SetAsLastSibling()
    actionOnRelease: SetActive(false)
    actionOnDestroy: Object.Destroy()
    defaultCapacity: 1, maxSize: 3

(2) Dim background creation — "DimBackground" with RectTransform + Image under canvasRoot

    - SetParent to canvasRoot (worldPositionStays: false)
    - RectTransform: anchorMin=zero, anchorMax=one, sizeDelta=zero (full screen)
    - Image: color = (0,0,0,0.5f), raycastTarget = true
    - Default state: SetActive(false)

(3) Field assignment — `_popupCanvasRoot`, `_prefab`, `_logClass`

**ShowConfirmAsync / ShowYesNoAsync:**
- Both delegate to internal helper `ShowPopupInternalAsync`
- Confirm → showCancelButton: false, YesNo → showCancelButton: true

**ShowPopupInternalAsync (private):**
- DismissActive() if existing popup
- Dim activate → Pool Get → view.Setup() → UniTaskCompletionSource create
- Subscribe view.OnResult → log output
- If ct is not None, ct.Register(() => DismissActive())
- await _activeUTCS.Task → return result

**OnPopupResult (private):**
- _activeUTCS.TrySetResult(result) then DismissActive()

**DismissActive (private):**
- If _activePopup is null, return immediately
- Local backup popup/utcs → null out fields
- popup.ResetView() → _pool.Release(popup) → Dim deactivate
- utcs.TrySetResult(false) if incomplete (forced close returns false)

**DismissAll:**
- DismissActive() then log output

### Rules
- Must NOT extend MonoBehaviour.
- `new` keyword is ONLY used by GlobalBootstrapper.
- No `GetComponent`, `FindObjectOfType` (Constitution §8).
  Exception: Dim background RectTransform/Image in constructor (directly created object).

---

## TASK-06 — CommonPopupView.prefab (Unity Editor Manual Work)

**Owner:** Developer (outside code generation scope)
**Priority:** After TASK-04

### Prefab Structure

    CommonPopupView (GameObject + CommonPopupView.cs + CanvasGroup)
    └── Panel (Image, popup background)
        ├── TitleText (TMP_Text)
        ├── MessageText (TMP_Text)
        └── ButtonArea (HorizontalLayoutGroup)
            ├── ConfirmButton (Button)
            │   └── ConfirmButtonText (TMP_Text)
            └── CancelButton (Button)
                └── CancelButtonText (TMP_Text)

### Steps
1. Create UI objects per structure above
2. Attach `CommonPopupView.cs` component
3. Connect SerializeField (or use Reset button for auto-assign)
4. Save as prefab in `Assets/_Game/App/Popup/Prefabs/`
5. Connect prefab reference to GlobalBootstrapper (SerializeField)

---

## TASK-07 — GlobalBootstrapper Integration Check

**Priority:** After TASK-05
**Action:** Verify existing GlobalBootstrapper code + modify if needed.

If `Assets/_Game/App/GlobalBootstrapper.cs` exists, verify:
1. `[SerializeField] private CommonPopupView _popupViewPrefab` field
2. PopupCanvas creation logic
3. `new PopupManager(canvasRoot, _popupViewPrefab)` instantiation
4. IPopupManager registered in GameContext

If any missing, record in decisions.md.

PopupCanvas creation reference:

    1. new GameObject("PopupCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster))
    2. SetParent under GlobalBootstrapper transform
    3. Canvas: renderMode = ScreenSpaceOverlay, sortingOrder = 100
    4. CanvasScaler: ScaleWithScreenSize, referenceResolution = 1080x1920
    5. new PopupManager(popupCanvasGO.transform, _popupViewPrefab) → register in GameContext

If `GlobalBootstrapper.cs` does not yet exist:
Record in decisions.md and proceed (separate Spec).

---

## TASK-08 — Validation

**Priority:** Final — after all tasks complete.

| # | Check | Method |
|---|---|---|
| V-01 | PopupRequest has no UnityEngine import | Code review |
| V-02 | IPopupManager has no UnityEngine import | Code review |
| V-03 | PopupManager does NOT extend MonoBehaviour | Code review |
| V-04 | CommonPopupView implements IPopupView | Code review |
| V-05 | Object Pool works correctly (Get/Release) | Play Mode test |
| V-06 | ShowConfirmAsync: popup displays + true on confirm | Play Mode test |
| V-07 | ShowYesNoAsync: true/false per button | Play Mode test |
| V-08 | DismissAll: popup closed + Dim deactivated | Play Mode test |
| V-09 | Dim background blocks underlying UI touches | Play Mode test |
| V-10 | No files created outside `Assets/_Game/` | File tree check |

---

## Claude Code Implementation Guide

- Read `CLAUDE.md` first before any implementation.
- **Files to create:**
  - `Assets/_Game/Core/Popup/PopupRequest.cs`
  - `Assets/_Game/Core/Popup/IPopupView.cs`
  - `Assets/_Game/Core/Popup/IPopupManager.cs`
  - `Assets/_Game/App/Popup/CommonPopupView.cs`
  - `Assets/_Game/App/PopupManager.cs`
- **Files to reference:**
  - `Assets/_Game/App/GlobalBootstrapper.cs` (if exists)
  - `Assets/_Game/App/GameContext.cs` (if exists)
- **Prefab (manual creation required):**
  - `Assets/_Game/App/Popup/Prefabs/CommonPopupView.prefab`
- **Implementation order:** TASK-01 → 02 → 03 → 04 → 05 → 06(manual) → 07 → 08
- **DO NOT** create files outside `Assets/_Game/`.
- **DO NOT** make PopupManager a MonoBehaviour.
- **DO NOT** reference or copy patterns from `Assets/_Game/Dev/`.
- If you make any judgment calls not covered by this Spec,
  record them in `.claude/specs/core/popup-manager/decisions.md`