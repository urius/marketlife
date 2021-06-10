using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITextPopupView : UIPopupBase
{
    public event Action Button1Clicked = delegate { };
    public event Action Button2Clicked = delegate { };

    [SerializeField] private TMP_Text _messageText;
    [SerializeField] private Button _leftButton;
    [SerializeField] private Text _leftButtonText;
    [SerializeField] private Button _midButton;
    [SerializeField] private Text _midButtonText;
    [SerializeField] private Button _rightButton;
    [SerializeField] private Text _rightButtonText;

    public void Setup(bool haveCloseButton = true, int bottomButtonsAmount = 0)
    {
        SetCloseButtonVisibility(haveCloseButton);
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

    private void OnButton1Click()
    {
        Button1Clicked();
    }

    private void OnButton2Click()
    {
        Button2Clicked();
    }

    public void SetMessageText(string text)
    {
        _messageText.text = text;
    }
}