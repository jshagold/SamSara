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
    public int ParentId;
    public List<int> NextNodeIds = new List<int>();

    [Header("Stats")]
    public StatGroup StartStats = new StatGroup();
    public StatGroup MaxStats = new StatGroup();

    [Header("Skills")]
    public List<SkillMasterData> SkillList = new List<SkillMasterData>();

    [Header("UI Layout")]
    public Vector2 Position;    // 화면의 노드 배치 좌표
}