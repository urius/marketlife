using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBottomPanelFriendTabActionButtonView : MonoBehaviour
{
    public event Action Clicked = delegate { };
    public event Action BuyButtonClicked = delegate { };

    [SerializeField] private TMP_Text _amountText;
    [SerializeField] private TMP_Text _timeText;
    [SerializeField] private Button _mainButton;
    [SerializeField] private Button _buyButton;
    [SerializeField] private TMP_Text _buyPriceText;

    public void Awake()
    {
        _mainButton.AddOnClickListener(OnMainButtonClicked);
        _buyButton.AddOnClickListener(OnBuyButtonClicked);
    }

    public void SetAmountText(string text)
    {
        _amountText.text = text;
    }

    public void SetTimeText(string text)
    {
        _timeText.text = text;
    }

    public void SetBuyPriceText(string text)
    {
        _buyPriceText.text = text;
    }

    public void SetChargingState(bool isChargingEnabled)
    {
        _amountText.gameObject.SetActive(!isChargingEnabled);
        _timeText.gameObject.SetActive(isChargingEnabled);
        _mainButton.interactable = !isChargingEnabled;
        _buyButton.gameObject.SetActive(isChargingEnabled);
    }

    private void OnBuyButtonClicked()
    {
        BuyButtonClicked();
    }

    private void OnMainButtonClicked()
    {
        Clicked();
    }
}
