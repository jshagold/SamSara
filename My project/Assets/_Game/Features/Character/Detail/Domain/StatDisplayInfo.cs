using UnityEngine;

public struct StatDisplayInfo
{
    public string Label;
    public float CurrentValue;
    public float StartValue;
    public float MaxValue;

    public float Ratio
    {
        get
        {
            if(MaxValue <= StartValue) return 0f;

            float current = Mathf.Clamp(CurrentValue, StartValue, MaxValue);
            return (current - StartValue) / (MaxValue - StartValue);
        }
    }
}