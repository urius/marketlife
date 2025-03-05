using System;
using System.Collections.Generic;
using Src.Common;
using Src.View.UI;
using Src.View.UI.Bottom_Panel;
using UnityEngine;

namespace Src.View
{
    public class ViewsCache
    {
        private static Lazy<ViewsCache> _instance = new Lazy<ViewsCache>();
        public static ViewsCache Instance => _instance.Value;

        private const int ScrollBoxItemsCacheSize = 10;

        private readonly PrefabsHolder _prefabsHolder;
        private readonly PoolCanvasProvider _poolCanvasProvider;
        private readonly Queue<UIBottomPanelScrollItemView> _cachedScrollBoxItems = new Queue<UIBottomPanelScrollItemView>();

        public ViewsCache()
        {
            _prefabsHolder = PrefabsHolder.Instance;
            _poolCanvasProvider = PoolCanvasProvider.Instance;
        }

        public UIBottomPanelScrollItemView GetOrCreateDefaultScrollBoxItem()
        {
            if (_cachedScrollBoxItems.Count <= 0)
            {
                var itemGo = GameObject.Instantiate(_prefabsHolder.UIBottomPanelScrollItemPrefab, _poolCanvasProvider.PoolCanvasTransform);
                var itemView = itemGo.GetComponent<UIBottomPanelScrollItemView>();
                _cachedScrollBoxItems.Enqueue(itemView);
            }

            return _cachedScrollBoxItems.Dequeue();
        }

        public void ReturnOrDestroyScrollBoxItem(UIBottomPanelScrollItemView scrollBoxItem)
        {
            scrollBoxItem.CancelAllAnimations();
            if (_cachedScrollBoxItems.Count >= ScrollBoxItemsCacheSize)
            {
                GameObject.Destroy(scrollBoxItem.gameObject);
            }
            else
            {
                scrollBoxItem.transform.SetParent(_poolCanvasProvider.PoolCanvasTransform);
                _cachedScrollBoxItems.Enqueue(scrollBoxItem);
            }
        }
    }
}
