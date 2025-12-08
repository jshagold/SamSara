using System;
using System.Collections.Generic;

[Serializable]
public class DailtyStateData
{
    public int CurrentDay;

    public Dictionary<int, bool[]> CharacterActionMap;
}