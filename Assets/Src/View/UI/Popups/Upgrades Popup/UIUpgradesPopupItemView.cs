using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIUpgradesPopupItemView : MonoBehaviour
{
    public Action<UIUpgradesPopupItemView> BuyClicked = delegate { };

    [SerializeField] private TMP_Text _tittleText;
    [SerializeField] private TMP_Text _decsriptionText;
    [SerializeField] private TMP_Text _unlockText;
    [SerializeField] private Image _iconImage;
    [SerializeField] private TMP_Text _unlockRequirement1Text;
    [SerializeField] private Image _unlockRequirement1Image;
    [SerializeField] private TMP_Text _unlockRequirement2Text;
    [SerializeField] private Image _unlockRequirement2Image;
    [SerializeField] private Button _buyButton;
    [SerializeField] private UIPriceLabelView _buyButtonPrice;

    public void Awake()
    {
        _buyButton.AddOnClickListener(OnBuyClicked);
    }

    public void SetupUnlockedState(bool isUnlocked)
    {
        _unlockText.gameObject.SetActive(!isUnlocked);
        _buyButton.gameObject.SetActive(isUnlocked);
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

    public void SetUnlockState(string text, int requirementsCount)
    {
        _unlockText.gameObject.SetActive(true);
        _unlockText.text = text;
        _unlockRequirement1Text.gameObject.SetActive(requirementsCount > 0);
        _unlockRequirement2Text.gameObject.SetActive(requirementsCount > 1);
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
}
