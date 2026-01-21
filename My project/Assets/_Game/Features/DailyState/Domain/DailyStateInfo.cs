using Newtonsoft.Json;
using System.Collections.Generic;

public class DailyStateInfo
{
    public int currentDay;

    public Dictionary<int, bool[]> characterActionMap = new Dictionary<int, bool[]>();

    [JsonConstructor]
    public DailyStateInfo() {}
}