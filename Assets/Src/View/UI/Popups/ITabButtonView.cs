using System;
using UnityEngine;

public interface ITabButtonView
{
    event Action Clicked;

    RectTransform RectTransform { get; }

    void SetInteractable(bool isEnabled);
    void SetText(string text);
}
