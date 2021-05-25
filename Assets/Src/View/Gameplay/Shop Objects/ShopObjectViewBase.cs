using UnityEngine;

public class ShopObjectViewBase : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] _allSprites;

    public void SetAllSpritesAlpha(float alpha)
    {
        foreach (var sprite in _allSprites)
        {
            sprite.color = sprite.color.SetAlpha(alpha);
        }
    }

    public void SetAllSpritesColor(Color color)
    {
        foreach (var sprite in _allSprites)
        {
            sprite.color = color;
        }
    }
}
