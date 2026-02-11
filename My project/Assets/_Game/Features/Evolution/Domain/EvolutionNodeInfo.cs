using System;
using System.Collections.Generic;

[Serializable]
public class EvolutionNodeInfo
{
    public int Id;
    public string Name;
    public string Desc;
    public string PortraitName;

    public int EvolutionLevel;

    public StatGroup StartStats = new StatGroup();
    public StatGroup MaxStats = new StatGroup();

    public List<SkillInfo> SkillList = new List<SkillInfo>();

    public List<int> NextNodeIds = new List<int>();
    public float PositionX;
    public float PositionY;
    public EvolutionStateType State;

    public EvolutionNodeInfo() { }
}