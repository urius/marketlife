using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIContentPopupWithDescriptionAndButtons : UIContentPopupView
{
    public event Action<int> ButtonClicked = delegate { };

    [SerializeField] private TMP_Text _descrriptionText;
    [SerializeField] private Button[] _buttons;
    [SerializeField] private TMP_Text[] _buttonsTexts;

    protected Button[] Buttons => _buttons;

    public override void Awake()
    {
        base.Awake();

        for (var i = 0; i < _buttons.Length; i++)
        {
            var index = i;
            _buttons[i].AddOnClickListener(() => OnButtonClicked(index));
        }
    }

    public void SetDescriptionText(string text)
    {
        _descrriptionText.text = text;
    }

    public void SetupButton(int buttonIndex, Sprite sprite, string text)
    {
        _buttons[buttonIndex].image.sprite = sprite;
        _buttonsTexts[buttonIndex].text = text;
    }

    private void OnButtonClicked(int buttonIndex)
    {
        ButtonClicked(buttonIndex);
    }
}
