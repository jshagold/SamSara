using Newtonsoft.Json;
using System.Collections.Generic;

public class DailyState
{
    public int currentDay;

    public Dictionary<string, bool[]> characterActionMap;

    [JsonConstructor]
    public DailyState(int currentDay, Dictionary<string, bool[]> characterActionMap)
    {
        this.currentDay = currentDay;
        this.characterActionMap = characterActionMap ?? new Dictionary<string, bool[]>();
    }
}