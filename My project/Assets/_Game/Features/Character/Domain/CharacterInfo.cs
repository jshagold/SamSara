using System;

[Serializable]
public class CharacterInfo
{
    public int Id;
    public string Name;
    public string Description;

    public EvolutionNodeInfo CurrentEvolutionNode;

    public StatGroup CurrentStats = new StatGroup();

    public CharacterInfo() { }
}