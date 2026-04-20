using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class StoveController : MonoBehaviour
{
    [Space(10)]
    [ReadOnly]
    public float _heatLevelValue = 0f;
    [ReadOnly]
    public HeatLevelType _heatLevelType = HeatLevelType.None;

    public float HeatLevelValue { get => _heatLevelValue; set => _heatLevelValue = value; }
    public float HeatMax => HeatSettings.HeatMax;
    public float HeatRatio => HeatMax <= 0f ? 0f : Mathf.Clamp01(HeatLevelValue / HeatMax);

    public event Action<HeatLevelType> HeatTypeChanged;

    private StoveHeatSettings HeatSettings => GameSettings.Current.StoveHeat;

    private void OnEnable()
    {
        FuelController.OnFuelConsumedWithAmount += OnFuelConsumed;
    }

    private void OnDisable()
    {
        FuelController.OnFuelConsumedWithAmount -= OnFuelConsumed;
    }
    private void Start()
    {
        _heatLevelValue = HeatMax;
        ValidateHeatLevelType();
    }

    private void Update()
    {
        HeatDrain();
    }

    private void HeatDrain()
    {
        HeatLevelValue -= Time.deltaTime * HeatSettings.HeatDrainRate;
        HeatLevelValue = Mathf.Clamp(HeatLevelValue, 0, HeatMax);

        ValidateHeatLevelType();
    }

    private void ValidateHeatLevelType()
    {
        HeatLevelThresholds thresholds = HeatSettings.Thresholds;

        HeatLevelType newHeatLevelType =
            _heatLevelValue > thresholds.Hot ? HeatLevelType.Hot :
            _heatLevelValue > thresholds.Stable ? HeatLevelType.Stable :
            _heatLevelValue > thresholds.Low ? HeatLevelType.Low :
            HeatLevelType.Critical;

        if (newHeatLevelType == _heatLevelType)
            return;

        _heatLevelType = newHeatLevelType;
        HeatTypeChanged?.Invoke(_heatLevelType);
    }

    #region Event Based Functions
    private void OnFuelConsumed(float heatAddedPerFuel)
    {
        _heatLevelValue += heatAddedPerFuel;
        _heatLevelValue = Mathf.Clamp(_heatLevelValue, 0, HeatMax);

        ValidateHeatLevelType();
    }
    #endregion
}

public enum HeatLevelType
{
    None,
    Hot,
    Stable,
    Low,
    Critical
}
