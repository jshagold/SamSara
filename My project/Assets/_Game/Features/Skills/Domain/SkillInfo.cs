
using System;
using UnityEngine;

[Serializable]
public class SkillInfo
{
    public int Id;
    public string Name;
    public string Desc;
    public Sprite Icon;
    public float Cooldown;

    public SkillInfo() { }
}