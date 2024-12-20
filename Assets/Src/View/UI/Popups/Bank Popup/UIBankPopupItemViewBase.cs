using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIBankPopupItemViewBase : MonoBehaviour
{
    public event Action<UIBankPopupItemViewBase> Clicked = delegate { };

    [SerializeField] protected Image IconImage;
    [SerializeField] protected TMP_Text AmountText;
    [SerializeField] protected Button BuyButton;
    [SerializeField] protected Button MainButton;
    [SerializeField] protected TMP_Text PriceText;

    public virtual void Awake()
    {
        SetAvailable(true);

        BuyButton.AddOnClickListener(OnClick);
        MainButton.AddOnClickListener(OnClick);
    }

    public virtual void SetAvailable(bool isAvailable)
    {
        MainButton.interactable = isAvailable;
        BuyButton.interactable = isAvailable;
    }

    public void SetIconSprite(Sprite sprite)
    {
        IconImage.sprite = sprite;
    }

    public void SetAmountText(string amountText)
    {
        AmountText.text = amountText;
    }

    public void SetAmountTextColor(Color color)
    {
        AmountText.color = color;
    }

    public void SetPriceText(string text)
    {
        PriceText.text = text;
    }

    private void OnClick()
    {
        Clicked(this);
    }
}
