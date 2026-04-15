# ShopSystem — Decisions

D-01 [DECISION] ShopUseCase.ActivateMerchant(int merchantId) 시그니처에서 currentDay 파라미터를 제거하고 내부적으로 ICharacterRunRepository.RunData.Day를 사용함.
**Why:** Tasks 스펙에는 ActivateMerchant(merchantId, currentDay)로 정의되어 있으나, EventPresenter는 CharacterRunRepository에 직접 접근하지 않아 currentDay를 전달할 수단이 없음. ShopUseCase가 이미 ICharacterRunRepository를 보유하므로 내부 조회가 더 깔끔한 DDD 패턴.
**How to apply:** CheckAndExpireMerchant()도 동일하게 인자 없이 내부에서 currentDay를 조회.

D-02 [DECISION] EventDialogue에 parameterized constructor 추가 (EventSO.cs 수정).
**Why:** MerchantDialogueAdapter.ToEventDialogues()가 MerchantDialogue를 EventDialogue로 변환해야 하는데, [SerializeField] private 필드는 외부에서 설정 불가. 생성자 추가가 Unity 직렬화와 충돌하지 않으며 기존 .asset 파일에 영향 없음.
**How to apply:** 코드에서 EventDialogue를 직접 생성할 때는 반드시 parameterized constructor를 사용.

D-03 [DECISION] ChoiceListView에 ShowChoices(string[], Action<int>) 오버로드 추가.
**Why:** 상인 "거래"/"떠나보내기" 선택지는 EventChoice SO 없이 단순 문자열만 필요. 기존 ShowChoices(EventChoice[])와 공존하며 역방향 호환성 유지.
**How to apply:** EventChoice SO 없이 단순 선택지 표시가 필요한 모든 상황에서 재사용 가능.

D-04 [DECISION] EventDialogueOverlayController에 RunDialoguesWithChoicesAsync(EventDialogue[], string[]) 메서드 추가.
**Why:** 기존 RunEventAsync(int eventId)는 EventUseCase를 통해 SO를 로드함. 상인 대화는 SO 없이 직접 대사 데이터를 전달해야 하므로 별도 진입점 필요. 코드 재사용(RunRawDialogueLoopAsync 내부)으로 로직 중복 없음.
**How to apply:** EventSO 없는 동적 대화 시퀀스에서 재사용 가능.

D-05 [DECISION] MaintenanceView.ShowShopMode() 기존 동작을 ShowShopPanelMode()로 교체하고 _shopView(stub) SerializeField를 유지하되 이벤트 연결은 _shopPanelView로 이전.
**Why:** 기존 ShopView stub은 placeholder였으며 ShopPanelView가 실제 구현체. ShopView.cs 파일은 삭제하지 않고 유지하여 Unity 씬 파일 참조 오류를 방지(Manual Work에서 교체).
**How to apply:** Unity Editor 수동 작업 시 씬의 ShopView 컴포넌트를 ShopPanelView로 교체하고 Inspector 재연결 필요.

D-06 [DECISION] MaintenancePresenter.Initialize()를 async UniTaskVoid InitializeAsync()로 변경하여 CheckAndExpireMerchant를 await.
**Why:** CheckAndExpireMerchant는 만료 시 SaveDataAsync를 호출하는 비동기 작업. 씬 초기화 시 완료를 보장해야 ShopButton 표시 상태가 올바르게 설정됨.
**How to apply:** Initialize()는 퍼블릭 동기 진입점을 유지하고 내부에서 Forget()으로 비동기 체인 시작.

D-07 [DECISION] Patch-001: 재고 0 구매버튼 비활성화 제거 — 모든 구매 차단은 팝업(post-block) 방식으로 통일.
**Why:** 재고 0 pre-block(버튼 비활성화)과 골드 부족 post-block(팝업)이 혼재해 UX 불일치 발생. 모든 차단 상황을 팝업으로 통일.
**How to apply:** ShopItemSlotView.Setup()/UpdateStock()에서 SetInteractable 호출 제거. SetInteractable() 메서드도 제거.

D-08 [DECISION] Patch-001: 구매 성공 시 CharacterInfoPanelView(상단 골드)도 즉시 갱신.
**Why:** ShopPanelView 골드만 갱신하고 상단 TopBar GoldView는 갱신하지 않아 씬 재진입 전까지 표시 불일치 발생.
**How to apply:** CharacterInfoPanelView.UpdateGold() → MaintenanceView.UpdateTopBarGold() 체인 추가. MaintenancePresenter 구매 성공 분기에서 호출.

D-09 [DECISION] 구매 차단 조건(재고 없음, 골드 부족)을 구매 확인 팝업 전에 선행 체크한다.
**Why:** 기존 흐름은 "구매 확인" 팝업 → 사용자 확인 클릭 → 차단 팝업 순서였음. 사용자 입장에서 확인 팝업이 닫히면서 아무 피드백도 없는 것처럼 보여 버그로 인식. 재고 없음/골드 부족은 구매 시도 자체가 불가능하므로 확인 팝업을 거칠 이유가 없음.
**How to apply:** ProcessPurchaseAsync에서 GetRemainingStock(potionId) → 재고 0이면 즉시 "재고 없음" return. RunData.Gold < potion.Price이면 즉시 "골드 부족" return. 두 체크 모두 통과한 경우에만 "구매 확인" 팝업 표시.

---

## v2.0.0

D-10 [SPEC-GAP] Tasks v2.0.0 Task 5에서 ShopUseCase DI 변경 대상 파일을 MaintenanceSceneBootstrapper.cs로 명시했으나, 실제 ShopUseCase 생성 위치는 GameContext.cs.
**Why:** MaintenanceSceneBootstrapper는 gameContext.ShopUseCase를 프로퍼티로 가져올 뿐이며 직접 new ShopUseCase(...)를 하지 않음. 스펙 작성 시 파일 명이 잘못 기재된 것으로 판단.
**How to apply:** 수정 대상은 GameContext.cs (Step 2 UseCase 생성 블록). _inventoryUseCase 생성을 _shopUseCase 생성보다 앞으로 이동하고, ShopUseCase 생성자에 _inventoryUseCase를 4번째 인자로 추가.

D-11 [DECISION] PotionSO의 IItemData 구현은 명시적 인터페이스 구현(explicit interface implementation)을 사용한다 (ItemName, ItemType, IconSpriteKey).
**Why:** PotionSO 고유 API(PotionName, SpriteKey)와 IItemData API(ItemName, IconSpriteKey)가 의미상 중복이지만 이름이 다름. 암시적 구현으로 새 프로퍼티를 추가하면 PotionSO 외부 사용자에게 혼란스러운 중복 API가 노출됨. 명시적 구현으로 IItemData 캐스팅 시에만 접근 가능하게 분리.
**How to apply:** IItemData를 파라미터로 받는 코드에서는 IItemData 인터페이스를 통해 접근. PotionSO 직접 참조 코드는 기존 PotionName/SpriteKey 사용 유지.

D-12 [DECISION] ShopUseCase.PurchasePotion에서 InventoryFull 체크를 재고/골드 체크보다 먼저 수행한다.
**Why:** Tasks v2.0.0 명세 준수. 인벤토리가 꽉 찬 상태에서 골드 차감 후 AddItem 실패 시 골드 손실이 발생하는 버그를 방지하기 위해 가장 먼저 체크.
**How to apply:** CanAddItem → OutOfStock → InsufficientGold 순서 유지. AddItem 실패(race condition 등)는 방어적으로 InventoryFull을 반환.

D-13 [DECISION] 인벤토리 가득 참 체크를 ProcessPurchaseAsync의 선행 체크 블록으로 이동했다 (D-09 패턴 적용).
**Why:** switch-case에서 InventoryFull 팝업을 띄우는 방식은 "구매 확인" 팝업이 닫히는 프레임과 새 팝업이 열리는 프레임이 겹칠 때 PopupManager 내부 상태 미초기화로 두 번째 ShowConfirmAsync 호출이 무시되는 타이밍 버그 발생. 재고 없음/골드 부족과 동일하게 "구매 확인" 팝업 이전에 선행 체크하는 것이 올바른 패턴.
**How to apply:** ProcessPurchaseAsync에서 _gameContext.InventoryUseCase.CanAddItem(potionId) 체크를 재고·골드 체크 직후, ShowYesNoAsync 이전에 추가. switch의 InventoryFull case는 race condition 방어용으로 유지.

D-14 [DECISION] MerchantDialogue 클래스와 MerchantDialogueAdapter를 삭제하고, MerchantSO의 대사 필드를 EventDialogue[]로 통일했다.
**Why:** MerchantDialogue는 EventDialogue의 subset(portrait 키 없음)으로, 어댑터 변환 계층이 불필요한 indirection이었음. EventDialogue[]를 MerchantSO에서 직접 사용하면 Inspector에서 portrait 키도 직접 설정 가능하고, 기획 변경 시 EventDialogue 하나만 수정하면 됨.
**How to apply:** MerchantSO.GreetingDialogues/FarewellDialogues는 EventDialogue[] 반환. ShopUseCase.GetGreetingDialogues()도 EventDialogue[] 반환. MaintenancePresenter에서 변환 없이 직결.
**주의:** MerchantSO .asset 파일의 기존 대사 데이터(_text → _dialogueText 필드명 불일치)는 직렬화 초기화됨 — Unity Editor에서 재입력 필요.
**Why:** Tasks v2.0.0 명세 준수. 인벤토리가 꽉 찬 상태에서 골드 차감 후 AddItem 실패 시 골드 손실이 발생하는 버그를 방지하기 위해 가장 먼저 체크.
**How to apply:** CanAddItem → OutOfStock → InsufficientGold 순서 유지. AddItem 실패(race condition 등)는 방어적으로 InventoryFull을 반환.
