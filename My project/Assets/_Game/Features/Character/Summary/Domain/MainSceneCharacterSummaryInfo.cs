using System;

public class MainSceneCharacterSummaryInfo
{
    public int CharacterId;

    public StatInfo CurrentHp = new StatInfo();
    public StatInfo MaxHp = new StatInfo();

    public bool[] ActionFlags = Array.Empty<bool>();

    public MainSceneCharacterSummaryInfo() {}
}