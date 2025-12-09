using System;
using System.Collections.Generic;

[Serializable]
public class DailtyStateData
{
    public int CurrentDay;

    public Dictionary<string, bool[]> CharacterActionMap;
}