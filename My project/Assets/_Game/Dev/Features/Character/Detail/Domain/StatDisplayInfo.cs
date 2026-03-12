using System;

public class StatDisplayInfo
{
    public StatType Type;
    public string Label;
    public float CurrentValue;
    public float StartValue;
    public float MaxValue;

    /// <summary>Normalized progress from StartValue to MaxValue in [0, 1].</summary>
    public float Ratio
    {
        get
        {
            if (MaxValue <= StartValue) return 0f;

            float current = Math.Clamp(CurrentValue, StartValue, MaxValue);
            return (current - StartValue) / (MaxValue - StartValue);
        }
    }
}