using Newtonsoft.Json;
using System;
using System.Collections.Generic;

[Serializable]
public class InventorySaveData
{
    public List<ItemSaveData> ItemList = new List<ItemSaveData>();

    public InventorySaveData() { }   
}