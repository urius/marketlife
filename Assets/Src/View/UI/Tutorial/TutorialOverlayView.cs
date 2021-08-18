using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialOverlayView : MonoBehaviour
{
    public event Action Clicked = delegate { };

    [SerializeField] private RectTransform _rootRect;
    [SerializeField] private Image _rootImage;
    [SerializeField] private RectTransform _highlightRect;
    [SerializeField] private Image _highlightImage;
    [SerializeField] private RectTransform _bgRect;
    [SerializeField] private Image _bgImage;
    [SerializeField] private RectTransform _popupBodyRect;
    [SerializeField] private CanvasGroup _popupBodyCanvasGroup;
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _messageText;
    [SerializeField] private Button _okButton;
    [SerializeField] private Text _okButtonText;
    [SerializeField] private Sprite _roundSprite;
    [SerializeField] private Sprite _roundedSquareSprite;
    [SerializeField] private RectTransform _manRect;


    private Camera _camera;
    private Color _bgDefaultColor;

    public RectTransform RootRect => _rootRect;
    public RectTransform HighlightRect => _highlightRect;

    public void Setup(Camera camera)
    {
        _camera = camera;
    }

    public bool AreClicksBlocked => _rootImage.raycastTarget;

    public void SetClickBlockState(bool isBlocked)
    {
        _rootImage.raycastTarget = isBlocked;
    }

    public void Awake()
    {
        _bgDefaultColor = _bgImage.color;
        _bgImage.color = _bgImage.color.SetAlpha(0);
        _popupBodyCanvasGroup.alpha = 0;

        _messageText.enableWordWrapping = false;

        DisableHighlight();

        _okButton.AddOnClickListener(OnClicked);
    }

    public void OnDestroy()
    {
        LeanTween.cancel(gameObject);
    }

    public void PlaceManLeft()
    {
        _manRect.anchoredPosition = new Vector2(80, _manRect.anchoredPosition.y);
        _manRect.anchorMin = new Vector2(0, _manRect.anchorMin.y);
        _manRect.anchorMax = new Vector2(0, _manRect.anchorMax.y);
        _manRect.localScale = new Vector3(-1, 1, 1);
    }

    public void PlaceManRight()
    {
        _manRect.anchoredPosition = new Vector2(-80, _manRect.anchoredPosition.y);
        _manRect.anchorMin = new Vector2(1, _manRect.anchorMin.y);
        _manRect.anchorMax = new Vector2(1, _manRect.anchorMax.y);
        _manRect.localScale = new Vector3(1, 1, 1);
    }

    public void Appear(bool animateBgFlag = false, bool animatePopupFlag = false)
    {
        if (animateBgFlag)
        {
            LeanTween.value(gameObject, c => { _bgImage.color = c; }, _bgDefaultColor.SetAlpha(0), _bgDefaultColor, 0.5f);
        }
        else
        {
            _bgImage.color = _bgDefaultColor;
        }

        if (animatePopupFlag)
        {
            LeanTween.value(gameObject, a => { _popupBodyCanvasGroup.alpha = a; }, 0f, 1f, 0.2f).setDelay(0.5f);
        }
        else
        {
            _popupBodyCanvasGroup.alpha = 1;
        }
    }

    public void SetTitle(string titleText)
    {
        _titleText.text = titleText;
    }

    public void SetMessageText(string messageText)
    {
        _messageText.text = messageText;
        CorrectPopupSize();
    }

    public void SetButtonText(string buttonText)
    {
        _okButtonText.text = buttonText;
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

    public void DisableButton()
    {
        _okButton.gameObject.SetActive(false);
    }

    public void DisablePopup()
    {
        _popupBodyRect.gameObject.SetActive(false);
    }

    public void HighlightScreenRoundArea(Vector3 worldPoint, Vector2 size, bool animated = false)
    {
        _highlightImage.sprite = _roundSprite;
        _highlightImage.type = Image.Type.Simple;
        ShowHighlightAreaInternal(worldPoint, size, animated);
    }

    public void HighlightScreenSquareArea(Vector3 worldPoint, Vector2 size, bool animated = false)
    {
        _highlightImage.sprite = _roundedSquareSprite;
        _highlightImage.type = Image.Type.Sliced;
        ShowHighlightAreaInternal(worldPoint, size, animated);
    }

    private void ShowHighlightAreaInternal(Vector3 inputWorldPoint, Vector2 size, bool animated)
    {
        var screenPoint =_camera.WorldToScreenPoint(inputWorldPoint);//todo check redundant
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(_rootRect, screenPoint, _camera, out var worldPoint))// todo check redundant
        {
            _highlightRect.position = worldPoint;
            if (animated)
            {
                UpdateBg();
                LeanTween.value(gameObject, v => SetHighlightSize(v), Vector2.zero, size, 0.5f);
            }
            else
            {
                SetHighlightSize(size);
            }
        }
    }

    public void SetHighlightPosition(Vector3 inputWorldPoint)
    {
        var screenPoint = _camera.WorldToScreenPoint(inputWorldPoint);
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(_rootRect, screenPoint, _camera, out var worldPoint))
        {
            _highlightRect.position = worldPoint;
            UpdateBg();
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
