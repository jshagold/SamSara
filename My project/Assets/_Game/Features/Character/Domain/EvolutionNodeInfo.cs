using System.Collections.Generic;
using UnityEngine;

public class EvolutionNodeInfo
{
    public int NodeId;
    public string NodeName;
    public string Desc;
    public Sprite Portrait;

    public int EvolutionLevel;

    public List<int> NextEvolutionNodeIds;

    public StatGroup StartStats;
    public StatGroup MaxStats;

    public List<SkillMasterData> SkillList;

    public EvolutionNodeInfo() { }
}