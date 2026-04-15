# ShopSystem — Tasks

**Version:** 2.0.0 | **Date:** 2026-04-15
**Feature:** ShopSystem v2.0.0
**Based on:** Specify v2.0.0 / Plan v2.0.0
**Constitution Ref:** §1~§11 fully verified

---

## Constitution Checklist

| Section | Applied | Details |
|---|---|---|
| §1 Folder Structure | YES | Existing Features/Shop/, Features/Inventory/ modifications only. No new files |
| §2 Zero Guessing | YES | Read existing code first, then modify. No Singleton. Data/Logic separation maintained |
| §3 Bootstrapper | YES | MaintenanceSceneBootstrapper DI wiring change |
| §4 Feature Module | YES | Cross-feature via InventoryUseCase public API only |
| §5 Async/Popup | YES | UniTask. Inventory full feedback via IPopupManager |
| §6 GameContext | YES | No changes (InventoryUseCase already registered) |
| §7 UI Standards | YES | Fail Fast, Reset() existing maintained |
| §8 Coding Standards | YES | _camelCase, suffixes, _logClass, SafeCleanup |
| §9 Data Persistence | YES | Save-on-Action maintained. Stat save removed, inventory save handled inside AddItem |
| §10 Wrapper Pattern | YES | Existing interfaces maintained |
| §11 Libraries | YES | UniTask, Newtonsoft.Json |

---

## Pre-Implementation Checklist

Claude Code MUST verify the following before starting implementation.

- Read CLAUDE.md first
- Read and understand the **current implementation state** of these existing files:
    - Assets/_Game/Features/Shop/Domain/ShopUseCase.cs (PurchasePotion current logic)
    - Assets/_Game/Features/Shop/Domain/PurchaseResult.cs (current enum)
    - Assets/_Game/Features/Shop/MasterData/PotionSO.cs (current field structure)
    - Assets/_Game/Features/Inventory/Domain/IItemData.cs (current property definition)
    - Assets/_Game/Features/Inventory/Domain/InventoryUseCase.cs (CanAddItem, AddItem API)
    - Assets/_Game/Features/Inventory/Domain/ItemType.cs (enum check)
    - Assets/_Game/Features/Maintenance/Presentation/MaintenanceSceneBootstrapper.cs (ShopUseCase creation location)
    - Assets/_Game/Features/Maintenance/Presentation/MaintenancePresenter.cs (OnPurchaseRequested current logic)
    - Assets/_Game/App/GameContext.cs (InventoryUseCase accessor check)

---

## Files to Modify

| # | File | Path | Action |
|---|---|---|---|
| 1 | IItemData.cs | Assets/_Game/Features/Inventory/Domain/ | Modify |
| 2 | PotionSO.cs | Assets/_Game/Features/Shop/MasterData/ | Modify |
| 3 | PurchaseResult.cs | Assets/_Game/Features/Shop/Domain/ | Modify |
| 4 | ShopUseCase.cs | Assets/_Game/Features/Shop/Domain/ | Modify |
| 5 | MaintenanceSceneBootstrapper.cs | Assets/_Game/Features/Maintenance/Presentation/ | Modify |
| 6 | MaintenancePresenter.cs | Assets/_Game/Features/Maintenance/Presentation/ | Modify |
| 7 | decisions.md | .claude/specs/features/shop-system/ | Modify (append to existing) |

---

## Implementation Tasks

### Task 1: IItemData.cs — IconSpriteKey Type Adjustment (§4, §8)

Modify Assets/_Game/Features/Inventory/Domain/IItemData.cs

- If IconSprite (Sprite type) currently exists -> change to IconSpriteKey (string)
- If already string type, verify property name and skip
- Post-Phase X project standard: string key + ISpriteLoader async loading

---

### Task 2: PotionSO — Add IItemData Interface Implementation (§4, §8)

Modify Assets/_Game/Features/Shop/MasterData/PotionSO.cs

- Change class declaration: PotionSO : ScriptableObject -> PotionSO : ScriptableObject, IItemData
- Add IItemData property implementations (existing field mapping):
    - public int Id => _id; (if existing property conflicts, use explicit interface implementation)
    - public string ItemName => _potionName;
    - public string Description => _description;
    - public ItemType ItemType => ItemType.Consumable;
    - public string IconSpriteKey => _spriteKey;
- Keep all existing PotionSO-specific properties (_targetStat, _effectValue, _price, etc.) unchanged
- ItemType enum references Features/Inventory/Domain/ItemType.cs

---

### Task 3: PurchaseResult — Add InventoryFull (§8)

Modify Assets/_Game/Features/Shop/Domain/PurchaseResult.cs

- Add InventoryFull to enum
- Before: { Success, InsufficientGold, OutOfStock }
- After: { Success, InsufficientGold, OutOfStock, InventoryFull }

---

### Task 4: ShopUseCase — Dependency + PurchasePotion Logic Change (§2, §4, §5, §9)

Modify Assets/_Game/Features/Shop/Domain/ShopUseCase.cs

**Constructor change:**
- Add InventoryUseCase inventoryUseCase parameter to existing constructor
- Add private readonly _inventoryUseCase field

**PurchasePotion method change:**
1. Call _inventoryUseCase.CanAddItem(potionId) -> if false, return PurchaseResult.InventoryFull
2. Check stock -> if depleted, return PurchaseResult.OutOfStock
3. Check gold -> if insufficient, return PurchaseResult.InsufficientGold
4. Deduct gold (keep existing code)
5. **DELETE existing stat increase code** (remove all targetStat += effectValue related code)
6. Call _inventoryUseCase.AddItem(potionId) (inventory save handled internally)
7. _shopRepo.DecrementStock(potionId) (keep existing)
8. _characterRunRepo.SaveDataAsync() (gold change only)
9. _shopRepo.SaveDataAsync() (stock change)
10. Return PurchaseResult.Success

> **Note:** CanAddItem is checked first, so AddItem should not fail in normal flow. Still check AddItem return value and handle error cases defensively.

---

### Task 5: MaintenanceSceneBootstrapper — DI Wiring Change (§3)

Modify Assets/_Game/Features/Maintenance/Presentation/MaintenanceSceneBootstrapper.cs

- Add GameContext.InventoryUseCase as additional parameter when creating ShopUseCase
- Before: new ShopUseCase(shopRepo, shopMasterDataRepo, characterRunRepo)
- After: new ShopUseCase(shopRepo, shopMasterDataRepo, characterRunRepo, gameContext.InventoryUseCase)

---

### Task 6: MaintenancePresenter — Purchase Result Handling Change (§5, §8)

Modify Assets/_Game/Features/Maintenance/Presentation/MaintenancePresenter.cs

**OnPurchaseRequested (or purchase handling method) change:**

- Add PurchaseResult.InventoryFull branch -> CommonPopupView "Inventory is full" popup
- Change PurchaseResult.Success handling:
    - Before: show stat change amount (e.g., "Strength increased by 10!")
    - After: show item acquisition message (e.g., "Acquired Strength Potion!")
    - Use PotionSO.PotionName to build message
- **DELETE post-Success stat refresh code** (stats are not changed)
- Keep existing gold + stock UI refresh

---

### Task 7: decisions.md Update

Append v2.0.0 judgment records to existing .claude/specs/features/shop-system/decisions.md

- Record any v2.0.0 implementation judgments not covered by Spec with tags: [DECISION], [BACKLOG], or [SPEC-GAP]

---

## Validation

| # | Item | How to Verify |
|---|---|---|
| V-01 | No compile errors in Unity console | Console check |
| V-02 | PotionSO implements IItemData interface (no Inspector change, compile-only check) | Code check |
| V-03 | Potion purchase -> gold deduction + item added to inventory + stock -1 (no stat change) | Editor Play + save check |
| V-04 | Inventory full (3 slots with different items) + new potion purchase attempt -> "Inventory is full" popup | Editor Play |
| V-05 | Same potion already in inventory + purchase -> stack quantity +1 (not inventory full) | Editor Play |
| V-06 | Insufficient gold -> purchase button disabled (existing behavior maintained) | Editor Play |
| V-07 | Stock 0 -> purchase button disabled (existing behavior maintained) | Editor Play |
| V-08 | Purchase success message is "Acquired ~!" format (not stat change display) | Editor Play |

---

## Manual Work

None. (v2.0.0 is code modifications only. No new scene/prefab/asset work.)

---

## Implementation Status

| Task | Status | Notes |
|---|---|---|
| Task 1: IItemData.cs IconSpriteKey | DONE | Sprite → string, using UnityEngine 제거 |
| Task 2: PotionSO IItemData 구현 | DONE | 명시적 인터페이스 구현 (ItemName, ItemType, IconSpriteKey) |
| Task 3: PurchaseResult InventoryFull | DONE | |
| Task 4: ShopUseCase 리팩터링 | DONE | InventoryUseCase 주입, ApplyPotionEffect 제거 |
| Task 5: DI Wiring (GameContext.cs) | DONE | ⚠️ SPEC-GAP: MaintenanceSceneBootstrapper 아님, GameContext가 실제 생성 위치. D-10 참조 |
| Task 6: MaintenancePresenter 구매 처리 | DONE | InventoryFull 분기 추가, Success 메시지 변경 |
| Task 7: decisions.md 업데이트 | DONE | D-10, D-11, D-12 추가 |

---

## Claude Code Delivery Guide

- Run `claude` from project root
- CLAUDE.md is auto-loaded
- Deliver .claude/specs/features/shop-system/tasks.md for sequential implementation
- **MUST read existing code first** to understand current implementation state before modifying
- Record any judgment calls in .claude/specs/features/shop-system/decisions.md with tags: [DECISION], [BACKLOG], or [SPEC-GAP]
- Do NOT create files outside Assets/_Game/ (except decisions.md)