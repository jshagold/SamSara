
using System;

[Serializable]
public class StatGroup
{
    public StatInfo Hp;
    public StatInfo Strength;
    public StatInfo Toughness;
    public StatInfo Agility;

    public StatGroup() { }

    public StatGroup(StatInfo hp, StatInfo strength, StatInfo toughness, StatInfo agility)
    {
        Hp = hp;
        Strength = strength;
        Toughness = toughness;
        Agility = agility;
    }
}