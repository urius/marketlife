using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBillboardPopupView : UIPopupBase
{
    public event Action ApplyClicked = delegate { };

    [SerializeField] private TMP_Text _messageText;
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private Button _applyButton;

    public string InputFieldText => _inputField.text;

    public override void Awake()
    {
        base.Awake();
        _applyButton.AddOnClickListener(OnApplyClick);
    }

    public void SetMessageText(string text)
    {
        _messageText.text = text;
    }

    public void SetInputFieldText(string text)
    {
        _inputField.text = text;
    }

    private void OnApplyClick()
    {
        ApplyClicked();
    }
}