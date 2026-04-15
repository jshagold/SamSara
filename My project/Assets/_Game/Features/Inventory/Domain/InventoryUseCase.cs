using Cysharp.Threading.Tasks;
using Samsara.Features.Character.Domain;
using Samsara.Features.Character.MasterData;
using Samsara.Features.Shop.Domain;
using Samsara.Features.Shop.MasterData;
using UnityEngine;

namespace Samsara.Features.Inventory.Domain
{
    public class InventoryUseCase
    {
        private readonly string _logClass = $"[{nameof(InventoryUseCase)}]";

        private readonly IInventoryRepository      _inventoryRepo;
        private readonly ICharacterRunRepository   _characterRunRepo;
        private readonly IShopMasterDataRepository _shopMasterDataRepo;

        public InventoryUseCase(
            IInventoryRepository      inventoryRepo,
            ICharacterRunRepository   characterRunRepo,
            IShopMasterDataRepository shopMasterDataRepo)
        {
            _inventoryRepo      = inventoryRepo;
            _characterRunRepo   = characterRunRepo;
            _shopMasterDataRepo = shopMasterDataRepo;
        }

        // ── Query ──

        /// <summary>해당 아이템을 인벤토리에 추가할 수 있는지 확인한다.</summary>
        public bool CanAddItem(int itemId)
        {
            var slots = _inventoryRepo.GetSlots();
            foreach (var slot in slots)
            {
                if (slot.ItemId == itemId) return true; // 동일 아이템 스택 가능
                if (slot.IsEmpty)          return true; // 빈 슬롯 존재
            }
            return false;
        }

        /// <summary>인벤토리가 꽉 찼는지 반환한다.</summary>
        public bool IsInventoryFull()
        {
            var slots = _inventoryRepo.GetSlots();
            foreach (var slot in slots)
                if (slot.IsEmpty) return false;
            return true;
        }

        /// <summary>3개 슬롯 전체의 UI 표시 데이터를 반환한다.</summary>
        public InventorySlotDisplayData[] GetSlotDisplayData()
        {
            var slots  = _inventoryRepo.GetSlots();
            var result = new InventorySlotDisplayData[slots.Length];

            for (int i = 0; i < slots.Length; i++)
            {
                var slot = slots[i];

                if (slot.IsEmpty)
                {
                    result[i] = new InventorySlotDisplayData { IsEmpty = true };
                    continue;
                }

                PotionSO potion = null;
                try { potion = _shopMasterDataRepo.GetPotion(slot.ItemId); }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"{_logClass} GetPotion 실패 (slotIndex={i}, itemId={slot.ItemId}): {e.Message}");
                    result[i] = new InventorySlotDisplayData { IsEmpty = true };
                    continue;
                }

                result[i] = new InventorySlotDisplayData
                {
                    IsEmpty   = false,
                    ItemName  = potion.PotionName,
                    IconSprite = potion.Sprite,
                    Quantity  = slot.Quantity,
                    ItemType  = ItemType.Consumable
                };
            }

            return result;
        }

        /// <summary>지정 슬롯의 상세 데이터를 반환한다. 슬롯이 비어있거나 포션 조회 실패 시 null.</summary>
        public ItemDetailData GetItemDetail(int slotIndex)
        {
            var slot = _inventoryRepo.GetSlots()[slotIndex];
            if (slot.IsEmpty) return null;

            PotionSO potion;
            try { potion = _shopMasterDataRepo.GetPotion(slot.ItemId); }
            catch (System.Exception e)
            {
                Debug.LogWarning($"{_logClass} GetItemDetail: GetPotion 실패 (slotIndex={slotIndex}, itemId={slot.ItemId}): {e.Message}");
                return null;
            }

            return new ItemDetailData
            {
                ItemName    = potion.PotionName,
                Description = potion.Description,
                IconSprite  = potion.Sprite,
                TargetStat  = potion.TargetStat,
                EffectValue = potion.EffectValue,
                Quantity    = slot.Quantity
            };
        }

        // ── Commands ──

        /// <summary>아이템을 인벤토리에 추가한다. 동일 아이템이면 스택, 없으면 빈 슬롯에 등록.</summary>
        public async UniTask<AddItemResult> AddItem(int itemId, int quantity = 1)
        {
            var slots = _inventoryRepo.GetSlots();

            // 동일 아이템 스택
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].ItemId == itemId)
                {
                    _inventoryRepo.SetSlot(i, itemId, slots[i].Quantity + quantity);
                    await _inventoryRepo.SaveDataAsync();
                    Debug.Log($"{_logClass} AddItem 스택: itemId={itemId}, quantity={slots[i].Quantity}");
                    return AddItemResult.Success;
                }
            }

            // 빈 슬롯에 신규 등록
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].IsEmpty)
                {
                    _inventoryRepo.SetSlot(i, itemId, quantity);
                    await _inventoryRepo.SaveDataAsync();
                    Debug.Log($"{_logClass} AddItem 신규: slotIndex={i}, itemId={itemId}, quantity={quantity}");
                    return AddItemResult.Success;
                }
            }

            Debug.Log($"{_logClass} AddItem 실패: 인벤토리 가득 참.");
            return AddItemResult.InventoryFull;
        }

        /// <summary>지정 슬롯의 아이템을 사용한다. 수량이 0이 되면 슬롯을 비운다.</summary>
        public async UniTask<UseItemResult> UseItem(int slotIndex)
        {
            var slot = _inventoryRepo.GetSlots()[slotIndex];

            if (slot.IsEmpty)
                return UseItemResult.Fail(UseItemFailReason.EmptySlot);

            PotionSO potion;
            try { potion = _shopMasterDataRepo.GetPotion(slot.ItemId); }
            catch (System.Exception e)
            {
                Debug.LogWarning($"{_logClass} UseItem: GetPotion 실패 (itemId={slot.ItemId}): {e.Message}");
                return UseItemResult.Fail(UseItemFailReason.NotUsable);
            }

            // 스탯 적용
            var runData = _characterRunRepo.RunData;
            switch (potion.TargetStat)
            {
                case StatType.Hp:        runData.Hp        += potion.EffectValue; break;
                case StatType.Strength:  runData.Strength  += potion.EffectValue; break;
                case StatType.Toughness: runData.Toughness += potion.EffectValue; break;
                case StatType.Agility:   runData.Agility   += potion.EffectValue; break;
            }
            _characterRunRepo.MarkDirty();

            // 슬롯 수량 감소
            int newQuantity = slot.Quantity - 1;
            if (newQuantity == 0)
                _inventoryRepo.ClearSlot(slotIndex);
            else
                _inventoryRepo.SetSlot(slotIndex, slot.ItemId, newQuantity);

            await _inventoryRepo.SaveDataAsync();
            await _characterRunRepo.SaveDataAsync();

            Debug.Log($"{_logClass} UseItem 성공: slotIndex={slotIndex}, stat={potion.TargetStat}, value={potion.EffectValue}");
            return UseItemResult.Success(potion.TargetStat, potion.EffectValue);
        }

        /// <summary>지정 슬롯의 아이템을 지정 수량만큼 버린다.</summary>
        public async UniTask<bool> DiscardItem(int slotIndex, int quantity)
        {
            var slot = _inventoryRepo.GetSlots()[slotIndex];

            if (slot.IsEmpty)
                return false;

            if (quantity > slot.Quantity)
                return false;

            int newQuantity = slot.Quantity - quantity;
            if (newQuantity == 0)
                _inventoryRepo.ClearSlot(slotIndex);
            else
                _inventoryRepo.SetSlot(slotIndex, slot.ItemId, newQuantity);

            await _inventoryRepo.SaveDataAsync();

            Debug.Log($"{_logClass} DiscardItem: slotIndex={slotIndex}, discarded={quantity}, remaining={newQuantity}");
            return true;
        }
    }
}
