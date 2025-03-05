using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Src.View.UI.Popups.LevelUp_Popup
{
    public class UILevelUpPopupItemView : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _leftText;
        [SerializeField] private Color _greenColor;
        public Color GreenColor => _greenColor;
        [SerializeField] private Color _blueColor;
        public Color BlueColor => _blueColor;
        [SerializeField] private Color _orangeColor;
        public Color OrangeColor => _orangeColor;

        public void SetImageSprite(Sprite sprite)
        {
            _image.gameObject.SetActive(sprite != null);
            _image.sprite = sprite;
        }

        public void SetText(string text, TextAlignmentOptions alignmentOptions = TextAlignmentOptions.Left)
        {
            _leftText.alignment = alignmentOptions;
            _leftText.text = text;
        }

        public void SetTextColor(Color color)
        {
            _leftText.color = color;
        }
    }
}
