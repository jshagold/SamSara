## D-01 — mutation 메서드는 _isDirty = true만 설정, Save는 UseCase가 호출

**Date:** 2026-03-23
**Context:** V-07 검증 중 mutation을 연속 호출 시 IOException: Sharing violation 발생.
각 mutation이 내부에서 SaveAsync().Forget()을 호출하기 때문에, 빠른 연속 호출 시
복수의 Task가 동시에 동일 파일에 WriteAllText를 시도함.
**Problem:** mutation 내부에서 SaveAsync().Forget()을 호출하면 동시 쓰기 충돌이 발생하며,
이를 막기 위한 _saveInProgress 플래그 등의 동시성 제어는 불필요한 복잡도를 초래함.
**Decision:** CharacterRepository(D-01)와 동일한 방식으로 통일.
mutation 메서드는 _isDirty = true만 설정하고, SaveAsync() 호출 책임은 UseCase에 위임.
Repository는 데이터 상태 관리, 저장 타이밍 결정은 UseCase 레이어의 책임.
