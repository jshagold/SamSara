# StageScene — Patch-001

## Patch ID: Patch-001
## Type: bugfix
## Related: Constitution §7 (Reset() Auto-Assignment)

---

## Context

Per Constitution §7, MonoBehaviour View classes must implement Reset() for editor-time auto-assignment using GetComponentInChildren<T>(). This requirement was omitted from Tasks, resulting in all StageScene View files missing the Reset() method.

---

## Changes Required

Add Reset() method to all View files listed below. Each file's [SerializeField] fields should be auto-assigned via GetComponentInChildren<T>(). Constitution §7 exception: Transform and RectTransform are NOT auto-assigned.

- Assets/_Game/Features/StageScene/Presentation/StageView.cs
- Assets/_Game/Features/StageScene/Presentation/TopBar/DayView.cs
- Assets/_Game/Features/StageScene/Presentation/TopBar/BackButtonView.cs
- Assets/_Game/Features/StageScene/Presentation/TopBar/OptionButtonView.cs
- Assets/_Game/Features/StageScene/Presentation/Background/BackgroundView.cs
- Assets/_Game/Features/StageScene/Presentation/Map/NodeView.cs
- Assets/_Game/Features/StageScene/Presentation/Map/NodeMapView.cs
- Assets/_Game/Features/StageScene/Presentation/Map/CharacterMarkerView.cs
- Assets/_Game/Features/StageScene/Presentation/Popup/StageCompletePopupView.cs

---

## Files to Reference
- Constitution §7 (Reset() Auto-Assignment)
- Assets/_Game/Features/MainScene/Presentation/MainView.cs (existing Reset() pattern reference)

---

## Verification
- [ ] No compile errors in Unity console
- [ ] Each View auto-assigns SerializeField fields when Reset is executed in editor

---

## Claude Code Implementation Guide
- Read CLAUDE.md first before any modification
- Files to modify: all 9 View files listed above
- Files to reference: MainScene MainView.cs for existing Reset() pattern
- For each View file: add private void Reset() method that assigns each [SerializeField] field using GetComponentInChildren<T>() or GetComponentsInChildren<T>()
- Exception: Do NOT auto-assign Transform or RectTransform fields (Constitution §7)
- DO NOT create files outside Assets/_Game/
- If you make any judgment calls not covered by this Patch, record them in .claude/specs/features/stage-scene/decisions.md with [DECISION] tag