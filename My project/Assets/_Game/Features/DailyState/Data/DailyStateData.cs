using System;
using System.Collections.Generic;

[Serializable]
public class DailyStateData
{
    public int CurrentDay;

    public Dictionary<string, bool[]> CharacterActionMap;

    public DailyStateData(int currentDay, Dictionary<string, bool[]> characterActionMap)
    {
        this.CurrentDay = currentDay;
        this.CharacterActionMap = characterActionMap;
    }
}