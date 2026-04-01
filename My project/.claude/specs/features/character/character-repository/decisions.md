# CharacterRepository — Decisions

**Version:** 1.1.0 | **Date:** 2026-04-01 | **Status:** ✅ Complete
**Feature:** CharacterRepository
**Constitution Ref:** §9

---

## D-01 — InitializeNewRun() does not call SaveDataAsync() internally

**Date:** 2026-03-19

**Context:** Spec (Tasks) specified that InitializeNewRun() should call SaveDataAsync().Forget() internally.

**Problem:** Calling SaveDataAsync().Forget() inside InitializeNewRun() causes IOException: Sharing violation
when the caller also invokes SaveDataAsync() immediately after, resulting in concurrent writes to the same file.

**Decision:** Removed SaveDataAsync().Forget() from InitializeNewRun(). Only sets _isDirty = true.
Save responsibility belongs to the caller (UseCase).

---

## D-02 [DECISION] — Patch-001 MarkDirty() 선행 반영

**Date:** 2026-04-01

**Context:** Patch-001 (MarkDirty 메서드 추가)이 MiniGame Tasks 구현 시 선행 처리됨.
- ICharacterRunRepository.MarkDirty() 인터페이스 추가 — MiniGame D-01 SPEC-GAP 처리 과정에서 반영.
- CharacterRunRepository.MarkDirty() 구현 — 원본 코드에 이미 존재. 인터페이스에 선언만 추가.
- MiniGameUseCase.ApplyResultAndSave() — MarkDirty() 호출 후 SaveDataAsync() 호출 패턴 이미 구현 완료.
- Patch-001 기준 신규 코드 변경 없음. Validation 항목 전부 충족 상태.