using System;
using UnityEngine;

[Serializable]
public sealed class StoveLightSettings
{
    [Min(0f)] public float MinLightIntensity = 0.5f;
    [Min(0f)] public float MaxLightIntensity = 6f;

    public void Validate()
    {
        MinLightIntensity = Mathf.Max(0f, MinLightIntensity);
        MaxLightIntensity = Mathf.Max(MinLightIntensity, MaxLightIntensity);
    }
}
