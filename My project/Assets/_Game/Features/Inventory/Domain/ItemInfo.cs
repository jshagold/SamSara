public class ItemInfo
{
    public int ItemId { get; private set; }
    public int Count { get; private set; }

    public ItemInfo(int itemId, int count)
    {
        ItemId = itemId;
        Count = count;
    }

    public void UpdateCount(int count)
    {
        Count = count;
    }
}