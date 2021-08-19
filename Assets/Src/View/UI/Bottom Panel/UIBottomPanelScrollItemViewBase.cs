using System;
using UnityEngine;
using UnityEngine.UI;

public class UIBottomPanelScrollItemViewBase : MonoBehaviour
{
    public event Action<UIBottomPanelScrollItemViewBase> Clicked = delegate { };

    [SerializeField] private Button _button;

    private RectTransform _rectTransform;

    protected RectTransform RectTransform => _rectTransform;

    public virtual void Awake()
    {
        _rectTransform = transform as RectTransform;

        _button.AddOnClickListener(OnButtonClick);
    }

    public void SetAnchoredPosition(Vector2 position)
    {
        _rectTransform.anchoredPosition = position;
    }

    private void OnButtonClick()
    {
        Clicked(this);
    }
}
