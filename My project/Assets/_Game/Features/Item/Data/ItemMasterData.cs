using UnityEngine;

[CreateAssetMenu(fileName = "ItemMasterData", menuName = "Samsara/Data/Item Master Data")]
public class ItemMasterData : ScriptableObject
{
    [Header("Identity")]
    public int Id;
    public string Name;
    [TextArea] public string Description;
    public ItemType Type;

    [Header("Visual")]
    public Sprite Icon;

    [Header("Settings")]
    public int MaxStackCount = 99;   // 최대 소지 개수
}