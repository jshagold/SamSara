using System;
using Newtonsoft.Json;

[Serializable]
public struct StatInfo
{
    [JsonProperty("label")]
    public string Label;
    [JsonProperty("value")]
    public float Value;
    [JsonProperty("limit")]
    public float Limit;

    [JsonIgnore]
    public float Ratio => (Limit > 0) ? Value / Limit : 0f;

    [JsonIgnore]
    public bool IsMax => Value >= Limit;
}