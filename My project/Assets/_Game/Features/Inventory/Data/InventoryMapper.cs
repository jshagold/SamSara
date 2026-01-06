using System.Collections.Generic;
using System.Linq;

public static class InventoryMapper
{
    // SaveData(DTO) -> Domain
    public static InventoryInfo ToDomain(this InventoryData data)
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

    // Domain -> SaveData(DTO)
    public static InventoryData ToData(this InventoryInfo domain)
    {
        if(domain == null || domain.ItemList == null)
        {
            return new InventoryData();
        }

        var itemDataList = domain.ItemList
            .Select(item => new ItemData(item.ItemId, item.Count))
            .ToList();

        return new InventoryData(itemDataList);        
    }
}