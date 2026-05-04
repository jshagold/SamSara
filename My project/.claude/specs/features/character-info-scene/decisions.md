# CharacterInfoScene — Decisions

D-01 [DECISION] SceneKey.cs 수정 스킵 — CharacterInfo, EvolutionTree 이미 존재.
**Why:** Tasks v1.0.0 Task 1에서 SceneKey에 CharacterInfo, EvolutionTree 추가를 요구했으나, 실제 파일에 이미 두 값이 존재함.
**How to apply:** Task 1 건너뜀. 나머지 Task는 정상 진행.

D-02 [DECISION] StatListView.Reset() 미구현 — Inspector 수동 배정 필요.
**Why:** 4개의 TMP_Text(_hpText, _strengthText, _toughnessText, _agilityText)를 GetComponentsInChildren<TMP_Text>()로 자동 배정하면 계층 순서에 의존하게 됨. 순서가 어긋날 경우 스탯이 잘못된 텍스트에 표시되는 버그가 발생하며, 런타임에서 발견하기 어려움.
**How to apply:** Unity Editor Inspector에서 StatListView 컴포넌트의 4개 TMP_Text 필드를 올바른 텍스트 오브젝트에 수동 연결.

D-03 [DECISION] SkillDescriptionPopupView.Reset()에서 TMP_Text 4개 자동 배정 제외.
**Why:** _skillNameText, _skillDescriptionText, _damageText, _effectText 4개의 TMP_Text는 GetComponentsInChildren 순서가 팝업 계층 구성에 의존하여 내용 오할당 위험. _closeButton, _skillIconImage만 자동 배정.
**How to apply:** Inspector에서 TMP_Text 4개 수동 연결.

D-04 [DECISION] CharacterInfoPresenter.Initialize()를 동기 메서드로 구현 (Phase 1).
**Why:** Phase 1은 ISpriteLoader를 주입하지 않아 모든 초기화 작업이 동기적임. async UniTaskVoid 패턴은 Phase 2(스프라이트 로딩 추가 시)에서 적용 예정.
**How to apply:** Phase 2에서 ISpriteLoader를 Presenter 생성자에 추가하고 Initialize()를 InitializeAsync().Forget() 패턴으로 전환.

D-05 [DECISION] Phase 1에서 Sprite 로딩 생략 — CharacterSprite, SkillIcon, EvolutionIcon 모두 null.
**Why:** Tasks 스펙에 CharacterInfoPresenter 생성자 파라미터에 ISpriteLoader가 없음. Phase 1 scope 명시("Phase 1: placeholder"). Bootstrapper에서도 ISpriteLoader 전달 없음.
**How to apply:** Phase 2에서 ISpriteLoader를 Presenter에 주입하고, Initialize()를 async로 전환하여 sprite 로딩 추가.

D-06 [DECISION] InventoryView를 Phase 1 stub에서 Phase 2 실구현으로 업그레이드.
**Why:** CharacterInfoScene에서 인벤토리 슬롯 클릭 시 아이템 상세 팝업이 표시되지 않는 버그. Phase 1 InventoryView는 GameObject[] _inventorySlots만 보유하고 클릭 이벤트 없음. InventoryUseCase.GetSlotDisplayData() / GetItemDetail()은 이미 구현 완료 상태여서 View 레이어만 추가하면 됨.
**How to apply:** InventorySlotView(슬롯 단위) + InventoryView(집계) + ItemDetailPopupView(팝업)를 새로 추가. InventoryView._slots는 Inspector 수동 배정(Reset() 미구현, 이유: D-02와 동일 — 순서 의존 위험).

D-07 [DECISION] ItemDetailPopupView의 TMP_Text 4개 Reset() 자동 배정 제외.
**Why:** _itemNameText, _descriptionText, _effectText, _quantityText는 팝업 계층 구성에 따라 순서가 달라질 수 있어 GetComponentsInChildren 자동 배정 시 내용 오할당 위험 (D-03과 동일한 사유).
**How to apply:** Inspector에서 TMP_Text 4개 수동 연결. _closeButton만 Reset()에서 자동 배정.

D-08 [DECISION] CharacterInfoPresenter 생성자에 InventoryUseCase 추가 — 슬롯 초기화 및 팝업 표시 담당.
**Why:** 인벤토리 팝업 기능을 Presenter에 추가하면서 InventoryUseCase 의존성 필요. 기존 CharacterInfoUseCase는 캐릭터 도메인(스탯, 스킬, 진화)만 담당하고 인벤토리 도메인은 InventoryUseCase의 책임이므로 별도 주입이 적절.
**How to apply:** CharacterInfoSceneBootstrapper에서 gameContext.InventoryUseCase를 취득해 Presenter 생성자 2번째 인자로 전달.

D-09 [DECISION] ItemDetailPopupView에 사용/버리기 버튼 추가 — InventoryUseCase.UseItem/DiscardItem 연동.
**Why:** 초기 구현에서 닫기 버튼만 포함했으나 InventoryUseCase에 UseItem/DiscardItem이 이미 구현되어 있음에도 View에 해당 버튼이 없어 기능이 노출되지 않는 문제. 아이템 상세 팝업은 정보 표시뿐 아니라 액션 진입점이어야 함.
**How to apply:** _useButton, _discardButton 필드 추가. Inspector에서 수동 배정 (Reset() 자동 배정 제외 — 버튼 순서 모호성).

D-10 [DECISION] 버리기는 YesNo 확인 팝업 선행, 사용은 즉시 실행 후 결과 Confirm 팝업.
**Why:** 버리기는 복구 불가 액션이므로 실수 방지를 위해 확인 필요. 아이템 사용은 상세 화면에서 의도적으로 진입하는 액션이므로 이중 확인 불필요 — 결과 피드백(스탯 +N)만 표시.
**How to apply:** DiscardItemAsync()에서 ShowYesNoAsync 선행. UseItemAsync()에서 즉시 실행 후 ShowConfirmAsync로 피드백.

D-11 [DECISION] 사용/버리기 완료 후 RefreshStats()/RefreshInventory() 호출로 화면 즉시 갱신.
**Why:** 아이템 사용 시 스탯이 변경되고, 버리기/사용 후 슬롯 수량이 감소하므로 StatListView와 InventoryView를 즉시 갱신해야 UI와 데이터 불일치 방지.
**How to apply:** UseItemAsync 완료 후 RefreshStats + RefreshInventory, DiscardItemAsync 완료 후 RefreshInventory만 호출.

D-12 [SPEC-GAP] v2.0.0 인벤토리 작업(commit `df89a16`, 2026-04-15)에서 patch-002의 EvolutionTree 내비게이션이 우발적으로 회귀됨 — 2026-05-04 복구.
**Why:** 
- 2026-04-07 patch-002 적용(commit `b794367`): `HandleEvolutionStageClicked()`를 "Coming Soon" 팝업 stub에서 `_sceneNavigator.NavigateToAsync(SceneKey.EvolutionTree).Forget();`로 교체.
- 2026-04-15 v2.0.0 인벤토리 기능 추가(commit `df89a16`): `CharacterInfoPresenter.cs`를 230줄 규모로 리라이트하면서(126줄 → 251줄) 식별자 일괄 리네임(예: `EvolutionStageButtonView` → `EvolutionStageButton`, `InfoScrollView` → `InfoScroll` 등) 수반. 이 리라이트 과정에서 `HandleEvolutionStageClicked()` 본문이 patch-002 이전 상태("준비 중 / 진화 트리는 준비 중입니다" 팝업)로 되돌아감.
- v2.0.0 spec(`tasks.md` v2.0.0 §1 "Scope: Presentation layer changes only" — 인벤토리 추가만 명시) 및 `decisions.md` D-01~D-11 어디에도 EvolutionTree 내비게이션을 되돌리는 결정/지시 없음. 사용자도 그런 지시를 내린 적 없음을 확인(2026-05-04).
- 즉 v2.0.0 구현자(Claude)가 "파일 수정"을 "파일 재작성"으로 해석하면서 patch-002 변경분을 보존하지 못한 우발적 회귀.
- ReplayScene 작업 중 컴파일 에러(CS0535, CharacterRepository Patch-003 별건) 조사 과정에서 23개 patch 일괄 audit 시 발견(2026-05-04).

**How to apply:**
- `CharacterInfoPresenter.cs:128-132` 본문을 `_sceneNavigator.NavigateToAsync(SceneKey.EvolutionTree).Forget();` 단일 호출로 복구 (2026-05-04 적용).
- 향후 "기존 파일 Modify" 지시 시 구현자는 **재작성이 아니라 최소 차분 패치** 원칙으로 작업해야 함. tasks.md v2.0.0 Prerequisites §2가 이미 "Read the following existing files before making changes"를 명시했음에도 위반된 사례 — 향후 패치 작성 시 "Preserve all existing behavior except the specified additions" 류의 명시 필요 검토.
