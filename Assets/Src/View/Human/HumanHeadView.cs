using UnityEngine;

public class HumanHeadView : HumanHeadViewBase
{
    public const string HAIR_PREFIX = "Hair";
    public const string GLASSES_PREFIX = "Glasses";

    [SerializeField] private Animator _headAnimator;
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
        _headBaseSprite.sprite = GraphicsManager.GetHumanSprite("HeadBase");
        _brow1Sprite.sprite = GraphicsManager.GetHumanSprite("Brow1Base");
        _brow2Sprite.sprite = GraphicsManager.GetHumanSprite("Brow1Base");
        _eye1Sprite.sprite = GraphicsManager.GetHumanSprite("Eye2Base");
        _eyePupil1Sprite.sprite = GraphicsManager.GetHumanSprite("EyePupilBase");
        _eye2Sprite.sprite = GraphicsManager.GetHumanSprite("Eye2Base");
        _eyePupil2Sprite.sprite = GraphicsManager.GetHumanSprite("EyePupilBase");
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

    public override void SetHair(int hairId)
    {
        _hairSprite.sprite = GraphicsManager.GetHumanSprite($"{HAIR_PREFIX}{hairId}");
    }

    public void SetMouth(int mouthId)
    {
        _mouthSprite.sprite = GraphicsManager.GetHumanSprite($"Mouth{mouthId}");
    }

    public void SetGlasses(int glassId)
    {
        if (glassId == 0)
        {
            _glassesSprite.sprite = null;
        }
        else
        {
            _glassesSprite.sprite = GraphicsManager.GetHumanSprite($"{GLASSES_PREFIX}{glassId}");
        }
    }
}