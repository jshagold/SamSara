
using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[Serializable]
public class EvolutionNodeData
{
    [Header("Node Info")]
    public int NodeId; 
    public string NodeName;
    [TextArea] public string Desc;
    public Sprite Portrait;

    [Header("Level Info")]
    public int Level; // 진화 단계 레벨

    [Header("Branching")]
    public List<int> NextEvolutionNodeIds;

    [Header("Stats")]
    public StatGroup StartStats;
    public StatGroup MaxStats;

    [Header("Skills")]
    public List<SkillMasterData> SkillList;
}