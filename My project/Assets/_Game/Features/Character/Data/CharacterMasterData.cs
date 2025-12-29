using UnityEngine;

[CreateAssetMenu(fileName = "CharacterMasterData", menuName = "Samsara/Character Master Data")]
public class CharacterMasterData : ScriptableObject
{
    [Header("Identity")]
    public int id;
    public string characterName;
    [TextArea] public string desc;

    [Header("Visual")]
    public Sprite portrait;

    [Header("Base Stats")]
    public float baseHp;
    public int baseStrength;
    public int baseToughness;
    public int baseAgility;

    [Header("Limit Stats")]
    public float limitHp;
    public int limitStrength;
    public int limitToughness;
    public int limitAgility;

    [Header("Skills")]
    public SkillMasterData mainSkill;
    public SkillMasterData subSkill1;
    public SkillMasterData subSkill2;

}