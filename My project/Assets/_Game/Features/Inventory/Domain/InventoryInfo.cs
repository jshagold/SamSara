using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class InventoryInfo
{
    public List<ItemInfo> ItemList { get; private set; }

    public int Money => GetItemCount(ItemConstants.MONEY_ID);

    public InventoryInfo(List<ItemInfo> itemList)
    { 
        this.ItemList = itemList; 
    }



    public int GetItemCount(int itemId)
    {
        var item = ItemList.FirstOrDefault(item => item.ItemId == itemId);
        return item != null ? item.Count : 0;
    }

    public ItemInfo GetItem(int itemId)
    {
        return ItemList.FirstOrDefault(item => item.ItemId == itemId);
    }
}
