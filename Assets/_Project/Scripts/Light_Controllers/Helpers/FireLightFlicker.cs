using UnityEngine;

[RequireComponent(typeof(Light))]
public class FireLightFlicker : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Light targetLight;

    private Vector3 initialLocalPosition;
    private float baseIntensity;
    private float baseRange;
    private float intensityNoiseSeed;
    private float rangeNoiseSeed;
    private float posXNoiseSeed;
    private float posYNoiseSeed;
    private float posZNoiseSeed;

    public float BaseIntensity { get => baseIntensity; set => baseIntensity = value; }

    private FireLightFlickerSettings FlickerSettings => GameSettings.Current.FireLightFlicker;

    private void Awake()
    {
        if (targetLight == null)
            targetLight = GetComponent<Light>();

        initialLocalPosition = transform.localPosition;

        intensityNoiseSeed = Random.Range(0f, 999f);
        rangeNoiseSeed = Random.Range(0f, 999f);
        posXNoiseSeed = Random.Range(0f, 999f);
        posYNoiseSeed = Random.Range(0f, 999f);
        posZNoiseSeed = Random.Range(0f, 999f);

        FireLightFlickerSettings settings = FlickerSettings;
        BaseIntensity = settings.BaseIntensity;
        baseRange = settings.BaseRange;

        if (targetLight != null)
        {
            targetLight.intensity = BaseIntensity;
            targetLight.range = baseRange;
        }
    }

    private void Update()
    {
        if (targetLight == null)
            return;

        FireLightFlickerSettings settings = FlickerSettings;
        baseRange = settings.BaseRange;

        float time = Time.time * settings.FlickerSpeed;

        float intensityNoise = Mathf.PerlinNoise(intensityNoiseSeed, time) * 2f - 1f;
        float rangeNoise = Mathf.PerlinNoise(rangeNoiseSeed, time * 0.85f) * 2f - 1f;

        float targetIntensity = BaseIntensity + intensityNoise * settings.IntensityVariation;
        float targetRange = baseRange + rangeNoise * settings.RangeVariation;

        targetLight.intensity = Mathf.Lerp(targetLight.intensity, targetIntensity, Time.deltaTime * settings.Smoothing);
        targetLight.range = Mathf.Lerp(targetLight.range, targetRange, Time.deltaTime * settings.Smoothing);

        if (settings.UsePositionJitter)
        {
            float posTime = Time.time * settings.PositionJitterSpeed;

            float x = (Mathf.PerlinNoise(posXNoiseSeed, posTime) * 2f - 1f) * settings.PositionJitterAmount;
            float y = (Mathf.PerlinNoise(posYNoiseSeed, posTime * 0.9f) * 2f - 1f) * settings.PositionJitterAmount * 0.5f;
            float z = (Mathf.PerlinNoise(posZNoiseSeed, posTime * 1.1f) * 2f - 1f) * settings.PositionJitterAmount;

            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                initialLocalPosition + new Vector3(x, y, z),
                Time.deltaTime * settings.Smoothing
            );
        }
    }

    private void OnDisable()
    {
        if (targetLight != null)
        {
            targetLight.intensity = BaseIntensity;
            targetLight.range = baseRange;
        }

        transform.localPosition = initialLocalPosition;
    }
}
