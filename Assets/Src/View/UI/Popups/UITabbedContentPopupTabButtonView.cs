using System;
using Src.Common_Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Src.View.UI.Popups
{
    public class UITabbedContentPopupTabButtonView : MonoBehaviour, ITabButtonView
    {
        public event Action Clicked = delegate { };

        [SerializeField] private Button _button;
        [SerializeField] private Text _text;

        public RectTransform RectTransform => transform as RectTransform;

        public void Awake()
        {
            _button.AddOnClickListener(OnButtonClick);
        }

        public void SetInteractable(bool isEnabled)
        {
            _button.interactable = isEnabled;
        }

        public void SetText(string text)
        {
            _text.text = text;
        }

        private void OnButtonClick()
        {
            Clicked();
        }
    }
}
