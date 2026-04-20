using System;
using UnityEngine;

[Serializable]
public sealed class FireLightFlickerSettings
{
    [Min(0f)] public float BaseIntensity = 4f;
    [Min(0f)] public float BaseRange = 8f;
    [Min(0f)] public float IntensityVariation = 0.8f;
    [Min(0f)] public float RangeVariation = 0.35f;
    [Min(0f)] public float FlickerSpeed = 6f;
    [Min(0f)] public float Smoothing = 12f;
    public bool UsePositionJitter = true;
    [Min(0f)] public float PositionJitterAmount = 0.03f;
    [Min(0f)] public float PositionJitterSpeed = 4f;

    public void Validate()
    {
        BaseIntensity = Mathf.Max(0f, BaseIntensity);
        BaseRange = Mathf.Max(0f, BaseRange);
        IntensityVariation = Mathf.Max(0f, IntensityVariation);
        RangeVariation = Mathf.Max(0f, RangeVariation);
        FlickerSpeed = Mathf.Max(0f, FlickerSpeed);
        Smoothing = Mathf.Max(0f, Smoothing);
        PositionJitterAmount = Mathf.Max(0f, PositionJitterAmount);
        PositionJitterSpeed = Mathf.Max(0f, PositionJitterSpeed);
    }
}
