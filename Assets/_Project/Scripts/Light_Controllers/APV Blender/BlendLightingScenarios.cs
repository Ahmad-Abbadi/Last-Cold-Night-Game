using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class BlendLightingScenarios : MonoBehaviour
{
    private StoveController stoveController;
    private ProbeReferenceVolume probRefVolume;

    private string currentScenario;
    private string fromScenario;
    private string toScenario;

    private float blendTimer;
    private bool isBlending;

    private LightingScenarioSettings ScenarioSettings => GameSettings.Current.LightingScenarios;

    private void Awake()
    {
        probRefVolume = ProbeReferenceVolume.instance;

        if (probRefVolume == null)
        {
            Debug.LogError("ProbeReferenceVolume instance not found.");
            enabled = false;
            return;
        }

        currentScenario = ScenarioSettings.HotScenario;
        probRefVolume.lightingScenario = currentScenario;
        probRefVolume.numberOfCellsBlendedPerFrame = ScenarioSettings.NumberOfCellsBlendedPerFrame;
    }

    private void OnEnable()
    {
        stoveController = FindAnyObjectByType<StoveController>();
        if (stoveController != null)
            stoveController.HeatTypeChanged += OnHeatTypeChanged;
    }

    private void OnDisable()
    {
        if (stoveController != null)
            stoveController.HeatTypeChanged -= OnHeatTypeChanged;
    }

    private void Update()
    {
        HandleDebugInput();
        UpdateBlend();
    }

    private void HandleDebugInput()
    {
        LightingScenarioSettings settings = ScenarioSettings;
        if (!settings.EnableDebugKeyboardInput || Keyboard.current == null)
            return;

        if (WasPressed(settings.HotDebugKey)) BlendToScenario01();
        else if (WasPressed(settings.StableDebugKey)) BlendToScenario02();
        else if (WasPressed(settings.LowDebugKey)) BlendToScenario03();
        else if (WasPressed(settings.CriticalDebugKey)) BlendToScenario04();
    }

    private bool WasPressed(Key key)
    {
        return key != Key.None
            && Keyboard.current != null
            && Keyboard.current[key].wasPressedThisFrame;
    }

    private void UpdateBlend()
    {
        if (!isBlending)
            return;

        blendTimer += Time.deltaTime;
        LightingScenarioSettings settings = ScenarioSettings;
        float t = Mathf.Clamp01(blendTimer / settings.BlendDuration);
        float smoothT = settings.BlendCurve == null ? t : Mathf.Clamp01(settings.BlendCurve.Evaluate(t));

        probRefVolume.BlendLightingScenario(toScenario, smoothT);

        if (t >= 1f)
        {
            isBlending = false;
            currentScenario = toScenario;
        }
    }

    private void StartBlend(string targetScenario)
    {
        if (string.IsNullOrWhiteSpace(targetScenario))
            return;

        if (isBlending && targetScenario == toScenario)
            return;

        if (!isBlending && targetScenario == currentScenario)
            return;

        fromScenario = currentScenario;
        toScenario = targetScenario;
        blendTimer = 0f;
        isBlending = true;

        probRefVolume.numberOfCellsBlendedPerFrame = ScenarioSettings.NumberOfCellsBlendedPerFrame;
        probRefVolume.lightingScenario = fromScenario;
        probRefVolume.BlendLightingScenario(toScenario, 0f);
    }

    private void OnHeatTypeChanged(HeatLevelType type)
    {
        StartBlend(GetScenarioForHeatType(type));
    }

    private string GetScenarioForHeatType(HeatLevelType type)
    {
        LightingScenarioSettings settings = ScenarioSettings;
        switch (type)
        {
            case HeatLevelType.Hot:
                return settings.HotScenario;
            case HeatLevelType.Stable:
                return settings.StableScenario;
            case HeatLevelType.Low:
                return settings.LowScenario;
            case HeatLevelType.Critical:
                return settings.CriticalScenario;
            default:
                return settings.CriticalScenario;
        }
    }

    public void BlendToScenario01() => StartBlend(ScenarioSettings.HotScenario);
    public void BlendToScenario02() => StartBlend(ScenarioSettings.StableScenario);
    public void BlendToScenario03() => StartBlend(ScenarioSettings.LowScenario);
    public void BlendToScenario04() => StartBlend(ScenarioSettings.CriticalScenario);
}
