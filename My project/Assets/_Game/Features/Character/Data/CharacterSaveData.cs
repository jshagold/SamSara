using System;

[Serializable]
public class CharacterSaveData
{
    public int CharacterId;

    public int CurrentNodeId;

    public StatGroup CurrentStats = new StatGroup();

    public CharacterSaveData() { }
}