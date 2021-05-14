using System;
using UnityEngine;
using UnityEngine.UI;

public class UIBottomPanelScrollItemView : MonoBehaviour
{
    public event Action<UIBottomPanelScrollItemView> Clicked = delegate { };

    [SerializeField] private UIPriceLabelView _priceLabel;
    [SerializeField] private Image _imageSprite;
    [SerializeField] private Button _button;
    [SerializeField] private UIHintableView _hintableView;

    public void Awake()
    {
        _button.onClick.AddListener(OnButtonClick);
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

    private void OnButtonClick()
    {
        Clicked(this);
    }

    internal void Reset()
    {
        _priceLabel.gameObject.SetActive(false);
        _imageSprite.gameObject.SetActive(false);
        _hintableView.SetEnabled(false);
    }
}
