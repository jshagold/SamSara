
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SkillInfo
{
    public int Id;
    public string Name;
    public string Desc;
    public Sprite Icon;

    public Sprite EffectVisual;

    public float DamageMultiplier;

    public List<SkillCostData> CostList = new List<SkillCostData>();
    public List<SkillEffectData> EffectList = new List<SkillEffectData>();

    public int LinkedQtePatternId = -1;
    public bool HasQte => LinkedQtePatternId >= 0;

    public SkillInfo() { }
}