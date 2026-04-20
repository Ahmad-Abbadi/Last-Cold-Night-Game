using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WoodFuelManager : SerializedMonoBehaviour
{
    [SerializeField] private Queue<WoodFuel> fuelWood;

    [Header("Settings")]
    [SerializeField] private bool snapToHandOnStart = false;

    [Header("References")]
    [SerializeField] private Transform _handTransform;
    [SerializeField] private Transform[] _targetTransform;
    private Transform[] _targetRotationTransform;

    [Header("Hand Hold Offset")]
    [SerializeField] private Vector3 _handPositionOffset;
    [SerializeField] private Vector3 _handRotationOffset;

    public Queue<WoodFuel> FuelWood { get => fuelWood; set => fuelWood = value; }

    private void Awake()
    {
        _targetRotationTransform = (Transform[])_targetTransform.Clone();

        System.Random rand = new System.Random();
        foreach (WoodFuel wood in fuelWood)
        {
            int randomIndex = rand.Next(_targetTransform.Length);

            wood._handTransform = _handTransform;
            wood._targetTransform = _targetTransform[randomIndex];
            wood._targetRotationTransform = _targetRotationTransform[randomIndex];
            wood._handPositionOffset = _handPositionOffset;
            wood._handRotationOffset = _handRotationOffset;
        }
    }
    public void DoWoodFuelAnimation(Action action)
    {
        if (fuelWood == null || fuelWood.Count == 0)
            return;

        WoodFuel woodFuelMovement = fuelWood.Dequeue();

        if (snapToHandOnStart)
        {
            woodFuelMovement.SnapWoodToHand();
        }

        void OnMoveCompleted()
        {
            woodFuelMovement.MoveCompleted -= OnMoveCompleted;
            action?.Invoke();
        }

        woodFuelMovement.MoveCompleted += OnMoveCompleted;
        woodFuelMovement.MoveWoodToTargetPosition();
    }
}