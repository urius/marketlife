using System;
using System.Collections.Generic;
using UnityEngine;

namespace Src.View.UI.Popups
{
    public class UITabbedContentPopupView : UIContentPopupView
    {
        public event Action<int> TabButtonClicked = delegate { };

        [SerializeField] private GameObject _tabButtonPrefab;
        [SerializeField] private RectTransform _tabButtonsContainerRectTransform;

        private readonly List<Action> _disposeActions = new();
        private readonly List<ITabButtonView> _tabButtons = new();

        public void SetupTabButtons(string[] tabNames)
        {
            if (_tabButtons.Count > 0)
            {
                throw new InvalidOperationException("Tab buttons are already set up");
            }

            for (var i = 0; i < tabNames.Length; i++)
            {
                CreateTabButton(i, tabNames[i]);
            }
        }

        public void SetTabButtonSelected(int buttonIndex)
        {
            for (var i = 0; i < _tabButtons.Count; i++)
            {
                _tabButtons[i].SetInteractable(buttonIndex != i);
            }
        }

        public void SetTabButtonVisibility(int buttonIndex, bool isVisible)
        {
            _tabButtons[buttonIndex].RectTransform.gameObject.SetActive(isVisible);
        }

        public ITabButtonView GetTabButton(int index)
        {
            if (index >= 0 && index < _tabButtons.Count)
            {
                return _tabButtons[index];
            }

            return null;
        }

        private void CreateTabButton(int i, string tabName)
        {
            var buttonGo = Instantiate(_tabButtonPrefab, _tabButtonsContainerRectTransform);
            var buttonView = buttonGo.GetComponent<ITabButtonView>();
            void OnCategoryButtonClick()
            {
                TabButtonClicked(i);
            }
            buttonView.Clicked += OnCategoryButtonClick;
            _disposeActions.Add(() => buttonView.Clicked -= OnCategoryButtonClick);
            buttonView.SetText(tabName);

            var rectTransform = buttonView.RectTransform;
            var pos = rectTransform.localPosition;
            pos.x = i * rectTransform.rect.width;
            rectTransform.localPosition = pos;

            _tabButtons.Add(buttonView);
        }

        private void OnDestroy()
        {
            _disposeActions.ForEach(a => a());
        }
    }
}
