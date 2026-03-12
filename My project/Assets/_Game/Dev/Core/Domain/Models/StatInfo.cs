using System;
using Newtonsoft.Json;

[Serializable]
public struct StatInfo
{
    [JsonProperty("type")]
    public StatType Type;
    [JsonProperty("value")]
    public float Value;
}