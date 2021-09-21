using System;
using UnityEngine;
using UnityEngine.UI;

public class UIGetDailyBonusButtonView : MonoBehaviour
{
    public event Action Clicked = delegate { };

    [SerializeField] private Button _button;
    [SerializeField] private Image _image;
    [SerializeField] private Sprite _skinSpriteRed;
    [SerializeField] private Sprite _skinSpriteGreen;
    [SerializeField] private Sprite _skinSpriteBlue;
    [SerializeField] private Sprite _skinSpriteYellow;
    [SerializeField] private UIHintableView _hintableView;

    public void Awake()
    {
        _button.AddOnClickListener(OnClickHandler);
    }

    public void ShowRed()
    {
        _image.sprite = _skinSpriteRed;
    }

    public void ShowGreen()
    {
        _image.sprite = _skinSpriteGreen;
    }

    public void ShowBlue()
    {
        _image.sprite = _skinSpriteBlue;
    }

    public void ShowYellow()
    {
        _image.sprite = _skinSpriteYellow;
    }

    public void SetEnabled(bool isEnabled)
    {
        _button.interactable = isEnabled;
    }

    public void SetVisibility(bool isVisible)
    {
        gameObject.SetActive(isVisible);
        _hintableView.SetEnabled(isVisible);
    }

    public void SetupHintText(string text)
    {
        _hintableView.DisplayText = text;
    }

    private void OnClickHandler()
    {
        Clicked();
    }
}
