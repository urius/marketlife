using System;
using Src.Common;
using Src.Common_Utils;
using Src.View.UI.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Src.View.UI.Popups.Upgrades_Popup
{
    public class UIUpgradesPopupItemView : MonoBehaviour
    {
        public event Action<UIUpgradesPopupItemView> BuyClicked = delegate { };
        public event Action<UIUpgradesPopupItemView> AdsClicked = delegate { };

        [SerializeField] private TMP_Text _tittleText;
        [SerializeField] private TMP_Text _decsriptionText;
        [SerializeField] private TMP_Text _statusText;
        [SerializeField] private TMP_Text _unlockText;
        [SerializeField] private Image _iconImage;
        [SerializeField] private TMP_Text _unlockRequirement1Text;
        [SerializeField] private Image _unlockRequirement1Image;
        [SerializeField] private TMP_Text _unlockRequirement2Text;
        [SerializeField] private Image _unlockRequirement2Image;
        [SerializeField] private Button _adsButton;
        [SerializeField] private TMP_Text _adsButtonText;
        [SerializeField] private TMP_Text _adsCooldownText;
        [SerializeField] private Button _buyButton;
        [SerializeField] private UIPriceLabelView _buyButtonPrice;

        public void Awake()
        {
            _adsButton.AddOnClickListener(OnAdsClicked);
            _buyButton.AddOnClickListener(OnBuyClicked);
        }

        public void SetIconSprite(Sprite sprite)
        {
            _iconImage.sprite = sprite;
        }

        public void SetupState(bool isUnlocked, bool isCharged, bool getViaAdAvailable)
        {
            _decsriptionText.gameObject.SetActive(isUnlocked);
            _unlockText.gameObject.SetActive(!isUnlocked);
            _buyButton.gameObject.SetActive(!isCharged && isUnlocked);
            _adsButton.gameObject.SetActive(getViaAdAvailable && !isCharged && isUnlocked);
            _statusText.gameObject.SetActive(isCharged);
        }

        public void SetAdsButtonCooldown(int cooldown)
        {
            var isOnCooldownState = cooldown > 0;
            _adsButton.interactable = !isOnCooldownState;
            _adsCooldownText.gameObject.SetActive(isOnCooldownState);
            _adsButtonText.gameObject.SetActive(!isOnCooldownState);

            if (isOnCooldownState)
            {
                _adsCooldownText.text = FormattingHelper.ToSeparatedTimeFormat(cooldown);
            }
        }

        public void SetPrice(bool isGold, int amount)
        {
            _buyButtonPrice.SetType(isGold);
            _buyButtonPrice.Value = amount;
        }

        public void SetBuyButtonInteractable(bool isInteractable)
        {
            _buyButton.interactable = isInteractable;
        }

        public void SetTitleText(string text)
        {
            _tittleText.text = text;
        }

        public void SetDescriptionText(string text)
        {
            _decsriptionText.text = text;
        }

        public void SetStatusText(string text)
        {
            _statusText.text = text;
        }

        public void SetAdsButtonText(string text)
        {
            _adsButtonText.text = text;
        }

        public void SetUnlockState(string text, int requirementsCount)
        {
            _unlockText.gameObject.SetActive(requirementsCount > 0);
            _unlockText.text = text;
            _unlockRequirement1Text.gameObject.SetActive(requirementsCount > 0);
            _unlockRequirement2Text.gameObject.SetActive(requirementsCount > 1);
            _decsriptionText.gameObject.SetActive(requirementsCount <= 0);
        }

        public void SetUnlockRequirementData(int requirementIndex, string text, Sprite iconSprite)
        {
            switch (requirementIndex)
            {
                case 0:
                    _unlockRequirement1Text.text = text;
                    _unlockRequirement1Image.sprite = iconSprite;
                    break;
                case 1:
                    _unlockRequirement2Text.text = text;
                    _unlockRequirement2Image.sprite = iconSprite;
                    break;
            }
        }

        private void OnBuyClicked()
        {
            BuyClicked(this);
        }

        private void OnAdsClicked()
        {
            AdsClicked(this);
        }
    }
}
