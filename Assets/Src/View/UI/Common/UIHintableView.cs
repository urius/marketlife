using UnityEngine;
using UnityEngine.EventSystems;

namespace Src.View.UI.Common
{
    public class UIHintableView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public string DisplayText;

        [SerializeField] private HintPositionType _hintPositionType;
        [SerializeField] private string _localizationKey;
        [SerializeField] private RectTransform _hintContainer;
        [SerializeField] private Vector2 _positionOffset;

        private bool _isEnabled = true;
        private bool _isShowing = false;

        public void SetEnabled(bool isEnabled)
        {
            if (isEnabled == false)
            {
                Hide();
            }
            _isEnabled = isEnabled;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_isEnabled
                && !_isShowing
                && (!string.IsNullOrEmpty(_localizationKey) || !string.IsNullOrEmpty(DisplayText)))
            {
                Show();
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_isEnabled)
            {
                Hide();
            }
        }

        private void Show()
        {
            if (string.IsNullOrEmpty(DisplayText) == false)
            {
                HintViewManager.Instance.ShowText(transform, _hintPositionType, DisplayText, _positionOffset);
            }
            else
            {
                HintViewManager.Instance.ShowLocalizable(transform, _hintPositionType, _localizationKey, _positionOffset);
            }
            _isShowing = true;
        }


        private void Hide()
        {
            if (_isShowing)
            {
                HintViewManager.Instance.Hide();
                _isShowing = false;
            }
        }
    }
}
