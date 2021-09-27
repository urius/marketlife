using UnityEngine;

public struct ViewsFactory
{
    public SpriteRenderer CreateFloor(Transform parentTransform, int numericId)
    {
        var result = CreateFloorViewInternal(parentTransform);
        result.sprite = SpritesProvider.Instance.GetFloorSprite(numericId);
        return result;
    }

    public SpriteRenderer CreateGrassFloor(Transform parentTransform, int numericId)
    {
        var result = CreateFloorViewInternal(parentTransform);
        result.sprite = SpritesProvider.Instance.GetGrassSprite(numericId);
        return result;
    }

    public SpriteRenderer CreateWall(Transform parentTransform, int numericId)
    {
        var wallGo = GameObject.Instantiate(PrefabsHolder.Instance.WallPrefab, parentTransform);
        var result = wallGo.GetComponent<SpriteRenderer>();
        result.sprite = SpritesProvider.Instance.GetWallSprite(numericId);
        return result;
    }

    public (Transform transform, SpriteRenderer sprite) CreateWindow(Transform parentTransform, int numericId)
    {
        var windowGo = GameObject.Instantiate(PrefabsHolder.Instance.WindowPrefab, parentTransform);
        var spriteRenderer = windowGo.GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sprite = SpritesProvider.Instance.GetWindowSprite(numericId);
        return (windowGo.transform, spriteRenderer);
    }

    public DoorView CreateDoor(Transform parentTransform, int numericId)
    {
        var doorGo = GameObject.Instantiate(PrefabsHolder.Instance.DoorPrefab, parentTransform);
        var doorlView = doorGo.GetComponent<DoorView>();
        doorlView.SetDoorId(numericId);
        return doorlView;
    }

    private SpriteRenderer CreateFloorViewInternal(Transform parentTransform)
    {
        var floorGo = GameObject.Instantiate(PrefabsHolder.Instance.FloorPrefab, parentTransform);
        var result = floorGo.GetComponent<SpriteRenderer>();
        return result;
    }
}


