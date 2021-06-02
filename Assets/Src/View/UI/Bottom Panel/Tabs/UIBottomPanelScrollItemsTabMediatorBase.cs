using System;
using System.Collections.Generic;
using System.Linq;

public abstract class UIBottomPanelScrollItemsTabMediatorBase<TViewModel> : UIBottomPanelSubMediatorBase
{
    private const int MaxDisplayedAmount = 8;

    private readonly UpdatesProvider _updatesProvider;
    private readonly ScrollBoxView _scrollBoxView;
    private readonly LinkedList<(UIBottomPanelScrollItemView View, TViewModel ViewModel)> _items;

    private TViewModel[] _viewModels;
    private int _shownIndexFrom = 0;
    private int _shownIndexTo = -1;
    private float _lastContentPosition;

    public UIBottomPanelScrollItemsTabMediatorBase(BottomPanelView view)
        : base(view)
    {
        _updatesProvider = UpdatesProvider.Instance;
        _scrollBoxView = view.ScrollBoxView;

        _items = new LinkedList<(UIBottomPanelScrollItemView, TViewModel)>();
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

        foreach (var item in _items)
        {
            HideItem(item.View);
        }
        _items.Clear();
        _shownIndexFrom = 0;
        _shownIndexTo = -1;

        HideScrollBox();

        base.Unmediate();
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
        UIBottomPanelScrollItemView itemView;
        //remove items from left
        while (_shownIndexFrom < indexFrom)
        {
            itemView = _items.First.Value.View;
            HideItem(itemView);
            _items.RemoveFirst();
            _shownIndexFrom++;
        }
        //remove items from right
        while (_shownIndexTo > indexTo)
        {
            itemView = _items.Last.Value.View;
            HideItem(itemView);
            _items.RemoveLast();
            _shownIndexTo--;
        }

        TViewModel viewModel;
        //add items to left
        while (_shownIndexFrom > indexFrom)
        {
            _shownIndexFrom--;
            viewModel = _viewModels[_shownIndexFrom];
            itemView = GetOrCreateScrollBoxItemAt(_shownIndexFrom);
            _items.AddFirst((itemView, viewModel));
            ShowItem(itemView, viewModel);
        }
        //add items to right
        while (_shownIndexTo < indexTo)
        {
            _shownIndexTo++;
            viewModel = _viewModels[_shownIndexTo];
            itemView = GetOrCreateScrollBoxItemAt(_shownIndexTo);
            _items.AddLast((itemView, viewModel));
            ShowItem(itemView, viewModel);
        }
    }

    private void ShowItem(UIBottomPanelScrollItemView itemView, TViewModel viewModel)
    {
        SetupItem(itemView, viewModel);
        ActivateItem(itemView);
    }

    private void HideItem(UIBottomPanelScrollItemView itemView)
    {
        DeactivateItem(itemView);
        ReturnOrDestroyScrollBoxItem(itemView);
    }

    abstract protected IEnumerable<TViewModel> GetViewModelsToShow();
    abstract protected void SetupItem(UIBottomPanelScrollItemView itemView, TViewModel viewModel);
    abstract protected void HandleClick(TViewModel viewModel);

    private void ActivateItem(UIBottomPanelScrollItemView itemView)
    {
        itemView.Clicked += OnItemClicked;
    }

    private void DeactivateItem(UIBottomPanelScrollItemView itemView)
    {
        itemView.Clicked -= OnItemClicked;
    }

    private void OnItemClicked(UIBottomPanelScrollItemView itemView)
    {
        var viewModel = _items.First(t => t.View == itemView).ViewModel;
        HandleClick(viewModel);
    }
}
