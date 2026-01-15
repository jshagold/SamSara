using System;
using Newtonsoft.Json;

[Serializable]
public struct StatInfo
{
    [JsonIgnore]
    public string Label;
    [JsonProperty("value")]
    public float Value;
}