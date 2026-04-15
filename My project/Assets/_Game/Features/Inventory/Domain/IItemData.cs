using UnityEngine;

namespace Samsara.Features.Inventory.Domain
{
    public interface IItemData
    {
        int      Id          { get; }
        string   ItemName    { get; }
        string   Description { get; }
        ItemType ItemType    { get; }
        Sprite   IconSprite  { get; }
    }
}
