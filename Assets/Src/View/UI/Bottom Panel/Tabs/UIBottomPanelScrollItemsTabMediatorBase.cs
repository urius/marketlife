using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class UIBottomPanelScrollItemsTabMediatorBase<TViewModel> : UINotMonoMediatorBase
{
    protected readonly BottomPanelView View;

    private const int MaxDisplayedAmount = 8;

    protected readonly LinkedList<(UIBottomPanelScrollItemView View, TViewModel ViewModel)> DisplayedItems;

    private readonly UpdatesProvider _updatesProvider;
    private readonly ViewsCache _viewsCache;
    private readonly ScrollBoxView _scrollBoxView;

    private TViewModel[] _viewModels;
    private int _shownIndexFrom = 0;
    private int _shownIndexTo = -1;
    private float _lastContentPosition;
    private readonly float _slotWidth;

    public UIBottomPanelScrollItemsTabMediatorBase(BottomPanelView view)
        : base(view.transform as RectTransform)
    {
        _updatesProvider = UpdatesProvider.Instance;
        _viewsCache = ViewsCache.Instance;

        View = view;
        _slotWidth = View.ScrollBoxView.SlotWidth;
        _scrollBoxView = view.ScrollBoxView;

        DisplayedItems = new LinkedList<(UIBottomPanelScrollItemView, TViewModel)>();
    }

    public override void Mediate()
    {
        base.Mediate();

        ShowScrollBox();

        _viewModels = GetViewModelsToShow().ToArray();

        _scrollBoxView.SetContentWidth(_viewModels.Length * _scrollBoxView.SlotWidth);
        _scrollBoxView.SetContentPosition(_lastContentPosition);
        UpdateDisplayItems();

        Activate();
    }

    public override void Unmediate()
    {
        Deactivate();

        foreach (var item in DisplayedItems)
        {
            HideItem(item);
        }
        DisplayedItems.Clear();
        _shownIndexFrom = 0;
        _shownIndexTo = -1;

        HideScrollBox();

        base.Unmediate();
    }

    abstract protected IEnumerable<TViewModel> GetViewModelsToShow();
    abstract protected void SetupItem(UIBottomPanelScrollItemView itemView, TViewModel viewModel);
    abstract protected void HandleClick(TViewModel viewModel);

    protected virtual void BeforeHideItem(UIBottomPanelScrollItemView itemView, TViewModel viewModel) { }

    protected virtual void ActivateItem(UIBottomPanelScrollItemView itemView)
    {
        itemView.Clicked += OnItemClicked;
    }

    protected virtual void DeactivateItem(UIBottomPanelScrollItemView itemView)
    {
        itemView.Clicked -= OnItemClicked;
    }

    protected UIBottomPanelScrollItemView GetViewByViewModel(TViewModel viewModel)
    {
        foreach (var item in DisplayedItems)
        {
            if (item.ViewModel.Equals(viewModel))
            {
                return item.View;
            }
        }

        return null;
    }

    protected UIBottomPanelScrollItemView GetOrCreateScrollBoxItemAt(int index)
    {
        var result = _viewsCache.GetOrCreateScrollBoxItem();
        result.Reset();
        result.transform.SetParent(View.ScrollBoxView.Content);
        result.SetAnchoredPosition(new Vector2(_slotWidth * (0.5f + index), 0));

        return result;
    }

    protected void ReturnOrDestroyScrollBoxItem(UIBottomPanelScrollItemView scrollBoxItem)
    {
        _viewsCache.ReturnOrDestroyScrollBoxItem(scrollBoxItem);
    }

    protected void ShowScrollBox()
    {
        View.ScrollBoxView.gameObject.SetActive(true);
    }

    protected void HideScrollBox()
    {
        View.ScrollBoxView.gameObject.SetActive(false);
    }

    private void Activate()
    {
        _updatesProvider.RealtimeUpdate += OnRealtimeUpdate;
    }

    private void Deactivate()
    {
        _updatesProvider.RealtimeUpdate -= OnRealtimeUpdate;
    }

    private void OnRealtimeUpdate()
    {
        var pos = _scrollBoxView.GetContentPosition();
        if (Math.Abs(pos - _lastContentPosition) > 0.05f)
        {
            _lastContentPosition = pos;
            UpdateDisplayItems();
        }
    }

    private void UpdateDisplayItems()
    {
        var renderIndexFrom = GetRenderStartIndexFromContentPosition(_lastContentPosition);
        DisplayItemsFromIndex(renderIndexFrom);
    }

    private int GetRenderStartIndexFromContentPosition(float contentPosition)
    {
        var startDisplayIndex = (int)Math.Max(0, -(int)contentPosition / _scrollBoxView.SlotWidth);
        return Math.Max(startDisplayIndex - 1, 0);
    }

    private void DisplayItemsFromIndex(int indexFrom)
    {
        var indexTo = Math.Min(_viewModels.Length - 1, indexFrom + MaxDisplayedAmount);
        //remove items from left
        while (_shownIndexFrom < indexFrom)
        {
            HideItem(DisplayedItems.First.Value);
            DisplayedItems.RemoveFirst();
            _shownIndexFrom++;
        }
        //remove items from right
        while (_shownIndexTo > indexTo)
        {
            HideItem(DisplayedItems.First.Value);
            DisplayedItems.RemoveLast();
            _shownIndexTo--;
        }

        UIBottomPanelScrollItemView itemView;
        TViewModel viewModel;
        //add items to left
        while (_shownIndexFrom > indexFrom)
        {
            _shownIndexFrom--;
            viewModel = _viewModels[_shownIndexFrom];
            itemView = GetOrCreateScrollBoxItemAt(_shownIndexFrom);
            DisplayedItems.AddFirst((itemView, viewModel));
            ShowItem(itemView, viewModel);
        }
        //add items to right
        while (_shownIndexTo < indexTo)
        {
            _shownIndexTo++;
            viewModel = _viewModels[_shownIndexTo];
            itemView = GetOrCreateScrollBoxItemAt(_shownIndexTo);
            DisplayedItems.AddLast((itemView, viewModel));
            ShowItem(itemView, viewModel);
        }
    }

    private void ShowItem(UIBottomPanelScrollItemView itemView, TViewModel viewModel)
    {
        SetupItem(itemView, viewModel);
        ActivateItem(itemView);
    }

    private void HideItem((UIBottomPanelScrollItemView View, TViewModel ViewModel) item)
    {
        BeforeHideItem(item.View, item.ViewModel);
        DeactivateItem(item.View);
        ReturnOrDestroyScrollBoxItem(item.View);
    }

    private void OnItemClicked(UIBottomPanelScrollItemView itemView)
    {
        var viewModel = DisplayedItems.First(t => t.View == itemView).ViewModel;
        HandleClick(viewModel);
    }
}
