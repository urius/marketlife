using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIHintableView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private HintPositionType _hintPositionType;
    [SerializeField] private string _localizationKey;
    [SerializeField] private RectTransform _hintContainer;
    [SerializeField] private Vector2 _positionOffset;
    [SerializeField] private float _maxBGWidth = 220;

    private bool _isEnabled = true;
    private bool _isShowing = false;

    public void SetEnabled(bool isEnabled)
    {
        if (isEnabled == false)
        {
            Hide();
        }
        _isEnabled = isEnabled;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_isEnabled && !_isShowing)
        {
            Show();
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (_isEnabled)
        {
            Hide();
        }
    }

    private void Show()
    {
        HintViewManager.Instance.Show(transform, _hintPositionType, _localizationKey, _maxBGWidth, _positionOffset);
        _isShowing = true;
    }


    private void Hide()
    {
        if (_isShowing)
        {
            HintViewManager.Instance.Hide();
            _isShowing = false;
        }
    }
}
