using System.Collections.Generic;
using System.Linq;

public static class UserInventoryMapper
{
    // SaveData(DTO) -> Domain
    public static UserInventoryInfo ToDomain(this UserInventoryData data)
    {
        if (data == null || data.ItemList == null)
        {
            return new UserInventoryInfo(new List<UserItemInfo>());
        }

        var itemInfoList = data.ItemList
            .Select(item => new UserItemInfo(item.ItemId, item.Count))
            .ToList();
        return new UserInventoryInfo(itemInfoList);
    }

    // Domain -> SaveData(DTO)
    public static UserInventoryData ToData(this UserInventoryInfo domain)
    {
        if(domain == null || domain.ItemList == null)
        {
            return new UserInventoryData();
        }

        var itemDataList = domain.ItemList
            .Select(item => new UserItemData(item.ItemId, item.Count))
            .ToList();

        return new UserInventoryData(itemDataList);        
    }
}