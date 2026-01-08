using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EvolutionNodeInfo
{
    public int Id;
    public string Name;
    public string Desc;
    public Sprite Portrait;

    public int EvolutionLevel;

    public List<int> NextEvolutionNodeIds = new List<int>();

    public StatGroup StartStats = new StatGroup();
    public StatGroup MaxStats = new StatGroup();

    public List<SkillInfo> SkillList = new List<SkillInfo>();

    public EvolutionNodeInfo() { }
}