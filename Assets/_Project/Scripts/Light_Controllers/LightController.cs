using MoreMountains.Feedbacks;
using UnityEngine;

public class LightController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private StoveController stoveController;

    [Header("Effects/Feedbacks")]
    [SerializeField] private MMF_Player stoveFireLightPop;

    [Space(10)]
    [SerializeField] private FireLightFlicker stoveLightFlicker;

    private StoveLightSettings LightSettings => GameSettings.Current.StoveLight;

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

        StoveLightSettings settings = LightSettings;
        stoveLightFlicker.BaseIntensity = Mathf.Lerp(settings.MinLightIntensity, settings.MaxLightIntensity, stoveController.HeatRatio);
    }
}
