using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILevelUpPopupItemView : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TMP_Text _leftText;

    public void SetImageSprite(Sprite sprite)
    {
        _image.gameObject.SetActive(sprite != null);
        _image.sprite = sprite;
    }

    public void SetLeftText(string text, TextAlignmentOptions alignmentOptions = TextAlignmentOptions.Left)
    {
        _leftText.alignment = alignmentOptions;
        _leftText.text = text;
    }
}
