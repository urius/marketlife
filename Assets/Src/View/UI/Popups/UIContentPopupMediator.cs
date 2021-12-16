using System.Collections.Generic;
using UnityEngine;

public abstract class UIContentPopupMediator : IMediator
{
    protected int TopPadding = 20;
    protected Queue<(RectTransform Transform, GameObject Prefab)> DisplayedItems = new Queue<(RectTransform Transform, GameObject Prefab)>();
    protected Dictionary<GameObject, Queue<RectTransform>> CachedItemsByPrefab = new Dictionary<GameObject, Queue<RectTransform>>(2);

    private PoolCanvasProvider _poolCanvasProvider;
    private int _putPointer;

    public UIContentPopupMediator()
    {
        _poolCanvasProvider = PoolCanvasProvider.Instance;
    }

    public abstract void Mediate();
    public virtual void Unmediate()
    {
        foreach (var kvp in CachedItemsByPrefab)
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
        if (CachedItemsByPrefab.ContainsKey(prefab))
        {
            var queue = CachedItemsByPrefab[prefab];
            if (queue.Count > 0)
            {
                var item = queue.Dequeue();
                item.SetParent(PopupView.ContentRectTransform);
                result = item;
            }
        }
        else
        {
            CachedItemsByPrefab[prefab] = new Queue<RectTransform>();
        }

        if (result == null)
        {
            var go = GameObject.Instantiate(prefab, PopupView.ContentRectTransform);
            result = go.GetComponent<RectTransform>();
        }
        DisplayedItems.Enqueue((result, prefab));
        PutNext(result);

        return result;
    }


    protected virtual void ClearDisplayedItems()
    {
        foreach (var displayedItem in DisplayedItems)
        {
            displayedItem.Transform.SetParent(_poolCanvasProvider.PoolCanvasTransform);
            CachedItemsByPrefab[displayedItem.Prefab].Enqueue(displayedItem.Transform);
        }

        _putPointer = TopPadding;
        PopupView.SetContentHeight(0);
        DisplayedItems.Clear();
    }

    protected void PutNextSpace(int height = 20)
    {
        _putPointer += height;
    }

    private void PutNext(RectTransform itemTransform)
    {
        itemTransform.anchoredPosition = new Vector2(0, -_putPointer);
        _putPointer += (int)itemTransform.rect.height;

        PopupView.SetContentHeight(_putPointer);
    }
}
