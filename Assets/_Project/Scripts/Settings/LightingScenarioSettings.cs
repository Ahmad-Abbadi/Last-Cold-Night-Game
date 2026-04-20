using System;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public sealed class LightingScenarioSettings
{
    public string HotScenario = "Heat";
    public string StableScenario = "Stable";
    public string LowScenario = "Cold";
    public string CriticalScenario = "Critical";

    [Min(0.01f)] public float BlendDuration = 5f;
    public AnimationCurve BlendCurve = new AnimationCurve(
        new Keyframe(0f, 0f, 2f, 2f),
        new Keyframe(1f, 1f, 0f, 0f)
    );
    [Min(1)] public int NumberOfCellsBlendedPerFrame = 64;

    public bool EnableDebugKeyboardInput = true;
    public Key HotDebugKey = Key.Digit1;
    public Key StableDebugKey = Key.Digit2;
    public Key LowDebugKey = Key.Digit3;
    public Key CriticalDebugKey = Key.Digit4;

    public void Validate()
    {
        BlendDuration = Mathf.Max(0.01f, BlendDuration);
        if (BlendCurve == null) BlendCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        NumberOfCellsBlendedPerFrame = Mathf.Max(1, NumberOfCellsBlendedPerFrame);
    }
}
