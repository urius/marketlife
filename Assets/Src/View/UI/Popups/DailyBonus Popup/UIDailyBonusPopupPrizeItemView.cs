using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDailyBonusPopupPrizeItemView : MonoBehaviour
{
    public event Action<UIDailyBonusPopupPrizeItemView> DoubleButtonClicked = delegate { };

    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private TMP_Text _dayText;
    [SerializeField] private Image _iconImage;
    [SerializeField] private TMP_Text _valueText;
    [SerializeField] private Button _doubleButton;

    public void SetAlpha(float alpha)
    {
        _canvasGroup.alpha = alpha;
    }

    public void SetDayText(string text)
    {
        _dayText.text = text;
    }

    public void SetValueText(string text)
    {
        _valueText.text = text;
    }

    public void SetIconSprite(Sprite sprite)
    {
        _iconImage.sprite = sprite;
    }

    public void SetDoubleButtonInteractable(bool isInteractable)
    {
        _doubleButton.interactable = isInteractable;
    }

    public void SetDoubleButtonVisible(bool isVisible)
    {
        _doubleButton.gameObject.SetActive(isVisible);
    }

    private void Awake()
    {
        _doubleButton.AddOnClickListener(OnDoubleButtonClicked);
    }

    private void OnDoubleButtonClicked()
    {
        DoubleButtonClicked(this);
    }
}
