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
    public List<SkillMasterData> SkillList = new List<SkillMasterData>();

    // TODO 인벤토리 추가해야함

    public CharacterDetailInfo() { }
}