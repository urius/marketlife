using UnityEngine;

namespace Src.View.UI.Popups
{
    public class UIContentPopupView : UIPopupBase
    {
        [SerializeField] private RectTransform _contentRectTransform;
        public RectTransform ContentRectTransform => _contentRectTransform;

        public void SetContentHeight(float height)
        {
            var size = _contentRectTransform.sizeDelta;
            size.y = height;
            _contentRectTransform.sizeDelta = size;
        }

        internal void SetContentYPosition(float position)
        {
            var pos = _contentRectTransform.anchoredPosition;
            pos.y = position;
            _contentRectTransform.anchoredPosition = pos;
        }
    }
}
