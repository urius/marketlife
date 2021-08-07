using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialOverlayView : MonoBehaviour
{
    public event Action Clicked = delegate { };

    [SerializeField] private RectTransform _rootRect;
    [SerializeField] private RectTransform _highlightRect;
    [SerializeField] private RectTransform _bgRect;
    [SerializeField] private Image _bgImage;
    [SerializeField] private RectTransform _popupBodyRect;
    [SerializeField] private CanvasGroup _popupBodyCanvasGroup;
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _messageText;
    [SerializeField] private Button _okButton;
    [SerializeField] private Text _okButtonText;

    private Camera _camera;
    private Color _bgDefaultColor;

    public void Setup(Camera camera)
    {
        _camera = camera;
    }

    public void Awake()
    {
        _bgDefaultColor = _bgImage.color;
        _bgImage.color = _bgImage.color.SetAlpha(0);

        _messageText.enableWordWrapping = false;

        _okButton.AddOnClickListener(OnClicked);
    }

    public void Start()
    {
        DisableHighlight();
        Appear();
    }

    public void Appear()
    {
        _popupBodyCanvasGroup.alpha = 0;
        LeanTween.value(gameObject, c => { _bgImage.color = c; }, _bgDefaultColor.SetAlpha(0), _bgDefaultColor, 0.6f);
        LeanTween.value(gameObject, a => { _popupBodyCanvasGroup.alpha = a; }, 0f, 1f, 0.2f).setDelay(0.6f);
    }

    public void SetTexts(string titleText, string messageText, string buttontext)
    {
        _titleText.text = titleText;
        _messageText.text = messageText;
        _okButtonText.text = buttontext;

        CorrectPopupSize();
    }

    public void SetQuadrant(int quadrant)
    {
        var manHeight = 100;
        var offsetValues = new Vector2(_popupBodyRect.rect.width * 0.5f + 100, _popupBodyRect.rect.height * 0.5f);
        switch (quadrant)
        {
            case 0:
                offsetValues = Vector2.zero;
                break;
            case 2:
                offsetValues.y = -offsetValues.y - manHeight;
                break;
            case 3:
                offsetValues.y = -offsetValues.y - manHeight;
                offsetValues.x = -offsetValues.x;
                break;
            case 4:
                offsetValues.x = -offsetValues.x;
                break;
        }

        _popupBodyRect.anchoredPosition = offsetValues;
    }

    public void DisableHighlight()
    {
        SetHighlightSize(Vector2.zero);
    }

    public void HighlightScreenPoint(Vector3 screenPoint, Vector2 size)
    {
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(_rootRect, screenPoint, _camera, out var worldPoint))
        {
            _highlightRect.position = worldPoint;
            SetHighlightSize(size);
        }
    }

    public void SetHighlightSize(Vector2 size)
    {
        _highlightRect.sizeDelta = size;
        UpdateBg();
    }

    private void UpdateBg()
    {
        _bgRect.position = _rootRect.TransformPoint(Vector3.zero);
        _bgRect.sizeDelta = _rootRect.rect.size;
    }

    private void OnClicked()
    {
        Clicked();
    }

    private void CorrectPopupSize()
    {
        _messageText.ForceMeshUpdate();

        var size = _bgRect.sizeDelta;
        var textRectTransform = _messageText.rectTransform;
        var anchorXMult = 1 / (textRectTransform.anchorMax.x - textRectTransform.anchorMin.x);
        var anchorYMult = 1 / (textRectTransform.anchorMax.y - textRectTransform.anchorMin.y);
        size.x = _messageText.textBounds.size.x * anchorXMult + textRectTransform.offsetMin.x - textRectTransform.offsetMax.x + 1;
        size.y = _messageText.textBounds.size.y * anchorYMult + textRectTransform.offsetMin.y - textRectTransform.offsetMax.y + 20;
        _popupBodyRect.sizeDelta = size;
    }
}
