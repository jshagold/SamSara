# CharacterRepository — Decisions

**Version:** 1.2.0 | **Date:** 2026-04-06 | **Status:** ✅ Complete
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

---

## D-03 [DECISION] — Patch-002 IsReincarnationPending 필드 패턴

**Date:** 2026-04-06

**Context:** Patch-002는 `[JsonProperty] private bool _isReincarnationPending` + public property 패턴을 명시했으나,
기존 CharacterRunData의 모든 필드는 `public bool/int/string FieldName;` 직접 선언 방식을 사용 중.

**Decision:** "Read existing patterns and write consistently" 지시에 따라 `public bool IsReincarnationPending;` 단일 필드로 추가.
Newtonsoft.Json은 저장 파일에 해당 키가 없을 경우 bool 기본값 `false`로 역직렬화하므로 하위 호환성 유지됨.

---

## D-04 [SPEC-GAP] — Patch-003 Files to modify에 ICharacterRunRepository.cs 누락

**Date:** 2026-05-04

**Context:** Patch-003 §3에서 `CharacterRunRepository.InitializeNewRun` 시그니처를
`(RunConfigSO config)` → `(RunConfigSO config, int? overrideEvolutionNodeId = null)`로 변경 명시.
Validation §line 106에 "compiles without errors" 검증 항목도 명시.
그러나 "Claude Code Implementation Guide" §line 121-123 "Files to modify" 목록에는
구현 클래스(`Data/CharacterRunRepository.cs`)와 데이터 클래스(`Data/CharacterRunData.cs`)만 포함되고
**인터페이스 파일(`Domain/ICharacterRunRepository.cs`)은 누락됨**.

**Problem:** C#은 클래스가 인터페이스를 구현 중일 때, optional parameter가 추가된 시그니처를
별개 오버로드로 취급하여 인터페이스 계약 위반(CS0535)이 발생함.
구현 커밋(`aac7142`)은 "Files to modify" 리스트만 따라 인터페이스를 미수정하여 CS0535 발생.
약 2주간 미발견 상태로 잠재했고, ReplayScene 구현 시점(2026-05-04)에 GameContext.ResetRunForReplayAsync에서 두-인자 호출이 추가되며 컴파일 에러로 표면화.

**Root cause (이중 결함):**
1. **Spec 작성 시점:** §3 본문에서 시그니처 변경을 명시했으나 그 변경이 인터페이스 계약에 미치는 영향(인터페이스도 함께 수정 필요)을 누락.
2. **구현 시점(aac7142):** "Files to modify" 리스트를 명령서로만 읽고, 클래스가 `ICharacterRunRepository`를 구현 중이라는 사실에 대한 의미적 추론 부재. Validation §106의 "compiles without errors" 항목을 실제 빌드로 검증하지 않음. CLAUDE.md SPEC-GAP 정책(`[SPEC-GAP]` 태그로 decisions.md 기록)도 트리거되지 못함.

**Decision:** 
- `Domain/ICharacterRunRepository.cs:9`에 optional 파라미터 추가하여 즉시 복구 (2026-05-04 적용).
- `patch-003.md`에 retroactive 정정 박스 추가하여 누락 사실 명시.
- 향후 패치 작성 규칙: **"클래스 시그니처 변경 시 해당 클래스가 구현하는 인터페이스 파일도 'Files to modify'에 반드시 포함"** 을 패치 작성 체크리스트에 추가 검토 필요.