using UnityEngine;

namespace Src.View.Gameplay.Human
{
    public class HumanHeadViewProfile : HumanHeadViewBase
    {
        public const string HAIR_PREFIX = "Hair";
        public const string HAIR_POSTFIX = "_Profile";

        [SerializeField] private SpriteRenderer _hatSprite;
        [SerializeField] private SpriteRenderer _hairSprite;
        [SerializeField] private SpriteRenderer _headBaseSprite;

        protected void Start()
        {
            _headBaseSprite.sprite = SpritesProvider.GetHumanSprite("HeadBase_Profile");
        }

        public override void SetHatSprite(Sprite hatSprite)
        {
            _hatSprite.sprite = hatSprite;
            _hatSprite.gameObject.SetActive(hatSprite != null);
        }

        public override void SetHair(int hairId)
        {
            _hairSprite.sprite = SpritesProvider.GetHumanSprite($"{HAIR_PREFIX}{hairId}{HAIR_POSTFIX}");
        }
    }
}
