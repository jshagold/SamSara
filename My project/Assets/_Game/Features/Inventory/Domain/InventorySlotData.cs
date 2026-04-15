namespace Samsara.Features.Inventory.Domain
{
    [System.Serializable]
    public class InventorySlotData
    {
        /// <summary>보유 아이템 ID. 빈 슬롯이면 -1.</summary>
        public int ItemId;

        /// <summary>보유 수량. 빈 슬롯이면 0.</summary>
        public int Quantity;

        public bool IsEmpty => ItemId == -1;

        public InventorySlotData()
        {
            ItemId   = -1;
            Quantity = 0;
        }
    }
}
