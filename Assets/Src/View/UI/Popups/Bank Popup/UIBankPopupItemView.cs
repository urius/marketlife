using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBankPopupItemView : MonoBehaviour
{
    public event Action<UIBankPopupItemView> Clicked = delegate { };

    [SerializeField] private Image _iconImage;
    [SerializeField] private TMP_Text _amountText;
    [SerializeField] private TMP_Text _extraPercentText;
    [SerializeField] private Button _buyButton;
    [SerializeField] private Button _mainButton;
    [SerializeField] private TMP_Text _priceText;

    public void Awake()
    {
        _buyButton.AddOnClickListener(OnClick);
        _mainButton.AddOnClickListener(OnClick);
    }

    public void SetIconSprite(Sprite sprite)
    {
        _iconImage.sprite = sprite;
    }

    public void SetAmountText(string amountText)
    {
        _amountText.text = amountText;
    }

    public void SetExtraPercentText(string text)
    {
        _extraPercentText.text = text;
    }

    public void SetPriceText(string text)
    {
        _priceText.text = text;
    }

    private void OnClick()
    {
        Clicked(this);
    }
}
