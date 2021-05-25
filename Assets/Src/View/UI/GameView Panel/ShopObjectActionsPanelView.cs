using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ShopObjectActionsPanelView : MonoBehaviour
{
    public event Action RotateRightClicked = delegate { };
    public event Action MoveClicked = delegate { };
    public event Action RotateLeftClicked = delegate { };
    public event Action RemoveClicked = delegate { };
    public event Action PointerExit = delegate { };

    private const float AppearAnimationDuration = 0.3f;

    [SerializeField] private Button _rotateRightButton;
    [SerializeField] private Button _moveButton;
    [SerializeField] private Button _rotateLeftButton;
    [SerializeField] private Button _removeButton;

    private RectTransform _rotateRightButtonRect;
    private RectTransform _rotateLeftButtonRect;
    private RectTransform _removeButtonRect;

    public Dictionary<Button, Vector2> _buttonsDefaultPositions = new Dictionary<Button, Vector2>();

    public void Awake()
    {
        _rotateRightButton.onClick.AddListener(OnRotateRightClicked);
        _moveButton.onClick.AddListener(OnMoveClicked);
        _rotateLeftButton.onClick.AddListener(OnRotateLeftClicked);
        _removeButton.onClick.AddListener(OnRemoveClicked);

        _rotateRightButtonRect = _rotateRightButton.transform as RectTransform;
        _rotateLeftButtonRect = _rotateLeftButton.transform as RectTransform;
        _removeButtonRect = _removeButton.transform as RectTransform;

        _buttonsDefaultPositions[_rotateRightButton] = _rotateRightButtonRect.anchoredPosition;
        _buttonsDefaultPositions[_rotateLeftButton] = _rotateLeftButtonRect.anchoredPosition;
        _buttonsDefaultPositions[_removeButton] = _removeButtonRect.anchoredPosition;
    }

    public void SetupButtons(bool showRotateButtons = true, bool showRemoveButton = true)
    {
        _rotateRightButton.gameObject.SetActive(showRotateButtons);
        _rotateLeftButton.gameObject.SetActive(showRotateButtons);
        _removeButton.gameObject.SetActive(showRemoveButton);
    }

    public UniTask AppearAsync()
    {
        var tsc = new UniTaskCompletionSource();

        LeanTween.cancel(_rotateRightButtonRect);
        LeanTween.cancel(_rotateLeftButtonRect);
        LeanTween.cancel(_removeButtonRect);
        _rotateRightButtonRect.anchoredPosition = Vector2.zero;
        _rotateLeftButtonRect.anchoredPosition = Vector2.zero;
        _removeButtonRect.anchoredPosition = Vector2.zero;
        LeanTween.move(_rotateRightButtonRect, _buttonsDefaultPositions[_rotateRightButton], AppearAnimationDuration).setEaseOutBack();
        LeanTween.move(_rotateLeftButtonRect, _buttonsDefaultPositions[_rotateLeftButton], AppearAnimationDuration).setEaseOutBack();
        LeanTween.move(_removeButtonRect, _buttonsDefaultPositions[_removeButton], AppearAnimationDuration).setEaseOutBack()
            .setOnComplete(() => tsc.TrySetResult());
        return tsc.Task;
    }

    private void OnRemoveClicked()
    {
        RemoveClicked();
    }

    private void OnRotateLeftClicked()
    {
        RotateLeftClicked();
    }

    private void OnMoveClicked()
    {
        MoveClicked();
    }

    private void OnRotateRightClicked()
    {
        RotateRightClicked();
    }
}
