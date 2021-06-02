using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ScrollBoxView : MonoBehaviour
{
    private const float DefaultDuration = 0.2f;
    private const float DefaultFastDuration = 0.1f;

    [SerializeField] private RectTransform _contentTransform;
    public RectTransform Content => _contentTransform;

    [SerializeField] private RectTransform _viewportTransform;
    [SerializeField] private GridLayoutGroup _gridLayoutGroup;
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private Button _scrollLeft;
    [SerializeField] private Button _rewindLeft;
    [SerializeField] private Button _scrollRight;
    [SerializeField] private Button _rewindRight;
    [SerializeField] private float _slotWidth;
    public float SlotWidth => _slotWidth;

    private float _viewportWidth;
    private float _cellWidth;

    //private List<RectTransform> _items = new List<RectTransform>(10);

    public void Awake()
    {
        _viewportWidth = _viewportTransform.rect.width;
        _cellWidth = _gridLayoutGroup.cellSize.x;

        _scrollRight.onClick.AddListener(OnScrollRigthClick);
        _rewindRight.onClick.AddListener(OnRewindRigthClick);
        _scrollLeft.onClick.AddListener(OnScrollLeftClick);
        _rewindLeft.onClick.AddListener(OnRewindLeftClick);
    }

    public void AddItem(RectTransform itemTransform)
    {
        itemTransform.parent = _contentTransform;
        //_items.Add(itemTransform);
    }

    public void SetContentWidth(float width)
    {
        var size = _contentTransform.sizeDelta;
        size.x = width;
        _contentTransform.sizeDelta = size;
    }

    public void SetContentPosition(float position)
    {
        var pos = _contentTransform.anchoredPosition;
        pos.x = position;
        _contentTransform.anchoredPosition = pos;
    }

    public void OnDragEnded()
    {
        var currentContentPos = _contentTransform.anchoredPosition.x;

        if (!IsOutOfBounds(currentContentPos))
        {
            var correctedContentPos = -currentContentPos + _cellWidth * 0.5f;
            var closestItemIndex = (int)Math.Floor(correctedContentPos / _cellWidth);

            ScrollToIndex(closestItemIndex, DefaultFastDuration);
        }
    }

    private void ScrollToIndex(int closestItemIndex, float duration = DefaultDuration)
    {
        var newContentPos = -closestItemIndex * _cellWidth;
        LeanTween.cancel(_contentTransform);
        LeanTween.moveX(_contentTransform, newContentPos, duration);
    }

    private void OnScrollRigthClick()
    {
        AnimateScrollButton(_scrollRight, 15);
        var _ = ProcessScrollAsync(1);
    }

    private void OnRewindRigthClick()
    {
        AnimateScrollButton(_rewindRight, 15);
        var _ = ProcessScrollAsync(3);
    }

    private void OnScrollLeftClick()
    {
        AnimateScrollButton(_scrollLeft, -15);
        var _ = ProcessScrollAsync(-1);
    }

    private void OnRewindLeftClick()
    {
        AnimateScrollButton(_rewindLeft, -15);
        var _ = ProcessScrollAsync(-3);
    }

    private async UniTaskVoid ProcessScrollAsync(int delta)
    {
        if (LeanTween.isTweening(_contentTransform) || delta == 0)
        {
            return;
        }

        var direction = delta > 0 ? 1 : -1;
        var currentContentPos = _contentTransform.anchoredPosition.x;
        var newContentPos = currentContentPos - _gridLayoutGroup.cellSize.x * delta;

        LeanTween.cancel(_contentTransform);
        if ((direction > 0 && IsPositionOutOfRightBound(currentContentPos, 1)) || (direction < 0 && IsPositionOutOfLeftBound(currentContentPos, 1)))
        {
            _scrollRect.enabled = false;
            await LeanTweenHelper.BounceXAsync(_contentTransform, -1 * direction * _gridLayoutGroup.cellSize.x * 0.5f);
            _scrollRect.enabled = true;
        }
        else
        {
            newContentPos = ClampPosition(newContentPos);
            if (Math.Abs(newContentPos - currentContentPos) < 1)
            {
                _contentTransform.anchoredPosition = new Vector2(newContentPos, _contentTransform.anchoredPosition.y);
            }
            else
            {
                LeanTween.moveX(_contentTransform, newContentPos, DefaultDuration);
            }
        }
    }

    private float ClampPosition(float contentPos)
    {
        return Mathf.Clamp(contentPos, _viewportWidth - _contentTransform.rect.width, 0);
    }

    private bool IsPositionOutOfRightBound(float position, float offset = 0)
    {
        return position + _contentTransform.rect.width < _viewportWidth + offset;
    }

    private bool IsPositionOutOfLeftBound(float position, float offset = 0)
    {
        return position > 0 - offset;
    }

    private bool IsOutOfBounds(float position, float offset = 0)
    {
        return IsPositionOutOfRightBound(position, offset) || IsPositionOutOfLeftBound(position, offset);
    }

    private void AnimateScrollButton(Button scrollButton, int deltaX)
    {
        var rectTransform = scrollButton.transform as RectTransform;

        if (LeanTween.isTweening(rectTransform)) return;

        LeanTween.cancel(rectTransform);
        LeanTweenHelper.BounceX(rectTransform, deltaX, 0.2f, 0.6f);
    }
}
