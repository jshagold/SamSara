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
