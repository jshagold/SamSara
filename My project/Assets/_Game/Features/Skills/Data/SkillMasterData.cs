using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillMasterData", menuName = "Samsara/Skills/Skill Master Data")]
public class SkillMasterData : ScriptableObject
{
    [Header("Identity")]
    public int id;
    public string skillName;
    [TextArea] public string desc;

    [Header("Visual")]
    public Sprite icon;
    public Sprite effectVisual; // 이펙트 프리팹 또는 비주얼

    [Header("Combat Stats")]
    [Tooltip("공격력 계수 ex:1.5 = 150%")]
    public float damageMultiplier;

    [Header("Cost/Effect")]
    public List<SkillCostData> skillCostList = new List<SkillCostData>();
    public List<SkillEffectData> skillEffectList = new List<SkillEffectData>();

    [Header("References")]
    [Tooltip("QTE 패턴 ID (QteMasterData 조회용)")]
    public int qtePatternId;
    
}