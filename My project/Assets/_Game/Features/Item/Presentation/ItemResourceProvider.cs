using UnityEngine;

public class ItemResourceProvider : IItemResourceProvider
{
    private readonly string _logClass = $"{nameof(ItemResourceProvider)}";
    
    private readonly IItemMasterRepository _masterRepo;
    public ItemResourceProvider(IItemMasterRepository masterRepo)
    {
        _masterRepo = masterRepo;
    }

    public Sprite GetIconSprite(int itemId)
    {
        var masterData = _masterRepo.GetData(itemId);
        if (masterData == null)
        {
            Debug.LogWarning($"{_logClass} MasterData not found skillId:{itemId}");
            return null;
        }

        return masterData.Icon;
    }
}