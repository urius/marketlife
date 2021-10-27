using TMPro;
using UnityEngine;

public class UIOfflineReportPopupOverallItemView : MonoBehaviour
{
    [SerializeField] private TMP_Text _leftText;
    [SerializeField] private TMP_Text _rightText;

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
}
