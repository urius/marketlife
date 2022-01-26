using System;
using System.Collections.Generic;
using UnityEngine;

public class VirtualListDisplayer<TView, TViewModel>
    where TView : MonoBehaviour
    where TViewModel : class
{
    private readonly RectTransform _contentTransform;
    private readonly GameObject _itemPrefab;
    private readonly Rect _itemRect;
    private readonly Action<TView, TViewModel> _showItemDelegate;
    private readonly Action<TView, TViewModel> _hideItemDelegate;
    private readonly Queue<(GameObject GameObject, TView View)> _cachedItems = new Queue<(GameObject, TView)>();
    private readonly LinkedList<(GameObject GameObject, TView View, int Index)> _displayedItems = new LinkedList<(GameObject, TView, int)>();
    //
    private TViewModel[] _viewModels;
    private int _displayedItemsAmount;

    public VirtualListDisplayer(
        RectTransform contentTransform,
        GameObject itemPrefab,
        int displayedItemsAmount,
        Action<TView, TViewModel> showItemDelegate,
        Action<TView, TViewModel> hideItemDelegate)
    {
        _contentTransform = contentTransform;
        _itemPrefab = itemPrefab;
        _itemRect = GetGameObjectRect(_itemPrefab);
        _displayedItemsAmount = displayedItemsAmount;
        _showItemDelegate = showItemDelegate;
        _hideItemDelegate = hideItemDelegate;
    }

    public TViewModel[] ViewModels => _viewModels;
    public IEnumerable<(GameObject GameObject, TView View, int Index)> DisplayedItems => _displayedItems;

    private int FirstShownIndex => _displayedItems.First.Value.Index;
    private int LastShownIndex => _displayedItems.Last.Value.Index;

    private Rect GetGameObjectRect(GameObject itemPrefab)
    {
        var rectTransform = itemPrefab.transform as RectTransform;
        return rectTransform.rect;
    }

    public void SetupViewModels(TViewModel[] viewModels)
    {
        ClearItems();
        _viewModels = viewModels;
        SetContentHeight(_itemRect.height * viewModels.Length);
    }

    public void ClearItems()
    {
        while (_displayedItems.Count > 0)
        {
            HideItem(_displayedItems.First.Value);
        }
    }

    public void UpdateDisplayedItems()
    {
        var indexFrom = (int)(_contentTransform.anchoredPosition.y / _itemRect.height);
        ShowFromIndexInternal(indexFrom - 1);
    }

    public float GetContentPositionOnIndex(int index)
    {
        var clampedIndex = Mathf.Clamp(index, 0, _viewModels.Length - 1);
        return _itemRect.height * clampedIndex;
    }

    private void ShowFromIndexInternal(int index)
    {
        var indexFrom = Mathf.Max(index, 0);
        var indexTo = Mathf.Min(index + _displayedItemsAmount - 1, _viewModels.Length - 1);

        while (_displayedItems.Count > 0
            && indexFrom > FirstShownIndex)
        {
            HideItem(_displayedItems.First.Value);
        }

        while (_displayedItems.Count > 0
            && indexTo < LastShownIndex)
        {
            HideItem(_displayedItems.Last.Value);
        }

        if (indexFrom < _viewModels.Length)
        {
            if (_displayedItems.Count <= 0)
            {
                var newIndex = indexFrom;
                var item = AddItem(newIndex);
                _displayedItems.AddFirst((item.GameObject, item.View, newIndex));
            }

            while (indexFrom < FirstShownIndex)
            {
                var newIndex = FirstShownIndex - 1;
                var item = AddItem(newIndex);
                _displayedItems.AddFirst((item.GameObject, item.View, newIndex));
            }

            while (indexTo > LastShownIndex)
            {
                var newIndex = LastShownIndex + 1;
                var item = AddItem(newIndex);
                _displayedItems.AddLast((item.GameObject, item.View, newIndex));
            }
        }
    }

    private void SetContentHeight(float height)
    {
        var size = _contentTransform.sizeDelta;
        size.y = height;
        _contentTransform.sizeDelta = size;
    }

    private (GameObject GameObject, TView View) AddItem(int index)
    {
        var viewModel = _viewModels[index];
        GameObject itemGo;
        TView itemView;
        if (_cachedItems.Count > 0)
        {
            (itemGo, itemView) = _cachedItems.Dequeue();
        }
        else
        {
            itemGo = GameObject.Instantiate(_itemPrefab, _contentTransform);
            itemView = itemGo.GetComponentInChildren<TView>();
        }

        var rectTransform = itemGo.transform as RectTransform;
        rectTransform.anchoredPosition = new Vector2(0, -_itemRect.height * index);

        _showItemDelegate(itemView, viewModel);
        itemGo.SetActive(true);
        return (itemGo, itemView);
    }

    private void HideItem((GameObject GameObject, TView View, int Index) item)
    {
        _hideItemDelegate(item.View, _viewModels[item.Index]);
        item.GameObject.SetActive(false);

        _cachedItems.Enqueue((item.GameObject, item.View));
        _displayedItems.Remove(item);
    }
}
