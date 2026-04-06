using MoreMountains.Feedbacks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FuelUIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FuelController fuelController;

    [Space(10)]
    [SerializeField] private TextMeshProUGUI fuelAmountText;

    [Header("Effects/Feedbacks")]
    [SerializeField] private MMF_Player fuelAmountEffect;
    [SerializeField] private MMF_Player fuelDepletedEffect;


    private void Awake()
    {
        fuelController = GetComponent<FuelController>();

    }

    private void OnEnable()
    {
        FuelController.OnFuelAmountChanged += OnFuelAmountChanged;
        FuelController.OnFuelDepleted += OnFuelDepleted;
    }

    private void OnDisable()
    {
        FuelController.OnFuelAmountChanged -= OnFuelAmountChanged;
        FuelController.OnFuelDepleted -= OnFuelDepleted;
    }

    private void Start()
    {
        UpdateFuelAmountText(fuelController.OwendFuel);
    }


    private void OnFuelAmountChanged()
    {
        UpdateFuelAmountText(fuelController.OwendFuel);
    }

    private void OnFuelDepleted()
    {
        fuelDepletedEffect?.PlayFeedbacks();
    }

    private void UpdateFuelAmountText(float amount)
    {
        fuelAmountText.text = "Fuel Amount: " + $"{amount}";
        fuelAmountEffect?.PlayFeedbacks();
    }
}
