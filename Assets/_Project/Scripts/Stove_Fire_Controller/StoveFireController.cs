using System;
using UnityEngine;

public class StoveFireController : MonoBehaviour
{
    [Serializable]
    private struct CachedParticleSettings
    {
        public ParticleSystem.MinMaxCurve StartLifetime;
        public float StartSizeMultiplier;
        public float StartSpeedMultiplier;
        public float SimulationSpeed;
        public float EmissionRateOverTimeMultiplier;
    }

    [Header("References")]
    [SerializeField] private StoveController stoveController;
    [SerializeField] private ParticleSystem[] stoveFire = Array.Empty<ParticleSystem>();

    [Header("Heat Response")]
    [SerializeField, Min(0f)] private float smoothingTime = 0.35f;
    [SerializeField] private AnimationCurve heatResponseCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Emission")]
    [SerializeField, Min(0f)] private float emissionMultiplierAtMinHeat = 0.2f;
    [SerializeField, Min(0f)] private float emissionMultiplierAtMaxHeat = 1.3f;

    [Header("Shape")]
    [SerializeField, Min(0f)] private float sizeMultiplierAtMinHeat = 0.7f;
    [SerializeField, Min(0f)] private float sizeMultiplierAtMaxHeat = 1.15f;
    [SerializeField, Min(0f)] private float lifetimeMultiplierAtMinHeat = 0.9f;
    [SerializeField, Min(0f)] private float lifetimeMultiplierAtMaxHeat = 1.05f;

    [Header("Motion")]
    [SerializeField, Min(0f)] private float speedMultiplierAtMinHeat = 0.75f;
    [SerializeField, Min(0f)] private float speedMultiplierAtMaxHeat = 1.2f;
    [SerializeField, Min(0f)] private float simulationSpeedMultiplierAtMinHeat = 0.85f;
    [SerializeField, Min(0f)] private float simulationSpeedMultiplierAtMaxHeat = 1.2f;

    [Header("Optional Light")]
    [SerializeField, Min(0f)] private float lightIntensityMultiplierAtMinHeat = 0.4f;
    [SerializeField, Min(0f)] private float lightIntensityMultiplierAtMaxHeat = 1.15f;
    [SerializeField, Min(0f)] private float lightRangeMultiplierAtMinHeat = 0.75f;
    [SerializeField, Min(0f)] private float lightRangeMultiplierAtMaxHeat = 1.1f;

    private CachedParticleSettings[] cachedParticleSettings = Array.Empty<CachedParticleSettings>();
    private float cachedLightIntensity;
    private float cachedLightRange;
    private float smoothedVisualHeat;
    private float visualHeatVelocity;
    private bool visualsInitialized;

    private void Reset()
    {
        stoveController = GetComponent<StoveController>() ?? GetComponentInParent<StoveController>();
    }

    private void Awake()
    {
        if (stoveController == null)
            stoveController = GetComponent<StoveController>() ?? GetComponentInParent<StoveController>();

        CacheParticleSettings();
    }

    private void OnDisable()
    {
        RestoreOriginalVisuals();
        visualsInitialized = false;
        visualHeatVelocity = 0f;
    }

    private void LateUpdate()
    {
        if (stoveController == null)
            return;

        float targetHeat = GetTargetHeatRatio();

        if (!visualsInitialized)
        {
            smoothedVisualHeat = targetHeat;
            ApplyVisuals(smoothedVisualHeat);
            visualsInitialized = true;
            return;
        }

        smoothedVisualHeat = SmoothHeat(targetHeat);
        ApplyVisuals(smoothedVisualHeat);
    }

    private void CacheParticleSettings()
    {
        if (stoveFire == null)
        {
            cachedParticleSettings = Array.Empty<CachedParticleSettings>();
            return;
        }

        cachedParticleSettings = new CachedParticleSettings[stoveFire.Length];

        for (int i = 0; i < stoveFire.Length; i++)
        {
            ParticleSystem particleSystem = stoveFire[i];
            if (particleSystem == null)
                continue;

            ParticleSystem.MainModule main = particleSystem.main;
            ParticleSystem.EmissionModule emission = particleSystem.emission;

            cachedParticleSettings[i] = new CachedParticleSettings
            {
                StartLifetime = main.startLifetime,
                StartSizeMultiplier = main.startSizeMultiplier,
                StartSpeedMultiplier = main.startSpeedMultiplier,
                SimulationSpeed = main.simulationSpeed,
                EmissionRateOverTimeMultiplier = emission.rateOverTimeMultiplier
            };
        }
    }

    

    private float GetTargetHeatRatio()
    {
        return stoveController == null ? 0f : Mathf.Clamp01(stoveController.HeatRatio);
    }

    private float SmoothHeat(float targetHeat)
    {
        if (smoothingTime <= 0f)
            return targetHeat;

        return Mathf.SmoothDamp(
            smoothedVisualHeat,
            targetHeat,
            ref visualHeatVelocity,
            smoothingTime,
            Mathf.Infinity,
            Time.deltaTime
        );
    }

    private void ApplyVisuals(float visualHeat)
    {
        float evaluatedHeat = EvaluateHeatResponse(visualHeat);

        ApplyParticleVisuals(evaluatedHeat);
    }

    private float EvaluateHeatResponse(float visualHeat)
    {
        float clampedHeat = Mathf.Clamp01(visualHeat);
        return heatResponseCurve == null
            ? clampedHeat
            : Mathf.Clamp01(heatResponseCurve.Evaluate(clampedHeat));
    }

    private void ApplyParticleVisuals(float evaluatedHeat)
    {
        if (stoveFire == null || cachedParticleSettings == null || cachedParticleSettings.Length != stoveFire.Length)
            return;

        float emissionMultiplier = Mathf.Lerp(emissionMultiplierAtMinHeat, emissionMultiplierAtMaxHeat, evaluatedHeat);
        float sizeMultiplier = Mathf.Lerp(sizeMultiplierAtMinHeat, sizeMultiplierAtMaxHeat, evaluatedHeat);
        float speedMultiplier = Mathf.Lerp(speedMultiplierAtMinHeat, speedMultiplierAtMaxHeat, evaluatedHeat);
        float simulationSpeedMultiplier = Mathf.Lerp(simulationSpeedMultiplierAtMinHeat, simulationSpeedMultiplierAtMaxHeat, evaluatedHeat);
        float lifetimeMultiplier = Mathf.Lerp(lifetimeMultiplierAtMinHeat, lifetimeMultiplierAtMaxHeat, evaluatedHeat);

        for (int i = 0; i < stoveFire.Length; i++)
        {
            ParticleSystem particleSystem = stoveFire[i];
            if (particleSystem == null)
                continue;

            CachedParticleSettings cachedSettings = cachedParticleSettings[i];
            ParticleSystem.MainModule main = particleSystem.main;
            ParticleSystem.EmissionModule emission = particleSystem.emission;

            emission.rateOverTimeMultiplier = cachedSettings.EmissionRateOverTimeMultiplier * emissionMultiplier;
            main.startSizeMultiplier = cachedSettings.StartSizeMultiplier * sizeMultiplier;
            main.startSpeedMultiplier = cachedSettings.StartSpeedMultiplier * speedMultiplier;
            main.simulationSpeed = cachedSettings.SimulationSpeed * simulationSpeedMultiplier;
            main.startLifetime = ScaleLifetime(cachedSettings.StartLifetime, lifetimeMultiplier);
        }
    }

  

    private void RestoreOriginalVisuals()
    {
        if (stoveFire != null && cachedParticleSettings != null && cachedParticleSettings.Length == stoveFire.Length)
        {
            for (int i = 0; i < stoveFire.Length; i++)
            {
                ParticleSystem particleSystem = stoveFire[i];
                if (particleSystem == null)
                    continue;

                CachedParticleSettings cachedSettings = cachedParticleSettings[i];
                ParticleSystem.MainModule main = particleSystem.main;
                ParticleSystem.EmissionModule emission = particleSystem.emission;

                emission.rateOverTimeMultiplier = cachedSettings.EmissionRateOverTimeMultiplier;
                main.startSizeMultiplier = cachedSettings.StartSizeMultiplier;
                main.startSpeedMultiplier = cachedSettings.StartSpeedMultiplier;
                main.simulationSpeed = cachedSettings.SimulationSpeed;
                main.startLifetime = cachedSettings.StartLifetime;
            }
        }
    }

    private static ParticleSystem.MinMaxCurve ScaleLifetime(ParticleSystem.MinMaxCurve sourceLifetime, float multiplier)
    {
        ParticleSystem.MinMaxCurve scaledLifetime = sourceLifetime;

        switch (sourceLifetime.mode)
        {
            case ParticleSystemCurveMode.Constant:
                scaledLifetime.constant = sourceLifetime.constant * multiplier;
                break;

            case ParticleSystemCurveMode.TwoConstants:
                scaledLifetime.constantMin = sourceLifetime.constantMin * multiplier;
                scaledLifetime.constantMax = sourceLifetime.constantMax * multiplier;
                break;

            case ParticleSystemCurveMode.Curve:
            case ParticleSystemCurveMode.TwoCurves:
                scaledLifetime.curveMultiplier = sourceLifetime.curveMultiplier * multiplier;
                break;
        }

        return scaledLifetime;
    }
}
