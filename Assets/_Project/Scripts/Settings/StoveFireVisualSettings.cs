using System;
using UnityEngine;

[Serializable]
public sealed class StoveFireVisualSettings
{
    [Min(0f)] public float SmoothingTime = 0.35f;
    public AnimationCurve HeatResponseCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Min(0f)] public float EmissionMultiplierAtMinHeat = 0.2f;
    [Min(0f)] public float EmissionMultiplierAtMaxHeat = 1.3f;

    [Min(0f)] public float SizeMultiplierAtMinHeat = 0.7f;
    [Min(0f)] public float SizeMultiplierAtMaxHeat = 1.15f;
    [Min(0f)] public float LifetimeMultiplierAtMinHeat = 0.9f;
    [Min(0f)] public float LifetimeMultiplierAtMaxHeat = 1.05f;

    [Min(0f)] public float SpeedMultiplierAtMinHeat = 0.75f;
    [Min(0f)] public float SpeedMultiplierAtMaxHeat = 1.2f;
    [Min(0f)] public float SimulationSpeedMultiplierAtMinHeat = 0.85f;
    [Min(0f)] public float SimulationSpeedMultiplierAtMaxHeat = 1.2f;

    [Min(0f)] public float LightIntensityMultiplierAtMinHeat = 0.4f;
    [Min(0f)] public float LightIntensityMultiplierAtMaxHeat = 1.15f;
    [Min(0f)] public float LightRangeMultiplierAtMinHeat = 0.75f;
    [Min(0f)] public float LightRangeMultiplierAtMaxHeat = 1.1f;

    public void Validate()
    {
        SmoothingTime = Mathf.Max(0f, SmoothingTime);
        if (HeatResponseCurve == null) HeatResponseCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        EmissionMultiplierAtMinHeat = Mathf.Max(0f, EmissionMultiplierAtMinHeat);
        EmissionMultiplierAtMaxHeat = Mathf.Max(0f, EmissionMultiplierAtMaxHeat);
        SizeMultiplierAtMinHeat = Mathf.Max(0f, SizeMultiplierAtMinHeat);
        SizeMultiplierAtMaxHeat = Mathf.Max(0f, SizeMultiplierAtMaxHeat);
        LifetimeMultiplierAtMinHeat = Mathf.Max(0f, LifetimeMultiplierAtMinHeat);
        LifetimeMultiplierAtMaxHeat = Mathf.Max(0f, LifetimeMultiplierAtMaxHeat);
        SpeedMultiplierAtMinHeat = Mathf.Max(0f, SpeedMultiplierAtMinHeat);
        SpeedMultiplierAtMaxHeat = Mathf.Max(0f, SpeedMultiplierAtMaxHeat);
        SimulationSpeedMultiplierAtMinHeat = Mathf.Max(0f, SimulationSpeedMultiplierAtMinHeat);
        SimulationSpeedMultiplierAtMaxHeat = Mathf.Max(0f, SimulationSpeedMultiplierAtMaxHeat);
        LightIntensityMultiplierAtMinHeat = Mathf.Max(0f, LightIntensityMultiplierAtMinHeat);
        LightIntensityMultiplierAtMaxHeat = Mathf.Max(0f, LightIntensityMultiplierAtMaxHeat);
        LightRangeMultiplierAtMinHeat = Mathf.Max(0f, LightRangeMultiplierAtMinHeat);
        LightRangeMultiplierAtMaxHeat = Mathf.Max(0f, LightRangeMultiplierAtMaxHeat);
    }
}
