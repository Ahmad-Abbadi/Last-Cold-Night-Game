using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodFuelManager : SerializedMonoBehaviour
{
    [SerializeField] private Stack<WoodFuel> fuelWood;

    [Header("References")]
    [SerializeField] private Transform _handTransform;
    [SerializeField] private Transform[] _targetTransform;
    [SerializeField] private Transform woodSpawenPoint;

    [Header("Spawn")]
    [SerializeField] private float delayBetweenSpawns = 0.05f;
    [SerializeField] private Vector3 randomPositionOffset = new Vector3(0.08f, 0.02f, 0.08f);
    [SerializeField] private Vector3 randomRotationOffset = new Vector3(8f, 180f, 8f);

    private Transform[] _targetRotationTransform;

    private System.Random rand;
    private WoodFuelManagerSettings settings;

    public Stack<WoodFuel> FuelWood { get => fuelWood; set => fuelWood = value; }

    private WoodFuelManagerSettings ManagerSettings => GameSettings.Current.WoodFuelManager;

    private void Awake()
    {
        if (fuelWood == null)
            fuelWood = new Stack<WoodFuel>();

        if (_targetTransform == null || _targetTransform.Length == 0)
            return;

        _targetRotationTransform = (Transform[])_targetTransform.Clone();

        rand = new System.Random();
        settings = ManagerSettings;
    }

    private void AssignWoodData()
    {
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

        WoodFuel woodFuelMovement = fuelWood.Pop();

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

    public void InitializeWood(int amount)
    {
        if (fuelWood == null)
            fuelWood = new Stack<WoodFuel>();

        StartCoroutine(InitializeWoodRoutine(amount));
    }

    private IEnumerator InitializeWoodRoutine(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Vector3 spawnPosition = woodSpawenPoint.position + GetRandomSpawnOffset();
            Quaternion spawnRotation = woodSpawenPoint.rotation * Quaternion.Euler(GetRandomSpawnRotation());

            GameObject woodInstance = Instantiate(
                ManagerSettings.WoodPrefab,
                spawnPosition,
                spawnRotation
            );

            WoodFuel woodFuelComponent = woodInstance.GetComponent<WoodFuel>();
            if (woodFuelComponent == null)
            {
                woodFuelComponent = woodInstance.AddComponent<WoodFuel>();
            }

            fuelWood.Push(woodFuelComponent);

            yield return new WaitForSeconds(delayBetweenSpawns);
        }

        AssignWoodData();
    }

    private Vector3 GetRandomSpawnOffset()
    {
        return new Vector3(
            UnityEngine.Random.Range(-randomPositionOffset.x, randomPositionOffset.x),
            UnityEngine.Random.Range(-randomPositionOffset.y, randomPositionOffset.y),
            UnityEngine.Random.Range(-randomPositionOffset.z, randomPositionOffset.z)
        );
    }

    private Vector3 GetRandomSpawnRotation()
    {
        return new Vector3(
            UnityEngine.Random.Range(-randomRotationOffset.x, randomRotationOffset.x),
            UnityEngine.Random.Range(-randomRotationOffset.y, randomRotationOffset.y),
            UnityEngine.Random.Range(-randomRotationOffset.z, randomRotationOffset.z)
        );
    }
}