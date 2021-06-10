using System;
using System.Collections.Generic;
using UnityEngine;

public class UIOrderProductsPopup : UIPopupBase
{
    public event Action<int> CategoryButtonClicked = delegate { };

    [SerializeField] private RectTransform _contentRectTransform;
    public RectTransform ContentRectTransform => _contentRectTransform;
    [SerializeField] private GameObject _categoryButtonPrefab;
    [SerializeField] private RectTransform _categoryButtonsContainerRectTransform;

    private readonly List<Action> _disposeActions = new List<Action>();
    private readonly List<UIOrderProductCategoryButtonView> _categoryButtons = new List<UIOrderProductCategoryButtonView>();

    public void SetupTabButtons(string[] tabNames)
    {
        if (_categoryButtons.Count > 0)
        {
            throw new InvalidOperationException("Category buttons are already set up");
        }

        for (var i = 0; i < tabNames.Length; i++)
        {
            CreateTabButton(i, tabNames[i]);
        }
    }

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

    public void SetTabButtonSelected(int buttonIndex)
    {
        for (var i = 0; i < _categoryButtons.Count; i++)
        {
            _categoryButtons[i].SetInteractable(buttonIndex != i);
        }
    }

    private void CreateTabButton(int i, string tabName)
    {
        var buttonGo = Instantiate(_categoryButtonPrefab, _categoryButtonsContainerRectTransform);
        var buttonView = buttonGo.GetComponent<UIOrderProductCategoryButtonView>();
        void OnCategoryButtonClick()
        {
            CategoryButtonClicked(i);
        }
        buttonView.Clicked += OnCategoryButtonClick;
        _disposeActions.Add(() => buttonView.Clicked -= OnCategoryButtonClick);
        buttonView.SetText(tabName);

        var pos = buttonView.transform.localPosition;
        pos.x = i * (buttonView.transform as RectTransform).rect.width;
        buttonView.transform.localPosition = pos;

        _categoryButtons.Add(buttonView);
    }

    private void OnDestroy()
    {
        _disposeActions.ForEach(a => a());
    }
}
