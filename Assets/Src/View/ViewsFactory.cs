using UnityEngine;

public struct ViewsFactory
{
    public SpriteRenderer CreateFloor(Transform parentTransform, int numericId)
    {
        var floorGo = GameObject.Instantiate(PrefabsHolder.Instance.FloorPrefab, parentTransform);
        var result = floorGo.GetComponent<SpriteRenderer>();
        result.sprite = SpritesProvider.Instance.GetFloorSprite(numericId);

        return result;
    }
}
