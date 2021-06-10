using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIOrderProductItemView : MonoBehaviour
{
    public event Action<UIOrderProductItemView> Cliked = delegate { };

    [SerializeField] private Button _button;
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private Image _productIcon;
    [SerializeField] private UIPriceLabelView _priceLabelView;

    public void Awake()
    {
        _button.AddOnClickListener(OnButtonClick);
    }

    public void SetTitleText(string text)
    {
        _titleText.text = text;
    }

    public void SetDescriptionText(string text)
    {
        _descriptionText.text = text;
    }

    public void SetProductIcon(Sprite sprite)
    {
        _productIcon.sprite = sprite;
    }

    public void SetPrice(bool isGold, int amount)
    {
        _priceLabelView.SetType(isGold);
        _priceLabelView.Value = amount;
    }

    private void OnButtonClick()
    {
        Cliked(this);
    }
}
