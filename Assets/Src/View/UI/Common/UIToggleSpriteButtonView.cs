using System;
using UnityEngine;
using UnityEngine.UI;

public class UIToggleSpriteButtonView : MonoBehaviour
{
    public event Action Clicked = delegate { };

    [SerializeField] private Button _button;
    [SerializeField] private Image _iconImage;
    [SerializeField] private Sprite _spriteToggledOn;
    [SerializeField] private Sprite _spriteToggledOff;

    public void Awake()
    {
        _button.AddOnClickListener(OnButtonClick);
    }

    public void ToggleOn()
    {
        _iconImage.sprite = _spriteToggledOn;
    }

    public void ToggleOff()
    {
        _iconImage.sprite = _spriteToggledOff;
    }

    public void SetInteractable(bool isInteractable)
    {
        _button.interactable = isInteractable;
    }

    private void OnButtonClick()
    {
        Clicked();
    }
}
