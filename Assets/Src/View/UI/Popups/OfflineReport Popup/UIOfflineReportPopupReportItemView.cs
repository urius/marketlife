using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIOfflineReportPopupReportItemView : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TMP_Text _leftText;
    [SerializeField] private TMP_Text _rightText;

    public void SetImageSprite(Sprite sprite)
    {
        _image.sprite = sprite;
    }

    public void SetLeftText(string text)
    {
        _leftText.text = text;
    }

    public void SetRightText(string text)
    {
        _rightText.text = text;
    }
}
