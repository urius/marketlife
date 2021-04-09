using UnityEngine;

public class HumanHeadViewProfile : HumanHeadViewBase
{
    public const string HAIR_PREFIX = "Hair";
    public const string HAIR_POSTFIX = "_Profile";

    [SerializeField] private SpriteRenderer _hairSprite;
    [SerializeField] private SpriteRenderer _headBaseSprite;

    protected void Start()
    {
        _headBaseSprite.sprite = GraphicsManager.GetHumanSprite("HeadBase_Profile");
    }

    public override void SetHair(int hairId)
    {
        _hairSprite.sprite = GraphicsManager.GetHumanSprite($"{HAIR_PREFIX}{hairId}{HAIR_POSTFIX}");
    }
}
