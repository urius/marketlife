using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PrefabsHolder", menuName = "Scriptable Objects/Holders/PrefabsHolder")]
public class PrefabsHolder : ScriptableObject
{
    public static PrefabsHolder Instance { get; private set; }

    public GameObject Human;
    public GameObject HeadPrefab;
    public GameObject HeadProfilePrefab;

    public GameObject FloorPrefab;
    public GameObject WallPrefab;
    public GameObject DoorPrefab;

    public GameObject CashDesk1Prefab;

    public GameObject Shelf1Prefab;
    public GameObject Shelf1ProfilePrefab;

    public (GameObject, GameObject) GetShelfPrefabs(int level)
    {
        return level switch
        {
            1 => (Shelf1Prefab, Shelf1ProfilePrefab),
            _ => (Shelf1Prefab, Shelf1ProfilePrefab),//throw new ArgumentOutOfRangeException(nameof(level), $"GetShelfPrefabs: Unsupported shelf level {level}"),
        };
    }

    public (GameObject, GameObject) GetCashDeskPrefabs(int level)
    {
        return level switch
        {
            1 => (CashDesk1Prefab, null),
            _ => throw new ArgumentOutOfRangeException(nameof(level), $"GetShelfPrefabs: Unsupported shelf level {level}"),
        };
    }

    private void OnEnable()
    {
        Instance = this;
    }
}
