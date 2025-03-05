using Src.Common_Utils;
using UnityEngine;

namespace Src.View.Gameplay.Shop_Objects
{
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
}
