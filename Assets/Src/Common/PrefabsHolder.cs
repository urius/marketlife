using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PrefabsHolder", menuName = "Scriptable Objects/Holders/PrefabsHolder")]
public class PrefabsHolder : ScriptableObject
{
    public static PrefabsHolder Instance { get; private set; }

    public GameObject Human;
    public GameObject HeadPrefab;
    public GameObject HeadProfilePrefab;

    public GameObject WhiteSquarePrefab;

    public GameObject FloorPrefab;
    public GameObject WallPrefab;
    public GameObject DoorPrefab;

    public GameObject CashDesk1Prefab;

    public GameObject Shelf1Prefab;
    public GameObject Shelf1ProfilePrefab;
    public GameObject Shelf2Prefab;
    public GameObject Shelf2ProfilePrefab;
    public GameObject Shelf3Prefab;
    public GameObject Shelf3ProfilePrefab;
    public GameObject Shelf4Prefab;
    public GameObject Shelf4ProfilePrefab;
    public GameObject Shelf5Prefab;
    public GameObject Shelf5ProfilePrefab;

    //Interface
    public GameObject UIBottomPanelScrollItem;

    public (GameObject, GameObject) GetShopObjectPrefabs(ShopObjectType type, int level)
    {
        return type switch
        {
            ShopObjectType.Shelf => GetShelfPrefabs(level),
            ShopObjectType.CashDesk => GetCashDeskPrefabs(level),
            _ => throw new ArgumentOutOfRangeException(nameof(level), $"GetShopObjectPrefabs: Unsupported ShopObjectType {type}"),
        };
    }

    public (GameObject, GameObject) GetShelfPrefabs(int level)
    {
        return level switch
        {
            1 => (Shelf1Prefab, Shelf1ProfilePrefab),
            2 => (Shelf2Prefab, Shelf2ProfilePrefab),
            3 => (Shelf3Prefab, Shelf3ProfilePrefab),
            4 => (Shelf4Prefab, Shelf4ProfilePrefab),
            5 => (Shelf5Prefab, Shelf5ProfilePrefab),
            _ => throw new ArgumentOutOfRangeException(nameof(level), $"GetShelfPrefabs: Unsupported shelf level {level}"),
        };
    }

    public (GameObject, GameObject) GetCashDeskPrefabs(int level)
    {
        return level switch
        {
            1 => (CashDesk1Prefab, null),
            _ => throw new ArgumentOutOfRangeException(nameof(level), $"GetShelfPrefabs: Unsupported cashdesk level {level}"),
        };
    }

    private void OnEnable()
    {
        Instance = this;
    }
}
