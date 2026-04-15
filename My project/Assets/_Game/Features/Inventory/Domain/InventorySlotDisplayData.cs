using UnityEngine;

namespace Samsara.Features.Inventory.Domain
{
    /// <summary>인벤토리 슬롯 UI 표시용 데이터 (Value Object).</summary>
    public struct InventorySlotDisplayData
    {
        public bool     IsEmpty;
        public string   ItemName;
        public Sprite   IconSprite;
        public int      Quantity;
        public ItemType ItemType;
    }
}
