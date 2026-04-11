using Sirenix.OdinInspector;
using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WoodFuel : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Settings")]
    [SerializeField] private float _moveDuration = 0.6f;
    [SerializeField] private float _arcHeight = 0.25f;
    [SerializeField] private float _rotationLerpSpeed = 12f;
    [SerializeField] private float _stopDistance = 0.02f;
    [SerializeField] private bool _releasePhysicsOnFinish = false;

    [Header("Refs")]
    [SerializeField] private Transform _handTransform;
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private Transform _targetRotationTransform;

    [Header("Hand Hold Offset")]
    [SerializeField] private Vector3 _handPositionOffset;
    [SerializeField] private Vector3 _handRotationOffset;

    private Vector3 _resetPosition;
    private Quaternion _resetRotation;

    private Vector3 _moveStartPosition;
    private Quaternion _moveStartRotation;
    private Vector3 _moveEndPosition;
    private Quaternion _moveEndRotation;

    private float _moveTimer;
    private bool _isMoving;

    public event Action MoveCompleted;

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

        float t = Mathf.Clamp01(_moveTimer / _moveDuration);
        float easedT = Mathf.SmoothStep(0f, 1f, t);

        Vector3 linearPosition = Vector3.Lerp(_moveStartPosition, _moveEndPosition, easedT);
        float arc = Mathf.Sin(easedT * Mathf.PI) * _arcHeight;
        Vector3 finalPosition = linearPosition + Vector3.up * arc;

        rb.MovePosition(finalPosition);

        Quaternion targetRotation = Quaternion.Slerp(_moveStartRotation, _moveEndRotation, easedT);
        Quaternion smoothedRotation = Quaternion.Slerp(rb.rotation, targetRotation, _rotationLerpSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(smoothedRotation);

        if (Vector3.Distance(rb.position, _moveEndPosition) <= _stopDistance || t >= 1f)
        {
            rb.MovePosition(_moveEndPosition);
            rb.MoveRotation(_moveEndRotation);

            _isMoving = false;

            if (_releasePhysicsOnFinish)
            {
                rb.isKinematic = false;
                MoveCompleted?.Invoke();
            }
            else
                rb.isKinematic = true;
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

        if (_handTransform != null)
        {
            _moveStartPosition = _handTransform.TransformPoint(_handPositionOffset);
            _moveStartRotation = _handTransform.rotation * Quaternion.Euler(_handRotationOffset);

            rb.position = _moveStartPosition;
            rb.rotation = _moveStartRotation;
        }
        else
        {
            _moveStartPosition = rb.position;
            _moveStartRotation = rb.rotation;
        }

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