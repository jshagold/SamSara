
using System;

[Serializable]
public class UserItemData
{
    public int ItemId;
    public int Count;

    public UserItemData() { }

    public UserItemData(int itemId, int count)
    {
        ItemId = itemId;
        Count = count;
    }
}