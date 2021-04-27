using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIGameViewPanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public event Action PointerDown = delegate { };
    public event Action PointerUp = delegate { };

    public void OnPointerDown(PointerEventData eventData)
    {
        PointerDown();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        PointerUp();
    }
}
