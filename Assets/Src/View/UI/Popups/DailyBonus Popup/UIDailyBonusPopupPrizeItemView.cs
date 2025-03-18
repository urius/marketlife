using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Src.View.UI.Popups.DailyBonus_Popup
{
    public class UIDailyBonusPopupPrizeItemView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TMP_Text _dayText;
        [SerializeField] private Image _iconImage;
        [SerializeField] private TMP_Text _valueText;

        public void SetAlpha(float alpha)
        {
            _canvasGroup.alpha = alpha;
        }

        public void SetDayText(string text)
        {
            _dayText.text = text;
        }

        public void SetValueText(string text)
        {
            _valueText.text = text;
        }
    
        public void SetValueTextColor(Color color)
        {
            _valueText.color = color;
        }

        public void SetIconSprite(Sprite sprite)
        {
            _iconImage.sprite = sprite;
        }
    }
}
