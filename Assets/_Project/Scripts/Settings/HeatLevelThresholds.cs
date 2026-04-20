using System;

[Serializable]
public sealed class HeatLevelThresholds
{
    public float Hot = 75f;
    public float Stable = 60f;
    public float Low = 45f;
    public float Critical = 22f;
}
