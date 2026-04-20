using System;
using UnityEngine;

[Serializable]
public sealed class VolumeBlendSettings
{
    [Min(0.01f)] public float BlendDuration = 1f;
    public AnimationCurve BlendCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    public void Validate()
    {
        BlendDuration = Mathf.Max(0.01f, BlendDuration);
        if (BlendCurve == null) BlendCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    }
}
