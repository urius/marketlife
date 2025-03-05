using System;
using Src.Common_Utils;
using Src.View.UI.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Src.View.UI.Bottom_Panel.Tabs
{
    public class UIBottomPanelFriendTabActionButtonView : MonoBehaviour
    {
        public event Action Clicked = delegate { };
        public event Action BuyButtonClicked = delegate { };

        [SerializeField] private TMP_Text _amountText;
        [SerializeField] private TMP_Text _timeText;
        [SerializeField] private Button _mainButton;
        [SerializeField] private Button _buyButton;
        [SerializeField] private TMP_Text _buyPriceText;
        [SerializeField] private UIHintableView _hintableView;

        public void Awake()
        {
            _mainButton.AddOnClickListener(OnMainButtonClicked);
            _buyButton.AddOnClickListener(OnBuyButtonClicked);
        }

        public void SetAmountTextVisibility(bool isVisible)
        {
            _amountText.gameObject.SetActive(isVisible);
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
            _hintableView.SetEnabled(!isChargingEnabled);
            SetAmountTextVisibility(!isChargingEnabled);
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
}
