using Samsara.Features.Inventory.Domain;

namespace Samsara.Features.Inventory.Data
{
    [System.Serializable]
    public class InventoryRunData
    {
        public InventorySlotData[] Slots;

        public InventoryRunData()
        {
            Slots = new InventorySlotData[3];
            for (int i = 0; i < 3; i++)
                Slots[i] = new InventorySlotData();
        }
    }
}
