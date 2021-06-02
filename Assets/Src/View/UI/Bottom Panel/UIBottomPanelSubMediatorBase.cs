using System.Collections.Generic;
using UnityEngine;

public class UIBottomPanelSubMediatorBase : UINotMonoMediatorBase
{
    protected readonly BottomPanelView View;

    private const int CacheSize = 10;

    private readonly PrefabsHolder _prefabsHolder;
    private readonly PoolCanvasProvider _poolCanvasProvider;
    private readonly Queue<UIBottomPanelScrollItemView> _cachedScrollBoxItems = new Queue<UIBottomPanelScrollItemView>();

    private readonly float _slotWidth;

    public UIBottomPanelSubMediatorBase(BottomPanelView view)
        : base(view.transform as RectTransform)
    {
        View = view;
        _slotWidth = View.ScrollBoxView.SlotWidth;

        _prefabsHolder = PrefabsHolder.Instance;
        _poolCanvasProvider = PoolCanvasProvider.Instance;
    }

    protected UIBottomPanelScrollItemView GetOrCreateScrollBoxItemAt(int index)
    {
        if (_cachedScrollBoxItems.Count <= 0)
        {
            var itemGo = GameObject.Instantiate(_prefabsHolder.UIBottomPanelScrollItemPrefab, _poolCanvasProvider.PoolCanvasTransform);
            var itemView = itemGo.GetComponent<UIBottomPanelScrollItemView>();
            _cachedScrollBoxItems.Enqueue(itemView);
        }

        var result = _cachedScrollBoxItems.Dequeue();
        result.Reset();
        result.transform.SetParent(View.ScrollBoxView.Content);
        result.SetAnchoredPosition(new Vector2(_slotWidth * (0.5f + index), 0));
        //result.gameObject.SetActive(true);

        return result;
    }

    protected void ReturnOrDestroyScrollBoxItem(UIBottomPanelScrollItemView scrollBoxItem)
    {
        if (_cachedScrollBoxItems.Count >= CacheSize)
        {
            GameObject.Destroy(scrollBoxItem.gameObject);
        }
        else
        {
            //scrollBoxItem.gameObject.SetActive(false);
            scrollBoxItem.transform.SetParent(_poolCanvasProvider.PoolCanvasTransform);
            _cachedScrollBoxItems.Enqueue(scrollBoxItem);
        }
    }

    protected void ShowScrollBox()
    {
        View.ScrollBoxView.gameObject.SetActive(true);
    }

    protected void HideScrollBox()
    {
        View.ScrollBoxView.gameObject.SetActive(false);
    }
}
