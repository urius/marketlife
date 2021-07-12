using System.Collections.Generic;
using UnityEngine;

public abstract class UIContentPopupMediator : IMediator
{
    private const int TopPadding = 20;

    protected Queue<(RectTransform Transform, GameObject Prefab)> _displayedItems = new Queue<(RectTransform Transform, GameObject Prefab)>();
    protected Dictionary<GameObject, Queue<RectTransform>> _cachedItemsByPrefab = new Dictionary<GameObject, Queue<RectTransform>>(2);

    private PoolCanvasProvider _poolCanvasProvider;
    private int _putPointer;

    public UIContentPopupMediator()
    {
        _poolCanvasProvider = PoolCanvasProvider.Instance;
    }

    public abstract void Mediate();
    public virtual void Unmediate()
    {
        foreach (var kvp in _cachedItemsByPrefab)
        {
            var queue = kvp.Value;
            while (queue.Count > 0)
            {
                var rectTransform = queue.Dequeue();
                GameObject.Destroy(rectTransform.gameObject);
            }
        }
    }

    protected abstract UIContentPopupView PopupView { get; }

    protected RectTransform GetOrCreateItemToDisplay(GameObject prefab)
    {
        RectTransform result = null;
        if (_cachedItemsByPrefab.ContainsKey(prefab))
        {
            var queue = _cachedItemsByPrefab[prefab];
            if (queue.Count > 0)
            {
                var item = queue.Dequeue();
                item.SetParent(PopupView.ContentRectTransform);
                result = item;
            }
        }
        else
        {
            _cachedItemsByPrefab[prefab] = new Queue<RectTransform>();
        }

        if (result == null)
        {
            var go = GameObject.Instantiate(prefab, PopupView.ContentRectTransform);
            result = go.GetComponent<RectTransform>();
        }
        _displayedItems.Enqueue((result, prefab));
        PutNext(result);

        return result;
    }


    protected void ClearDisplayedItems()
    {
        foreach (var displayedItem in _displayedItems)
        {
            displayedItem.Transform.SetParent(_poolCanvasProvider.PoolCanvasTransform);
            _cachedItemsByPrefab[displayedItem.Prefab].Enqueue(displayedItem.Transform);
        }

        _putPointer = TopPadding;
        PopupView.SetContentHeight(0);
        _displayedItems.Clear();
    }

    private void PutNext(RectTransform itemTransform)
    {
        itemTransform.anchoredPosition = new Vector2(0, -_putPointer);
        _putPointer += (int)itemTransform.rect.height;

        PopupView.SetContentHeight(_putPointer);
    }
}
