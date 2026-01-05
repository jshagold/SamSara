using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillMasterData", menuName = "Samsara/Skills/Skill Master Data")]
public class SkillMasterData : ScriptableObject
{
    [Header("Identity")]
    public int SkillId;
    public string SkillName;
    [TextArea] public string Desc;

    [Header("Visual")]
    public Sprite Icon;
    public Sprite EffectVisual; // 이펙트 프리팹 또는 비주얼

    [Header("Combat Stats")]
    [Tooltip("공격력 계수 ex:1.5 = 150%")]
    public float DamageMultiplier;

    [Header("Cost/Effect")]
    public List<SkillCostData> SkillCostList = new List<SkillCostData>();
    public List<SkillEffectData> SkillEffectList = new List<SkillEffectData>();

    [Header("References")]
    [Tooltip("QTE 패턴 없으면 None")]
    public QtePatternData LinkedQtePattern;
    
}