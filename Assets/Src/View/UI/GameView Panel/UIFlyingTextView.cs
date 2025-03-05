using TMPro;
using UnityEngine;

namespace Src.View.UI.GameView_Panel
{
    public class UIFlyingTextView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;

        private RectTransform _flyingPartRect;

        public void Awake()
        {
            _flyingPartRect = _text.transform as RectTransform;
        }

        public void SetText(string text)
        {
            _text.text = text;
        }

        public void SetTextColor(Color color)
        {
            _text.color = color;
        }

        public virtual void SetAlpha(float alpha)
        {
            _text.alpha = alpha;
        }

        public void SetOffsetY(float offset)
        {
            var pos = _flyingPartRect.anchoredPosition;
            pos.y = offset;
            _flyingPartRect.anchoredPosition = pos;
        }
    }
}
