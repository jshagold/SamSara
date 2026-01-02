using UnityEngine;

[CreateAssetMenu(fileName = "ItemMasterData", menuName = "Samsara/Data/Item Master Data")]
public class ItemMasterData : ScriptableObject
{
    [Header("Identity")]
    public int ItemId;
    public string Name;
    [TextArea] public string Description;
    public ItemType ItemType;

    [Header("Visual")]
    public Sprite Icon;

    [Header("Settings")]
    public int MaxStackCount;   // 최대 소지 개수
}