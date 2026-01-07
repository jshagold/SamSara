using UnityEditor.Localization.Plugins.XLIFF.V12;

public static class ItemMapper
{
    // MasterData -> Domain
    public static ItemInfo ToDomain(this ItemMasterData masterData, int count = 0)
    {
        if (masterData == null) return null;

        return new ItemInfo
        {
            Id = masterData.Id,
            Name = masterData.Name,
            Description = masterData.Description,
            Type = masterData.Type,
            Icon = masterData.Icon,
            MaxStackCount = masterData.MaxStackCount,

            Count = count,
        };
    }
}