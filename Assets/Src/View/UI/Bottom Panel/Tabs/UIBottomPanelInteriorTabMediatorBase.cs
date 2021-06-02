using System;
using System.Collections.Generic;
using System.Linq;

public abstract class UIBottomPanelInteriorTabMediatorBase<TViewModel> : UIBottomPanelSubMediatorBase
{
    private const int MaxViewsAmount = 10;

    private readonly LinkedList<(UIBottomPanelScrollItemView View, TViewModel ViewModel)> _items = new LinkedList<(UIBottomPanelScrollItemView, TViewModel)>();

    private TViewModel[] _viewModels;
    private int _shownIndexFrom = 0;
    private int _shownIndexTo = -1;

    public UIBottomPanelInteriorTabMediatorBase(BottomPanelView view)
        : base(view)
    {
    }

    public override void Mediate()
    {
        base.Mediate();

        ShowScrollBox();

        _viewModels = GetViewModelsToShow().ToArray();

        DisplayItems(0, MaxViewsAmount);
        View.ScrollBoxView.SetContentWidth(_viewModels.Length * View.ScrollBoxView.SlotWidth);
        View.ScrollBoxView.SetContentPosition(0);
    }

    public override void Unmediate()
    {
        foreach (var item in _items)
        {
            HideItem(item.View);
        }
        _items.Clear();

        HideScrollBox();

        base.Unmediate();
    }

    private void DisplayItems(int indexFrom, int indexTo)
    {
        indexTo = Math.Min(_viewModels.Length - 1, indexTo);
        UIBottomPanelScrollItemView itemView;
        while (_shownIndexFrom < indexFrom)
        {
            itemView = _items.First.Value.View;
            HideItem(itemView);
            _items.RemoveFirst();
            _shownIndexFrom++;
        }
        while (_shownIndexTo > indexTo)
        {
            itemView = _items.Last.Value.View;
            HideItem(itemView);
            _items.RemoveLast();
            _shownIndexTo--;
        }

        TViewModel viewModel;
        while (_shownIndexFrom > indexFrom)
        {
            _shownIndexFrom--;
            viewModel = _viewModels[_shownIndexFrom];
            itemView = GetOrCreateScrollBoxItemAt(_shownIndexFrom);
            _items.AddFirst((itemView, viewModel));
            ShowItem(itemView, viewModel);
        }
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
