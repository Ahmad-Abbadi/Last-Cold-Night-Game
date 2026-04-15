using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class BlendLightingScenarios : MonoBehaviour
{
    private StoveController stoveController;
    private ProbeReferenceVolume probRefVolume;

    [Header("Scenarios")]
    [SerializeField] private string scenario01;
    [SerializeField] private string scenario02;
    [SerializeField] private string scenario03;
    [SerializeField] private string scenario04;

    [Header("Blend")]
    [SerializeField] private float blendDuration = 2.5f;
    [SerializeField] private AnimationCurve blendCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [Min(1)][SerializeField] private int numberOfCellsBlendedPerFrame = 64;

    private string currentScenario;
    private string fromScenario;
    private string toScenario;

    private float blendTimer;
    private bool isBlending;

    private void Awake()
    {
        probRefVolume = ProbeReferenceVolume.instance;

        if (probRefVolume == null)
        {
            Debug.LogError("ProbeReferenceVolume instance not found.");
            enabled = false;
            return;
        }

        currentScenario = scenario01;
        probRefVolume.lightingScenario = currentScenario;
        probRefVolume.numberOfCellsBlendedPerFrame = numberOfCellsBlendedPerFrame;
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
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame) BlendToScenario01();
        else if (Keyboard.current.digit2Key.wasPressedThisFrame) BlendToScenario02();
        else if (Keyboard.current.digit3Key.wasPressedThisFrame) BlendToScenario03();
        else if (Keyboard.current.digit4Key.wasPressedThisFrame) BlendToScenario04();
    }

    private void UpdateBlend()
    {
        if (!isBlending)
            return;

        blendTimer += Time.deltaTime;
        float t = Mathf.Clamp01(blendTimer / blendDuration);
        float smoothT = blendCurve.Evaluate(t);

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

        probRefVolume.lightingScenario = fromScenario;
        probRefVolume.BlendLightingScenario(toScenario, 0f);
    }

    private void OnHeatTypeChanged(HeatLevelType type)
    {
        switch (type)
        {
            case HeatLevelType.Hot: BlendToScenario01(); break;
            case HeatLevelType.Stable: BlendToScenario02(); break;
            case HeatLevelType.Low: BlendToScenario03(); break;
            case HeatLevelType.Critical: BlendToScenario04(); break;
        }
    }

    public void BlendToScenario01() => StartBlend(scenario01);
    public void BlendToScenario02() => StartBlend(scenario02);
    public void BlendToScenario03() => StartBlend(scenario03);
    public void BlendToScenario04() => StartBlend(scenario04);
}