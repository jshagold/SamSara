
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EvolutionNodeData
{
    [Header("Node Info")]
    public int Id; 
    public string Name;
    [TextArea] public string Desc;
    public Sprite Portrait;

    [Header("Level Info")]
    public int Level; // 진화 단계 레벨

    [Header("Branching")]
    public List<int> NextEvolutionNodeIds = new List<int>();

    [Header("Stats")]
    public StatGroup StartStats = new StatGroup();
    public StatGroup MaxStats = new StatGroup();

    [Header("Skills")]
    public List<SkillMasterData> SkillList = new List<SkillMasterData>();
}