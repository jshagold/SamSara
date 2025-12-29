using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[Serializable]
public class SkillEffectData
{
    [JsonProperty("type")]
    [JsonConverter(typeof(StringEnumConverter))]
    public SkillEffectType Type; // 효과 종류 (상태이상, 디버프/버프, 기타)

    [JsonProperty("duration")]
    public float Duration;  // 효과 지속 시간

    [JsonProperty("power")]
    public float Power; // 효과 강도 (ex: 출혈 틱데미지10, 힘버프 30%)
}