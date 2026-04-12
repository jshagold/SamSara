# Event MasterData — Decisions

D-01 [DECISION] Unity는 `Nullable<T>`를 직렬화하지 못하므로 `StatType?` 대신 `_useStatType (bool)` + `_statType (StatType)` 쌍으로 구현함. 공개 프로퍼티 `StatType`은 `StatType?`를 반환하여 Spec 인터페이스를 유지. 디자이너는 Inspector에서 `_useStatType = true`로 설정해야 StatChange 결과에서 stat이 적용됨.

D-02 [DECISION] `EventResult`에서 `StatType` enum을 `Features/Character/MasterData`에서 직접 참조함. Patch-001 지시에 따른 것이며, MasterData 데이터 레이어 간 enum 참조이므로 로직 의존성 없음. Cross-feature 통신 규칙(§4)은 Domain/Logic 레이어에 적용되며 MasterData enum 공유는 허용 범위로 판단.
