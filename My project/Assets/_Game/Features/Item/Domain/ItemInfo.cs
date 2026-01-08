using UnityEngine;

public class ItemInfo
{
    // Identity
    public int Id;

    public string Name;
    public string Description;
    public ItemType Type;
    public Sprite Icon;
    public int MaxStackCount;

    public int Count;

    public bool IsMaxStacked => Count >= MaxStackCount;

    public ItemInfo() { }
}