public class UserItemInfo
{
    public int ItemId { get; private set; }
    public int Count { get; private set; }

    public UserItemInfo(int itemId, int count)
    {
        ItemId = itemId;
        Count = count;
    }

    public void UpdateCount(int count)
    {
        Count = count;
    }
}