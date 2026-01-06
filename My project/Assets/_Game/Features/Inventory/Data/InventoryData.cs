using Newtonsoft.Json;
using System;
using System.Collections.Generic;

[Serializable]
public class InventoryData
{
    public List<ItemData> ItemList;

    public InventoryData()
    {
        this.ItemList = new List<ItemData>();
    }

    [JsonConstructor]
    public InventoryData(List<ItemData> itemList)
    {
        this.ItemList = itemList;
    }   
}