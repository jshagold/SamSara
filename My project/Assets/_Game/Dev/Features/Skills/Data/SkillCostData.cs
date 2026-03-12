
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[Serializable]
public class SkillCostData
{
    [JsonProperty("type")]
    [JsonConverter(typeof(StringEnumConverter))]
    public SkillCostType Type;

    [JsonProperty("value")]
    public float Value; // 시간 또는 소모량
}