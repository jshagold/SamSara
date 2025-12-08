using System;
using System.Collections.Generic;

[Serializable]
public class GameStateData
{
    public int CurrentDay;

    public Dictionary<int, bool[]> CharacterActionMap;
}