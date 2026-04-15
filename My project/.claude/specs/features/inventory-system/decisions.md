# InventorySystem — Decisions

Record any judgment calls not covered by the Spec here.
Tags: [DECISION], [BACKLOG], [SPEC-GAP]

---

D-01 [DECISION] GetPotion() null 체크를 try-catch로 대체
Tasks.md UseItem Step 2에서 "if null, return Fail(NotUsable)"로 명시하나, ShopMasterDataRepository.GetPotion()은 null 반환이 아닌 InvalidOperationException throw. IShopMasterDataRepository 인터페이스의 null 반환 계약을 보장할 수 없으므로 UseItem, GetSlotDisplayData, GetItemDetail 모두 try-catch로 구현. GetSlotDisplayData에서 예외 발생 시 빈 슬롯으로 표시.

D-02 [SPEC-GAP] GameContext에 ResetAllRunData() 또는 Reincarnation 메서드 없음
Tasks.md Task 12에서 "ResetAllRunData() (or reincarnation method): Add _inventoryRepo.ResetRunData()" 명시. 현재 GameContext.cs에는 해당 메서드가 존재하지 않음. InventoryRepository.ResetRunData()는 구현 완료했으나 GameContext에서 호출하는 Reset 메서드가 없어 연결하지 못함. 해당 메서드가 추가될 때 _inventoryRepo.ResetRunData() 연결 필요.

D-03 [SPEC-GAP] UseItem 시 Hp가 MaxHp를 초과할 수 있음
Tasks.md에서 Hp 포션 사용 시 MaxHp 상한 처리 미정의. 현재 구현에서는 runData.Hp += potion.EffectValue 직접 적용으로 MaxHp 초과 가능. Phase 6 이후 스펙 확정 필요.

D-04 [DECISION] CharacterRunRepository 스탯 직접 수정 패턴
ICharacterRunRepository에 스탯 증가 전용 메서드가 없으므로, RunData 프로퍼티를 통해 필드 직접 수정 후 MarkDirty() 호출 패턴 사용. 기존 MiniGameUseCase 등에서도 동일 패턴 사용 확인.
