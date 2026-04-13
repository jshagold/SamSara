# ShopSystem — Tasks

**Version:** 1.0.0 | **Date:** 2026-04-13
**Feature:** ShopSystem (Phase 5-B)
**Based on:** Specify v1.0.0 / Plan v1.0.0
**Constitution Ref:** §1~§11 fully verified

---

## Constitution Checklist

| Section | Applied | Details |
|---|---|---|
| §1 Folder Structure | YES | Features/Shop/ with Data/Domain/Presentation/MasterData |
| §2 Zero Guessing | YES | All paths/class names explicit. No Singleton. Data/Logic separation. |
| §3 Bootstrapper | YES | No separate scene. MaintenanceSceneBootstrapper Patch for DI wiring. |
| §4 Feature Module | YES | Clean Architecture layers. Cross-feature via Interface only. |
| §5 Async/Popup | YES | UniTask. Purchase confirmation via IPopupManager. |
| §6 GameContext | YES | ShopMasterDataRepo, ShopRepo, ShopUseCase added. LoadAllDataAsync/SaveAllDataSync extended. |
| §7 UI Standards | YES | Fail Fast, Reset() Auto-Assignment, Component Caching |
| §8 Coding Standards | YES | _camelCase, suffixes, _logClass, SafeCleanup |
| §9 Data Persistence | YES | Save-on-Action, Dual-Mode Saving, Dirty Flag, ThreadPool I/O |
| §10 Wrapper Pattern | YES | IShopRepository -> ShopRepository, IShopMasterDataRepository -> ShopMasterDataRepository |
| §11 Libraries | YES | UniTask, DOTween, TMP, Newtonsoft.Json |

---

## Pre-Implementation Checklist

Claude Code MUST verify the following before starting implementation.

- Read CLAUDE.md first
- Read and understand these existing files:
    - Assets/_Game/Features/Event/MasterData/EventSO.cs (EventResult.MerchantId pattern)
    - Assets/_Game/Features/Event/Presentation/EventPresenter.cs (ShopEncounter handling location)
    - Assets/_Game/Features/Event/Presentation/EventDialogueOverlayController.cs (reuse pattern)
    - Assets/_Game/Features/Event/Presentation/Dialogue/DialogueView.cs (dialogue UI structure)
    - Assets/_Game/Features/Event/Presentation/Dialogue/ChoiceListView.cs (choice UI structure)
    - Assets/_Game/Features/Character/Data/CharacterRunData.cs
    - Assets/_Game/Features/Character/Data/CharacterRunRepository.cs
    - Assets/_Game/Features/Character/Domain/ICharacterRunRepository.cs
    - Assets/_Game/Features/Character/MasterData/StatType.cs
    - Assets/_Game/Features/Maintenance/Presentation/MaintenanceSceneBootstrapper.cs
    - Assets/_Game/Features/Maintenance/Presentation/MaintenancePresenter.cs
    - Assets/_Game/Features/Maintenance/Presentation/MaintenanceView.cs
    - Assets/_Game/App/GameContext.cs
- Create folders:
    - Assets/_Game/Features/Shop/Data/
    - Assets/_Game/Features/Shop/Domain/
    - Assets/_Game/Features/Shop/MasterData/
    - Assets/_Game/Features/Shop/Presentation/
    - .claude/specs/features/shopsystem/

---

## Files to Create/Modify

| # | File | Path | Action |
|---|---|---|---|
| 1 | PotionSO.cs | Assets/_Game/Features/Shop/MasterData/ | Create |
| 2 | MerchantDialogue.cs | Assets/_Game/Features/Shop/MasterData/ | Create |
| 3 | MerchantSaleItem.cs | Assets/_Game/Features/Shop/MasterData/ | Create |
| 4 | MerchantSO.cs | Assets/_Game/Features/Shop/MasterData/ | Create |
| 5 | ShopRunData.cs | Assets/_Game/Features/Shop/Domain/ | Create |
| 6 | PurchaseResult.cs | Assets/_Game/Features/Shop/Domain/ | Create |
| 7 | ShopItemInfo.cs | Assets/_Game/Features/Shop/Domain/ | Create |
| 8 | IShopRepository.cs | Assets/_Game/Features/Shop/Domain/ | Create |
| 9 | ShopRepository.cs | Assets/_Game/Features/Shop/Data/ | Create |
| 10 | IShopMasterDataRepository.cs | Assets/_Game/Features/Shop/Domain/ | Create |
| 11 | ShopMasterDataRepository.cs | Assets/_Game/Features/Shop/Data/ | Create |
| 12 | ShopUseCase.cs | Assets/_Game/Features/Shop/Domain/ | Create |
| 13 | MerchantDialogueAdapter.cs | Assets/_Game/Features/Shop/Presentation/ | Create |
| 14 | ShopItemSlotView.cs | Assets/_Game/Features/Shop/Presentation/ | Create |
| 15 | ShopPanelView.cs | Assets/_Game/Features/Shop/Presentation/ | Create |
| 16 | GameContext.cs | Assets/_Game/App/ | Modify |
| 17 | EventPresenter.cs | Assets/_Game/Features/Event/Presentation/ | Modify |
| 18 | MaintenanceSceneBootstrapper.cs | Assets/_Game/Features/Maintenance/Presentation/ | Modify |
| 19 | MaintenancePresenter.cs | Assets/_Game/Features/Maintenance/Presentation/ | Modify |
| 20 | MaintenanceView.cs | Assets/_Game/Features/Maintenance/Presentation/ | Modify |
| 21 | decisions.md | .claude/specs/features/shopsystem/ | Create (empty) |

---

## Implementation Tasks

### Task 1: MasterData — PotionSO (§1, §4, §8)

Create Assets/_Game/Features/Shop/MasterData/PotionSO.cs
- Inherit ScriptableObject
- [SerializeField] private fields: _id (int), _potionName (string), _description (string), _targetStat (StatType), _effectValue (int), _price (int), _sprite (Sprite)
- Public read-only properties for all fields
- Include _logClass
- StatType references Features/Character/MasterData/StatType.cs (MasterData enum sharing allowed per Event MasterData D-02 precedent)

### Task 2: MasterData — MerchantDialogue (§1, §8)

Create Assets/_Game/Features/Shop/MasterData/MerchantDialogue.cs
- [Serializable] class
- Fields: _speakerName (string), _text (string), _speakerPosition (SpeakerPosition)
- SpeakerPosition references Features/Event/MasterData/SpeakerPosition.cs
- Public read-only properties

### Task 3: MasterData — MerchantSaleItem (§1, §8)

Create Assets/_Game/Features/Shop/MasterData/MerchantSaleItem.cs
- [Serializable] class
- Fields: _potionId (int), _stock (int)
- Public read-only properties

### Task 4: MasterData — MerchantSO (§1, §4, §8)

Create Assets/_Game/Features/Shop/MasterData/MerchantSO.cs
- Inherit ScriptableObject
- [SerializeField] private fields: _id (int), _merchantName (string), _portrait (Sprite), _shopSprite (Sprite), _stayDuration (int), _saleItems (MerchantSaleItem[]), _greetingDialogues (MerchantDialogue[]), _farewellDialogues (MerchantDialogue[])
- Public read-only properties for all fields
- Include _logClass

### Task 5: Domain — ShopRunData (§6, §8, §9)

Create Assets/_Game/Features/Shop/Domain/ShopRunData.cs
- Pure C# data class (no logic, §2)
- Fields: ActiveMerchantId (int, default -1), MerchantAppearedDay (int, default 0), RemainingStock (Dictionary<int, int>)
- Newtonsoft.Json serialization target

### Task 6: Domain — PurchaseResult (§8)

Create Assets/_Game/Features/Shop/Domain/PurchaseResult.cs
- enum: Success, InsufficientGold, OutOfStock

### Task 7: Domain — ShopItemInfo (§8)

Create Assets/_Game/Features/Shop/Domain/ShopItemInfo.cs
- Pure C# DTO
- Fields: Potion (PotionSO), RemainingStock (int), Price (int)

### Task 8: Domain — IShopRepository (§10)

Create Assets/_Game/Features/Shop/Domain/IShopRepository.cs
- Interface definition
- Methods: LoadDataAsync() -> UniTask, SaveDataAsync() -> UniTask, SaveDataSync() -> void, GetActiveMerchantId() -> int, GetMerchantAppearedDay() -> int, SetActiveMerchant(int, int, Dictionary<int,int>) -> void, ClearActiveMerchant() -> void, GetRemainingStock() -> Dictionary<int,int>, DecrementStock(int) -> void, ResetRunData() -> void

### Task 9: Data — ShopRepository (§9, §10)

Create Assets/_Game/Features/Shop/Data/ShopRepository.cs
- Implement IShopRepository
- Save path: Application.persistentDataPath/shop_run.json
- Dirty Flag pattern (§9)
- Dual-Mode Saving: SaveDataAsync (UniTask.RunOnThreadPool) + SaveDataSync (§9)
- Newtonsoft.Json serialization (§11)
- Include _logClass

### Task 10: Domain — IShopMasterDataRepository (§10)

Create Assets/_Game/Features/Shop/Domain/IShopMasterDataRepository.cs
- Interface definition
- Methods: GetMerchant(int) -> MerchantSO, GetAllMerchants() -> MerchantSO[], GetPotion(int) -> PotionSO, GetAllPotions() -> PotionSO[]

### Task 11: Data — ShopMasterDataRepository (§1, §10)

Create Assets/_Game/Features/Shop/Data/ShopMasterDataRepository.cs
- Implement IShopMasterDataRepository
- Resources.LoadAll<PotionSO>("MasterData/Shop") + Resources.LoadAll<MerchantSO>("MasterData/Shop")
- Dictionary cache (id -> SO)
- Include _logClass

### Task 12: Domain — ShopUseCase (§2, §4, §5, §8, §9)

Create Assets/_Game/Features/Shop/Domain/ShopUseCase.cs
- Pure C# (not MonoBehaviour)
- Constructor DI: IShopRepository, IShopMasterDataRepository, ICharacterRunRepository
- UseCase holds no state (§2 Data/Logic separation)
- Implement methods: ActivateMerchant, CheckAndExpireMerchant, IsShopAvailable, GetActiveMerchantData, GetShopItems, PurchasePotion, GetGreetingDialogues (see Plan §5)
- PurchasePotion: check stock -> check gold -> deduct gold + increase stat + stock -1 -> CharacterRunRepository.SaveDataAsync() + ShopRepository.SaveDataAsync()
- Include _logClass

### Task 13: Presentation — MerchantDialogueAdapter (§4, §8)

Create Assets/_Game/Features/Shop/Presentation/MerchantDialogueAdapter.cs
- Static utility class
- ToEventDialogues(MerchantDialogue[]) -> EventDialogue[] — convert MerchantDialogue to EventDialogue format
- Adapter for EventDialogueOverlay UI reuse

### Task 14: Presentation — ShopItemSlotView (§7, §8)

Create Assets/_Game/Features/Shop/Presentation/ShopItemSlotView.cs
- MonoBehaviour
- [SerializeField] private fields: _potionIcon (Image), _potionNameText (TMP_Text), _priceText (TMP_Text), _stockText (TMP_Text), _purchaseButton (Button)
- Reset() Auto-Assignment (§7)
- Methods: Setup(ShopItemInfo, Action<int>), SetInteractable(bool), UpdateStock(int)
- Include _logClass

### Task 15: Presentation — ShopPanelView (§7, §8)

Create Assets/_Game/Features/Shop/Presentation/ShopPanelView.cs
- MonoBehaviour
- [SerializeField] private fields: _shopItemContainer (Transform), _goldText (TMP_Text), _closeButton (Button), _merchantSpriteImage (Image), _shopItemSlotPrefab (ShopItemSlotView)
- Reset() Auto-Assignment (§7) — _shopItemContainer, _goldText, _closeButton auto; _shopItemSlotPrefab manual
- Methods: Show(List<ShopItemInfo>, int), Hide(), UpdateGold(int), UpdateSlot(int, int)
- Include _logClass

### Task 16: GameContext Modification (§6)

Modify Assets/_Game/App/GameContext.cs
- Add properties: ShopMasterDataRepo (IShopMasterDataRepository), ShopRepo (IShopRepository), ShopUseCase (ShopUseCase)
- In constructor: create ShopMasterDataRepository + LoadAll, create ShopRepository, create ShopUseCase (DI)
- Add ShopRepository.LoadDataAsync() to LoadAllDataAsync()
- Add ShopRepository.SaveDataSync() to SaveAllDataSync()

### Task 17: EventPresenter Modification — ShopEncounter Patch (§4, §5)

Modify Assets/_Game/Features/Event/Presentation/EventPresenter.cs
- ShopEncounter result handling: existing stub -> ShopUseCase.ActivateMerchant(merchantId, currentDay) call
- Only call when eventResult.MerchantId is not null
- Change result popup message to "A merchant will visit the maintenance area"
- Add ShopUseCase dependency to EventPresenter constructor
- Modify EventSceneBootstrapper to inject GameContext.ShopUseCase into EventPresenter

### Task 18: MaintenanceSceneBootstrapper Modification (§3)

Modify Assets/_Game/Features/Maintenance/Presentation/MaintenanceSceneBootstrapper.cs
- Get ShopUseCase from GameContext and inject into MaintenancePresenter

### Task 19: MaintenancePresenter Modification (§4, §5, §8)

Modify Assets/_Game/Features/Maintenance/Presentation/MaintenancePresenter.cs
- Add ShopUseCase dependency to constructor
- On scene enter: await ShopUseCase.CheckAndExpireMerchant(currentDay) -> if IsShopAvailable(), View.ShowShopButton()
- OnShopButtonTapped(): show merchant dialogue UI (reuse EventDialogueOverlay, convert via MerchantDialogueAdapter) -> "Trade"/"Leave" choices
- ShowShopPanel(): ShopUseCase.GetShopItems() -> View.ShopPanelView.Show()
- OnPurchaseRequested(int potionId): IPopupManager purchase confirmation -> ShopUseCase.PurchasePotion() -> refresh UI on result

### Task 20: MaintenanceView Modification (§7, §8)

Modify Assets/_Game/Features/Maintenance/Presentation/MaintenanceView.cs
- Add [SerializeField]: _shopButton (Button or GameObject), _shopPanelView (ShopPanelView)
- Add methods: ShowShopButton(), HideShopButton(), OnShopButtonTapped event wiring

### Task 21: decisions.md Empty File

Create .claude/specs/features/shopsystem/decisions.md as empty file.

---

## Validation

| # | Item | How to Verify |
|---|---|---|
| V-01 | No compile errors in Unity console | Console check |
| V-02 | PotionSO .asset fields visible in Inspector | Editor |
| V-03 | MerchantSO .asset fields visible in Inspector (sale list, dialogues, etc.) | Editor |
| V-04 | GameContext has ShopMasterDataRepo, ShopRepo, ShopUseCase accessible | Code check |
| V-05 | ShopRunData save/load works (shop_run.json) | Save file check |
| V-06 | Shop button activates after merchant activation + Maintenance entry | Editor Play |
| V-07 | Shop button -> dialogue UI -> "Trade" -> shop panel displays | Editor Play |
| V-08 | Potion purchase -> gold deduction + stat increase + stock -1 | Editor Play + save check |
| V-09 | Insufficient gold -> purchase button disabled or failure popup | Editor Play |
| V-10 | Stock 0 -> purchase button disabled | Editor Play |
| V-11 | Merchant stay expired -> shop button hidden on Maintenance entry | Editor Play |
| V-12 | ShopEncounter event -> merchant active flag saved | Save check |
| V-13 | Reincarnation -> ShopRunData reset | Save check |

---

## Manual Work (After Claude Code Implementation)

| # | Task |
|---|---|
| M-01 | Place ShopPanelView GameObject in MaintenanceScene + Inspector wiring |
| M-02 | Create ShopItemSlot Prefab (Features/Shop/Presentation/Prefabs/ShopItemSlot.prefab) — Image + TMP_Text x3 + Button |
| M-03 | Wire MaintenanceView Inspector: _shopButton, _shopPanelView |
| M-04 | Create 4 test PotionSO .asset files (Resources/MasterData/Shop/) — placeholder price values |
| M-05 | Create 1 test MerchantSO .asset file (Resources/MasterData/Shop/) — basic merchant, 4 potions, 3 Day stay |
| M-06 | Place merchant sprite placeholders (Art/Sprites/) |

---

## Claude Code Delivery Guide

- Run `claude` from project root
- CLAUDE.md is auto-loaded
- Deliver .claude/specs/features/shop-system/tasks.md for sequential implementation
- Record any judgment calls in .claude/specs/features/shop-system/decisions.md with tags: [DECISION], [BACKLOG], or [SPEC-GAP]
- Do NOT create files outside Assets/_Game/ (except decisions.md)