using System;
using Src.Common_Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Src.View.UI.Popups.LeaderboardsPopup
{
    public class UILeaderboardsPopupTabButtonView : MonoBehaviour, ITabButtonView
    {
        public event Action Clicked = delegate { };

        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private GameObject _placeInLBIndicator;

        public RectTransform RectTransform => transform as RectTransform;

        public void Awake()
        {
            _button.AddOnClickListener(OnButtonClick);
            _placeInLBIndicator.SetActive(false);
        }

        public void SetInteractable(bool isEnabled)
        {
            _button.interactable = isEnabled;
        }

        public void SetText(string text)
        {
            _text.text = text;
        }

        public void SetPlaceIndicatorVisibility(bool isVisible)
        {
            _placeInLBIndicator.SetActive(isVisible);
        }

        private void OnButtonClick()
        {
            Clicked();
        }
    }
}
