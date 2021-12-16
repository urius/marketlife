using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIOfflineReportPopupSellItemView : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TMP_Text _amountText;
    [SerializeField] private TMP_Text _cashText;
    [SerializeField] private TMP_Text _expText;

    public void SetImageSprite(Sprite sprite)
    {
        _image.gameObject.SetActive(sprite != null);
        _image.sprite = sprite;
    }

    public void SetAmountText(string text)
    {
        _amountText.text = text;
    }

    public void SetCashText(string text)
    {
        _cashText.text = text;
    }

    public void SetExpText(string text)
    {
        _expText.text = text;
    }
}
