using System;
using UnityEngine;

[Serializable]
public sealed class WoodFuelMotionSettings
{
    [Min(0.01f)] public float MoveDuration = 0.6f;
    public AnimationCurve MoveCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [Min(0f)] public float ArcHeight = 0.25f;
    [Min(0f)] public float RotationLerpSpeed = 12f;
    [Min(0f)] public float StopDistance = 0.05f;
    public bool ReleasePhysicsOnFinish = true;

    public void Validate()
    {
        MoveDuration = Mathf.Max(0.01f, MoveDuration);
        if (MoveCurve == null) MoveCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        ArcHeight = Mathf.Max(0f, ArcHeight);
        RotationLerpSpeed = Mathf.Max(0f, RotationLerpSpeed);
        StopDistance = Mathf.Max(0f, StopDistance);
    }
}
