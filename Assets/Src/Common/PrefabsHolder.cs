using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PrefabsHolder", menuName = "Scriptable Objects/Holders/PrefabsHolder")]
public class PrefabsHolder : ScriptableObject
{
    public static PrefabsHolder Instance { get; private set; }

    public const string PSStarsName = "PS_Stars";

    public GameObject Human;
    public GameObject HeadPrefab;
    public GameObject HeadProfilePrefab;

    public GameObject WhiteSquarePrefab;

    public GameObject FloorPrefab;
    public GameObject OnFloorItemPrefab;
    public GameObject WallPrefab;
    public GameObject WindowPrefab;
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

    //GUI
    public GameObject TutorialCellPointerPrefab;

    //Interface
    public GameObject UIRaycastsBlocker;
    public GameObject UIBottomPanelScrollItemPrefab;
    public GameObject UIHintPrefab;
    public GameObject UIShelfActionsPrefab;
    public GameObject UIFlyingTextImagePrefab;
    public GameObject UIFlyingTextPrefab;
    public GameObject UITextPopupPrefab;
    public GameObject UIPlacingProductPrefab;
    public GameObject UIContentPopupPrefab;
    public GameObject UIContentPopupPlusPrefab;
    public GameObject UITabbedContentPopupPrefab;
    public GameObject UIOrderProductPopupItemPrefab;
    public GameObject UIShelfContentPopupItemPrefab;
    public GameObject UIWarehousePopupItemPrefab;
    public GameObject UIUpgradePopupItemPrefab;
    public GameObject UIOfflineReportPopupCaptionItemPrefab;
    public GameObject UIOfflineReportPopupItemPrefab;
    public GameObject UILevelUpPopupCaptionItemPrefab;
    public GameObject UILevelUpPopupItemPrefab;
    public GameObject UITutorialOverlayPrefab;

    //remote
    private Dictionary<string, GameObject> _remotePrefabs = new Dictionary<string, GameObject>();

    public (GameObject, GameObject) GetShopObjectPrefabs(ShopObjectType type, int level)
    {
        return type switch
        {
            ShopObjectType.Shelf => GetShelfPrefabs(level),
            ShopObjectType.CashDesk => GetCashDeskPrefabs(level),
            _ => throw new ArgumentOutOfRangeException(nameof(level), $"GetShopObjectPrefabs: Unsupported ShopObjectType {type}"),
        };
    }

    public (GameObject, GameObject) GetShelfPrefabs(int numericId)
    {
        return numericId switch
        {
            1 => (Shelf1Prefab, Shelf1ProfilePrefab),
            2 => (Shelf2Prefab, Shelf2ProfilePrefab),
            3 => (Shelf3Prefab, Shelf3ProfilePrefab),
            4 => (Shelf4Prefab, Shelf4ProfilePrefab),
            5 => (Shelf5Prefab, Shelf5ProfilePrefab),
            _ => throw new ArgumentOutOfRangeException(nameof(numericId), $"GetShelfPrefabs: Unsupported shelf numericId {numericId}"),
        };
    }

    public (GameObject, GameObject) GetCashDeskPrefabs(int numericId)
    {
        return numericId switch
        {
            1 => (CashDesk1Prefab, null),
            _ => throw new ArgumentOutOfRangeException(nameof(numericId), $"GetShelfPrefabs: Unsupported cashdesk numericId {numericId}"),
        };
    }

    public void SetupRemotePrefab(string name, GameObject prefab)
    {
        _remotePrefabs[name] = prefab;
    }

    public GameObject GetRemotePrefab(string name)
    {
        return _remotePrefabs[name];
    }

    private void OnEnable()
    {
        Instance = this;
    }
}
