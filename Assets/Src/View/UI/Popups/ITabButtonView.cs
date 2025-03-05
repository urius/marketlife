using System;
using UnityEngine;

namespace Src.View.UI.Popups
{
    public interface ITabButtonView
    {
        event Action Clicked;

        RectTransform RectTransform { get; }

        void SetInteractable(bool isEnabled);
        void SetText(string text);
    }
}
