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

    private void OnEnable()
    {
        Instance = this;
    }
}
