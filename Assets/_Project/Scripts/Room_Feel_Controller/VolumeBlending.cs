using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class VolumeBlending : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private StoveController stoveController;

    [Header("Blending Settings")]
    [SerializeField, Min(0.01f)] private float blendDuration = 1f;

    private Coroutine _blendRoutine;
    private Volume _currentVolume;
    private bool _isInitialized;

    private void OnEnable()
    {
        if (stoveController != null)
            stoveController.HeatTypeChanged += OnHeatTypeChanged;
    }

    private void OnDisable()
    {
        if (stoveController != null)
            stoveController.HeatTypeChanged -= OnHeatTypeChanged;
    }

    private void OnHeatTypeChanged(HeatLevelType type)
    {
        Volume targetVolume = RoomBlackboard.Instance.Volumes[type];
        BlendVolume(targetVolume);
    }

    public void BlendVolume(Volume targetVolume)
    {
        if (targetVolume == null)
        {
            Debug.LogWarning("BlendVolume called with null.");
            return;
        }

        var allVolumes = RoomBlackboard.Instance.Volumes.Values;

        if (!_isInitialized)
        {
            if (_blendRoutine != null)
            {
                StopCoroutine(_blendRoutine);
                _blendRoutine = null;
            }

            foreach (var volume in allVolumes)
            {
                if (volume == null) continue;
                volume.weight = (volume == targetVolume) ? 1f : 0f;
            }

            _currentVolume = targetVolume;
            _isInitialized = true;
            return;
        }

        if (_currentVolume == targetVolume)
        {
            foreach (var volume in allVolumes)
            {
                if (volume == null) continue;
                volume.weight = (volume == targetVolume) ? 1f : 0f;
            }

            return;
        }

        if (_blendRoutine != null)
        {
            StopCoroutine(_blendRoutine);
            _blendRoutine = null;
        }

        _blendRoutine = StartCoroutine(BlendCoroutine(targetVolume));
    }

    private IEnumerator BlendCoroutine(Volume targetVolume)
    {
        var allVolumes = RoomBlackboard.Instance.Volumes.Values;
        Dictionary<Volume, float> startWeights = new Dictionary<Volume, float>();

        foreach (var volume in allVolumes)
        {
            if (volume == null) continue;
            startWeights[volume] = volume.weight;
        }

        float elapsed = 0f;

        while (elapsed < blendDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / blendDuration);

            t = t * t * (3f - 2f * t);

            foreach (var volume in allVolumes)
            {
                if (volume == null) continue;

                float startWeight = startWeights[volume];
                float endWeight = (volume == targetVolume) ? 1f : 0f;

                volume.weight = Mathf.Lerp(startWeight, endWeight, t);
            }

            yield return null;
        }

        foreach (var volume in allVolumes)
        {
            if (volume == null) continue;
            volume.weight = (volume == targetVolume) ? 1f : 0f;
        }

        _currentVolume = targetVolume;
        _blendRoutine = null;
    }
}