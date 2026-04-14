# InventorySystem — Tasks

**Version:** 1.0.0 | **Date:** 2026-04-14 | **Status:** Confirmed
**Feature:** InventorySystem (Phase 5-C)
**Constitution References:** §1, §2, §3, §4, §5, §6, §7, §8, §9, §10, §11

---

## 1. Overview

Implementation guide for Claude Code to build the InventorySystem Feature based on the Plan. Creates Data + Domain layer files. No Presentation layer. No scene. No manual tasks.

---

## 2. Prerequisites

- Read CLAUDE.md first
- Read the following existing files before implementation:
  - Assets/_Game/App/GameContext.cs
  - Assets/_Game/Features/Character/Domain/ICharacterRunRepository.cs
  - Assets/_Game/Features/Character/Data/CharacterRepository.cs (pattern reference)
  - Assets/_Game/Features/Shop/Domain/IShopMasterDataRepository.cs
  - Assets/_Game/Features/Shop/Data/ShopMasterDataRepository.cs
  - Assets/_Game/Features/Shop/MasterData/PotionSO.cs
  - Assets/_Game/Features/Shop/Data/ShopRepository.cs (Repository pattern reference)
- Create folder: Assets/_Game/Features/Inventory/
- Create folder: .claude/specs/features/inventory-system/

---

## 3. Files to Create

| Order | File | Path | Action |
|---|---|---|---|
| 1 | ItemType.cs | Features/Inventory/Domain/ | New |
| 2 | IItemData.cs | Features/Inventory/Domain/ | New |
| 3 | InventorySlotData.cs | Features/Inventory/Domain/ | New |
| 4 | AddItemResult.cs | Features/Inventory/Domain/ | New |
| 5 | UseItemResult.cs | Features/Inventory/Domain/ | New |
| 6 | InventorySlotDisplayData.cs | Features/Inventory/Domain/ | New |
| 7 | ItemDetailData.cs | Features/Inventory/Domain/ | New |
| 8 | InventoryRunData.cs | Features/Inventory/Data/ | New |
| 9 | IInventoryRepository.cs | Features/Inventory/Domain/ | New |
| 10 | InventoryRepository.cs | Features/Inventory/Data/ | New |
| 11 | InventoryUseCase.cs | Features/Inventory/Domain/ | New |
| 12 | GameContext.cs | App/ | Modify |
| 13 | decisions.md | .claude/specs/features/inventory-system/ | New (empty) |

**Total: 13 Tasks** (12 new + 1 modify)

---

## 4. Implementation Order & Instructions

### Task 1 — Create ItemType.cs

- enum ItemType. Namespace: match project standard.
- Values: Consumable = 0.
- Add comment for future extension (Equipment, KeyItem, etc.).
- Location: Assets/_Game/Features/Inventory/Domain/ItemType.cs.

---

### Task 2 — Create IItemData.cs

- interface IItemData. Common item reference interface for the inventory system.
- Properties:
  - int Id { get; }
  - string ItemName { get; }
  - string Description { get; }
  - ItemType ItemType { get; }
  - Sprite IconSprite { get; }
- Requires using UnityEngine; (for Sprite).
- Location: Assets/_Game/Features/Inventory/Domain/IItemData.cs.

> **Note:** Adding IItemData implementation to PotionSO is NOT in this Tasks scope. Handled in separate ShopSystem Patch.

---

### Task 3 — Create InventorySlotData.cs

- Serializable class. [System.Serializable] attribute.
- Fields:
  - public int ItemId — Stored item ID. -1 if empty slot.
  - public int Quantity — Held quantity. 0 if empty slot.
- Read-only property:
  - public bool IsEmpty => ItemId == -1
- Default constructor: ItemId = -1, Quantity = 0.
- Location: Assets/_Game/Features/Inventory/Domain/InventorySlotData.cs.

---

### Task 4 — Create AddItemResult.cs

- enum AddItemResult.
- Values: Success, InventoryFull.
- Location: Assets/_Game/Features/Inventory/Domain/AddItemResult.cs.

---

### Task 5 — Create UseItemResult.cs

- Class. Data object for item use result.
- Fields:
  - public bool IsSuccess
  - public UseItemFailReason FailReason
  - public StatType AffectedStat (valid on success)
  - public int EffectValue (valid on success)
- Define UseItemFailReason enum in same file: None, EmptySlot, NotUsable.
- Static factory methods:
  - static UseItemResult Success(StatType stat, int value)
  - static UseItemResult Fail(UseItemFailReason reason)
- StatType: reference existing project StatType enum. Check location first.
- Location: Assets/_Game/Features/Inventory/Domain/UseItemResult.cs.

---

### Task 6 — Create InventorySlotDisplayData.cs

- struct. UI display data.
- Fields:
  - public bool IsEmpty
  - public string ItemName
  - public Sprite IconSprite
  - public int Quantity
  - public ItemType ItemType
- Location: Assets/_Game/Features/Inventory/Domain/InventorySlotDisplayData.cs.

---

### Task 7 — Create ItemDetailData.cs

- Class. Long-press popup detail data.
- Fields:
  - public string ItemName
  - public string Description
  - public Sprite IconSprite
  - public StatType TargetStat
  - public int EffectValue
  - public int Quantity
- Location: Assets/_Game/Features/Inventory/Domain/ItemDetailData.cs.

---

### Task 8 — Create InventoryRunData.cs

- Serializable class. [System.Serializable] attribute.
- Field: public InventorySlotData[] Slots
- Default constructor: Slots = new InventorySlotData[3], each initialized as new InventorySlotData() (empty state).
- Location: Assets/_Game/Features/Inventory/Data/InventoryRunData.cs.

---

### Task 9 — Create IInventoryRepository.cs

- interface IInventoryRepository.
- Methods:
  - UniTask LoadDataAsync()
  - UniTask SaveDataAsync()
  - void SaveDataSync()
  - InventorySlotData[] GetSlots()
  - void SetSlot(int slotIndex, int itemId, int quantity)
  - void ClearSlot(int slotIndex)
  - void ResetRunData()
  - void MarkDirty()
- Requires using Cysharp.Threading.Tasks; (§11).
- Location: Assets/_Game/Features/Inventory/Domain/IInventoryRepository.cs.

---

### Task 10 — Create InventoryRepository.cs

- IInventoryRepository implementation. Pure C# class (NOT MonoBehaviour).
- Reference existing ShopRepository.cs for identical pattern.
- _logClass required (§8).
- Fields:
  - private InventoryRunData _runData
  - private bool _isDirty
  - private readonly string _savePath — Application.persistentDataPath + "/inventory_run_data.json"
- LoadDataAsync():
  - UniTask.RunOnThreadPool for file read (§9).
  - If file missing, create new InventoryRunData() default.
  - Newtonsoft.Json JsonConvert.DeserializeObject (§9, §11).
- SaveDataAsync():
  - If _isDirty is false, return immediately (§9 Dirty Flag).
  - UniTask.RunOnThreadPool for file write.
  - JsonConvert.SerializeObject(_runData).
  - After save, _isDirty = false.
- SaveDataSync():
  - If _isDirty is false, return immediately.
  - Synchronous file write (File.WriteAllText). Async FORBIDDEN (§9).
  - After save, _isDirty = false.
- GetSlots() → return _runData.Slots.
- SetSlot(int slotIndex, int itemId, int quantity):
  - _runData.Slots[slotIndex].ItemId = itemId
  - _runData.Slots[slotIndex].Quantity = quantity
  - _isDirty = true
- ClearSlot(int slotIndex):
  - _runData.Slots[slotIndex] = new InventorySlotData() (empty state).
  - _isDirty = true
- ResetRunData():
  - _runData = new InventoryRunData() (3 empty slots).
  - _isDirty = true
- MarkDirty() → _isDirty = true.
- Location: Assets/_Game/Features/Inventory/Data/InventoryRepository.cs.

---

### Task 11 — Create InventoryUseCase.cs

- Pure C# class (NOT MonoBehaviour).
- _logClass required (§8).
- Constructor injection (§2, §3):
  - IInventoryRepository _inventoryRepo
  - ICharacterRunRepository _characterRunRepo
  - IShopMasterDataRepository _shopMasterDataRepo

**CanAddItem(int itemId) → bool:**
1. Iterate _inventoryRepo.GetSlots().
2. If slot.ItemId == itemId found → return true (stack to existing).
3. If slot.IsEmpty found → return true (new registration).
4. Neither → return false.

**AddItem(int itemId, int quantity = 1) → async UniTask<AddItemResult>:**
1. Search GetSlots() for slot with ItemId == itemId.
2. Found: SetSlot(index, itemId, slot.Quantity + quantity) → SaveDataAsync() → Success.
3. Not found: search for first IsEmpty slot.
4. Found: SetSlot(index, itemId, quantity) → SaveDataAsync() → Success.
5. Not found: return InventoryFull.

**UseItem(int slotIndex) → async UniTask<UseItemResult>:**
1. GetSlots()[slotIndex] → if IsEmpty, return Fail(EmptySlot).
2. _shopMasterDataRepo.GetPotion(slot.ItemId) → if null, return Fail(NotUsable).
3. Increase stat in _characterRunRepo: potion.TargetStat by potion.EffectValue. (Check existing CharacterRunRepository stat change method and call it.)
4. slot.Quantity - 1. If 0, ClearSlot(slotIndex); else SetSlot(slotIndex, slot.ItemId, slot.Quantity - 1).
5. _inventoryRepo.SaveDataAsync().
6. Call _characterRunRepo save method.
7. Return Success(potion.TargetStat, potion.EffectValue).

**DiscardItem(int slotIndex, int quantity) → async UniTask<bool>:**
1. GetSlots()[slotIndex] → if IsEmpty, return false.
2. If quantity > slot.Quantity, return false.
3. newQuantity = slot.Quantity - quantity.
4. If newQuantity == 0, ClearSlot(slotIndex); else SetSlot(slotIndex, slot.ItemId, newQuantity).
5. SaveDataAsync() → return true.

**GetSlotDisplayData() → InventorySlotDisplayData[]:**
1. Create 3-slot array.
2. Each slot: if IsEmpty, return empty InventorySlotDisplayData.
3. If occupied, query _shopMasterDataRepo.GetPotion(itemId).
4. Extract name, icon, ItemType from PotionSO + combine with quantity.

**GetItemDetail(int slotIndex) → ItemDetailData:**
1. Query PotionSO for the slot.
2. If null, return null.
3. Combine name, description, icon, targetStat, effectValue, quantity.

**IsInventoryFull() → bool:**
- True if 0 IsEmpty slots in GetSlots().

Location: Assets/_Game/Features/Inventory/Domain/InventoryUseCase.cs.

---

### Task 12 — Modify GameContext.cs

- **Read current code first.** (Process Rule 7)
- Add fields:
  - private readonly InventoryRepository _inventoryRepo
  - private readonly InventoryUseCase _inventoryUseCase
- Add public accessors:
  - public IInventoryRepository InventoryRepo => _inventoryRepo
  - public InventoryUseCase InventoryUseCase => _inventoryUseCase
- In constructor:
  - _inventoryRepo = new InventoryRepository().
  - _inventoryUseCase = new InventoryUseCase(_inventoryRepo, _characterRunRepo, _shopMasterDataRepo).
  - Match exact existing variable names from current code. DO NOT GUESS (§2).
- LoadAllDataAsync():
  - Add _inventoryRepo.LoadDataAsync() to existing UniTask.WhenAll.
- SaveAllDataSync():
  - Add _inventoryRepo.SaveDataSync().
- ResetAllRunData() (or reincarnation method):
  - Add _inventoryRepo.ResetRunData().
  - **Verify exact method name from current code.** If missing, record in decisions.md.

---

### Task 13 — Create decisions.md

- Empty file. Path: .claude/specs/features/inventory-system/decisions.md.
- Content:

    # InventorySystem — Decisions
    
    Record any judgment calls not covered by the Spec here.
    Tags: [DECISION], [BACKLOG], [SPEC-GAP]

---

## 5. Validation

| # | Item | Verification |
|---|---|---|
| V-01 | InventoryRepository.LoadDataAsync() — initializes 3 empty slots when no file | Delete save file, test load |
| V-02 | InventoryRepository.SaveDataAsync() — skips write when Dirty Flag is false | Call SaveDataAsync() after load, verify file timestamp unchanged |
| V-03 | AddItem — same item stack behavior | AddItem(1, 2) then AddItem(1, 3) → slot has ItemId=1, Quantity=5 |
| V-04 | AddItem — new registration in empty slot | From empty state, AddItem(1) → registered in first slot |
| V-05 | AddItem — InventoryFull when 3 slots occupied with new item type | Register 3 types, then add 4th → InventoryFull |
| V-06 | UseItem — quantity decrease + clear slot at 0 | Quantity=1, UseItem → slot becomes empty |
| V-07 | DiscardItem — discard specified quantity | Quantity=5, DiscardItem(index, 3) → Quantity=2 |
| V-08 | GameContext.LoadAllDataAsync() includes InventoryRepository | Verify InventoryRepo accessible after app start |
| V-09 | GameContext.SaveAllDataSync() includes InventoryRepository | Code-level verification |
| V-10 | ResetRunData — all 3 slots empty | Add items then ResetRunData() → all empty |
| V-11 | No compile errors | Unity Editor build check |

---

## 6. Claude Code Implementation Guide

- Read CLAUDE.md first before any implementation
- Files to create: see §3 table above
- Files to reference: see §2 prerequisites
- Implementation order: Task 1 → 13 (dependency order)
- DO NOT create files outside Assets/_Game/
- If you make any judgment calls not covered by the Spec, record them in .claude/specs/features/inventory-system/decisions.md with appropriate tags: [DECISION], [BACKLOG], or [SPEC-GAP]