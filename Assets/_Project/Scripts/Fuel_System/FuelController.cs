using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class FuelController : MonoBehaviour
{
    [SerializeField, ReadOnly] private float ownedFuel;

    [Header("Refrences")]
    [SerializeField] private WoodFuelManager woodFuelManager;

    public float StartFuelAmount { get => FuelSettings.StartFuelAmount;}
    public float OwendFuel { get => ownedFuel;}
    public float HeatIncreasedByFuel { get => FuelSettings.HeatAddedPerFuel; }

    private FuelSettings FuelSettings => GameSettings.Current.Fuel;

    public static event Action<float> OnFuelConsumedWithAmount;
    public static event Action OnFuelConsumed;

    public static event Action OnFuelAmountChanged;

    public static event Action OnFuelDepleted;

    private void Awake()
    {
        ownedFuel = StartFuelAmount;
    }

    private void Update()
    {
        if(WasDebugConsumeFuelPressed())
        {
            ConsumeFuel();
        }
    }

    private bool WasDebugConsumeFuelPressed()
    {
        FuelSettings settings = FuelSettings;
        return settings.EnableDebugConsumeFuelInput
            && settings.DebugConsumeFuelKey != Key.None
            && Keyboard.current != null
            && Keyboard.current[settings.DebugConsumeFuelKey].wasPressedThisFrame;
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
        else
        {
            OnFuelWoodAnimationDone();
        }

    }

    private void OnFuelWoodAnimationDone()
    {
        OnFuelConsumedWithAmount?.Invoke(HeatIncreasedByFuel);
        OnFuelConsumed?.Invoke();
    }


    private void ChangeFuelAmount(float amount)
    {
        ownedFuel += amount;
        ownedFuel = Mathf.Clamp(ownedFuel, 0, StartFuelAmount);
        OnFuelAmountChanged?.Invoke();

    }
}
