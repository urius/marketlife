using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIWarehousePopupItemView : MonoBehaviour
{
    public event Action<UIWarehousePopupItemView> Clicked = delegate{};

    [SerializeField] private Button _button;
    [SerializeField] private Image _iconImage;
    [SerializeField] private TMP_Text _amountText;
    [SerializeField] private UIHintableView _hintableView;

    public void Awake()
    {
        _button.AddOnClickListener(OnButtonClick);
    }

    public void SetIconSprite(Sprite sprite)
    {
        _iconImage.sprite = sprite;
    }

    public void SetAmountText(string text)
    {
        _amountText.text = text;
    }

    public void SetupHint(string text)
    {
        _hintableView.DisplayText = text;
    }

    private void OnButtonClick()
    {
        Clicked(this);
    }
}
