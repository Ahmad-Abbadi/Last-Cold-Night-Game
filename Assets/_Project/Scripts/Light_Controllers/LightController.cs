using MoreMountains.Feedbacks;
using UnityEngine;

public class LightController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float maxLightIntensity;
    [SerializeField] private float minLightIntensity;

    [Header("References")]
    [SerializeField] private StoveController stoveController;

    [Header("Effects/Feedbacks")]
    [SerializeField] private MMF_Player stoveFireLightPop;

    [Space(10)]
    [SerializeField] private FireLightFlicker stoveLightFlicker;


    private void OnEnable()
    {
        FuelController.OnFuelConsumed += OnFuelConsumed;
    }

    private void OnDisable()
    {
        FuelController.OnFuelConsumed -= OnFuelConsumed;
    }

    private void Update()
    {
        UpdateLightFromHeat();
    }

    private void OnFuelConsumed()
    {
        stoveFireLightPop?.PlayFeedbacks();
    }


    private void UpdateLightFromHeat()
    {
        if (stoveController == null || stoveLightFlicker == null)
            return;

        stoveLightFlicker.BaseIntensity = Mathf.Lerp(minLightIntensity, maxLightIntensity, stoveController.HeatRatio);
    }
}
