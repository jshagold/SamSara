using System.Collections.Generic;
using UnityEngine;

public static class EvolutionNodeMapper
{
    // SaveData (Data) -> Domain
    public static EvolutionNodeInfo ToDomain(this EvolutionNodeData data)
    {
        if (data == null)
        {
            return null;
        }

        // TODO Skill List 변경해야함 (MasterData -> Domain)
        return new EvolutionNodeInfo
        {
            NodeId = data.NodeId,
            NodeName = data.NodeName,
            Desc = data.Desc,
            Portrait = data.Portrait,
            EvolutionLevel = data.Level,
            NextEvolutionNodeIds = data.NextEvolutionNodeIds,
            StartStats = data.StartStats,
            MaxStats = data.MaxStats,
            SkillList = data.SkillList,
        };
    }

    // Domain -> SaveData (Data)
    public static EvolutionNodeData ToData(this EvolutionNodeInfo domain)
    {
        if (domain == null)
        {
            return null;
        }

        // TODO Skill List 변경해야함 (Domain -> MasterData)
        return new EvolutionNodeData
        {
            NodeId = domain.NodeId,
            NodeName = domain.NodeName,
            Desc = domain.Desc,
            Portrait = domain.Portrait,
            Level = domain.EvolutionLevel,
            NextEvolutionNodeIds = domain.NextEvolutionNodeIds,
            StartStats = domain.StartStats,
            MaxStats = domain.MaxStats,
            SkillList = domain.SkillList,
        };
    }

}