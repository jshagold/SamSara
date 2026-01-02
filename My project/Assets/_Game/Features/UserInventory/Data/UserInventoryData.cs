using Newtonsoft.Json;
using System;
using System.Collections.Generic;

[Serializable]
public class UserInventoryData
{
    public List<UserItemData> ItemList;

    public UserInventoryData()
    {
        this.ItemList = new List<UserItemData>();
    }

    [JsonConstructor]
    public UserInventoryData(List<UserItemData> itemList)
    {
        this.ItemList = itemList;
    }   
}