using System;
using UnityEngine;

public class ShelfView : ShopObjectViewBase
{
    [SerializeField] private ProductFloor[] _productFloors;
    public int FloorsCount => _productFloors.Length;

    public void Awake()
    {
        for (var floorIndex = 0; floorIndex < FloorsCount; floorIndex++)
        {
            var spriteRenderers = _productFloors[floorIndex].ProductSprites;
            for (var i = 0; i < spriteRenderers.Length; i++)
            {
                spriteRenderers[i].sprite = null;
            }
        }
    }

    public void SetProductSpriteOnFloor(int floorIndex, Sprite sprite, float fullness)
    {
        var spriteRenderers = _productFloors[floorIndex].ProductSprites;
        for (var i = 0; i < spriteRenderers.Length; i++)
        {
            var rank = (float)i / spriteRenderers.Length;
            spriteRenderers[i].sprite = rank < fullness ? sprite : null;
        }
    }

    public void EmptyFloor(int floorIndex)
    {
        var spriteRenderers = _productFloors[floorIndex].ProductSprites;
        foreach (var spriteRenderer in spriteRenderers)
        {
            spriteRenderer.sprite = null;
        }
    }
}

[Serializable]
public class ProductFloor
{
    public SpriteRenderer[] ProductSprites;
}
