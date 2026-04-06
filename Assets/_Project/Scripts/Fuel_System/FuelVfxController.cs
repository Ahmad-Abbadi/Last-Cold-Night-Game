using MoreMountains.Feedbacks;
using UnityEngine;

public class FuelVfxController : MonoBehaviour
{
    [Header("Effects/Feedbacks")]
    [SerializeField] private MMF_Player stoveSpark;



    private void OnEnable()
    {
        FuelController.OnFuelConsumed += OnFuelConsumed;
    }

    private void OnDisable()
    {
        FuelController.OnFuelConsumed -= OnFuelConsumed;
    }

    private void OnFuelConsumed()
    {
        stoveSpark?.PlayFeedbacks();
    }
}
