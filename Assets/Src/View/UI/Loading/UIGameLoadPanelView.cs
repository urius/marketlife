using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGameLoadPanelView : MonoBehaviour
{
    [SerializeField] private TMP_Text _statusText;
    [SerializeField] private Image _loadingStripeImage;
    [SerializeField] private RectTransform _loadingBarMaskRectTransform;
    [SerializeField] private RectTransform _errorPanelRectTransform;
    [SerializeField] private TMP_Text _errorText;

    private float _maxBarWidth;

    public void Awake()
    {
        _maxBarWidth = _loadingBarMaskRectTransform.sizeDelta.x;
        _errorPanelRectTransform.gameObject.SetActive(false);
        SetBarScale(0);
    }

    public void SetBarScale(float scale)
    {
        var sizeDelta = _loadingBarMaskRectTransform.sizeDelta;
        var width = scale * _maxBarWidth;
        _loadingBarMaskRectTransform.sizeDelta = new Vector2(width, sizeDelta.y);
    }

    public void SetStatusText(string text)
    {
        _statusText.text = text;
    }

    public void ShowError(string text)
    {
        _errorPanelRectTransform.gameObject.SetActive(true);
        _loadingStripeImage.color = Color.red;
        _errorText.text = text;
    }
}
