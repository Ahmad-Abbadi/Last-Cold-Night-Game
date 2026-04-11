using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class StoveController : MonoBehaviour
{

    [Space(10)]
    [Range(0, 100), ReadOnly]
    public float _heatLevelValue = 0f;
    [ReadOnly]
    public HeatLevelType _heatLevelType = HeatLevelType.None;

    [Header("Settings")]
    [SerializeField] private float _heatMax = 100f;
    [SerializeField] private float _heatDrainRate = 1f;
    [Space(10)]
    [SerializeField] private HeatLevelSteps _heatMinSteps = new HeatLevelSteps();

    public float HeatLevelValue { get => _heatLevelValue; set => _heatLevelValue = value; }
    public float HeatMax { get => _heatMax; set => _heatMax = value; }
    public float HeatRatio => HeatMax <= 0f ? 0f : Mathf.Clamp01(HeatLevelValue / HeatMax);

    public event Action<HeatLevelType> HeatTypeChanged;

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
        _heatLevelValue = _heatMax;
        ValidateHeatLevelType();
    }

    private void Update()
    {
        HeatDrain();
    }

    private void HeatDrain()
    {
        HeatLevelValue -= Time.deltaTime * _heatDrainRate; 
        HeatLevelValue = Mathf.Clamp(HeatLevelValue, 0, HeatMax);

        ValidateHeatLevelType();
    }

    private void ValidateHeatLevelType()
    {
        HeatLevelType newHeatLevelType =
            _heatLevelValue > _heatMinSteps.Hot ? HeatLevelType.Hot :
            _heatLevelValue > _heatMinSteps.Stable ? HeatLevelType.Stable :
            _heatLevelValue > _heatMinSteps.Low ? HeatLevelType.Low :
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
        _heatLevelValue = Mathf.Clamp(_heatLevelValue, 0, _heatMax);

        ValidateHeatLevelType();
    }
    #endregion
}

[Serializable]
struct HeatLevelSteps
{
    public float Hot;
    public float Stable;
    public float Low;
    public float Critcal;
}

public enum HeatLevelType
{
    None,
    Hot,
    Stable,
    Low,
    Critical
}