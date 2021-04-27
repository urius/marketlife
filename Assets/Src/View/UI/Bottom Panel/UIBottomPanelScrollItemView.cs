using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBottomPanelScrollItemView : MonoBehaviour
{
    public event Action<UIBottomPanelScrollItemView> Clicked = delegate { };

    [SerializeField] private UIPriceLabelView _priceLabel;
    [SerializeField] private Image _imageSprite;
    [SerializeField] private Button _button;

    public void Awake()
    {
        _button.onClick.AddListener(OnButtonClick);
    }

    public void SetPrice(Price price)
    {
        _priceLabel.SetType(price.IsGold);
        _priceLabel.Value = price.Value;
    }

    public void SetImage(Sprite sprite)
    {
        _imageSprite.sprite = sprite;
    }

    private void OnButtonClick()
    {
        Clicked(this);
    }
}
