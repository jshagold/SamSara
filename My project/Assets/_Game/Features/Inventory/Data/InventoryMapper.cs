using System.Collections.Generic;
using System.Linq;

public static class InventoryMapper
{
    // SaveData(Data) -> Domain
    public static InventoryInfo ToDomain(this InventorySaveData data)
    {
        if (data == null || data.ItemList == null)
        {
            return new InventoryInfo(new List<ItemInfo>());
        }

        var itemInfoList = data.ItemList
            .Select(item => new ItemInfo(item.ItemId, item.Count))
            .ToList();
        return new InventoryInfo(itemInfoList);
    }

    // Domain -> SaveData(Data)
    public static InventorySaveData ToData(this InventoryInfo domain)
    {
        if(domain == null || domain.ItemList == null)
        {
            return new InventorySaveData();
        }

        var itemDataList = domain.ItemList
            .Select(item => new ItemData(item.ItemId, item.Count))
            .ToList();

        return new InventoryData(itemDataList);        
    }
}