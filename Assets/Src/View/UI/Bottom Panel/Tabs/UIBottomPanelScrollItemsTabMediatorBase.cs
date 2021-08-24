using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class UIBottomPanelScrollItemsTabMediatorBase<TView, TViewModel> : UIBottomPanelTabMediatorBase
    where TView : UIBottomPanelScrollItemViewBase
{
    private const int MaxDisplayedAmount = 8;

    protected readonly LinkedList<(TView View, TViewModel ViewModel)> DisplayedItems;

    private readonly UpdatesProvider _updatesProvider;
    private readonly AudioManager _audioManager;
    private readonly ScrollBoxView _scrollBoxView;

    private TViewModel[] _viewModels;
    private int _shownIndexFrom = 0;
    private int _shownIndexTo = -1;
    private float _lastContentPosition;
    private readonly float _slotWidth;

    public UIBottomPanelScrollItemsTabMediatorBase(BottomPanelView view)
        : base(view)
    {
        _updatesProvider = UpdatesProvider.Instance;
        _audioManager = AudioManager.Instance;

        _slotWidth = View.ScrollBoxView.SlotWidth;
        _scrollBoxView = view.ScrollBoxView;

        DisplayedItems = new LinkedList<(TView, TViewModel)>();
    }

    public override void Mediate()
    {
        base.Mediate();

        ShowScrollBox();
        RefreshScrollContent();

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

    abstract protected TView GetOrCreateItem();
    abstract protected void ReturnOrDestroyScrollBoxItem(TView itemView);
    abstract protected IEnumerable<TViewModel> GetViewModelsToShow();
    abstract protected void SetupItem(TView itemView, TViewModel viewModel);
    abstract protected void HandleClick(TViewModel viewModel);

    protected virtual void BeforeHideItem(TView itemView, TViewModel viewModel) { }

    protected void RefreshScrollContent()
    {
        _viewModels = GetViewModelsToShow().ToArray();

        _scrollBoxView.SetContentWidth(_viewModels.Length * _scrollBoxView.SlotWidth);
        _scrollBoxView.SetContentPosition(_lastContentPosition);
        UpdateDisplayItems();
    }

    protected TView GetViewByViewModel(TViewModel viewModel)
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

    protected TView GetOrCreateScrollBoxItemAt(int index)
    {
        var result = GetOrCreateItem();
        result.transform.SetParent(View.ScrollBoxView.Content);
        result.SetAnchoredPosition(new Vector2(_slotWidth * (0.5f + index), 0));

        return result;
    }

    protected virtual void ActivateItem(TView itemView, TViewModel viewModel)
    {
        itemView.Clicked += OnItemClicked;
    }

    protected virtual void DeactivateItem(TView itemView, TViewModel viewModel)
    {
        itemView.Clicked -= OnItemClicked;
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
            HideItem(DisplayedItems.Last.Value);
            DisplayedItems.RemoveLast();
            _shownIndexTo--;
        }

        TView itemView;
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

    private void ShowItem(TView itemView, TViewModel viewModel)
    {
        SetupItem(itemView, viewModel);
        ActivateItem(itemView, viewModel);
    }

    private void HideItem((TView View, TViewModel ViewModel) item)
    {
        DeactivateItem(item.View, item.ViewModel);
        BeforeHideItem(item.View, item.ViewModel);
        ReturnOrDestroyScrollBoxItem(item.View);
    }

    private void OnItemClicked(UIBottomPanelScrollItemViewBase itemView)
    {
        var viewModel = DisplayedItems.First(t => t.View == itemView).ViewModel;
        HandleClick(viewModel);
        _audioManager.PlaySound(SoundNames.Button5);
    }
}
