using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class UserInventoryInfo
{
    public List<UserItemInfo> ItemList {  get; private set; }

    public int Money => GetItemCount(ItemConstants.MONEY_ID);

    public UserInventoryInfo(List<UserItemInfo> itemList)
    { 
        this.ItemList = itemList; 
    }



    public int GetItemCount(int itemId)
    {
        var item = ItemList.FirstOrDefault(item => item.ItemId == itemId);
        return item != null ? item.Count : 0;
    }

    public UserItemInfo GetItem(int itemId)
    {
        return ItemList.FirstOrDefault(item => item.ItemId == itemId);
    }
}
