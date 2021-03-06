using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBottomPanelScrollItemView : UIBottomPanelScrollItemViewBase
{
    public event Action<UIBottomPanelScrollItemView> BottomButtonClicked = delegate { };
    public event Action<UIBottomPanelScrollItemView> RemoveButtonClicked = delegate { };
    //C5FFF6
    //97B6FC
    //848484
    [SerializeField] private UIPriceLabelView _priceLabel;
    [SerializeField] private Image _mainButtonImage;
    [SerializeField] private Image _imageSprite;
    [SerializeField] private RectTransform _imageRectTransform;
    [SerializeField] private TMP_Text _topText;
    [SerializeField] private RectTransform _percentLineContainerTransform;
    [SerializeField] private Image _percentLineImage;
    [SerializeField] private UIHintableView _hintableView;
    [SerializeField] private Button _bottomButton;
    [SerializeField] private UIPriceLabelView _bottomButtonPriceLabel;
    [SerializeField] private UIHintableView _bottomButtonHintableView;
    [SerializeField] private Button _removeButton;
    [SerializeField] private Color _liteGreenColor;
    [SerializeField] private Color _deepBlueColor;
    [SerializeField] private Color _textGrayColor;

    private CancellationTokenSource _animationsCts;

    public RectTransform TopTextRectTransform => _topText.transform as RectTransform;

    public override void Awake()
    {
        base.Awake();

        _bottomButton.AddOnClickListener(OnBottomButtonClick);
        _removeButton.AddOnClickListener(OnRemoveClicked);
    }

    public async UniTask AnimateJumpAsync()
    {
        CancelAllAnimations();
        _animationsCts = new CancellationTokenSource();
        await LeanTweenHelper.BounceYAsync(RectTransform, 40, _animationsCts.Token);
        CancelAllAnimations();
    }

    public void SetTopText(string text)
    {
        _topText.gameObject.SetActive(true);
        _topText.text = text;
    }

    public void SetPercentLineXScaleMultiplier(float multiplier)
    {
        _percentLineContainerTransform.gameObject.SetActive(true);
        var scale = _percentLineImage.transform.localScale;
        scale.x = multiplier;
        _percentLineImage.transform.localScale = scale;
    }

    public void SetPercentLineColor(Color color)
    {
        _percentLineContainerTransform.gameObject.SetActive(true);
        _percentLineImage.color = color;
    }

    public void SetupIconSize(float size)
    {
        _imageRectTransform.sizeDelta = new Vector2(size, size);
    }

    public void SetPrice(Price price)
    {
        _priceLabel.gameObject.SetActive(true);

        _priceLabel.SetType(price.IsGold);
        _priceLabel.Value = price.Value;
    }

    public void SetBottomButtonPrice(Price price)
    {
        _bottomButton.gameObject.SetActive(true);

        _bottomButtonPriceLabel.SetType(price.IsGold);
        _bottomButtonPriceLabel.Value = price.Value;
    }

    public void DisableBottomButton()
    {
        _bottomButton.gameObject.SetActive(false);
        _bottomButtonHintableView.SetEnabled(false);
    }

    public void SetImage(Sprite sprite)
    {
        _imageSprite.sprite = sprite;
        _imageSprite.gameObject.SetActive(sprite != null);
    }

    public void SetImageAlpha(float alpha)
    {
        _imageSprite.color = _imageSprite.color.SetAlpha(alpha);
    }

    public void SetupMainHint(string hintText)
    {
        _hintableView.DisplayText = hintText;
        _hintableView.SetEnabled(true);
    }

    public void SetupBottomButtonHint(string hintText)
    {
        _bottomButtonHintableView.DisplayText = hintText;
        _bottomButtonHintableView.SetEnabled(true);
    }

    public void DisableHint()
    {
        _hintableView.SetEnabled(false);
    }

    public void ShowRemoveButton()
    {
        _removeButton.gameObject.SetActive(true);
    }

    public void CancelAllAnimations()
    {
        if (_animationsCts != null)
        {
            var cts = _animationsCts;
            _animationsCts = null; //strange issue (parallel-like, but in same thread) if call CancelAllAnimations On Every second in parallel with animation
            cts.Cancel();
            cts.Dispose();
        }
    }

    public void SetSkinGreen()
    {
        _mainButtonImage.color = _liteGreenColor.SetAlpha(1);
    }

    public void SetSkinBlue()
    {
        _mainButtonImage.color = _deepBlueColor.SetAlpha(0.6f);
    }

    public void SetTextGray()
    {
        _topText.color = _textGrayColor;
    }

    public void SetTextDefaultColor()
    {
        _topText.color = Color.white;
    }

    public void Reset()
    {
        CancelAllAnimations();

        SetTextDefaultColor();
        _percentLineContainerTransform.gameObject.SetActive(false);
        _mainButtonImage.color = Color.white.SetAlpha(1);
        _topText.gameObject.SetActive(false);
        _priceLabel.gameObject.SetActive(false);
        _imageSprite.color = _imageSprite.color.SetAlpha(1);
        _imageSprite.gameObject.SetActive(false);
        _bottomButton.gameObject.SetActive(false);
        _hintableView.SetEnabled(false);
        _bottomButtonHintableView.SetEnabled(false);
        _removeButton.gameObject.SetActive(false);
        _imageRectTransform.sizeDelta = new Vector2(130, 130);
    }

    private void OnBottomButtonClick()
    {
        BottomButtonClicked(this);
    }

    private void OnRemoveClicked()
    {
        RemoveButtonClicked(this);
    }
}
