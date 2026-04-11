using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WoodFuelManager : SerializedMonoBehaviour
{
    [SerializeField] private Queue<WoodFuel> fuelWood;

    public Queue<WoodFuel> FuelWood { get => fuelWood; set => fuelWood = value; }

    public void DoWoodFuelAnimation(Action action)
    {
        if (fuelWood == null || fuelWood.Count == 0)
            return;

        WoodFuel woodFuelMovement = fuelWood.Dequeue();
        woodFuelMovement.SnapWoodToHand();

        void OnMoveCompleted()
        {
            woodFuelMovement.MoveCompleted -= OnMoveCompleted;
            action?.Invoke();
        }

        woodFuelMovement.MoveCompleted += OnMoveCompleted;
        woodFuelMovement.MoveWoodToTargetPosition();
    }
}