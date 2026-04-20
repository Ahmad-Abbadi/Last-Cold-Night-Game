using System;
using UnityEngine;

[Serializable]
public sealed class StoveHeatSettings
{
    [Min(0.01f)] public float HeatMax = 100f;
    [Min(0f)] public float HeatDrainRate = 1f;
    public HeatLevelThresholds Thresholds = new HeatLevelThresholds();

    public void Validate()
    {
        HeatMax = Mathf.Max(0.01f, HeatMax);
        HeatDrainRate = Mathf.Max(0f, HeatDrainRate);
        if (Thresholds == null) Thresholds = new HeatLevelThresholds();
    }
}
