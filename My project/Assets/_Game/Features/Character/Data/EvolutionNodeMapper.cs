using System.Collections.Generic;
using System.Linq;

public static class EvolutionNodeMapper
{
    // Data -> Domain
    public static EvolutionNodeInfo ToDomain(this EvolutionNodeData data)
    {
        if(data == null) return null;

        // TODO Skill List 변경해야함 (MasterData -> Domain)
        return new EvolutionNodeInfo
        {
            Id = data.Id,
            Name = data.Name,
            Desc = data.Desc,
            Portrait = data.Portrait,
            EvolutionLevel = data.Level,
            NextEvolutionNodeIds = new List<int>(data.NextEvolutionNodeIds),

            StartStats = data.StartStats.Clone(),
            MaxStats = data.MaxStats.Clone(),

            SkillList = data.SkillList
                .Select(skillMasterData => new SkillInfo
                {
                    Id = skillMasterData.Id,
                    Name = skillMasterData.Name,
                    Desc = skillMasterData.Desc,
                    Icon = skillMasterData.Icon,
                    EffectVisual = skillMasterData.EffectVisual,

                    DamageMultiplier = skillMasterData.DamageMultiplier,

                    CostList = new List<SkillCostData>(skillMasterData.CostList),
                    EffectList = new List<SkillEffectData>(skillMasterData.EffectList),

                    LinkedQtePatternId = skillMasterData.LinkedQtePattern != null
                        ? skillMasterData.LinkedQtePattern.Id 
                        : -1
                })
                .ToList(),
        };
    }
}