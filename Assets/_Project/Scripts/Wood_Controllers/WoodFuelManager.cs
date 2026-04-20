using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WoodFuelManager : SerializedMonoBehaviour
{
    [SerializeField] private Queue<WoodFuel> fuelWood;

    [Header("References")]
    [SerializeField] private Transform _handTransform;
    [SerializeField] private Transform[] _targetTransform;
    private Transform[] _targetRotationTransform;

    public Queue<WoodFuel> FuelWood { get => fuelWood; set => fuelWood = value; }

    private WoodFuelManagerSettings ManagerSettings => GameSettings.Current.WoodFuelManager;

    private void Awake()
    {
        if (fuelWood == null || _targetTransform == null || _targetTransform.Length == 0)
            return;

        _targetRotationTransform = (Transform[])_targetTransform.Clone();

        System.Random rand = new System.Random();
        WoodFuelManagerSettings settings = ManagerSettings;

        foreach (WoodFuel wood in fuelWood)
        {
            if (wood == null)
                continue;

            int randomIndex = rand.Next(_targetTransform.Length);

            wood._handTransform = _handTransform;
            wood._targetTransform = _targetTransform[randomIndex];
            wood._targetRotationTransform = _targetRotationTransform[randomIndex];
            wood._handPositionOffset = settings.HandPositionOffset;
            wood._handRotationOffset = settings.HandRotationOffset;
        }
    }
    public void DoWoodFuelAnimation(Action action)
    {
        if (fuelWood == null || fuelWood.Count == 0)
            return;

        WoodFuel woodFuelMovement = fuelWood.Dequeue();

        if (ManagerSettings.SnapToHandOnStart)
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
