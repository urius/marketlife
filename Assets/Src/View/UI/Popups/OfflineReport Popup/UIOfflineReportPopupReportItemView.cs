using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Src.View.UI.Popups.OfflineReport_Popup
{
    public class UIOfflineReportPopupReportItemView : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _leftText;
        [SerializeField] private TMP_Text _rightText;

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

        public void SetLeftTextColor(Color color)
        {
            _leftText.color = color;
        }

        public void SetRightText(string text, TextAlignmentOptions alignmentOptions = TextAlignmentOptions.Right)
        {
            _rightText.alignment = alignmentOptions;
            _rightText.text = text;
        }

        public void SetRightTextColor(Color color)
        {
            _rightText.color = color;
        }
    }
}
