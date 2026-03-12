using System;
using UnityEngine;

[Serializable]
public class ItemInfo
{
    // Identity
    public int Id;

    public string Name;
    public string Description;
    public ItemType Type;
    public string IconName;
    public int MaxStackCount;

    public int Count;

    public bool IsMaxStacked => Count >= MaxStackCount;

    public ItemInfo() { }
}