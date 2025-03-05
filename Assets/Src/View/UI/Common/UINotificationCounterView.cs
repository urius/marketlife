using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Src.View.UI.Common
{
    public class UINotificationCounterView : MonoBehaviour
    {
        [SerializeField] private Image _bgImage;
        [SerializeField] private TMP_Text _counterText;

        public void SetBgImageColor(Color color)
        {
            _bgImage.color = color;
        }

        public void SetCounterText(string text)
        {
            _counterText.text = text;
        }
    }
}
