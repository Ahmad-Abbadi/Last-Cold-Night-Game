using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FuelController : MonoBehaviour
{
    [SerializeField] private float startFuelAmount = 10f;
    [SerializeField] private float heatAddedPerFuel;

    [SerializeField] private float ownedFuel;

    [Header("Refrences")]
    [SerializeField] private WoodFuelManager woodFuelManager;

    public float StartFuelAmount { get => startFuelAmount;}
    public float OwendFuel { get => ownedFuel;}
    public float HeatIncreasedByFuel { get => heatAddedPerFuel; }


    public static event Action<float> OnFuelConsumedWithAmount;
    public static event Action OnFuelConsumed;

    public static event Action OnFuelAmountChanged;

    public static event Action OnFuelDepleted;

    private void Awake()
    {
        ownedFuel = startFuelAmount;
    }

    private void Update()
    {
        if(Keyboard.current.spaceKey.wasPressedThisFrame) 
        {
            ConsumeFuel();
        }
    }
    [Button]
    public void ConsumeFuel()
    {
        if (ownedFuel <= 0)
        {
            OnFuelDepleted?.Invoke();

            Debug.Log("No fuel left to consume.");
            return;
        }

        ChangeFuelAmount(-1f);

        if(woodFuelManager != null)
        {
            woodFuelManager.DoWoodFuelAnimation(OnFuelWoodAnimationDone);
        }


    }

    private void OnFuelWoodAnimationDone()
    {
        OnFuelConsumedWithAmount?.Invoke(heatAddedPerFuel);
        OnFuelConsumed?.Invoke();
    }


    private void ChangeFuelAmount(float amount)
    {
        ownedFuel += amount;
        ownedFuel = Mathf.Clamp(ownedFuel, 0, startFuelAmount);
        OnFuelAmountChanged?.Invoke();

    }
}
