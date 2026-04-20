using System;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public sealed class FuelSettings
{
    [Min(0f)] public float StartFuelAmount = 7f;
    [Min(0f)] public float HeatAddedPerFuel = 28f;
    public bool EnableDebugConsumeFuelInput = true;
    public Key DebugConsumeFuelKey = Key.Space;

    public void Validate()
    {
        StartFuelAmount = Mathf.Max(0f, StartFuelAmount);
        HeatAddedPerFuel = Mathf.Max(0f, HeatAddedPerFuel);
    }
}
