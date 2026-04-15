using Cysharp.Threading.Tasks;

namespace Samsara.Features.Inventory.Domain
{
    public interface IInventoryRepository
    {
        UniTask LoadDataAsync();
        UniTask SaveDataAsync();
        void SaveDataSync();

        InventorySlotData[] GetSlots();
        void SetSlot(int slotIndex, int itemId, int quantity);
        void ClearSlot(int slotIndex);
        void ResetRunData();
        void MarkDirty();
    }
}
