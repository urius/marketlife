using System;
using System.Collections.Generic;
using UnityEngine;

public class TutorialUIElementsProvider
{
    public static TutorialUIElementsProvider Instance => _instance.Value;
    private static Lazy<TutorialUIElementsProvider> _instance = new Lazy<TutorialUIElementsProvider>();

    private Dictionary<TutorialUIElement, Transform> _elements = new Dictionary<TutorialUIElement, Transform>();

    public RectTransform GetElementRectTransform(TutorialUIElement elementId)
    {
        _elements.TryGetValue(elementId, out var result);
        return result as RectTransform;
    }

    public Transform GetElementTransform(TutorialUIElement elementId)
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

    public void SetElement(TutorialUIElement elementId, Transform transform)
    {
        _elements[elementId] = transform;
    }
}

public enum TutorialUIElement
{
    None,
    BottomPanelWarehouseButton,
    BottomPanelWarehouseTabFirstFreeSlot,
    OrderProductsPopupFirstItem,
    BottomPanelWarehouseTabLastDeliveringSlot,
    BottomPanelWarehouseTabLastDeliveringSlotTime,
    ShopFloorTransform,
    TopPanelMoodBar,
    BottomPanelInteriorButton,
    BottomPanelFriendsButton,
    BottomPanelManageButton,
}
