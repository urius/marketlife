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
        _priceLabel.gameObject.SetActive(true);

        _priceLabel.SetType(price.IsGold);
        _priceLabel.Value = price.Value;
    }

    public void SetImage(Sprite sprite)
    {
        _imageSprite.sprite = sprite;
        _imageSprite.gameObject.SetActive(true);
    }

    private void OnButtonClick()
    {
        Clicked(this);
    }

    internal void Reset()
    {
        _priceLabel.gameObject.SetActive(false);
        _imageSprite.gameObject.SetActive(false);
    }
}
