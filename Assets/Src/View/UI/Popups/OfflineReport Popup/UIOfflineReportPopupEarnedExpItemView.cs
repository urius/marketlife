using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIOfflineReportPopupEarnedExpItemView : MonoBehaviour
{
    [SerializeField] private TMP_Text _leftText;
    [SerializeField] private TMP_Text _rightText;
    [SerializeField] private Image _image;

    public void SetLeftText(string text)
    {
        _leftText.text = text;
    }

    public void SetRightText(string text)
    {
        _rightText.text = text;
    }

    public void SetLeftText(string text, TextAlignmentOptions alignmentOptions)
    {
        _leftText.alignment = alignmentOptions;
        SetLeftText(text);
    }

    public void SetRightText(string text, TextAlignmentOptions alignmentOptions)
    {
        _rightText.alignment = alignmentOptions;
        SetRightText(text);
    }

    public void SetImageSprite(Sprite sprite)
    {
        _image.sprite = sprite;
    }
}
