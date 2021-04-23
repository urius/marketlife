using UnityEngine;

public class HumanView : MonoBehaviour
{
    public const string CLOTHES_PREFIX = "Clothes";
    public const string FOOT_CLOTHES_PREFIX = "FootClothes";

    [SerializeField] private HumanHeadView _headViewFaceSide;
    [SerializeField] private HumanHeadViewProfile _headViewProfile;
    [SerializeField] private Animator _faceSideBodyAnimator;
    [SerializeField] private Animator _profileSideBodyAnimator;

    [SerializeField] private GameObject _faceSideGO;
    [SerializeField] private Animator _bodyFaceSideAnimator;
    [SerializeField] private SpriteRenderer _bodyFaceSideTorsoSprite;
    [SerializeField] private SpriteRenderer _bodyFaceSideHand1Sprite;
    [SerializeField] private SpriteRenderer _bodyFaceSideHand1OuterSprite;
    [SerializeField] private SpriteRenderer _bodyFaceSideHand2Sprite;
    [SerializeField] private SpriteRenderer _bodyFaceSideHand2OuterSprite;
    [SerializeField] private SpriteRenderer _bodyFaceSideLeg1Sprite;
    [SerializeField] private SpriteRenderer _bodyFaceSideLeg1OuterSprite;
    [SerializeField] private SpriteRenderer _bodyFaceSideLeg2Sprite;
    [SerializeField] private SpriteRenderer _bodyFaceSideLeg2OuterSprite;

    [SerializeField] private GameObject _profileSideGO;
    [SerializeField] private Animator _bodyProfileSideAnimator;
    [SerializeField] private SpriteRenderer _bodyProfileSideTorsoSprite;
    [SerializeField] private SpriteRenderer _bodyProfileSideHand1Sprite;
    [SerializeField] private SpriteRenderer _bodyProfileSideHand1OuterSprite;
    [SerializeField] private SpriteRenderer _bodyProfileSideHand2Sprite;
    [SerializeField] private SpriteRenderer _bodyProfileSideHand2OuterSprite;
    [SerializeField] private SpriteRenderer _bodyProfileSideLeg1Sprite;
    [SerializeField] private SpriteRenderer _bodyProfileSideLeg1OuterSprite;
    [SerializeField] private SpriteRenderer _bodyProfileSideLeg2Sprite;
    [SerializeField] private SpriteRenderer _bodyProfileSideLeg2OuterSprite;

    private SpritesProvider _spritesProvider;

    private BodyState _bodyState = BodyState.Idle;
    private int _faceAnimationIndex = 0;

    void Start()
    {
        _spritesProvider = SpritesProvider.Instance;

        SetupSkins(1, 1, 1);
        SetGlasses(0);
        ShowSide(2);
    }

    public void SetBodyState(BodyState state)
    {
        _bodyState = state;
        UpdateBodyState();
    }

    public void ShowFaceAnimation(int index)
    {
        _faceAnimationIndex = index;
        UpdateFaceState();
    }

    public void SetHair(int hairId)
    {
        _headViewFaceSide.SetHair(hairId);
        _headViewProfile.SetHair(hairId);
    }

    public void SetGlasses(int glassesId)
    {
        _headViewFaceSide.SetGlasses(glassesId);
    }

    public void SetupSkins(int hairId, int bodyClothesId, int footClothesId)
    {
        SetHair(hairId);

        var handSkinSpriteName = "HandSkinWhite";
        _bodyFaceSideHand1Sprite.sprite = GetSprite(handSkinSpriteName);
        _bodyFaceSideHand2Sprite.sprite = GetSprite(handSkinSpriteName);
        _bodyProfileSideHand1Sprite.sprite = GetSprite(handSkinSpriteName);
        _bodyProfileSideHand2Sprite.sprite = GetSprite(handSkinSpriteName);
        var legSkinSpriteName = "FootSkinWhite";
        _bodyFaceSideLeg1Sprite.sprite = GetSprite(legSkinSpriteName);
        _bodyFaceSideLeg2Sprite.sprite = GetSprite(legSkinSpriteName);
        _bodyProfileSideLeg1Sprite.sprite = GetSprite(legSkinSpriteName);
        _bodyProfileSideLeg2Sprite.sprite = GetSprite(legSkinSpriteName);

        SetBodyClothes(bodyClothesId);
        SetFootClothes(footClothesId);
    }

    public void SetBodyClothes(int id)
    {
        _bodyFaceSideTorsoSprite.sprite = GetSprite($"{CLOTHES_PREFIX}{id}");
        _bodyProfileSideTorsoSprite.sprite = GetSprite($"{CLOTHES_PREFIX}{id}_Profile");
        var spriteName = $"HandClothes{id}";
        _bodyFaceSideHand1OuterSprite.sprite = GetSprite(spriteName);
        _bodyFaceSideHand2OuterSprite.sprite = GetSprite(spriteName);
        _bodyProfileSideHand1OuterSprite.sprite = GetSprite(spriteName);
        _bodyProfileSideHand2OuterSprite.sprite = GetSprite(spriteName);
    }

    public void SetFootClothes(int id)
    {
        var spriteName = $"{FOOT_CLOTHES_PREFIX}{id}";
        var spriteProfileName = $"{FOOT_CLOTHES_PREFIX}{id}_Profile";
        _bodyFaceSideLeg1OuterSprite.sprite = GetSprite(spriteName);
        _bodyFaceSideLeg2OuterSprite.sprite = GetSprite(spriteName);
        _bodyProfileSideLeg1OuterSprite.sprite = GetSprite(spriteProfileName);
        _bodyProfileSideLeg2OuterSprite.sprite = GetSprite(spriteProfileName);
    }

    public void ShowSide(int side)
    {
        side = Mathf.Clamp(side, 1, 4);
        switch(side)
        {
            case 1:
                _faceSideGO.SetActive(false);
                _profileSideGO.SetActive(true);
                transform.localScale = new Vector3(-1, 1, 1);
                break;
            case 2:
                _faceSideGO.SetActive(true);
                _profileSideGO.SetActive(false);
                transform.localScale = new Vector3(-1, 1, 1);
                break;
            case 3:
                _faceSideGO.SetActive(true);
                _profileSideGO.SetActive(false);
                transform.localScale = new Vector3(1, 1, 1);
                break;
            case 4:
                _faceSideGO.SetActive(false);
                _profileSideGO.SetActive(true);
                transform.localScale = new Vector3(1, 1, 1);
                break;
        }
        UpdateBodyState();
        UpdateFaceState();
    }

    private void UpdateFaceState()
    {
        _headViewFaceSide.ShowFaceAnimation(_faceAnimationIndex);
    }

    private void UpdateBodyState()
    {
        _faceSideBodyAnimator.SetInteger("animation_index", (int)_bodyState);
        _profileSideBodyAnimator.SetInteger("animation_index", (int)_bodyState);
    }

    private Sprite GetSprite(string spriteName)
    {
        return _spritesProvider.GetHumanSprite(spriteName);
    }
}

public enum BodyState
{
    Idle = 0,
    Walking = 1,
    Taking = 2,
}
