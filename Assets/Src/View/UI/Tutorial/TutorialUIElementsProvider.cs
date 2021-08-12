using System;
using System.Collections.Generic;
using UnityEngine;

public class TutorialUIElementsProvider
{
    public static TutorialUIElementsProvider Instance => _instance.Value;
    private static Lazy<TutorialUIElementsProvider> _instance = new Lazy<TutorialUIElementsProvider>();

    private Dictionary<TutorialUIElement, RectTransform> _elements = new Dictionary<TutorialUIElement, RectTransform>();

    public RectTransform GetElementRect(TutorialUIElement elementId)
    {
        _elements.TryGetValue(elementId, out var result);
        return result;
    }

    public bool HasElement(TutorialUIElement elementId)
    {
        return _elements.ContainsKey(elementId);
    }

    public bool ClearElement(TutorialUIElement elementId)
    {
        return _elements.Remove(elementId);
    }

    public void SetElement(TutorialUIElement elementId, RectTransform elementRect)
    {
        _elements[elementId] = elementRect;
    }
}

public enum TutorialUIElement
{
    None,
    BottomPanelWarehouseButton,
    BottomPanelWarehouseTabFirstFreeSlot,
    OrderProductsPopupFirstItem,
    BottomPanelWarehouseTabLastDeliveringSlot,
}
