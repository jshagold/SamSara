
using System;

[Serializable]
public class StatGroup
{
    public StatInfo Hp = new StatInfo();
    public StatInfo Strength = new StatInfo();
    public StatInfo Toughness = new StatInfo();
    public StatInfo Agility = new StatInfo();

    public StatGroup() { }

    public StatGroup Clone()
    {
        return new StatGroup
        {
            Hp = this.Hp,
            Strength = this.Strength,
            Toughness = this.Toughness,
            Agility = this.Agility
        };
    }
}