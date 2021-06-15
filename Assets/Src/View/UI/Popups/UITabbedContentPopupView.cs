using System;
using System.Collections.Generic;
using UnityEngine;

public class UITabbedContentPopupView : UIContentPopupView
{
    public event Action<int> TabButtonClicked = delegate { };

    [SerializeField] private GameObject _tabButtonPrefab;
    [SerializeField] private RectTransform _tabButtonsContainerRectTransform;

    private readonly List<Action> _disposeActions = new List<Action>();
    private readonly List<UITabbedContentPopupTabButtonView> _tabButtons = new List<UITabbedContentPopupTabButtonView>();

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

    private void CreateTabButton(int i, string tabName)
    {
        var buttonGo = Instantiate(_tabButtonPrefab, _tabButtonsContainerRectTransform);
        var buttonView = buttonGo.GetComponent<UITabbedContentPopupTabButtonView>();
        void OnCategoryButtonClick()
        {
            TabButtonClicked(i);
        }
        buttonView.Clicked += OnCategoryButtonClick;
        _disposeActions.Add(() => buttonView.Clicked -= OnCategoryButtonClick);
        buttonView.SetText(tabName);

        var pos = buttonView.transform.localPosition;
        pos.x = i * (buttonView.transform as RectTransform).rect.width;
        buttonView.transform.localPosition = pos;

        _tabButtons.Add(buttonView);
    }

    private void OnDestroy()
    {
        _disposeActions.ForEach(a => a());
    }
}
