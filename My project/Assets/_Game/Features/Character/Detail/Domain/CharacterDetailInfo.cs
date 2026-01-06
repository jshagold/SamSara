using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterDetailInfo
{
    [Header("Identity")]
    public int CharacterId;
    public string Name;
    public string Description;

    [Header("Visual")]
    public Sprite Portrait;
    public int EvolutionLevel;
    public int CurrentNodeId;

    [Header("Stats")]
    public StatGroup Stats;

    [Header("Skills")]
    public List<SkillInfo> SkillList = new List<SkillInfo>();

    [Header("Inventory")]
    public InventoryInfo Inventory;

    public CharacterDetailInfo() { }
}