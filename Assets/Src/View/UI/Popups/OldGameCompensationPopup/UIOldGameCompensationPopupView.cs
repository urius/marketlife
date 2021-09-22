using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIOldGameCompensationPopupView : UIPopupBase
{
    public event Action TakeClicked = delegate { };

    [SerializeField] private TMP_Text _messageText;
    [SerializeField] private TMP_Text _goldText;
    [SerializeField] private TMP_Text _cashText;
    [SerializeField] private Button _takeButton;
    public Button TakeButton => _takeButton;

    public override void Awake()
    {
        base.Awake();

        _takeButton.AddOnClickListener(OnTakeClicked);
    }

    public void SetGoldText(string text)
    {
        _goldText.text = text;
    }

    public void SetCashText(string text)
    {
        _cashText.text = text;
    }

    public void SetMessageText(string text)
    {
        _messageText.text = text;
    }

    private void OnTakeClicked()
    {
        TakeClicked();
    }
}
