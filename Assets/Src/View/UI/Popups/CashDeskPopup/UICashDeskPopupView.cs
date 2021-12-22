using System;
using UnityEngine;
using UnityEngine.UI;

public class UICashDeskPopupView : UIPopupBase
{
    public event Action<int> ChangeHairClicked = delegate { };
    public event Action<int> ChangeGlassesClicked = delegate { };
    public event Action<int> ChangeDressClicked = delegate { };
    public event Action ApplyClicked = delegate { };

    [SerializeField] private Button _hairNextButton;
    [SerializeField] private Button _hairPrevButton;
    [SerializeField] private Button _glassesNextButton;
    [SerializeField] private Button _glassesPrevButton;
    [SerializeField] private Button _dressNextButton;
    [SerializeField] private Button _dressPrevButton;
    [SerializeField] private Button _applyButton;
    [SerializeField] private Image _hairImage;
    [SerializeField] private Image _glassesImage;
    [SerializeField] private Image _torsoImage;
    [SerializeField] private Image _leftHandImage;
    [SerializeField] private Image _rightHandImage;

    override public void Awake()
    {
        base.Awake();

        _hairNextButton.AddOnClickListener(OnHairNextClicked);
        _hairPrevButton.AddOnClickListener(OnHairPrevClicked);
        _glassesNextButton.AddOnClickListener(OnGlassesNextClicked);
        _glassesPrevButton.AddOnClickListener(OnGlassesPrevClicked);
        _dressNextButton.AddOnClickListener(OnDressNextClicked);
        _dressPrevButton.AddOnClickListener(OnDressPrevClicked);
        _applyButton.AddOnClickListener(OnApplyClicked);
    }

    public void SetHairSprite(Sprite sprite)
    {
        _hairImage.sprite = sprite;
    }

    public void SetGlassesSprite(Sprite sprite)
    {
        _glassesImage.gameObject.SetActive(sprite != null);
        _glassesImage.sprite = sprite;
    }

    public void SetTorsoSprite(Sprite sprite)
    {
        _torsoImage.sprite = sprite;
    }

    public void SetLeftHandSprite(Sprite sprite)
    {
        _leftHandImage.sprite = sprite;
    }

    public void SetRightHandSprite(Sprite sprite)
    {
        _rightHandImage.sprite = sprite;
    }

    private void OnDressPrevClicked()
    {
        ChangeDressClicked(-1);
    }

    private void OnDressNextClicked()
    {
        ChangeDressClicked(1);
    }

    private void OnGlassesPrevClicked()
    {
        ChangeGlassesClicked(-1);
    }

    private void OnGlassesNextClicked()
    {
        ChangeGlassesClicked(1);
    }

    private void OnHairPrevClicked()
    {
        ChangeHairClicked(-1);
    }

    private void OnHairNextClicked()
    {
        ChangeHairClicked(1);
    }

    private void OnApplyClicked()
    {
        ApplyClicked();
    }
}
