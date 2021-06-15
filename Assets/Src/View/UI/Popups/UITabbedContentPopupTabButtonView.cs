using System;
using UnityEngine;
using UnityEngine.UI;

public class UITabbedContentPopupTabButtonView : MonoBehaviour
{
    public event Action Clicked = delegate { };

    [SerializeField] private Button _button;
    [SerializeField] private Text _text;

    public void Awake()
    {
        _button.AddOnClickListener(OnButtonClick);
    }

    public void SetInteractable(bool isEnabled)
    {
        _button.interactable = isEnabled;
    }

    public void SetText(string text)
    {
        _text.text = text;
    }

    private void OnButtonClick()
    {
        Clicked();
    }
}
