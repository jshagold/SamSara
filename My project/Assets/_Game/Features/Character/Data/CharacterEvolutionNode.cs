
using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[Serializable]
public class CharacterEvolutionNode
{
    [Header("Node Info")]
    public int nodeId; 
    public string nodeName;
    [TextArea] public string desc;
    public Sprite portrait;

    [Header("Level Info")]
    public int level; // 진화 단계 레벨

    [Header("Branching")]
    public List<int> nextEvolutionNodeIds;

    [Header("Base Stats")]
    public float baseHp;
    public int baseStrength;
    public int baseToughness;
    public int baseAgility;

    [Header("Limit Stats")]
    public float limitHp;
    public int limitStrength;
    public int limitToughness;
    public int limitAgility;

    [Header("Skills")]
    public List<SkillMasterData> skillList;
}