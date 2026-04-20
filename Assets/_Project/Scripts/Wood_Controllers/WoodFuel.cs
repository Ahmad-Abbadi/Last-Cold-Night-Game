using Sirenix.OdinInspector;
using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WoodFuel : MonoBehaviour
{
    private Rigidbody rb;

    [HideInInspector] public Transform _handTransform;
    [HideInInspector] public Transform _targetTransform;
    [HideInInspector] public Transform _targetRotationTransform;

    [HideInInspector] public Vector3 _handPositionOffset;
    [HideInInspector] public Vector3 _handRotationOffset;

    private Vector3 _resetPosition;
    private Quaternion _resetRotation;

    private Vector3 _moveStartPosition;
    private Quaternion _moveStartRotation;
    private Vector3 _moveEndPosition;
    private Quaternion _moveEndRotation;

    private float _moveTimer;
    private bool _isMoving;

    public event Action MoveCompleted;

    private WoodFuelMotionSettings MotionSettings => GameSettings.Current.WoodFuelMotion;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        _resetPosition = transform.position;
        _resetRotation = transform.rotation;
    }

    private void FixedUpdate()
    {
        if (!_isMoving || _targetTransform == null)
            return;

        _moveTimer += Time.fixedDeltaTime;

        WoodFuelMotionSettings settings = MotionSettings;
        float t = Mathf.Clamp01(_moveTimer / settings.MoveDuration);
        float easedT = settings.MoveCurve == null ? t : Mathf.Clamp01(settings.MoveCurve.Evaluate(t));

        Vector3 linearPosition = Vector3.Lerp(_moveStartPosition, _moveEndPosition, easedT);
        float arc = Mathf.Sin(easedT * Mathf.PI) * settings.ArcHeight;
        Vector3 finalPosition = linearPosition + Vector3.up * arc;

        rb.MovePosition(finalPosition);

        Quaternion targetRotation = Quaternion.Slerp(_moveStartRotation, _moveEndRotation, easedT);
        Quaternion smoothedRotation = Quaternion.Slerp(rb.rotation, targetRotation, settings.RotationLerpSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(smoothedRotation);

        if (Vector3.Distance(rb.position, _moveEndPosition) <= settings.StopDistance || t >= 1f)
        {
            rb.MovePosition(_moveEndPosition);
            rb.MoveRotation(_moveEndRotation);

            _isMoving = false;

            if (settings.ReleasePhysicsOnFinish)
                rb.isKinematic = false;
            else
                rb.isKinematic = true;

            MoveCompleted?.Invoke();
        }
    }

    [Button]
    public void SnapWoodToHand()
    {
        if (_handTransform == null)
            return;

        Vector3 handWorldPosition = _handTransform.TransformPoint(_handPositionOffset);
        Quaternion handWorldRotation = _handTransform.rotation * Quaternion.Euler(_handRotationOffset);

        transform.position = handWorldPosition;
        transform.rotation = handWorldRotation;

        rb.position = handWorldPosition;
        rb.rotation = handWorldRotation;

        _resetPosition = handWorldPosition;
        _resetRotation = handWorldRotation;
    }

    [Button]
    public void MoveWoodToTargetPosition()
    {
        if (_targetTransform == null)
            return;

        
        _moveStartPosition = rb.position;
        _moveStartRotation = rb.rotation;

        _moveEndPosition = _targetTransform.position;
        _moveEndRotation = _targetRotationTransform != null ? _targetRotationTransform.rotation : rb.rotation;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        _moveTimer = 0f;
        _isMoving = true;
    }

    [Button]
    private void ResetWoodPosition()
    {
        _isMoving = false;
        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.position = _resetPosition;
        rb.rotation = _resetRotation;

        transform.position = _resetPosition;
        transform.rotation = _resetRotation;
    }
}
