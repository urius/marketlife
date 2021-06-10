using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIPopupBase : MonoBehaviour
{
    public event Action ButtonCloseClicked = delegate { };

    private const float AppearDurationSec = 0.4f;
    private const float DisppearDurationSec = 0.25f;

    [SerializeField] private Image _blockRaycastsImage;
    [SerializeField] private RectTransform _popupBodyRectTransform;
    [SerializeField] private Button _closeButton;
    [SerializeField] private TMP_Text _titleText;

    private CanvasGroup _popupBodyCanvasGroup;

    public void Awake()
    {
        _popupBodyCanvasGroup = _popupBodyRectTransform.gameObject.GetComponent<CanvasGroup>();

        _closeButton.AddOnClickListener(OnCloseClicked);
    }

    public void SetCloseButtonVisibility(bool isVisible)
    {
        _closeButton.gameObject.SetActive(isVisible);
    }

    public UniTask AppearAsync()
    {
        var tcs = new UniTaskCompletionSource();
        LeanTween.value(gameObject, a => _popupBodyCanvasGroup.alpha = a, 0, 1, 0.5f * AppearDurationSec);
        LeanTween.value(gameObject, p => _popupBodyRectTransform.anchoredPosition = p, new Vector2(0, -300), Vector2.zero, AppearDurationSec)
            .setEaseOutBack()
            .setOnComplete(() => tcs.TrySetResult());
        return tcs.Task;
    }

    public UniTask DisppearAsync()
    {
        var tcs = new UniTaskCompletionSource();
        _blockRaycastsImage.color = _blockRaycastsImage.color.SetAlpha(0);
        LeanTween.value(gameObject, a => _popupBodyCanvasGroup.alpha = a, 1, 0, DisppearDurationSec);
        LeanTween.value(gameObject, p => _popupBodyRectTransform.anchoredPosition = p, Vector2.zero, new Vector2(0, 300), DisppearDurationSec)
            .setEaseInBack()
            .setOnComplete(() => tcs.TrySetResult());
        return tcs.Task;
    }

    public void SetRaycastBlockerEnabled(bool isEnabled)
    {
        _blockRaycastsImage.gameObject.SetActive(isEnabled);
    }

    public void SetPopupAnchoredPosition(Vector2 anchoredPosition)
    {
        _popupBodyRectTransform.anchoredPosition = anchoredPosition;
    }

    public void SetTitleText(string text)
    {
        _titleText.text = text;
    }

    public void SetPopupAlpha(float alpha)
    {
        _popupBodyCanvasGroup.alpha = alpha;
    }

    private void OnCloseClicked()
    {
        ButtonCloseClicked();
    }
}
