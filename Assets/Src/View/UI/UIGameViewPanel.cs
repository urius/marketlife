using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Src.View.UI
{
    public class UIGameViewPanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public event Action PointerDown = delegate { };
        public event Action PointerUp = delegate { };
        public event Action PointerEnter = delegate { };
        public event Action PointerExit = delegate { };

        public void OnPointerDown(PointerEventData eventData)
        {
            PointerDown();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            PointerUp();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            PointerEnter();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            PointerExit();
        }
    }
}
