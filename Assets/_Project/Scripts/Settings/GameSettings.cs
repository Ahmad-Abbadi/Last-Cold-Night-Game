using UnityEngine;

[CreateAssetMenu(menuName = "Last Cold Night/Game Settings", fileName = "GameSettings")]
public class GameSettings : ScriptableObject
{
    private const string DefaultResourcePath = "GameSettings";

    private static GameSettings current;

    public StoveHeatSettings StoveHeat = new StoveHeatSettings();
    public FuelSettings Fuel = new FuelSettings();
    public FuelUiSettings FuelUi = new FuelUiSettings();
    public WoodFuelMotionSettings WoodFuelMotion = new WoodFuelMotionSettings();
    public WoodFuelManagerSettings WoodFuelManager = new WoodFuelManagerSettings();
    public StoveLightSettings StoveLight = new StoveLightSettings();
    public FireLightFlickerSettings FireLightFlicker = new FireLightFlickerSettings();
    public StoveFireVisualSettings StoveFireVisuals = new StoveFireVisualSettings();
    public LightingScenarioSettings LightingScenarios = new LightingScenarioSettings();
    public VolumeBlendSettings VolumeBlending = new VolumeBlendSettings();

    public static GameSettings Current
    {
        get
        {
            if (current == null)
            {
                current = Resources.Load<GameSettings>(DefaultResourcePath);

                if (current == null)
                {
                    Debug.LogWarning("GameSettings asset not found at Resources/GameSettings. Using runtime defaults.");
                    current = CreateInstance<GameSettings>();
                    current.name = "Runtime GameSettings Defaults";
                }
            }

            current.EnsureValid();
            return current;
        }
    }

    private void OnValidate()
    {
        EnsureValid();
    }

    private void EnsureValid()
    {
        if (StoveHeat == null) StoveHeat = new StoveHeatSettings();
        if (Fuel == null) Fuel = new FuelSettings();
        if (FuelUi == null) FuelUi = new FuelUiSettings();
        if (WoodFuelMotion == null) WoodFuelMotion = new WoodFuelMotionSettings();
        if (WoodFuelManager == null) WoodFuelManager = new WoodFuelManagerSettings();
        if (StoveLight == null) StoveLight = new StoveLightSettings();
        if (FireLightFlicker == null) FireLightFlicker = new FireLightFlickerSettings();
        if (StoveFireVisuals == null) StoveFireVisuals = new StoveFireVisualSettings();
        if (LightingScenarios == null) LightingScenarios = new LightingScenarioSettings();
        if (VolumeBlending == null) VolumeBlending = new VolumeBlendSettings();

        StoveHeat.Validate();
        Fuel.Validate();
        WoodFuelMotion.Validate();
        StoveLight.Validate();
        FireLightFlicker.Validate();
        StoveFireVisuals.Validate();
        LightingScenarios.Validate();
        VolumeBlending.Validate();
    }
}
