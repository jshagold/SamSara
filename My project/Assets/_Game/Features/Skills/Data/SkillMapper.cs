using System.Collections.Generic;
using System.Linq;

public static class SkillMapper
{
    // Data -> Domain
    public static SkillInfo ToDomain(this SkillMasterData masterData)
    {
        if(masterData == null) return null;

        return new SkillInfo
        {
            Id = masterData.Id,
            Name = masterData.Name,
            Desc = masterData.Desc,
            Icon = masterData.Icon,
            EffectVisual = masterData.EffectVisual,

            DamageMultiplier = masterData.DamageMultiplier,

            CostList = masterData.CostList != null
                ? masterData.CostList.ToList()
                : new List<SkillCostData>(),

            EffectList = masterData.EffectList != null
                ? masterData.EffectList.ToList()
                : new List<SkillEffectData>(),

            LinkedQtePatternId = masterData.LinkedQtePattern != null
                ? masterData.LinkedQtePattern.Id
                : 0,
        };
    }
}