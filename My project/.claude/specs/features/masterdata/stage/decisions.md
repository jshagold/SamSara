# Stage MasterData — Decisions

**Feature:** Stage MasterData | **Date:** 2026-04-19

---

--- Patch-002 ---

D-01 [DECISION] BattleNodeDataSO에 boss-flag 필드 없음 — 제거 불필요
  - Patch-002 스펙: "boss-flag-style 필드가 있으면 제거하라. 파일을 먼저 읽어 존재 여부 확인."
  - 파일 확인 결과 BattleNodeDataSO.cs에 _isBoss 또는 유사 필드가 없었음
    (EndingScene Patch-004에서 이미 제거됨).
  - 제거 작업 없이 필드 추가만 수행.

D-02 [DECISION] EndingCandidateSlot을 [Serializable] 클래스로 구현 (ScriptableObject 아님)
  - BattleNodeDataSO.[SerializeField] 필드로 직접 내장되므로 ScriptableObject 불가.
  - [Serializable] 일반 클래스로 구현 → Inspector에서 인라인 펼침(foldout) 표시됨.
  - EndingCondition, EnemySpawn 등 기존 인라인 직렬화 패턴과 동일.

D-03 [DECISION] EndingCandidateSlot.cs를 Ending/MasterData/ 위치에 생성 (T1 전제조건 처리)
  - EndingScene Tasks-MD v2.0.0 T1이 EndingCandidateSlot 생성을 요구하나 파일이 없었음.
  - Stage MasterData Patch-002 작업 범위에서 T1을 함께 처리.
  - 네임스페이스: Samsara.Features.Ending.MasterData (BattleNodeDataSO가 참조하는 위치).
