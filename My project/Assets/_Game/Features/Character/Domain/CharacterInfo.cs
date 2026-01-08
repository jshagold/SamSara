using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterInfo
{
    [Header("Identity")]
    public int Id;
    public string Name;
    public string Description;

    [Header("Visual")]
    public Sprite Portrait;
    public int EvolutionLevel;
    public int CurrentNodeId;

    [Header("Stats")]
    public StatGroup Stats = new StatGroup();

    [Header("Skills")]
    public List<SkillInfo> SkillList = new List<SkillInfo>();

    public CharacterInfo() { }
}