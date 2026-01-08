using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class InventoryInfo
{
    public List<ItemInfo> ItemList = new List<ItemInfo>();

    public InventoryInfo() { }
}
