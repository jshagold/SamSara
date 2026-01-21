using System;
using System.Collections.Generic;

[Serializable]
public class DailyStateSaveData
{
    public int currentDay;

    public Dictionary<int, bool[]> characterActionMap = new Dictionary<int, bool[]>();

    public DailyStateSaveData() {}
}