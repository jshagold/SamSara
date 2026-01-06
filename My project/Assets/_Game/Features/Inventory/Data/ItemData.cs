
using System;

[Serializable]
public class ItemData
{
    public int ItemId;
    public int Count;

    public ItemData() { }

    public ItemData(int itemId, int count)
    {
        ItemId = itemId;
        Count = count;
    }
}