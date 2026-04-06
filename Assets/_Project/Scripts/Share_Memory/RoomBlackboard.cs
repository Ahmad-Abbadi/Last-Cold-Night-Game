using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class RoomBlackboard : SerializedMonoBehaviour
{
    public static RoomBlackboard Instance;

    [SerializeField] Dictionary<HeatLevelType, Volume> volumes;

    public Dictionary<HeatLevelType, Volume> Volumes { get => volumes; set => volumes = value; }

    private void Awake()
    {
        if(Instance == null)
        {
           Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
}