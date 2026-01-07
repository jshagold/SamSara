using Newtonsoft.Json;
using System;
using System.Collections.Generic;

[Serializable]
public class InventorySaveData
{
    public List<ItemSaveData> ItemList;

    public InventorySaveData()
    {
        this.ItemList = new List<ItemSaveData>();
    }

    [JsonConstructor]
    public InventorySaveData(List<ItemSaveData> itemList)
    {
        this.ItemList = itemList;
    }   
}