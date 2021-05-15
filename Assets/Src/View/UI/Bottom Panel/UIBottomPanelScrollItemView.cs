using System;
using UnityEngine;
using UnityEngine.UI;

public class UIBottomPanelScrollItemView : MonoBehaviour
{
    public event Action<UIBottomPanelScrollItemView> Clicked = delegate { };

    [SerializeField] private UIPriceLabelView _priceLabel;
    [SerializeField] private Image _imageSprite;
    [SerializeField] private RectTransform _imageRectTransform;
    [SerializeField] private Button _button;
    [SerializeField] private UIHintableView _hintableView;

    public void Awake()
    {
        _button.onClick.AddListener(OnButtonClick);
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
        _imageSprite.gameObject.SetActive(true);
    }

    public void SetupHint(string hintText)
    {
        _hintableView.DisplayText = hintText;
        _hintableView.SetEnabled(true);
    }

    public void Reset()
    {
        _priceLabel.gameObject.SetActive(false);
        _imageSprite.gameObject.SetActive(false);
        _hintableView.SetEnabled(false);
        _imageRectTransform.sizeDelta = new Vector2(130, 130);
    }

    private void OnButtonClick()
    {
        Clicked(this);
    }
}
