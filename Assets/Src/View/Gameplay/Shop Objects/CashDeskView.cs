using Src.View.Gameplay.Human;
using UnityEngine;

namespace Src.View.Gameplay.Shop_Objects
{
    public class CashDeskView : ShopObjectViewBase
    {
        [SerializeField] private SpriteRenderer _hatSprite;
        [SerializeField] private HumanHeadView _cashManHeadView;
        [SerializeField] private SpriteRenderer _cashManTorsoView;
        [SerializeField] private SpriteRenderer _cashManLeftHandView;
        [SerializeField] private SpriteRenderer _cashManRightHandView;
        //
        public HumanHeadView HeadView => _cashManHeadView;

        public void SetHatSprite(Sprite hatSprite)
        {
            _hatSprite.sprite = hatSprite;
            _hatSprite.gameObject.SetActive(hatSprite != null);
        }

        public void SetTorsoSprite(Sprite sprite)
        {
            _cashManTorsoView.sprite = sprite;
        }

        public void SetHandsSprite(Sprite sprite)
        {
            _cashManLeftHandView.sprite = sprite;
            _cashManRightHandView.sprite = sprite;
        }
    }
}
