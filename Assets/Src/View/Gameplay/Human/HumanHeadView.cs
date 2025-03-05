using UnityEngine;

namespace Src.View.Gameplay.Human
{
    public class HumanHeadView : HumanHeadViewBase
    {
        [SerializeField] private Animator _headAnimator;
        [SerializeField] private SpriteRenderer _hatSprite;
        [SerializeField] private SpriteRenderer _hairSprite;
        [SerializeField] private SpriteRenderer _headBaseSprite;
        [SerializeField] private SpriteRenderer _brow1Sprite;
        [SerializeField] private SpriteRenderer _brow2Sprite;
        [SerializeField] private SpriteRenderer _eye1Sprite;
        [SerializeField] private SpriteRenderer _eyePupil1Sprite;
        [SerializeField] private SpriteRenderer _eye2Sprite;
        [SerializeField] private SpriteRenderer _eyePupil2Sprite;
        [SerializeField] private SpriteRenderer _mouthSprite;
        [SerializeField] private SpriteRenderer _glassesSprite;

        void Start()
        {
            _headBaseSprite.sprite = SpritesProvider.GetHumanSprite("HeadBase");
            _brow1Sprite.sprite = SpritesProvider.GetHumanSprite("Brow1Base");
            _brow2Sprite.sprite = SpritesProvider.GetHumanSprite("Brow1Base");
            _eye1Sprite.sprite = SpritesProvider.GetHumanSprite("Eye2Base");
            _eyePupil1Sprite.sprite = SpritesProvider.GetHumanSprite("EyePupilBase");
            _eye2Sprite.sprite = SpritesProvider.GetHumanSprite("Eye2Base");
            _eyePupil2Sprite.sprite = SpritesProvider.GetHumanSprite("EyePupilBase");
            SetMouth(1);
        }

        public void ShowFaceAnimation(int index)
        {
            _headAnimator.SetInteger("animation_index", index);
        }

        public void AnimationEventSwitchMouthSpriteRequested(int mouthId)
        {
            SetMouth(mouthId);
        }

        public override void SetHatSprite(Sprite hatSprite)
        {
            _hatSprite.sprite = hatSprite;
            _hatSprite.gameObject.SetActive(hatSprite != null);
        }

        public override void SetHair(int hairId)
        {
            _hairSprite.sprite = SpritesProvider.GetHumanHairSprite(hairId);
        }

        public void SetMouth(int mouthId)
        {
            _mouthSprite.sprite = SpritesProvider.GetHumanSprite($"Mouth{mouthId}");
        }

        public void SetGlasses(int glassId)
        {
            if (glassId == 0)
            {
                _glassesSprite.sprite = null;
            }
            else
            {
                _glassesSprite.sprite = SpritesProvider.GetHumanGlassesSprite(glassId);
            }
        }
    }
}
