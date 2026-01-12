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

    [Header("EvolutionNode")]
    public EvolutionNodeInfo CurrentEvolutionNode;

    [Header("Stats")]
    public StatGroup CurrentStats = new StatGroup();

    public CharacterInfo() { }
}