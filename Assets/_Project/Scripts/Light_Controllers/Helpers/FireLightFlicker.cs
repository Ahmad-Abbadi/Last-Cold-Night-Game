using UnityEngine;

[RequireComponent(typeof(Light))]
public class FireLightFlicker : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] private Light targetLight;
    [SerializeField] private float baseIntensity = 4f;
    [SerializeField] private float baseRange = 8f;

    [Header("Flicker")]
    [SerializeField] private float intensityVariation = 0.8f;
    [SerializeField] private float rangeVariation = 0.35f;
    [SerializeField] private float flickerSpeed = 6f;
    [SerializeField] private float smoothing = 12f;

    [Header("Position Jitter")]
    [SerializeField] private bool usePositionJitter = true;
    [SerializeField] private float positionJitterAmount = 0.03f;
    [SerializeField] private float positionJitterSpeed = 4f;

    private Vector3 initialLocalPosition;
    private float intensityNoiseSeed;
    private float rangeNoiseSeed;
    private float posXNoiseSeed;
    private float posYNoiseSeed;
    private float posZNoiseSeed;

    public float BaseIntensity { get => baseIntensity; set => baseIntensity = value; }

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

            BaseIntensity = targetLight.intensity;

            baseRange = targetLight.range;
    }

    private void Update()
    {
        float time = Time.time * flickerSpeed;

        float intensityNoise = Mathf.PerlinNoise(intensityNoiseSeed, time) * 2f - 1f;
        float rangeNoise = Mathf.PerlinNoise(rangeNoiseSeed, time * 0.85f) * 2f - 1f;

        float targetIntensity = BaseIntensity + intensityNoise * intensityVariation;
        float targetRange = baseRange + rangeNoise * rangeVariation;

        targetLight.intensity = Mathf.Lerp(targetLight.intensity, targetIntensity, Time.deltaTime * smoothing);
        targetLight.range = Mathf.Lerp(targetLight.range, targetRange, Time.deltaTime * smoothing);

        if (usePositionJitter)
        {
            float posTime = Time.time * positionJitterSpeed;

            float x = (Mathf.PerlinNoise(posXNoiseSeed, posTime) * 2f - 1f) * positionJitterAmount;
            float y = (Mathf.PerlinNoise(posYNoiseSeed, posTime * 0.9f) * 2f - 1f) * positionJitterAmount * 0.5f;
            float z = (Mathf.PerlinNoise(posZNoiseSeed, posTime * 1.1f) * 2f - 1f) * positionJitterAmount;

            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                initialLocalPosition + new Vector3(x, y, z),
                Time.deltaTime * smoothing
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