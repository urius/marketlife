using TMPro;
using UnityEngine;

public class UIBankPopupBuyableItemView : UIBankPopupItemViewBase
{
    [SerializeField] private TMP_Text _extraPercentText;
    [SerializeField] private TMP_Text _unavailableText;

    override public void SetAvailable(bool isAvailable)
    {
        base.SetAvailable(isAvailable);

        BuyButton.gameObject.SetActive(isAvailable);
        PriceText.gameObject.SetActive(isAvailable);
        _unavailableText?.gameObject.SetActive(!isAvailable);
    }

    public void SetExtraPercentText(string text)
    {
        _extraPercentText.text = text;
    }
}
