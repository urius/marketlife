using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBottomPanelScrollItemView : MonoBehaviour
{
    public event Action<UIBottomPanelScrollItemView> Clicked = delegate { };

    [SerializeField] private UIPriceLabelView _priceLabel;
    [SerializeField] private Image _imageSprite;
    [SerializeField] private RectTransform _imageRectTransform;
    [SerializeField] private Button _button;
    [SerializeField] private TMP_Text _topText;
    [SerializeField] private RectTransform _percentLineContainerTransform;
    [SerializeField] private Image _percentLineImage;
    [SerializeField] private UIHintableView _hintableView;

    private RectTransform _rectTransform;

    public void Awake()
    {
        _rectTransform = transform as RectTransform;

        _button.onClick.AddListener(OnButtonClick);
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

    public void SetImage(Sprite sprite)
    {
        _imageSprite.sprite = sprite;
        _imageSprite.gameObject.SetActive(sprite != null);
    }

    public void SetupHint(string hintText)
    {
        _hintableView.DisplayText = hintText;
        _hintableView.SetEnabled(true);
    }

    public void Reset()
    {
        _percentLineContainerTransform.gameObject.SetActive(false);
        _topText.gameObject.SetActive(false);
        _priceLabel.gameObject.SetActive(false);
        _imageSprite.gameObject.SetActive(false);
        _hintableView.SetEnabled(false);
        _imageRectTransform.sizeDelta = new Vector2(130, 130);
    }

    public void SetAnchoredPosition(Vector2 position)
    {
        _rectTransform.anchoredPosition = position;
    }

    private void OnButtonClick()
    {
        Clicked(this);
    }
}
