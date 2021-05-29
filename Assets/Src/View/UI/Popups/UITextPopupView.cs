using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITextPopupView : MonoBehaviour
{
    public event Action Button1Clicked = delegate { };
    public event Action Button2Clicked = delegate { };
    public event Action ButtonCloseClicked = delegate { };

    private const float AppearDurationSec = 0.4f;
    private const float DisppearDurationSec = 0.25f;

    [SerializeField] private Image _blockRaycastsImage;
    [SerializeField] private RectTransform _popupBodyRectTransform;
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _messageText;
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _leftButton;
    [SerializeField] private Text _leftButtonText;
    [SerializeField] private Button _midButton;
    [SerializeField] private Text _midButtonText;
    [SerializeField] private Button _rightButton;
    [SerializeField] private Text _rightButtonText;

    private CanvasGroup _popupBodyCanvasGroup;

    public void Awake()
    {
        _popupBodyCanvasGroup = _popupBodyRectTransform.gameObject.GetComponent<CanvasGroup>();
    }

    public void Setup(bool haveCloseButton = true, int bottomButtonsAmount = 0)
    {
        _closeButton.gameObject.SetActive(haveCloseButton);
        _leftButton.gameObject.SetActive(false);
        _midButton.gameObject.SetActive(false);
        _rightButton.gameObject.SetActive(false);
        switch (bottomButtonsAmount)
        {
            case 1:
                _midButton.gameObject.SetActive(true);
                _midButton.AddOnClickListener(OnButton1Click);
                break;
            case 2:
                _leftButton.gameObject.SetActive(true);
                _leftButton.AddOnClickListener(OnButton1Click);
                _rightButton.gameObject.SetActive(true);
                _rightButton.AddOnClickListener(OnButton2Click);
                break;
        }
        _closeButton.AddOnClickListener(OnCloseClicked);
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

    public void SetupButton(int buttonIndex, Sprite sprite, string text)
    {
        if (buttonIndex == 0)
        {
            _leftButton.image.sprite = sprite;
            _leftButtonText.text = text;
            _midButton.image.sprite = sprite;
            _midButtonText.text = text;
        }
        if (buttonIndex == 1)
        {
            _rightButton.image.sprite = sprite;
            _rightButtonText.text = text;
        }
    }

    public void SetRaycastBlockerEnabled(bool isEnabled)
    {
        _blockRaycastsImage.gameObject.SetActive(isEnabled);
    }

    private void OnButton1Click()
    {
        Button1Clicked();
    }

    private void OnButton2Click()
    {
        Button2Clicked();
    }

    public void SetPopupAnchoredPosition(Vector2 anchoredPosition)
    {
        _popupBodyRectTransform.anchoredPosition = anchoredPosition;
    }

    public void SetPopupAlpha(float alpha)
    {
        _popupBodyCanvasGroup.alpha = alpha;
    }

    public void SetTitleText(string text)
    {
        _titleText.text = text;
    }

    public void SetMessageText(string text)
    {
        _messageText.text = text;
    }

    private void OnCloseClicked()
    {
        ButtonCloseClicked();
    }
}