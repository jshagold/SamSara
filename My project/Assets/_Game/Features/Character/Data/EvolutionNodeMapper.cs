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
            PortraitName = data.Portrait != null ? data.Portrait.name : "null",
            EvolutionLevel = data.Level,
            NextEvolutionNodeIds = new List<int>(data.NextEvolutionNodeIds),

            StartStats = data.StartStats.Clone(),
            MaxStats = data.MaxStats.Clone(),

            SkillList = data.SkillList
                .Select(skillMasterData => skillMasterData.ToDomain())
                .ToList(),
        };
    }
}